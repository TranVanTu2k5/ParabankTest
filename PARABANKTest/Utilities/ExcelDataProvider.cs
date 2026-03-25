using NUnit.Framework;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PARABANKTest.Utilities
{
    public class ExcelDataProvider
    {
        private static string GetExcelPath()
        {
            return @"C:\BDCLPM\PARABANKTest\PARABANKTest\DataTest.xlsx";
        }

        // =========================
        // MODEL
        // =========================
        public class TestStep
        {
            public string Step { get; set; }
            public string Action { get; set; }
            public string Data { get; set; }
            public string Expected { get; set; }
            public string Image { get; set; }
        }

        public class TestCase
        {
            public string TestCaseId { get; set; }
            public string SheetName { get; set; }
            public List<TestStep> Steps { get; set; } = new List<TestStep>();
        }

        // =========================
        // READ FULL DATA
        // =========================
        public static List<TestCase> GetAllTestCases()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Tah");

            var allTestCases = new List<TestCase>();

            using (var package = new ExcelPackage(new FileInfo(GetExcelPath())))
            {
                var workbook = package.Workbook;

                foreach (var ws in workbook.Worksheets)
                {
                    // 🔥 chỉ lấy sheet dạng "TC ..."
                    if (!ws.Name.StartsWith("TC "))
                        continue;

                    TestContext.WriteLine($"\n===== SHEET: {ws.Name} =====");

                    int rowCount = ws.Dimension.End.Row;

                    string currentTC = "";
                    Dictionary<string, TestCase> testCases = new Dictionary<string, TestCase>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string tcId = ws.Cells[row, 2].Text.Trim();
                        string step = ws.Cells[row, 8].Text.Trim();
                        string action = ws.Cells[row, 9].Text.Trim();
                        string data = ws.Cells[row, 10].Text.Trim();
                        string expected = ws.Cells[row, 11].Text.Trim();
                        string image = ws.Cells[row, 16].Text.Trim();

                        // giữ TC nếu ô trống
                        if (!string.IsNullOrEmpty(tcId))
                        {
                            currentTC = tcId;
                        }

                        if (string.IsNullOrEmpty(currentTC))
                            continue;

                        // nếu chưa có testcase thì tạo mới
                        if (!testCases.ContainsKey(currentTC))
                        {
                            testCases[currentTC] = new TestCase
                            {
                                TestCaseId = currentTC,
                                SheetName = ws.Name   
                            };
                        }

                        // tạo step
                        TestStep stepObj = new TestStep
                        {
                            Step = step,
                            Action = action,
                            Data = data,
                            Expected = expected,
                            Image = image
                        };

                        testCases[currentTC].Steps.Add(stepObj);

                        // debug
                        TestContext.WriteLine(
                            $"Row {row} | TC={currentTC} | Step={step} | Action={action} | Data={data} | Expected={expected} | Image={image}"
                        );
                    }

                    // add vào list tổng
                    foreach (var tc in testCases.Values)
                    {
                        allTestCases.Add(tc);
                    }
                }
            }

            return allTestCases;
        }

        // =========================
        // DEBUG IN RA CHO M XEM
        // =========================
        public static void DebugReadExcel()
        {
            var all = GetAllTestCases();

            TestContext.WriteLine("\n========= FINAL RESULT =========");

            foreach (var tc in all)
            {
                TestContext.WriteLine($"\n=== {tc.TestCaseId} ===");

                foreach (var step in tc.Steps)
                {
                    TestContext.WriteLine(
                        $"Step {step.Step} | {step.Action} | Data={step.Data} | Expected={step.Expected} | Image={step.Image}"
                    );
                }
            }
        }

        public static void WriteResult(string sheetName, string testCaseId, string actual, string status, string image)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Tah");

            using (var package = new ExcelPackage(new FileInfo(GetExcelPath())))
            {
                var ws = package.Workbook.Worksheets[sheetName];
                int rows = ws.Dimension.End.Row;

                for (int i = 2; i <= rows; i++)
                {
                    string tcId = ws.Cells[i, 2].Text.Trim();

                    if (tcId == testCaseId)
                    {
                        // ===== GHI DATA =====
                        ws.Cells[i, 12].Value = actual; // Actual
                        ws.Cells[i, 13].Value = status; // Status
                        ws.Cells[i, 16].Value = image;  // Image

                        var statusCell = ws.Cells[i, 13];

                        // ===== FORMAT =====
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // ===== COLOR =====
                        if (status == "Passed")
                        {
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 239, 206)); // xanh đẹp
                        }
                        else if (status == "Failed")
                        {
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 199, 206)); // đỏ đẹp
                        }

                        break; // 🔥 cực quan trọng
                    }
                }

                package.Save();
            }
        }


    }
}