using Reqnroll;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace InventoryManagement.ReqnrollUITest.Hooks
{
    [Binding]
    public class WebHooks
    {
        // Reqnroll inyecta automáticamente el ScenarioContext
        private readonly ScenarioContext _scenarioContext;

        public WebHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void CreateWebDriver()
        {
            var driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            // GUARDAMOS el driver en el contexto con una clave "WebDriver"
            _scenarioContext["WebDriver"] = driver;
        }

        [AfterScenario]
        public void DestroyWebDriver()
        {
            if (_scenarioContext.ContainsKey("WebDriver"))
            {
                var driver = _scenarioContext["WebDriver"] as IWebDriver;
                driver?.Quit();
                driver?.Dispose();
            }
        }
    }
}