using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class SupplierPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private By BotonAgregarProveedor = By.XPath("//button[contains(text(), '+ Agregar Proveedor')]");
        private By ModalFormCard = By.CssSelector("app-supplier-form .form-card");
        private By InputNombre = By.CssSelector("app-supplier-form input[name='name']");
        private By InputNit = By.CssSelector("app-supplier-form input[name='nit']");
        private By InputTelefono = By.CssSelector("app-supplier-form input[name='phoneNumber']");
        private By InputEmail = By.CssSelector("app-supplier-form input[name='email']");
        private By InputContacto = By.CssSelector("app-supplier-form input[name='contactName']");
        private By InputDireccion = By.CssSelector("app-supplier-form input[name='address']");
        private By BotonGuardarForm = By.CssSelector("app-supplier-form button[type='submit']");
        private By MensajesDeErrorField = By.CssSelector(".error p");
        private By MensajeErrorGlobal = By.CssSelector(".error-message");
        private By MensajeExito = By.CssSelector(".success-message");
        private By TablaProveedores = By.CssSelector("app-supplier-table table.product-table");
        private By FilasTabla = By.CssSelector("app-supplier-table table.product-table tbody tr");
        private By BotonConfirmarModal = By.XPath("//app-confirm-modal//button[contains(@class, 'add')]");

        public SupplierPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void ClickAgregarProveedor() => _wait.Until(ExpectedConditions.ElementToBeClickable(BotonAgregarProveedor)).Click();
        public void EsperarQueElModalEsteAbierto() => _wait.Until(ExpectedConditions.ElementIsVisible(ModalFormCard));

        public void LlenarNombre(string nombre)
        {
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(InputNombre));
            input.Clear();
            if (!string.IsNullOrEmpty(nombre) && nombre != "[VACIO]") input.SendKeys(nombre);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarNit(string nit)
        {
            var input = _driver.FindElement(InputNit);
            input.Clear();
            if (!string.IsNullOrEmpty(nit) && nit != "[VACIO]") input.SendKeys(nit);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarTelefono(string telefono)
        {
            var input = _driver.FindElement(InputTelefono);
            input.Clear();
            if (!string.IsNullOrEmpty(telefono) && telefono != "[VACIO]") input.SendKeys(telefono);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarEmail(string email)
        {
            var input = _driver.FindElement(InputEmail);
            input.Clear();
            if (!string.IsNullOrEmpty(email) && email != "[VACIO]") input.SendKeys(email);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarContacto(string contacto)
        {
            var input = _driver.FindElement(InputContacto);
            input.Clear();
            if (!string.IsNullOrEmpty(contacto) && contacto != "[VACIO]") input.SendKeys(contacto);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarDireccion(string direccion)
        {
            var input = _driver.FindElement(InputDireccion);
            input.Clear();
            if (!string.IsNullOrEmpty(direccion) && direccion != "[VACIO]") input.SendKeys(direccion);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void ClickBotonFormulario()
        {
            var btn = _wait.Until(ExpectedConditions.ElementExists(BotonGuardarForm));
            if (btn.Enabled) btn.Click();
        }

        public void ClickOpcionEnTabla(string nombreProveedor, string opcion)
        {
            AbrirMenuOpciones(nombreProveedor);
            string textoBoton = opcion.Equals("Eliminar", StringComparison.OrdinalIgnoreCase) ? "Deshabilitar" : opcion;

            var xpathBoton = $"//tr[td[contains(text(), '{nombreProveedor}')]]//div[@class='options-menu']//button[contains(text(), '{textoBoton}')]";
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathBoton))).Click();
        }

        private void AbrirMenuOpciones(string nombreProveedor)
        {
            var xpathIcono = $"//tr[td[contains(text(), '{nombreProveedor}')]]//span[contains(@class, 'options-icon')]";
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathIcono))).Click();
        }

        public void ClickConfirmarEliminacion()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(BotonConfirmarModal)).Click();
            Thread.Sleep(500);
        }

        public bool EsperarQueElModalSeCierre()
        {
            try { return _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(ModalFormCard)); } 
            catch { return false; }
        }

        public bool ExisteMensajeEnPantalla(string mensajeEsperado)
        {
            try
            {
                var errores = _driver.FindElements(MensajesDeErrorField)
                    .Concat(_driver.FindElements(MensajeExito))
                    .Concat(_driver.FindElements(MensajeErrorGlobal));
                return errores.Any(e => e.Displayed && e.Text.Contains(mensajeEsperado));
            }
            catch { return false; }
        }

        public bool ExisteProveedorEnTablaSimple(string nombreProveedor)
        {
            try
            {
                var xpath = $"//table//td[contains(normalize-space(text()), '{nombreProveedor}')]";
                _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                return true;
            }
            catch { return false; }
        }

        public bool VerificarDatosProveedor(string nombreProveedor, string nit, string email)
        {
            try
            {
                var xpathFila = $"//tr[td[contains(normalize-space(text()), '{nombreProveedor}')]]";
                var fila = _driver.FindElement(By.XPath(xpathFila));
                
                var celdas = fila.FindElements(By.TagName("td"));
                // Assuming: Nombre, NIT, Dirección, Teléfono, Email, Contacto, Estado
                
                bool nombreCorrecto = celdas[0].Text.Contains(nombreProveedor);
                bool nitCorrecto = celdas[1].Text.Contains(nit);
                bool emailCorrecto = celdas[4].Text.Contains(email);
                
                return nombreCorrecto && nitCorrecto && emailCorrecto;
            }
            catch { return false; }
        }

        public bool VerificarEstadoProveedor(string nombreProveedor, string estadoEsperado)
        {
            try
            {
                var xpathEstado = $"//tr[td[contains(text(), '{nombreProveedor}')]]//td[7]/span";
                var estadoElemento = _driver.FindElement(By.XPath(xpathEstado));
                return estadoElemento.Text.Trim().ToUpper().Contains(estadoEsperado.ToUpper());
            }
            catch { return false; }
        }

        public bool ExisteTabla() => _driver.FindElements(TablaProveedores).Count > 0;
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