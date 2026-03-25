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
    public class RegisterTests
    {
        IWebDriver driver;
        RegisterPage registerPage;

        [Test, TestCaseSource(typeof(ExcelDataProvider), "GetAllTestCases")]
        public void Test_Register(ExcelDataProvider.TestCase tc)
        {
            // 🔥 CHẶN NGAY TỪ ĐẦU
            if (string.IsNullOrEmpty(tc.TestCaseId) ||
                !tc.TestCaseId.ToLower().Contains("register"))
            {
                Assert.Ignore("Không phải test register");
            }

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            registerPage = new RegisterPage(driver);

            TestContext.WriteLine($"\n===== RUN TC: {tc.TestCaseId} =====");

            string actual = "";
            string status = "Passed";
            string imagePath = "";

            try
            {
                foreach (var step in tc.Steps)
                {
                    ExecuteStep(step);
                }

                bool isPass = VerifyRegisterResult(tc);

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

            if (action.Contains("vào trang register"))
            {
                registerPage.GoToRegister();
                Thread.Sleep(1000);
            }
            else if (action.Contains("first name"))
            {
                registerPage.EnterFirstName(step.Data ?? "");
            }
            else if (action.Contains("last name"))
            {
                registerPage.EnterLastName(step.Data ?? "");
            }
            else if (action.Contains("address"))
            {
                registerPage.EnterAddress(step.Data ?? "");
            }
            else if (action.Contains("city"))
            {
                registerPage.EnterCity(step.Data ?? "");
            }
            else if (action.Contains("state"))
            {
                registerPage.EnterState(step.Data ?? "");
            }
            else if (action.Contains("zip"))
            {
                registerPage.EnterZip(step.Data ?? "");
            }
            else if (action.Contains("phone"))
            {
                registerPage.EnterPhone(step.Data ?? "");
            }
            else if (action.Contains("ssn"))
            {
                registerPage.EnterSSN(step.Data ?? "");
            }
            else if (action.Contains("username"))
            {
                registerPage.EnterUsername(step.Data ?? "");
            }
            else if (action.Contains("password") && !action.Contains("confirm"))
            {
                registerPage.EnterPassword(step.Data ?? "");
            }
            else if (action.Contains("confirm password"))
            {
                registerPage.EnterConfirmPassword(step.Data ?? "");
            }
            else if (action.Contains("click register"))
            {
                registerPage.ClickRegister();
                Thread.Sleep(1000);
            }
        }

        // ===== VERIFY =====
        private bool VerifyRegisterResult(ExcelDataProvider.TestCase tc)
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
                if (expected.Contains("your account was created successfully"))
                {
                    wait.Until(d => d.PageSource.ToLower().Contains("your account was created successfully"));
                    return true;
                }

                // ❌ USERNAME REQUIRED
                if (expected.Contains("username is required"))
                {
                    var error = wait.Until(d =>
                    {
                        var el = d.FindElements(By.CssSelector(".error"));
                        return el.Count > 0 ? el[0].Text.ToLower() : null;
                    });

                    return error.Contains("username is required");
                }

                // ❌ PASSWORD NOT MATCH
                if (expected.Contains("passwords did not match"))
                {
                    var error = wait.Until(d =>
                    {
                        var el = d.FindElements(By.CssSelector(".error"));
                        return el.Count > 0 ? el[0].Text.ToLower() : null;
                    });

                    return error.Contains("passwords did not match");
                }

                // ❌ USERNAME EXISTS
                if (expected.Contains("already exists"))
                {
                    var error = wait.Until(d =>
                    {
                        var el = d.FindElements(By.CssSelector(".error"));
                        return el.Count > 0 ? el[0].Text.ToLower() : null;
                    });

                    return error.Contains("already exists");
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
            if (driver.PageSource.ToLower().Contains("your account was created successfully"))
                return "Đăng ký thành công";

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
