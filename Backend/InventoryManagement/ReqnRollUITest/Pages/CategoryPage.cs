using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.Pages
{
    public class CategoryPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // --- LOCALIZADORES (Sin cambios) ---
        private By BotonAgregarCategoria = By.XPath("//button[contains(text(), '+ Agregar Categoría')]");
        private By ModalFormCard = By.CssSelector("app-category-form .form-card");
        private By InputNombreCat = By.CssSelector("app-category-form input[name='name']");
        private By InputDescripcionCat = By.CssSelector("app-category-form input[name='description']");
        private By BotonGuardarForm = By.CssSelector("app-category-form button[type='submit']");
        private By MensajesDeErrorField = By.CssSelector(".error p");
        private By MensajeErrorGlobal = By.CssSelector(".error-message");
        private By MensajeExito = By.CssSelector(".success-message");
        private By TablaCategorias = By.CssSelector("app-category-table table.product-table");
        private By FilasTabla = By.CssSelector("app-category-table table.product-table tbody tr");
        private By BotonConfirmarModal = By.XPath("//app-confirm-modal//button[contains(@class, 'add')]");

        public CategoryPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- MÉTODOS CRUD (Resumidos para brevedad, misma lógica que antes) ---
        public void ClickAgregarCategoria() => _wait.Until(ExpectedConditions.ElementToBeClickable(BotonAgregarCategoria)).Click();
        public void EsperarQueElModalEsteAbierto() => _wait.Until(ExpectedConditions.ElementIsVisible(ModalFormCard));

        public void LlenarNombre(string nombre)
        {
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(InputNombreCat));
            input.Clear();
            if (!string.IsNullOrEmpty(nombre) && nombre != "[VACIO]") input.SendKeys(nombre);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void LlenarDescripcion(string descripcion)
        {
            var input = _driver.FindElement(InputDescripcionCat);
            input.Clear();
            if (!string.IsNullOrEmpty(descripcion) && descripcion != "[VACIO]") input.SendKeys(descripcion);
            else { input.SendKeys("a"); input.SendKeys(Keys.Backspace); }
        }

        public void ClickBotonFormulario()
        {
            var btn = _wait.Until(ExpectedConditions.ElementExists(BotonGuardarForm));
            if (btn.Enabled) btn.Click();
        }

        public void ClickOpcionEnTabla(string nombreCategoria, string opcion)
        {
            AbrirMenuOpciones(nombreCategoria);
            string textoBoton = opcion.Equals("Eliminar", StringComparison.OrdinalIgnoreCase) ? "Deshabilitar" : opcion;

            var xpathBoton = $"//tr[td[contains(text(), '{nombreCategoria}')]]//div[@class='options-menu']//button[contains(text(), '{textoBoton}')]";
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathBoton))).Click();
        }

        private void AbrirMenuOpciones(string nombreCategoria)
        {
            var xpathIcono = $"//tr[td[contains(text(), '{nombreCategoria}')]]//span[contains(@class, 'options-icon')]";
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

        // --- VALIDACIÓN DE TABLA ---

        public bool ExisteCategoriaEnTablaSimple(string nombreCategoria)
        {
            try
            {
                var xpath = $"//table//td[contains(normalize-space(text()), '{nombreCategoria}')]";
                _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                return true;
            }
            catch { return false; }
        }

        // --- NUEVO MÉTODO PARA VALIDAR DESCRIPCIÓN ---
        public bool VerificarDescripcionCategoria(string nombreCategoria, string descripcionEsperada)
        {
            try
            {
                // Estrategia: Buscar la fila (tr) que contiene el Nombre en alguna celda (td), 
                // luego buscar la celda específica de descripción. 
                // Asumimos que la Descripción es la columna 2 (index 2). Si la tabla cambia, ajustar el td[2].

                var xpathDescripcion = $"//tr[td[contains(normalize-space(text()), '{nombreCategoria}')]]//td[2]";

                var celdaDescripcion = _driver.FindElement(By.XPath(xpathDescripcion));
                string textoActual = celdaDescripcion.Text.Trim();

                return textoActual.Contains(descripcionEsperada);
            }
            catch
            {
                return false;
            }
        }

        public bool VerificarEstadoCategoria(string nombreCategoria, string estadoEsperado)
        {
            try
            {
                // Asume Columna 3 es Estado
                var xpathEstado = $"//tr[td[contains(text(), '{nombreCategoria}')]]//td[3]/span";
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