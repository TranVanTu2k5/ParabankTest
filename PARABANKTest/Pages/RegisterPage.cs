using OpenQA.Selenium;

namespace PARABANKTest.Pages
{
    public class RegisterPage
    {
        private IWebDriver driver;

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // ===== LOCATOR =====
        private By linkRegister = By.LinkText("Register");

        private By firstName = By.Id("customer.firstName");
        private By lastName = By.Id("customer.lastName");
        private By address = By.Id("customer.address.street");
        private By city = By.Id("customer.address.city");
        private By state = By.Id("customer.address.state");
        private By zipCode = By.Id("customer.address.zipCode");
        private By phone = By.Id("customer.phoneNumber");
        private By ssn = By.Id("customer.ssn");

        private By username = By.Id("customer.username");
        private By password = By.Id("customer.password");
        private By confirmPassword = By.Id("repeatedPassword");

        private By btnRegister = By.CssSelector("input[value='Register']");

        // ===== ACTION =====
        public void GoToRegister()
        {
            driver.FindElement(linkRegister).Click();
        }

        public void EnterFirstName(string value) => driver.FindElement(firstName).SendKeys(value);
        public void EnterLastName(string value) => driver.FindElement(lastName).SendKeys(value);
        public void EnterAddress(string value) => driver.FindElement(address).SendKeys(value);
        public void EnterCity(string value) => driver.FindElement(city).SendKeys(value);
        public void EnterState(string value) => driver.FindElement(state).SendKeys(value);
        public void EnterZip(string value) => driver.FindElement(zipCode).SendKeys(value);
        public void EnterPhone(string value) => driver.FindElement(phone).SendKeys(value);
        public void EnterSSN(string value) => driver.FindElement(ssn).SendKeys(value);
        public void EnterUsername(string value) => driver.FindElement(username).SendKeys(value);
        public void EnterPassword(string value) => driver.FindElement(password).SendKeys(value);
        public void EnterConfirmPassword(string value) => driver.FindElement(confirmPassword).SendKeys(value);

        public void ClickRegister()
        {
            driver.FindElement(btnRegister).Click();
        }
    }
}
