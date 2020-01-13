using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Github2WandboxSeleniumTests
{
    public class SeleniumTest : IDisposable
    {
        static readonly string BaseUrl = "https://localhost:5001/";

        public IWebDriver Driver { get; private set; }

        public SeleniumTest()
        {
            Driver = new ChromeDriver();
        }

        public void Dispose()
        {
            Driver.Quit();
        }

        [Fact]
        public void should_generate_link_when_form_filled()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Driver.FindElement(By.Id("owner")).SendKeys("Cubix651");
            Driver.FindElement(By.Id("repository")).SendKeys("github2wandbox-testrepo");
            Driver.FindElement(By.Id("main_path")).SendKeys("singlefile-examples/a.cpp");
            Driver.FindElement(By.CssSelector("button:nth-child(1)")).Click();
            Driver.FindElement(By.CssSelector("input")).Click();
            string expectedGeneratedLink = BaseUrl + "Publish/Cubix651/github2wandbox-testrepo/singlefile-examples/a.cpp?compiler_standard=c%2B%2B2a";
            Assert.Equal(Driver.FindElement(By.LinkText(expectedGeneratedLink)).Text, expectedGeneratedLink);
            {
                string value = Driver.FindElement(By.CssSelector("input")).GetAttribute("value");
                Assert.Equal(expectedGeneratedLink, value);
            }
        }
    }
}
