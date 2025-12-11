using FluentAssertions;
using InventoryManagement.ReqnrollUITest.Pages;
using OpenQA.Selenium;
using Reqnroll;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Categories Management")]
    public class CategorySteps
    {
        private readonly IWebDriver _driver;
        private readonly CategoryPage _categoryPage;
        private readonly LoginPage _loginPage;

        // Variables para rastrear los datos ingresados y poder verificarlos al final
        private string _nombreCategoriaIngresada;
        private string _descripcionCategoriaIngresada;

        public CategorySteps(ScenarioContext context)
        {
            _driver = context["WebDriver"] as IWebDriver;
            _categoryPage = new CategoryPage(_driver);
            _loginPage = new LoginPage(_driver);
        }

        // --- BACKGROUND ---
        [Given(@"he iniciado sesión como ""(.*)""")]
        public void GivenHeIniciadoSesionComo(string rol)
        {
            _loginPage.LoginExitosoComoAdmin();
        }

        [Given(@"navego a la página Productos")]
        public void GivenNavegoALaPaginaProductos()
        {
            _driver.Navigate().GoToUrl("http://localhost:4200/products");
        }

        // --- CREATE STEPS ---
        [When(@"hago click en el botón ""(.*)""")]
        public void WhenHagoClickEnElBoton(string boton)
        {
            if (boton == "Agregar Categoría")
            {
                _categoryPage.ClickAgregarCategoria();
                _categoryPage.EsperarQueElModalEsteAbierto();
            }
        }

        [When(@"ingreso el nombre ""(.*)""")]
        public void WhenIngresoElNombre(string nombre)
        {
            string valor = nombre.Contains("[VACIO]") ? "" : nombre;
            _nombreCategoriaIngresada = valor; // Guardamos dato
            _categoryPage.LlenarNombre(valor);
        }

        [When(@"ingreso la descripción ""(.*)""")]
        public void WhenIngresoLaDescripcion(string descripcion)
        {
            string valor = descripcion.Contains("[VACIO]") ? "" : descripcion;
            _descripcionCategoriaIngresada = valor; // Guardamos dato
            _categoryPage.LlenarDescripcion(valor);
        }

        [When(@"hago click en el botón ""(.*)"" del formulario")]
        public void WhenHagoClickEnElBotonDelFormulario(string boton)
        {
            _categoryPage.ClickBotonFormulario();
        }

        // --- VALIDACIONES ---
        [Then(@"se debe mostrar el mensaje ""(.*)""")]
        public void ThenSeDebeMostrarElMensaje(string mensaje)
        {
            bool mensajeVisible = _categoryPage.ExisteMensajeEnPantalla(mensaje);
            mensajeVisible.Should().BeTrue($"Se esperaba el mensaje en pantalla: '{mensaje}'");
        }

        [Then(@"el modal debe cerrarse automaticamente")]
        public void ThenElModalDebeCerrarse()
        {
            bool cerrado = _categoryPage.EsperarQueElModalSeCierre();
            cerrado.Should().BeTrue("El modal debería haberse cerrado tras guardar.");
        }

        [Then(@"la categoría ""(.*)"" debe aparecer en la tabla")]
        public void ThenLaCategoriaDebeAparecerEnLaTabla(string nombreCategoria)
        {
            Thread.Sleep(1000);
            // Verificamos nombre
            _categoryPage.ExisteCategoriaEnTablaSimple(_nombreCategoriaIngresada)
                .Should().BeTrue($"El nombre '{_nombreCategoriaIngresada}' debería aparecer.");

            // Verificamos descripción (si se ingresó alguna)
            if (!string.IsNullOrEmpty(_descripcionCategoriaIngresada))
            {
                _categoryPage.VerificarDescripcionCategoria(_nombreCategoriaIngresada, _descripcionCategoriaIngresada)
                    .Should().BeTrue($"La descripción '{_descripcionCategoriaIngresada}' debería coincidir en la tabla.");
            }
        }

        // --- EDITAR Y LISTAR ---

        [Given(@"que existe al menos (.*) categoría creada previamente")]
        public void GivenExisteAlMenosCategoria(int cantidad)
        {
            if (!_categoryPage.ExisteTabla() || _categoryPage.ContarFilas() < cantidad)
            {
                GivenExisteCategoriaPrevia("Categoría Demo", "Descripción de prueba");
            }
        }

        [Given(@"que existe una categoría creada previamente con nombre [""“](.*)[""”] con descripción [""“](.*)[""”]")]
        [Given(@"existe una categoría activa con nombre [""“](.*)[""”] y descripción [""“](.*)[""”]")]
        public void GivenExisteCategoriaPrevia(string nombre, string descripcion)
        {
            if (_categoryPage.ExisteCategoriaEnTablaSimple(nombre)) return;

            _categoryPage.ClickAgregarCategoria();
            _categoryPage.EsperarQueElModalEsteAbierto();
            _categoryPage.LlenarNombre(nombre);
            _categoryPage.LlenarDescripcion(descripcion);
            _categoryPage.ClickBotonFormulario();
            _categoryPage.EsperarQueElModalSeCierre();
            Thread.Sleep(1000);
        }

        [When(@"hago click en el botón [""“](.*)[""”] de la categoría [""“](.*)[""”]")]
        public void WhenHagoClickEnBotonDeCategoria(string boton, string nombreCategoria)
        {
            _categoryPage.ClickOpcionEnTabla(nombreCategoria, boton);
        }

        [When(@"actualizo el nombre a ""(.*)""")]
        public void WhenActualizoElNombreA(string nombre)
        {
            _nombreCategoriaIngresada = nombre; // Actualizamos la variable de rastreo
            _categoryPage.LlenarNombre(nombre);
        }

        [When(@"actualizo la descripción a ""(.*)""")]
        public void WhenActualizoLaDescripcionA(string descripcion)
        {
            _descripcionCategoriaIngresada = descripcion; // Actualizamos la variable de rastreo
            _categoryPage.LlenarDescripcion(descripcion);
        }

        // --- STEP CORREGIDO: VERIFICA NOMBRE Y DESCRIPCIÓN ---
        [Then(@"la categoria se actualizo correctamente en la tabla")]
        public void ThenLaCategoriaSeActualizo()
        {
            Thread.Sleep(1000); // Esperar refresco de la grilla

            // 1. Verificar que el NOMBRE actualizado existe
            bool nombreExiste = _categoryPage.ExisteCategoriaEnTablaSimple(_nombreCategoriaIngresada);
            nombreExiste.Should().BeTrue($"El nombre de la categoría no se actualizó a '{_nombreCategoriaIngresada}' en la tabla.");

            // 2. Verificar que la DESCRIPCIÓN actualizada coincide para ese nombre
            bool descripcionCoincide = _categoryPage.VerificarDescripcionCategoria(_nombreCategoriaIngresada, _descripcionCategoriaIngresada);
            descripcionCoincide.Should().BeTrue($"La descripción no se actualizó a '{_descripcionCategoriaIngresada}' en la fila de '{_nombreCategoriaIngresada}'.");
        }

        // --- RESTO DEL CÓDIGO (Deshabilitar/Tablas) ---
        [When(@"hago click en el botón Eliminar de la categoría [""“](.*)[""”]")]
        public void WhenHagoClickEnBotonEliminarDeCategoria(string nombreCategoria)
        {
            _categoryPage.ClickOpcionEnTabla(nombreCategoria, "Deshabilitar");
        }

        [When(@"hago click en el botón Deshabilitar de la categoría [""“](.*)[""”]")]
        public void WhenHagoClickEnBotonDeshabilitarDeCategoria(string nombreCategoria)
        {
            _categoryPage.ClickOpcionEnTabla(nombreCategoria, "Deshabilitar");
        }

        [When(@"confirmo la eliminación en el modal")]
        public void WhenConfirmoEliminacion()
        {
            _categoryPage.ClickConfirmarEliminacion();
        }

        [Then(@"la categoría [""“](.*)[""”] ya aparece como [""“](.*)[""”] en la tabla")]
        public void ThenLaCategoriaApareceComoEnLaTabla(string nombreCategoria, string estado)
        {
            Thread.Sleep(1000);
            _categoryPage.VerificarEstadoCategoria(nombreCategoria, estado)
                .Should().BeTrue($"La categoría '{nombreCategoria}' debería tener estado '{estado}'");
        }

        [Then(@"debe mostrarse la tabla de categorías")]
        public void ThenDebeMostrarseLaTabla()
        {
            _categoryPage.ExisteTabla().Should().BeTrue();
        }

        [Then(@"la tabla debe contener al menos un registro")]
        public void ThenLaTablasDebeContenerAlMenosUnRegistro()
        {
            _categoryPage.ContarFilas().Should().BeGreaterThan(0);
        }

        [Then(@"cada registro debe mostrar enlaces ""(.*)"" y ""(.*)""")]
        public void ThenCadaRegistroDebeMostrarEnlaces(string enlace1, string enlace2)
        {
            _categoryPage.VerificarEnlacesEnFilas(enlace1, enlace2)
                .Should().BeTrue($"Se esperaban enlaces '{enlace1}' y '{enlace2}'");
        }
    }
}