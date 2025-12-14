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

        private By InputUsuario = By.Name("username");
        private By InputPassword = By.Name("password");
        private By BotonIngresar = By.CssSelector("button.login-btn");

        // Esto confirma que el login fue exitoso.
        private By ElementoDelDashboard = By.TagName("app-sidebar");

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

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

            // Limpieza de sesion previa
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.localStorage.clear();");
            ((IJavaScriptExecutor)_driver).ExecuteScript("window.sessionStorage.clear();");
            _driver.Manage().Cookies.DeleteAllCookies();
            
            // Recargar la página para aplicar la limpieza
            _driver.Navigate().Refresh(); 

            IngresarCredenciales("AdminPedro", "password123");
            ClickIngresar();

            try
            {
                _wait.Until(d => !d.Url.Contains("/login"));
                _wait.Until(ExpectedConditions.ElementIsVisible(ElementoDelDashboard));
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception("El login falló o tardó demasiado. No se detectó la redirección al Dashboard.");
            }
        }

        public void NavegarDesdeMenu(string nombrePagina)
        {
            try
            {
                _wait.Until(ExpectedConditions.ElementIsVisible(ElementoDelDashboard));

                var xpathLink = $"//app-sidebar//span[contains(@class, 'nav-text') and contains(text(), '{nombrePagina}')]";
                
                var link = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathLink)));
                link.Click();

                Thread.Sleep(500); 
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception($"No se pudo navegar a '{nombrePagina}'. Verifica que el menú lateral esté visible y el texto del enlace sea correcto. XPath usado: //app-sidebar//span[contains(text(), '{nombrePagina}')]");
            }
        }
    }
}