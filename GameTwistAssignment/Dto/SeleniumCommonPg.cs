using OpenQA.Selenium;


namespace GameTwistAssignment.Dto
{
    public class SeleniumCommonPg
    {
        private SeleniumContext seleniumContext;
        public SeleniumCommonPg(SeleniumContext _seleniumContext)
        {
            seleniumContext = _seleniumContext;
        }

        public void NavigateToUrl(string url)
        {
            seleniumContext.WebDriver.Navigate().GoToUrl(url);
        }
        public void ClickElementByCss(string css)
        {
            seleniumContext.WebDriver.FindElement(By.CssSelector(css)).Click();
        }
        public void ClickElementByXpath(string xpath)
        {
            seleniumContext.WebDriver.FindElement(By.XPath(xpath)).Click();
        }
        public void InputTextById(string id, string sendKeys)
        {
            seleniumContext.WebDriver.FindElement(By.Id(id)).SendKeys(sendKeys);
        }
        public void ClickElementById(string id)
        {
            seleniumContext.WebDriver.FindElement(By.Id(id)).Click();
        }
        public string GetTextById(string id)
        {
           return seleniumContext.WebDriver.FindElement(By.Id(id)).Text;
        }
    }
}
