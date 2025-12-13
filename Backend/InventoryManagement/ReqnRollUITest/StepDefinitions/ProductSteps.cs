using FluentAssertions;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.ReqnrollUITest.Pages;
using InventoryManagement.ReqnrollUITest.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Reqnroll;
using System.IO;
using System.Threading;

namespace InventoryManagement.ReqnrollUITest.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Products Management")]
    public class ProductSteps
    {
        private readonly IWebDriver _driver;
        private readonly ProductPage _productPage;
        private readonly LoginPage _loginPage;
        private static DbContextOptions<InventoryDbContext> _dbContextOptions;

        // Variables para rastrear los datos ingresados y poder verificarlos al final
        private string _nombreProductoIngresado;
        private string _descripcionProductoIngresado;
        private string _codigoSerialProductoIngresado;
        private string _stockProductoIngresado;

        public ProductSteps(ScenarioContext context)
        {
            _driver = context["WebDriver"] as IWebDriver;
            _productPage = new ProductPage(_driver);
            _loginPage = new LoginPage(_driver);

            if (_dbContextOptions == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var connectionString = "Server=localhost;Database=StagingInventoryManagementDB;User=root;Password=mysqlroosevelt14;";

                var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                _dbContextOptions = optionsBuilder.Options;
            }
        }

        // --- BACKGROUND ---
        [Given(@"he iniciado sesión como ""(.*)""")]
        public async Task GivenHeIniciadoSesionComo(string rol)
        {
            // Ensure the user exists in the database
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                var helper = new UiTestHelper(dbContext);
                var user = await helper.EnsureUserExistsAsync(rol);
                
                // *** KEY ADDITION: Ensure at least one category exists ***
                await helper.EnsureCategoryExistsAsync(user.Id);
            }

            // Now perform login
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
            if (boton == "Agregar Producto")
            {
                _productPage.ClickAgregarProducto();
                _productPage.EsperarQueElModalEsteAbierto();
            }
        }

        [When(@"ingreso el nombre ""(.*)""")]
        public void WhenIngresoElNombre(string nombre)
        {
            string valor = nombre.Contains("[VACIO]") ? "" : nombre;
            _nombreProductoIngresado = valor; 
            _productPage.LlenarNombre(valor);
        }

        [When(@"ingreso la descripción ""(.*)""")]
        public void WhenIngresoLaDescripcion(string descripcion)
        {
            string valor = descripcion.Contains("[VACIO]") ? "" : descripcion;
            _descripcionProductoIngresado = valor; 
            _productPage.LlenarDescripcion(valor);
        }

        [When(@"ingreso el codigo serial ""(.*)""")]
        public void WhenIngresoElCodigoSerial(string codigoSerial)
        {
            string valor = codigoSerial.Contains("[VACIO]") ? "" : codigoSerial;
            _codigoSerialProductoIngresado = valor;
            _productPage.LlenarCodigoSerial(valor);
        }

        [When(@"ingreso el total stock ""(.*)""")]
        public void WhenIngresoElStock(string stock)
        {
            string valor = stock.Contains("[VACIO]") ? "" : stock;
            _stockProductoIngresado = valor;
            _productPage.LlenarStock(valor);
        }

        [When(@"hago click en el botón ""(.*)"" del formulario")]
        public void WhenHagoClickEnElBotonDelFormulario(string boton)
        {
            _productPage.ClickBotonFormulario();
        }

        [Then(@"se debe mostrar el mensaje ""(.*)""")]
        public void ThenSeDebeMostrarElMensaje(string mensaje)
        {
            bool mensajeVisible = _productPage.ExisteMensajeEnPantalla(mensaje);
            mensajeVisible.Should().BeTrue($"Se esperaba el mensaje en pantalla: '{mensaje}'");
        }

        [Then(@"el modal debe cerrarse automaticamente")]
        public void ThenElModalDebeCerrarse()
        {
            bool cerrado = _productPage.EsperarQueElModalSeCierre();
            cerrado.Should().BeTrue("El modal debería haberse cerrado tras guardar.");
        }

        [Then(@"el producto ""(.*)"" debe aparecer en la tabla")]
        public void ThenElProductoDebeAparecerEnLaTabla(string nombreProducto)
        {
            Thread.Sleep(1000);
            // Verificamos nombre
            _productPage.ExisteProductoEnTabla(_nombreProductoIngresado)
                .Should().BeTrue($"El nombre '{_nombreProductoIngresado}' debería aparecer.");

            // Verificamos descripción (si se ingresó alguna)
            if (!string.IsNullOrEmpty(_descripcionProductoIngresado))
            {
                _productPage.VerificarDescripcionProducto(_nombreProductoIngresado, _descripcionProductoIngresado)
                    .Should().BeTrue($"La descripción '{_descripcionProductoIngresado}' debería coincidir en la tabla.");
            }


            // Verificamos stock (si se ingresó alguna)
            if (!string.IsNullOrEmpty(_stockProductoIngresado))
            {
                _productPage.VerificarStockProducto(_nombreProductoIngresado, _stockProductoIngresado)
                    .Should().BeTrue($"El stock '{_stockProductoIngresado}' debería coincidir en la tabla.");
            }
        }

        // --- EDITAR Y LISTAR ---

        [Given(@"que existe al menos (.*) producto creado previamente")]
        public void GivenExisteAlMenosProducto(int cantidad)
        {
            if (!_productPage.ExisteTabla() || _productPage.ContarFilas() < cantidad)
            {
                _productPage.ClickAgregarProducto();
                _productPage.EsperarQueElModalEsteAbierto();
                _productPage.LlenarNombre("Producto Demo");
                _productPage.LlenarDescripcion("Descripcion de prueba");
                _productPage.LlenarCodigoSerial("31652");
                _productPage.LlenarStock("14");
                _productPage.ClickBotonFormulario();
                _productPage.EsperarQueElModalSeCierre();
                Thread.Sleep(1000);
            }
        }

        [Given(@"que existe un producto creado previamente con nombre [""“](.*)[""”]")]
        [Given(@"existe un producto activo con nombre [""“](.*)[""”]")]
        public async Task GivenExisteProductoPrevio(string nombre)
        {
            if (_productPage.ExisteProductoEnTabla(nombre)) return;

            // Ensure category exists before creating product
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                var helper = new UiTestHelper(dbContext);
                var adminUser = await helper.EnsureUserExistsAsync("Admin");
                await helper.EnsureCategoryExistsAsync(adminUser.Id);
            }

            _productPage.ClickAgregarProducto();
            _productPage.EsperarQueElModalEsteAbierto();
            _productPage.LlenarNombre(nombre);
            _productPage.LlenarDescripcion("Descripcion de prueba");
            _productPage.LlenarCodigoSerial("31652");
            _productPage.LlenarStock("14");
            _productPage.ClickBotonFormulario();
            _productPage.EsperarQueElModalSeCierre();
            Thread.Sleep(1000);
        }

        [When(@"hago click en el botón [""“](.*)[""”] del producto [""“](.*)[""”]")]
        public void WhenHagoClickEnBotonDelProducto(string boton, string nombreProducto)
        {
            _productPage.ClickOpcionEnTabla(nombreProducto, boton);
        }

        [When(@"actualizo el nombre a ""(.*)""")]
        public void WhenActualizoElNombreA(string nombre)
        {
            _nombreProductoIngresado = nombre; // Actualizamos la variable de rastreo
            _productPage.LlenarNombre(nombre);
        }

        [When(@"actualizo la descripción a ""(.*)""")]
        public void WhenActualizoLaDescripcionA(string descripcion)
        {
            _descripcionProductoIngresado = descripcion; // Actualizamos la variable de rastreo
            _productPage.LlenarDescripcion(descripcion);
        }

        [When(@"actualizo el codigo serial a ""(.*)""")]
        public void WhenActualizoElCodigoSerialA(string codigoSerial)
        {
            _codigoSerialProductoIngresado = codigoSerial; // Actualizamos la variable de rastreo
            _productPage.LlenarCodigoSerial(codigoSerial);
        }

        [When(@"actualizo el stock a ""(.*)""")]
        public void WhenActualizoElStockA(string stock)
        {
            _stockProductoIngresado = stock; // Actualizamos la variable de rastreo
            _productPage.LlenarStock(stock);
        }

        [Then(@"el producto se actualizo correctamente en la tabla")]
        public void ThenProductoaSeActualizo()
        {
            Thread.Sleep(1000); 

            bool nombreExiste = _productPage.ExisteProductoEnTabla(_nombreProductoIngresado);
            nombreExiste.Should().BeTrue($"El nombre de la categoría no se actualizó a '{_nombreProductoIngresado}' en la tabla.");

            bool descripcionCoincide = _productPage.VerificarDescripcionProducto(_nombreProductoIngresado, _descripcionProductoIngresado);
            descripcionCoincide.Should().BeTrue($"La descripción no se actualizó a '{_descripcionProductoIngresado}' en la fila de '{_nombreProductoIngresado}'.");

            bool stockCoincide = _productPage.VerificarStockProducto(_nombreProductoIngresado, _stockProductoIngresado);
            stockCoincide.Should().BeTrue($"El stock no se actualizó a '{_stockProductoIngresado}' en la fila de '{_nombreProductoIngresado}'.");
        }

        [When(@"hago click en el botón Deshabilitar del producto [""“](.*)[""”]")]
        public void WhenHagoClickEnBotonDeshabilitarDeProducto(string nombreProducto)
        {
            _productPage.ClickOpcionEnTabla(nombreProducto, "Deshabilitar");
        }

        [When(@"confirmo la eliminación en el modal")]
        public void WhenConfirmoEliminacion()
        {
            _productPage.ClickConfirmarEliminacion();
        }

        [Then(@"el producto [""“](.*)[""”] ya aparece como [""“](.*)[""”] en la tabla")]
        public void ThenElProductoApareceComoEnLaTabla(string nombreProducto, string estado)
        {
            Thread.Sleep(1000);
            _productPage.VerificarEstadoProducto(nombreProducto, estado)
                .Should().BeTrue($"El producto '{nombreProducto}' debería tener estado '{estado}'");
        }

        [Then(@"debe mostrarse la tabla de productos")]
        public void ThenDebeMostrarseLaTabla()
        {
            _productPage.ExisteTabla().Should().BeTrue();
        }

        [Then(@"la tablas debe contener al menos un registro")]
        public void ThenLaTablasDebeContenerAlMenosUnRegistro()
        {
            _productPage.ContarFilas().Should().BeGreaterThan(0);
        }

        [Then(@"cada registro debe mostrar enlaces ""(.*)"" y ""(.*)""")]
        public void ThenCadaRegistroDebeMostrarEnlaces(string enlace1, string enlace2)
        {
            _productPage.VerificarEnlacesEnFilas(enlace1, enlace2)
                .Should().BeTrue($"Se esperaban enlaces '{enlace1}' y '{enlace2}'");
        }
    }
}