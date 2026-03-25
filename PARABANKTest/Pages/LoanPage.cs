using OpenQA.Selenium;

namespace PARABANKTest.Pages
{
    public class LoanPage
    {
        private IWebDriver driver;

        public LoanPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // ===== LOCATOR =====
        private By linkRequestLoan = By.LinkText("Request Loan");

        private By txtAmount = By.Id("amount");
        private By txtDownPayment = By.Id("downPayment");
        private By btnApply = By.CssSelector("input[value='Apply Now']");

        // ===== ACTION =====
        public void GoToLoan()
        {
            driver.FindElement(linkRequestLoan).Click();
        }

        public void EnterAmount(string value)
        {
            var el = driver.FindElement(txtAmount);
            el.Clear();
            el.SendKeys(value);
        }

        public void EnterDownPayment(string value)
        {
            var el = driver.FindElement(txtDownPayment);
            el.Clear();
            el.SendKeys(value);
        }

        public void ClickApply()
        {
            driver.FindElement(btnApply).Click();
        }

        public void Refresh()
        {
            driver.Navigate().Refresh();
        }
    }
}