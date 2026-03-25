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
    public class LoanTests
    {
        IWebDriver driver;
        LoginPage loginPage;
        LoanPage loanPage;

        [Test, TestCaseSource(typeof(ExcelDataProvider), "GetAllTestCases")]
        public void Test_Loan(ExcelDataProvider.TestCase tc)
        {
            if (string.IsNullOrEmpty(tc.TestCaseId) ||
                !tc.TestCaseId.ToLower().Contains("loan"))
            {
                Assert.Ignore("Không phải test loan");
            }

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            loginPage = new LoginPage(driver);
            loanPage = new LoanPage(driver);

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

                bool isPass = VerifyLoanResult(tc);

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

            // ===== LOGIN =====
            if (action.Contains("vào trang login"))
            {
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
                Thread.Sleep(1000);
            }
            else if (action.Contains("username"))
            {
                loginPage.EnterUsername(step.Data ?? "");

            }
            else if (action.Contains("password"))
            {
                loginPage.EnterPassword(step.Data ?? "");

            }
            else if (action.Contains("click login"))
            {
                loginPage.ClickLogin();

            }

            // ===== LOAN =====
            else if (action.Contains("request loan"))
            {
                loanPage.GoToLoan();
            }
            else if (action.Contains("amount"))
            {
                loanPage.EnterAmount(step.Data ?? "");
            }
            else if (action.Contains("down payment"))
            {
                loanPage.EnterDownPayment(step.Data ?? "");
            }
            else if (action.Contains("click apply"))
            {
                loanPage.ClickApply();
                Thread.Sleep(1500);
            }
            else if (action.Contains("f5"))
            {
                loanPage.Refresh();
                Thread.Sleep(1500);
            }
        }

        // ===== VERIFY =====
        private bool VerifyLoanResult(ExcelDataProvider.TestCase tc)
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
                if (expected.Contains("approved"))
                {
                    wait.Until(d => d.PageSource.ToLower().Contains("congratulations"));
                    return true;
                }

                // ❌ INTERNAL ERROR
                if (expected.Contains("internal error"))
                {
                    wait.Until(d => d.PageSource.ToLower().Contains("internal error"));
                    return true;
                }

                // ❌ AMOUNT INVALID
                if (expected.Contains("cannot grant"))
                {
                    wait.Until(d => d.PageSource.ToLower().Contains("cannot grant"));
                    return true;
                }

                // ❌ DOWN PAYMENT
                if (expected.Contains("sufficient funds"))
                {
                    wait.Until(d => d.PageSource.ToLower().Contains("sufficient funds"));
                    return true;
                }

                // 🔄 RESET FORM (F5)
                if (expected.Contains("form được xóa"))
                {
                    var amount = driver.FindElement(By.Id("amount")).GetAttribute("value");
                    var down = driver.FindElement(By.Id("downPayment")).GetAttribute("value");

                    return string.IsNullOrEmpty(amount) && string.IsNullOrEmpty(down);
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
            string page = driver.PageSource.ToLower();

            if (page.Contains("congratulations"))
                return "Loan approved";

            if (page.Contains("internal error"))
                return "Internal error";

            if (page.Contains("cannot grant"))
                return "Invalid amount";

            if (page.Contains("sufficient funds"))
                return "Down payment error";

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