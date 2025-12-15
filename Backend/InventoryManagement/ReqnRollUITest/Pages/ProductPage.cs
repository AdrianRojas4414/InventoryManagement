using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private By BotonAgregarProducto = By.XPath("//button[contains(text(), '+ Agregar Producto')]");
        private By ModalFormCard = By.CssSelector("app-product-form .form-card");
        private By InputNombreProd = By.CssSelector("app-product-form input[name='name']");
        private By InputDescripcionProd = By.CssSelector("app-product-form input[name='description']");
        private By InputSerialCodeProd = By.CssSelector("app-product-form input[name='serialCode']");
        private By InputStockProd = By.CssSelector("app-product-form input[name='totalStock']");
        private By BotonGuardarForm = By.CssSelector("app-product-form button[type='submit']");
        private By MensajesDeErrorField = By.CssSelector(".error p");
        private By MensajeErrorGlobal = By.CssSelector(".error-message");
        private By MensajeExito = By.CssSelector(".success-message");
        private By TablaCategorias = By.CssSelector("app-product-table table.product-table");
        private By FilasTabla = By.CssSelector("app-product-table table.product-table tbody tr");
        private By BotonConfirmarModal = By.XPath("//app-confirm-modal//button[contains(@class, 'add')]");

        public ProductPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void ClickAgregarProducto() => _wait.Until(ExpectedConditions.ElementToBeClickable(BotonAgregarProducto)).Click();
        public void EsperarQueElModalEsteAbierto() => _wait.Until(ExpectedConditions.ElementIsVisible(ModalFormCard));

        public void LlenarNombre(string nombre)
        {
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(InputNombreProd));
            input.Clear();
            if (!string.IsNullOrEmpty(nombre) && nombre != "[VACIO]") input.SendKeys(nombre);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarDescripcion(string descripcion)
        {
            var input = _driver.FindElement(InputDescripcionProd);
            input.Clear();
            if (!string.IsNullOrEmpty(descripcion) && descripcion != "[VACIO]") input.SendKeys(descripcion);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarCodigoSerial(string codigoSerial)
        {
            var input = _driver.FindElement(InputSerialCodeProd);
            input.Clear();
            if (!string.IsNullOrEmpty(codigoSerial) && codigoSerial != "[VACIO]") input.SendKeys(codigoSerial);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarStock(string stock)
        {
            var input = _driver.FindElement(InputStockProd);
            input.Clear();
            if (!string.IsNullOrEmpty(stock) && stock != "[VACIO]") input.SendKeys(stock);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void ClickBotonFormulario()
        {
            var btn = _wait.Until(ExpectedConditions.ElementExists(BotonGuardarForm));
            if (btn.Enabled) btn.Click();
        }

        public void ClickOpcionEnTabla(string nombreProducto, string opcion)
        {
            AbrirMenuOpciones(nombreProducto);
            string textoBoton = opcion.Equals("Eliminar", StringComparison.OrdinalIgnoreCase) ? "Deshabilitar" : opcion;

            var xpathBoton = $"//tr[td[contains(text(), '{nombreProducto}')]]//div[@class='options-menu']//button[contains(text(), '{textoBoton}')]";
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathBoton))).Click();
        }

        private void AbrirMenuOpciones(string nombreProducto)
        {
            var xpathIcono = $"//tr[td[contains(text(), '{nombreProducto}')]]//span[contains(@class, 'options-icon')]";
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathIcono))).Click();
        }

        public void ClickConfirmarEliminacion()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(BotonConfirmarModal)).Click();
            Thread.Sleep(500);
        }

        public bool EsperarQueElModalSeCierre()
        {
            try { return _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ModalFormCard)); } catch { return false; }
        }

        public bool ExisteMensajeEnPantalla(string mensajeEsperado)
        {
            try
            {
                var errores = _driver.FindElements(MensajesDeErrorField).Concat(_driver.FindElements(MensajeExito)).Concat(_driver.FindElements(MensajeErrorGlobal));
                return errores.Any(e => e.Displayed && e.Text.Contains(mensajeEsperado));
            }
            catch { return false; }
        }


        public bool ExisteProductoEnTabla(string nombreProducto)
        {
            try
            {
                var xpath = $"//table//td[contains(normalize-space(text()), '{nombreProducto}')]";
                _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                return true;
            }
            catch { return false; }
        }

        public bool VerificarDescripcionProducto(string nombreProducto, string descripcionEsperada)
        {
            try
            {
                var xpathDescripcion = $"//tr[td[contains(normalize-space(text()), '{nombreProducto}')]]//td[3]";

                var celdaDescripcion = _driver.FindElement(By.XPath(xpathDescripcion));
                string textoActual = celdaDescripcion.Text.Trim();

                return textoActual.Contains(descripcionEsperada);
            }
            catch
            {
                return false;
            }
        }

        public bool VerificarStockProducto(string nombreProducto, string stockEsperado)
        {
            try
            {
                var xpathDescripcion = $"//tr[td[contains(normalize-space(text()), '{nombreProducto}')]]//td[4]";

                var celdaDescripcion = _driver.FindElement(By.XPath(xpathDescripcion));
                string textoActual = celdaDescripcion.Text.Trim();

                return textoActual.Contains(stockEsperado);
            }
            catch
            {
                return false;
            }
        }

        public bool VerificarEstadoProducto(string nombreProducto, string estadoEsperado)
        {
            try
            {
                var xpathEstado = $"//tr[td[contains(text(), '{nombreProducto}')]]//td[5]/span";
                var estadoElemento = _driver.FindElement(By.XPath(xpathEstado));
                return estadoElemento.Text.Trim().ToUpper().Contains(estadoEsperado.ToUpper());
            }
            catch { return false; }
        }

        public bool ExisteTabla() => _driver.FindElements(TablaCategorias).Count > 0;
        public int ContarFilas() => _driver.FindElements(FilasTabla).Count;

        public bool VerificarEnlacesEnFilas(string opcion1, string opcion2)
        {
            var filas = _driver.FindElements(FilasTabla);
            if (filas.Count == 0) return false;
            try
            {
                var icono = filas[0].FindElement(By.CssSelector(".options-icon"));
                icono.Click();
                var menu = filas[0].FindElement(By.CssSelector(".options-menu"));
                string txt = menu.Text;
                if (opcion2 == "Eliminar") opcion2 = "Deshabilitar";
                icono.Click();
                return txt.Contains(opcion1) && txt.Contains(opcion2);
            }
            catch { return false; }
        }
    }
}