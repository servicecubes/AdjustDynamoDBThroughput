using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace AdjustDynamoDBThroughput.Custom
{
    public class CustomFunctions
    {
        public void WhenIWaitForTextToAppear(RemoteWebDriver driver, String paramText)
        {
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, 60));
            wait.Until(
                (d) =>
                    driver.FindElement(
                        By.XPath("//body//*[contains(text(),'" + paramText + "')]")));
        }
    }
}