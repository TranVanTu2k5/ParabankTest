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
    public class UpdateTests
    {
        IWebDriver driver;
        LoginPage loginPage;
        UpdateProfilePage updatePage;

        [Test, TestCaseSource(typeof(ExcelDataProvider), "GetAllTestCases")]
        public void Test_UpdateProfile(ExcelDataProvider.TestCase tc)
        {
            if (string.IsNullOrEmpty(tc.TestCaseId) ||
                !tc.TestCaseId.ToLower().Contains("update"))
            {
                Assert.Ignore("Không phải test update");
            }

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            loginPage = new LoginPage(driver);
            updatePage = new UpdateProfilePage(driver);

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

                bool isPass = VerifyUpdateResult(tc);

                if (!isPass)
                    throw new Exception("Kết quả không đúng với mong đợi");

                actual = GetActualResult();

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
            if (step == null || string.IsNullOrEmpty(step.Action))
                return;

            string action = step.Action.ToLower();

            if (action.Contains("vào trang login"))
            {
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
                Thread.Sleep(1500);
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
            else if (action.Contains("click login"))
            {
                loginPage.ClickLogin();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.PageSource.ToLower().Contains("welcome"));
            }
            else if (action.Contains("update contact info"))
            {
                updatePage.GoToUpdate();

            }
            else if (action.Contains("first name"))
            {
                updatePage.EnterFirstName(step.Data ?? "");
            }
            else if (action.Contains("last name"))
            {
                updatePage.EnterLastName(step.Data ?? "");
            }
            else if (action.Contains("address"))
            {
                updatePage.EnterAddress(step.Data ?? "");
            }
            else if (action.Contains("city"))
            {
                updatePage.EnterCity(step.Data ?? "");
            }
            else if (action.Contains("state"))
            {
                updatePage.EnterState(step.Data ?? "");
            }
            else if (action.Contains("zip"))
            {
                updatePage.EnterZip(step.Data ?? "");
            }
            else if (action.Contains("phone"))
            {
                updatePage.EnterPhone(step.Data ?? "");
            }
            else if (action.Contains("update profile"))
            {
                updatePage.ClickUpdate();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.PageSource.ToLower().Contains("updated")
                             || d.PageSource.ToLower().Contains("error")
                             || d.PageSource.ToLower().Contains("required"));
            }
        }

        private bool VerifyUpdateResult(ExcelDataProvider.TestCase tc)
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
                    d.PageSource.ToLower().Contains("updated address") ||
                    d.PageSource.ToLower().Contains("required")
                );

                if (expected.Contains("updated address"))
                    return driver.PageSource.ToLower().Contains("updated address");

                if (expected.Contains("first name is required"))
                    return CheckError("first name is required");

                if (expected.Contains("last name is required"))
                    return CheckError("last name is required");

                if (expected.Contains("address is required"))
                    return CheckError("address is required");

                if (expected.Contains("city is required"))
                    return CheckError("city is required");

                if (expected.Contains("zip code is required"))
                    return CheckError("zip code is required");

                if (expected.Contains("phone is required"))
                    return CheckError("phone is required");

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool CheckError(string keyword)
        {
            var errors = driver.FindElements(By.XPath("//*[contains(@id,'errors')]"));
            return errors.Any(e => e.Text.ToLower().Contains(keyword));
        }

        private string GetActualResult()
        {
            var page = driver.PageSource.ToLower();

            if (page.Contains("updated address"))
                return "Update thành công";

            var errors = driver.FindElements(By.XPath("//*[contains(@id,'errors')]"));
            if (errors.Count > 0)
                return errors[0].Text;

            return "Không xác định";
        }

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