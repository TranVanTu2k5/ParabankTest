using OpenQA.Selenium;

namespace PARABANKTest.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // 🔥 Locator lấy từ web Parabank
        private By txtUsername = By.Name("username");
        private By txtPassword = By.Name("password");
        private By btnLogin = By.CssSelector("input[value='Log In']");

        // 🔥 Actions
        public void EnterUsername(string username)
        {
            driver.FindElement(txtUsername).Clear();
            driver.FindElement(txtUsername).SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            driver.FindElement(txtPassword).Clear();
            driver.FindElement(txtPassword).SendKeys(password);
        }

        public void ClickLogin()
        {
            driver.FindElement(btnLogin).Click();
        }
    }
}