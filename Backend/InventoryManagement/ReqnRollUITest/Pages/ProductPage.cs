using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // URL Base
        private const string PageUrl = "http://localhost:4200/products";

        // --- LOCALIZADORES DE LA PÁGINA PRINCIPAL ---
        private By BotonAgregarProducto = By.XPath("//button[contains(text(), '+ Agregar Producto')]");
        private By BotonAgregarCategoria = By.XPath("//button[contains(text(), '+ Agregar Categoría')]");

        // --- LOCALIZADORES DEL FORMULARIO PRODUCTO ---
        private By ModalProductoTitulo = By.XPath("//div[@class='form-card']/h3[contains(text(), 'Producto')]");
        private By InputNombreProd = By.Name("name"); // Basado en name="name" del HTML
        private By InputDescripcionProd = By.Name("description");
        private By InputSerial = By.Name("serialCode");
        private By InputStock = By.Name("totalStock");
        private By BotonGuardarProd = By.CssSelector("button.add[type='submit']");

        // Dropdown de Categoría (Custom Select)
        private By DropdownCategoria = By.ClassName("custom-select");
        private By OpcionCategoria(string nombreCat) => By.XPath($"//div[@class='dropdown-option' and contains(text(), '{nombreCat}')]");

        public ProductPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- ACCIONES ---
        public void Navegar()
        {
            _driver.Navigate().GoToUrl(PageUrl);
        }

        public void ClickAgregarProducto() => Click(BotonAgregarProducto);

        // Método puente para abrir el modal de categoría desde la página principal
        public void ClickAgregarCategoria() => Click(BotonAgregarCategoria);

        public void LlenarFormularioProducto(string nombre, string desc, string serial, string stock, string categoria)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(ModalProductoTitulo));

            Escribir(InputNombreProd, nombre);
            Escribir(InputDescripcionProd, desc);
            Escribir(InputSerial, serial);
            Escribir(InputStock, stock);

            // Selección de categoría
            Click(DropdownCategoria);
            Click(OpcionCategoria(categoria));
        }

        public void GuardarProducto() => Click(BotonGuardarProd);

        // --- HELPERS PRIVADOS ---
        private void Click(By locator) => _wait.Until(ExpectedConditions.ElementToBeClickable(locator)).Click();
        private void Escribir(By locator, string texto)
        {
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(locator));
            element.Clear();
            element.SendKeys(texto);
        }
    }
}