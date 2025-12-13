using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private const string PageUrl = "http://localhost:4200/login";

        // --- LOCALIZADORES ---
        private By InputUsuario = By.Name("username");
        private By InputPassword = By.Name("password");
        private By BotonIngresar = By.CssSelector("button.login-btn");

        // Elemento que SOLO existe cuando ya iniciaste sesión (ej. el Sidebar o el Header)
        // Esto confirma que el login fue exitoso.
        private By ElementoDelDashboard = By.TagName("app-sidebar");

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- ACCIONES ---

        public void Navegar()
        {
            _driver.Navigate().GoToUrl(PageUrl);
        }

        public void IngresarCredenciales(string usuario, string password)
        {
            var txtUsuario = _wait.Until(ExpectedConditions.ElementIsVisible(InputUsuario));
            txtUsuario.Clear();
            txtUsuario.SendKeys(usuario);

            var txtPassword = _driver.FindElement(InputPassword);
            txtPassword.Clear();
            txtPassword.SendKeys(password);
        }

        public void ClickIngresar()
        {
            var btn = _wait.Until(ExpectedConditions.ElementToBeClickable(BotonIngresar));
            btn.Click();
        }

        public void LoginExitosoComoAdmin()
        {
            Navegar();
            // Asegúrate que estas credenciales son las correctas en tu BD local
            IngresarCredenciales("AdminPedro", "password123");
            ClickIngresar();

            // *** CORRECCIÓN CRÍTICA ***
            // Esperamos a que la URL ya NO contenga "login"
            // Esto asegura que Angular terminó de procesar el login y redirigió.
            try
            {
                _wait.Until(d => !d.Url.Contains("/login"));
            }
            catch (WebDriverTimeoutException)
            {
                // Si falla, es probable que las credenciales estén mal o el backend caído
                throw new Exception("El login falló o tardó demasiado. Seguimos en la página de login.");
            }
        }
    }
}