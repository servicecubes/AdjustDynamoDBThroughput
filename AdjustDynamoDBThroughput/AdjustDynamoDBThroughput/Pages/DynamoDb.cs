using System;
using System.Configuration;
using System.Threading;
using AdjustDynamoDBThroughput.Custom;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace AdjustDynamoDBThroughput.Pages
{
    public class DynamoDb
    {
        private RemoteWebDriver driver;
        private readonly string _url = ConfigurationManager.AppSettings["dynamoDbUrl"];
        private readonly string _organizationIdentifier = ConfigurationManager.AppSettings["organizationIdentifier"];

        public DynamoDb(RemoteWebDriver browser)
        {
            this.driver = browser;
            PageFactory.InitElements(browser, this);
        }

        [FindsBy(How = How.Id, Using = "ddbv2-shortcut-dashboard-link")]
        public IWebElement DashboardLinkElement { get; set; }

        [FindsBy(How = How.Id, Using = "ddbv2-shortcut-tables-link")]
        public IWebElement TablesLinkElement { get; set; }

        [FindsBy(How = How.Id, Using = "ddbv2-tables-filter-tables-text-field")]
        public IWebElement TablesFilterByTableNameElement { get; set; }

        [FindsBy(How = How.Id, Using = "ddbv2-tabs-capacity")]
        public IWebElement TableCapacityTabElement { get; set; }

        [FindsBy(How = How.Id, Using = "ddbv2-capacitytab-save-button")]
        public IWebElement CapacitySaveButtonElement { get; set; }

        [FindsBy(How = How.Id, Using = "ddbv2-tables-hide-table-details-button")]
        public IWebElement CapacityCloseButtonElement { get; set; }

        public void Navigate()
        {
            this.driver.Navigate().GoToUrl(_url);
        }

        public void GoToTables()
        {
            this.TablesLinkElement.Click();
        }

        public void GoToDashboard()
        {
            this.DashboardLinkElement.Click();
        }

        public void FilterByTableName()
        {
            this.TablesFilterByTableNameElement.Clear();
            this.TablesFilterByTableNameElement.SendKeys(_organizationIdentifier);
            Thread.Sleep(new TimeSpan(0, 0, 0, 3));
            var elementCountOfActiveTables = GetElementCountOfActiveTables();
            Assert.True(elementCountOfActiveTables > 19, "Not all tables are loaded properly after filtering with " + _organizationIdentifier + ".");
        }

        public void UpgradeAllThroughPuts()
        {
            var elementCounter = 1;
            CustomFunctions cstm = new CustomFunctions();
            var fileName = @"D:\Throughputs.xlsx";
            ExcelDataReader edr = new ExcelDataReader();
            string[] tableName = edr.ReadFromExcel(fileName, 0);            // 0 = 1st column in the excel file.
            string[] readCapacity = edr.ReadFromExcel(fileName, 1);         // 1 = 2nd column in the excel file.
            string[] writeCapacity = edr.ReadFromExcel(fileName, 2);        // 2 = 3rd column in the excel file.    
            string[] isTable = edr.ReadFromExcel(fileName, 3);              // 4 = 4th column in the excel file.
            int[] groupId = new int[41];
            groupId[1] = 1;
            int g = 0;

            for (int i = 0; i < 41; i++)
            {
                if (isTable[i] == "Yes")
                {
                    g++;
                    groupId[i] = g;
                }
                groupId[i] = g;
            }

            for (int x = 0; x < 40;)
            {
                if (x == 0)
                {
                    x++;
                }
                if (isTable[x] == "Yes")
                {
                    var groupIdOfTable = groupId[x];
                    var tableToOpen = _organizationIdentifier + "_" + tableName[x];
                    cstm.WhenIWaitForTextToAppear(driver, tableToOpen);
                    this.driver.FindElement(By.LinkText(tableToOpen)).Click();
                    cstm.WhenIWaitForTextToAppear(driver, "Close");
                    this.TableCapacityTabElement.Click();

                    while (groupId[x] == groupIdOfTable)
                    {
                        var readElement =
                            this.driver.FindElementByXPath(
                                "(//table//input[@id='ddbv2-shared-provisionthroughput-read'])[" + elementCounter + "]");

                        var writeElement =
                            this.driver.FindElementByXPath(
                                "(//table//input[@id='ddbv2-shared-provisionthroughput-write'])[" + elementCounter + "]");

                        readElement.Clear();
                        readElement.SendKeys(readCapacity[x]);

                        writeElement.Clear();
                        writeElement.SendKeys(writeCapacity[x]);

                        elementCounter++;
                        x++;
                        if (x == 40)
                        {
                            break;
                        }
                    }
                    elementCounter = 1;
                    this.CapacitySaveButtonElement.Click();
                    this.CapacityCloseButtonElement.Click();
                    WhenIWaitForLessThanFourUpdates(driver);
                }
            }
        }

        public void WhenIWaitForLessThanFourUpdates(RemoteWebDriver driver)
        {
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, 60));
            wait.Until(
                (d) =>
                    GetElementCountOfUpdatingTables() < 4);
        }

        public int GetElementCountOfActiveTables()
        {
            try
            {
                return this.driver.FindElements(By.XPath("//body//td//span[contains(text(),'Active')]")).Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetElementCountOfUpdatingTables()
        {
            try
            {
                return this.driver.FindElements(By.XPath("//body//td//span[contains(text(),'Updating')]")).Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
