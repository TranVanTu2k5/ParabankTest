using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PARABANKTest.Pages;
using PARABANKTest.Utilities;
using System;
using System.Linq;

namespace PARABANKTest.Tests
{
    public class LoginTests
    {
        IWebDriver driver;
        LoginPage loginPage;

        // ===== TEST =====
        [Test, TestCaseSource(typeof(ExcelDataProvider), "GetAllTestCases")]
        public void Test_Login(ExcelDataProvider.TestCase tc)
        {
            // 🔥 CHẶN NGAY TỪ ĐẦU
            if (string.IsNullOrEmpty(tc.TestCaseId) ||
                !tc.TestCaseId.ToLower().Contains("login"))
            {
                Assert.Ignore("Không phải test login");
            }

            // 🔥 CHỈ LOGIN MỚI TẠO DRIVER
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            loginPage = new LoginPage(driver);

            TestContext.WriteLine($"\n===== RUN TC: {tc.TestCaseId} =====");

            string actual = "";
            string status = "Passed";
            string imagePath = "";

            try
            {
                // 🚀 RUN STEP
                foreach (var step in tc.Steps)
                {
                    ExecuteStep(step);
                }

                // 🚀 VERIFY
                bool isPass = VerifyLoginResult(tc);

                if (!isPass)
                    throw new Exception("Kết quả không đúng với mong đợi");

                actual = GetActualResult();

                TestContext.WriteLine($"PASS: {tc.TestCaseId}");
            }
            catch (Exception ex)
            {
                status = "Failed";

                if (ex.Message.ToLower().Contains("stale element") ||
                    ex.Message.ToLower().Contains("no such element") ||
                    ex.Message.ToLower().Contains("timeout"))
                {
                    actual = "Không có chức năng này";
                }
                else
                {
                    actual = ex.Message;
                }

                imagePath = ScreenshotHelper.TakeScreenshot(driver, tc.TestCaseId);

                TestContext.WriteLine($"FAIL: {tc.TestCaseId}");
            }

            // 🚀 WRITE EXCEL
            ExcelDataProvider.WriteResult(
                tc.SheetName,
                tc.TestCaseId,
                actual,
                status,
                imagePath
            );
        }

        // ===== STEP =====
        private void ExecuteStep(ExcelDataProvider.TestStep step)
        {
            if (step == null || string.IsNullOrEmpty(step.Action))
                return;

            string action = step.Action.ToLower();

            if (action.Contains("vào trang"))
            {
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
                Thread.Sleep(1000);
            }
            else if (action.Contains("username") && !action.Contains("trống"))
            {
                loginPage.EnterUsername(step.Data ?? "");
            }
            else if (action.Contains("password") && !action.Contains("trống"))
            {
                loginPage.EnterPassword(step.Data ?? "");
            }
            else if (action.Contains("để trống username"))
            {
                loginPage.EnterUsername("");
            }
            else if (action.Contains("để trống password"))
            {
                loginPage.EnterPassword("");
            }
            else if (action.Contains("click login"))
            {
                loginPage.ClickLogin();
                Thread.Sleep(1000);
            }
        }

        // ===== VERIFY =====
        private bool VerifyLoginResult(ExcelDataProvider.TestCase tc)
        {
            string expected = tc.Steps
                .Where(s => !string.IsNullOrEmpty(s.Expected))
                .LastOrDefault()?.Expected?.ToLower();

            if (string.IsNullOrEmpty(expected))
                return false;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                // ✅ SUCCESS
                if (expected.Contains("đăng nhập thành công"))
                {
                    wait.Until(d => d.Url.Contains("overview"));
                    return true;
                }

                // ❌ SAI USER/PASS + SQL INJECTION
                if (expected.Contains("could not be verified"))
                {
                    var error = wait.Until(d =>
                    {
                        var el = d.FindElements(By.CssSelector(".error"));
                        return el.Count > 0 ? el[0].Text.ToLower() : null;
                    });

                    return error.Contains("could not be verified");
                }

                // ❌ THIẾU INPUT
                if (expected.Contains("please enter a username and password"))
                {
                    var error = wait.Until(d =>
                    {
                        var el = d.FindElements(By.CssSelector(".error"));
                        return el.Count > 0 ? el[0].Text.ToLower() : null;
                    });

                    return error.Contains("please enter a username and password");
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // ===== ACTUAL =====
        private string GetActualResult()
        {
            if (driver.Url.Contains("overview"))
                return "Đăng nhập thành công";

            var errors = driver.FindElements(By.CssSelector(".error"));
            if (errors.Count > 0)
                return errors[0].Text;

            return "Không xác định";
        }

        // ===== TEARDOWN =====
        [TearDown]
        public void Cleanup()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }
    }
}

