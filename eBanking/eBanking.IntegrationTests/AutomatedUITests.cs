using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace eBanking.IntegrationTests
{
    public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        public AutomatedUITests()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
        }

        // Start application without debugging (CTRL + F5)
        
        [Fact]
        public void ExchangeRateIndexTest()
        {
            _driver.Navigate()
                .GoToUrl("http://liss.matf.bg.ac.rs:5000/ExchangeRate");
            Assert.Equal("Index - Currency Explorer", _driver.Title);
            Assert.Contains("Insert for which date you want the exchange rate list in format YYYY-MM-DD:", _driver.PageSource); _driver.Navigate();
        }
        
        [Fact]
        public void ExchangeRateGenerateListTest()
        {
            _driver.Navigate()
                .GoToUrl("http://liss.matf.bg.ac.rs:5000/ExchangeRate");
            _driver.FindElement(By.Id("Date")).SendKeys("2021-04-01");
            _driver.FindElement(By.Id("Load"))
                .Click();
            Assert.Contains("HTG", _driver.PageSource);
            Assert.Contains("94.236046", _driver.PageSource);
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
