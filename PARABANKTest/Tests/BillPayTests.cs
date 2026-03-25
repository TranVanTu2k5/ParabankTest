using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PARABANKTest.Pages;
using PARABANKTest.Utilities;
using System;
using System.Linq;
using System.Threading;

namespace PARABANKTest.Tests
{
    public class BillPayTests
    {
        // Thêm null! để hết sạch gạch đỏ cảnh báo
        IWebDriver driver = null!;
        LoginPage loginPage = null!;
        BillPayPage billPayPage = null!;

        [Test, TestCaseSource(typeof(ExcelDataProvider), "GetAllTestCases")]
        public void Test_BillPay(ExcelDataProvider.TestCase tc)
        {
            if (string.IsNullOrEmpty(tc.TestCaseId) ||
    !           tc.TestCaseId.ToLower().Contains("bill"))
            {
                Assert.Ignore("Không phải test billpay");
            }

            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            loginPage = new LoginPage(driver);
            billPayPage = new BillPayPage(driver);

            TestContext.WriteLine("\n===== RUN TC: " + tc.TestCaseId + " =====");

            string actual = "";
            string status = "Passed";
            string imagePath = "";

            try
            {
                foreach (var step in tc.Steps)
                {
                    ExecuteStep(step);
                }

                bool isPass = VerifyBillResult(tc);

                if (!isPass)
                    throw new Exception("Kết quả không đúng với mong đợi");

                actual = GetActualBillResult();

                TestContext.WriteLine("PASS: " + tc.TestCaseId);
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

                TestContext.WriteLine("FAIL: " + tc.TestCaseId);
            }

            Thread.Sleep(2500);

            ExcelDataProvider.WriteResult(
                tc.SheetName,
                tc.TestCaseId,
                actual,
                status,
                imagePath
            );
        }

        private void ExecuteStep(ExcelDataProvider.TestStep step)
        {
            if (step == null || string.IsNullOrEmpty(step.Action)) return;
            string action = step.Action.ToLower();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // LOGIC LOGIN
            if (action.Contains("vào trang login"))
            {
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
            }
            else if (action.Contains("username")) loginPage.EnterUsername(step.Data ?? "");
            else if (action.Contains("password")) loginPage.EnterPassword(step.Data ?? "");
            else if (action.Contains("click login"))
            {
                loginPage.ClickLogin();
                // Đợi link Bill Pay hiện ra rồi Click bằng JS cho nhanh và né popup
                var link = wait.Until(d => d.FindElement(By.LinkText("Bill Pay")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", link);
            }


            if (action.Contains("payee name"))
                billPayPage.EnterPayeeName(step.Data ?? "");

            else if (action.Contains("address"))
                billPayPage.EnterAddress(step.Data ?? "");

            else if (action.Contains("city"))
                billPayPage.EnterCity(step.Data ?? "");

            else if (action.Contains("state"))
                billPayPage.EnterState(step.Data ?? "");

            else if (action.Contains("zip code"))
                billPayPage.EnterZip(step.Data ?? "");

            else if (action.Contains("phone"))
                billPayPage.EnterPhone(step.Data ?? "");

            // ⚠️ QUAN TRỌNG
            else if (action.Contains("verify account"))
                billPayPage.EnterVerifyAccount(step.Data ?? "");

            else if (action.Contains("account"))
                billPayPage.EnterAccount(step.Data ?? "");

            else if (action.Contains("amount"))
                billPayPage.EnterAmount(step.Data ?? "");

            else if (action.Contains("send payment"))
                billPayPage.ClickSendPayment();
        }

        private bool VerifyBillResult(ExcelDataProvider.TestCase tc)
        {
            string expected = tc.Steps
                .Where(s => !string.IsNullOrEmpty(s.Expected))
                .LastOrDefault()?.Expected?.ToLower();

            if (string.IsNullOrEmpty(expected))
                return false;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                wait.Until(d =>
                    d.PageSource.ToLower().Contains("bill payment complete") ||
                    d.PageSource.ToLower().Contains("required")
                );

                // SUCCESS
                if (expected.Contains("bill payment complete"))
                    return driver.PageSource.ToLower().Contains("bill payment complete");

                // ERROR REQUIRED
                if (expected.Contains("payee name is required"))
                    return CheckError("payee name is required");

                if (expected.Contains("address is required"))
                    return CheckError("address is required");

                if (expected.Contains("city is required"))
                    return CheckError("city is required");

                if (expected.Contains("state is required"))
                    return CheckError("state is required");

                if (expected.Contains("zip code is required"))
                    return CheckError("zip code is required");

                if (expected.Contains("phone is required"))
                    return CheckError("phone is required");

                if (expected.Contains("account is required"))
                    return CheckError("account is required");

                if (expected.Contains("verify account is required"))
                    return CheckError("verify account is required");

                if (expected.Contains("amount is required"))
                    return CheckError("amount is required");

                return false;
            }
            catch
            {
                return false;
            }
        }
        private string GetActualBillResult()
        {
            try
            {
                return driver.FindElement(By.XPath("//h1")).Text;
            }
            catch
            {
                return "Không lấy được kết quả";
            }
        }
        private bool CheckError(string keyword)
        {
            var errors = driver.FindElements(By.XPath("//*[contains(@class,'error')]"));

            return errors.Any(e =>
                e.Text.ToLower().Contains(keyword.ToLower().Trim())
            );
        }
        [TearDown]
        public void Cleanup()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null!;
            }
        }
    }
}