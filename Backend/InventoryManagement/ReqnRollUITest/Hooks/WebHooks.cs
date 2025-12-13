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
            var options = new ChromeOptions();

            //Desactiva la advertencia específica de "Password found in a data breach"
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);

            //Argumento adicional para forzar la desactivación de la funcionalidad
            options.AddArgument("--disable-features=PasswordLeakDetection");

            //(Opcional pero recomendado) Desactivar el gestor de contraseñas completamente
            // para evitar el popup de "¿Quieres guardar la contraseña?"
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            // Pasamos las opciones al constructor
            var driver = new ChromeDriver(options);
            
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