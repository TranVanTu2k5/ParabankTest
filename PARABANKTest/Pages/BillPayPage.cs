using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace PARABANKTest.Pages
{
    public class BillPayPage
    {
        private IWebDriver driver;

        public BillPayPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Hàm nhập "nhịp nhàng": dùng JS cho nhanh nhưng Sleep nhẹ để mắt người thấy
        private void FastInputWithPause(By locator, string value)
        {
            try
            {
                var el = driver.FindElement(locator);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].value = arguments[1];", el, value ?? "");
                // Nghỉ 150ms mỗi ô -> Tổng 10 ô khoảng 1.5 giây
                Thread.Sleep(150);
            }
            catch { }
        }

        public void GoToBillPay()
        {
            // Đợi trang load xong hoàn toàn mới bắt đầu đếm nhịp nhập liệu
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Name("payee.name")));
        }

        public void EnterPayeeName(string value) => FastInputWithPause(By.Name("payee.name"), value);
        public void EnterAddress(string value) => FastInputWithPause(By.Name("payee.address.street"), value);
        public void EnterCity(string value) => FastInputWithPause(By.Name("payee.address.city"), value);
        public void EnterState(string value) => FastInputWithPause(By.Name("payee.address.state"), value);
        public void EnterZip(string value) => FastInputWithPause(By.Name("payee.address.zipCode"), value);
        public void EnterPhone(string value) => FastInputWithPause(By.Name("payee.phoneNumber"), value);
        public void EnterAccount(string value) => FastInputWithPause(By.Name("payee.accountNumber"), value);
        public void EnterVerifyAccount(string value) => FastInputWithPause(By.Name("verifyAccount"), value);
        public void EnterAmount(string value) => FastInputWithPause(By.Name("amount"), value);

        public void ClickSendPayment()
        {
            var btn = driver.FindElement(By.CssSelector("input[value='Send Payment']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
        }

        public void Refresh() => driver.Navigate().Refresh();
    }
}
