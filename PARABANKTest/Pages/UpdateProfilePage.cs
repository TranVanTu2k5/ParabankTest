using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PARABANKTest.Pages
{
    public class UpdateProfilePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public UpdateProfilePage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ===== LOCATOR =====
        private By linkUpdate = By.LinkText("Update Contact Info");

        private By firstName = By.Id("customer.firstName");
        private By lastName = By.Id("customer.lastName");
        private By address = By.Id("customer.address.street");
        private By city = By.Id("customer.address.city");
        private By state = By.Id("customer.address.state");
        private By zipCode = By.Id("customer.address.zipCode");
        private By phone = By.Id("customer.phoneNumber");

        private By btnUpdate = By.CssSelector("input[value='Update Profile']");

        // ===== ACTION =====
        public void GoToUpdate()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(linkUpdate)).Click();

            // ❌ KHÔNG dùng VisibilityOfElementLocated (bị lỗi)
            // ✔ dùng cái này thay thế
            wait.Until(ExpectedConditions.ElementIsVisible(firstName));
        }

        public void EnterFirstName(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(firstName));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterLastName(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(lastName));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterAddress(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(address));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterCity(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(city));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterState(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(state));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterZip(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(zipCode));
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterPhone(string value)
        {
            var el = wait.Until(ExpectedConditions.ElementIsVisible(phone));
            el.Clear();
            el.SendKeys(value);
        }

        public void ClickUpdate()
        {
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(btnUpdate));

            ((IJavaScriptExecutor)driver)
                .ExecuteScript("arguments[0].scrollIntoView(true);", btn);

            System.Threading.Thread.Sleep(300);

            btn.Click();
        }
    }
}