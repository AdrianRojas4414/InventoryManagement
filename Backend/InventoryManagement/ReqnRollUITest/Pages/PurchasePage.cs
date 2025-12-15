using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class PurchasePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private By BotonAgregarCompra = By.XPath("//button[contains(text(), '+ Agregar Compra')]");
        private By ModalFormCard = By.CssSelector(".form-card");
        private By CustomSelectProveedor = By.CssSelector(".supplier-row .custom-select");
        private By DropdownOptionProveedor = By.CssSelector(".supplier-row .dropdown-option");
        private By BotonAgregarProducto = By.CssSelector(".btn-add-product");
        private By CustomSelectProducto = By.CssSelector(".custom-select.small");
        private By DropdownOptionProducto = By.CssSelector(".dropdown-option");
        private By InputCantidad = By.CssSelector("input[formControlName='quantity']");
        private By InputPrecio = By.CssSelector("input[formControlName='unitPrice']");
        private By BotonRegistrarCompra = By.XPath("//button[contains(text(), 'Registrar Compra')]");
        private By TablaCompras = By.CssSelector("table.product-table");
        private By FilasTabla = By.CssSelector("table.product-table tbody tr.main-row");

        public PurchasePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        public void ClickAgregarCompra()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(BotonAgregarCompra)).Click();
            Thread.Sleep(500);
        }

        public void EsperarQueElModalEsteAbierto()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(ModalFormCard));
        }

        public void SeleccionarPrimerProveedor()
        {
            var selectProveedor = _wait.Until(ExpectedConditions.ElementToBeClickable(CustomSelectProveedor));
            selectProveedor.Click();
            Thread.Sleep(300);

            var opcionProveedor = _wait.Until(ExpectedConditions.ElementToBeClickable(DropdownOptionProveedor));
            opcionProveedor.Click();
        }

        public void AgregarProducto(string nombreProducto, int cantidad, decimal precioUnitario)
        {
            var botonAgregar = _wait.Until(ExpectedConditions.ElementToBeClickable(BotonAgregarProducto));
            botonAgregar.Click();
            Thread.Sleep(500);

            var selectsProducto = _driver.FindElements(CustomSelectProducto);
            var ultimoSelect = selectsProducto.Last();
            ultimoSelect.Click();
            Thread.Sleep(300);

            var opciones = _driver.FindElements(DropdownOptionProducto);
            var opcionProducto = opciones.FirstOrDefault(o => o.Text.Contains(nombreProducto));
            if (opcionProducto != null)
            {
                opcionProducto.Click();
            }

            Thread.Sleep(300);

            var inputsCantidad = _driver.FindElements(InputCantidad);
            var ultimaCantidad = inputsCantidad.Last();
            ultimaCantidad.Clear();
            ultimaCantidad.SendKeys(cantidad.ToString());

            var inputsPrice = _driver.FindElements(InputPrecio);
            var ultimoPrecio = inputsPrice.Last();
            ultimoPrecio.Clear();
            ultimoPrecio.SendKeys(precioUnitario.ToString("F2"));

            Thread.Sleep(300);
        }

        public void ClickRegistrarCompra()
        {
            var boton = _wait.Until(ExpectedConditions.ElementToBeClickable(BotonRegistrarCompra));
            boton.Click();
        }

        public bool EsperarQueElModalSeCierre()
        {
            try
            {
                return _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ModalFormCard));
            }
            catch
            {
                return false;
            }
        }

        public bool VerificarCompraEnTabla(string totalEsperado)
        {
            Thread.Sleep(2000);
            try
            {
                var filas = _driver.FindElements(FilasTabla);
                return filas.Any(f => f.Text.Contains(totalEsperado));
            }
            catch
            {
                return false;
            }
        }
    }
}