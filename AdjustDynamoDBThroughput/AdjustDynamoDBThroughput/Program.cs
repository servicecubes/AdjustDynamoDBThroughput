using System;
using System.Threading;
using AdjustDynamoDBThroughput.Custom;
using AdjustDynamoDBThroughput.Pages;
using OpenQA.Selenium.Chrome;

namespace AdjustDynamoDBThroughput
{
    class Program
    {
        public CustomFunctions CstmCustomFunctions = new CustomFunctions();

        static void Main(string[] args)
        {
            CustomFunctions cstm = new CustomFunctions();
            ChromeDriver driver = new ChromeDriver();

            //DynamoDb dd = new DynamoDb(driver);
            //dd.UpgradeAllThroughPuts();


            var loginPage = new LoginPage(driver);
            loginPage.Navigate();
            cstm.WhenIWaitForTextToAppear(driver, "Sign-in using root account credentials");
            loginPage.Login();

            Console.WriteLine("If you have MFA enabled, please manually enter the MFA code and enter the AWS Console.");
            cstm.WhenIWaitForTextToAppear(driver, "Build a web app");

            var dynamoDbPage = new DynamoDb(driver);
            dynamoDbPage.Navigate();
            cstm.WhenIWaitForTextToAppear(driver, "Amazon DynamoDB is a fully managed non-relational database service that provides fast and predictable performance with seamless scalability.");

            dynamoDbPage.GoToTables();
            cstm.WhenIWaitForTextToAppear(driver, "Total write capacity");
            dynamoDbPage.FilterByTableName();
            dynamoDbPage.UpgradeAllThroughPuts();
        }
    }
}
