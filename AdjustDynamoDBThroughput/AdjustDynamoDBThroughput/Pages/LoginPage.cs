using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;

namespace AdjustDynamoDBThroughput.Pages
{
    public class LoginPage
    {
        private readonly string _url = ConfigurationManager.AppSettings["homePage"];
        private readonly string _userNameValue = ConfigurationManager.AppSettings["userName"];
        private readonly string _passwordValue = ConfigurationManager.AppSettings["userPass"];
private readonly string _accountValue = ConfigurationManager.AppSettings["account"];

        private readonly RemoteWebDriver driver;

        public LoginPage(RemoteWebDriver browser)
        {
            this.driver = browser;
            PageFactory.InitElements(browser, this);
        }

        [FindsBy(How = How.Id, Using = "account")]
        public IWebElement Account { get; set; }

        [FindsBy(How = How.Id, Using = "username")]
        public IWebElement UserName { get; set; }

        [FindsBy(How = How.Id, Using = "password")]
        public IWebElement Password { get; set; }

        [FindsBy(How = How.Id, Using = "signin_button")]
        public IWebElement SignInButton { get; set; }

        public void Navigate()
        {
            this.driver.Navigate().GoToUrl(_url);
        }

        public void Login()
        {
            this.Account.Clear();
            this.Account.SendKeys(_accountValue);
            this.UserName.Clear();
            this.UserName.SendKeys(_userNameValue);
            this.Password.Clear();
            this.Password.SendKeys(_passwordValue);
            this.SignInButton.Click();
        }
    }
}
