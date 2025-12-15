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
    [Scope(Feature = "Suppliers Management")]
    public class SupplierSteps
    {
        private readonly IWebDriver _driver;
        private readonly SupplierPage _supplierPage;
        private readonly LoginPage _loginPage;
        private static DbContextOptions<InventoryDbContext> _dbContextOptions;
        private string _nombreProveedorIngresado;
        private string _nitProveedorIngresado;
        private string _emailProveedorIngresado;

        public SupplierSteps(ScenarioContext context)
        {
            _driver = context["WebDriver"] as IWebDriver;
            _supplierPage = new SupplierPage(_driver);
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

        // ========================================================================
        // BACKGROUND STEPS
        // ========================================================================
        [Given(@"he iniciado sesión como ""(.*)""")]
        public async Task GivenHeIniciadoSesionComo(string rol)
        {
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                var helper = new UiTestHelper(dbContext);
                await helper.EnsureUserExistsAsync(rol);
            }

            _loginPage.LoginExitosoComoAdmin();
        }

        [Given(@"navego a la página Proveedores")]
        public void GivenNavegoALaPaginaProveedores()
        {
            _driver.Navigate().GoToUrl("http://localhost:4200/suppliers");
        }

        // ========================================================================
        // CREATE STEPS
        // ========================================================================
        [When(@"hago click en el botón ""([^""]*)""")] 
        public void WhenHagoClickEnElBoton(string boton)
        {
            if (boton == "Agregar Proveedor")
            {
                _supplierPage.ClickAgregarProveedor();
                _supplierPage.EsperarQueElModalEsteAbierto();
            }
        }

        [When(@"ingreso el nombre del proveedor ""(.*)""")]
        public void WhenIngresoElNombreDelProveedor(string nombre)
        {
            string valor = nombre.Contains("[VACIO]") ? "" : nombre;
            _nombreProveedorIngresado = valor;
            _supplierPage.LlenarNombre(valor);
        }

        [When(@"ingreso el nit ""(.*)""")]
        public void WhenIngresoElNit(string nit)
        {
            string valor = nit.Contains("[VACIO]") ? "" : nit;
            _nitProveedorIngresado = valor;
            _supplierPage.LlenarNit(valor);
        }

        [When(@"ingreso el telefono ""(.*)""")]
        public void WhenIngresoElTelefono(string telefono)
        {
            string valor = telefono.Contains("[VACIO]") ? "" : telefono;
            _supplierPage.LlenarTelefono(valor);
        }

        [When(@"ingreso el email ""(.*)""")]
        public void WhenIngresoElEmail(string email)
        {
            string valor = email.Contains("[VACIO]") ? "" : email;
            _emailProveedorIngresado = valor;
            _supplierPage.LlenarEmail(valor);
        }

        [When(@"ingreso el nombre de contacto ""(.*)""")]
        public void WhenIngresoElNombreDeContacto(string contacto)
        {
            string valor = contacto.Contains("[VACIO]") ? "" : contacto;
            _supplierPage.LlenarContacto(valor);
        }

        [When(@"ingreso la direccion ""(.*)""")]
        public void WhenIngresoLaDireccion(string direccion)
        {
            string valor = direccion.Contains("[VACIO]") ? "" : direccion;
            _supplierPage.LlenarDireccion(valor);
        }

        [When(@"hago click en el botón ""(.*)"" del formulario de proveedor")]
        public void WhenHagoClickEnElBotonDelFormularioDeProveedor(string boton)
        {
            _supplierPage.ClickBotonFormulario();
        }

        // ========================================================================
        // VALIDACIONES STEPS
        // ========================================================================
        [Then(@"se debe mostrar el mensaje de proveedor ""(.*)""")]
        public void ThenSeDebeMostrarElMensajeDeProveedor(string mensaje)
        {
            bool mensajeVisible = _supplierPage.ExisteMensajeEnPantalla(mensaje);
            mensajeVisible.Should().BeTrue($"Se esperaba el mensaje en pantalla: '{mensaje}'");
        }

        [Then(@"el modal de proveedor debe cerrarse automaticamente")]
        public void ThenElModalDeProveedorDebeCerrarse()
        {
            bool cerrado = _supplierPage.EsperarQueElModalSeCierre();
            cerrado.Should().BeTrue("El modal debería haberse cerrado tras guardar.");
        }

        [Then(@"el proveedor ""(.*)"" debe aparecer en la tabla")]
        public void ThenElProveedorDebeAparecerEnLaTabla(string nombreProveedor)
        {
            Thread.Sleep(1000);
            _supplierPage.VerificarDatosProveedor(_nombreProveedorIngresado, _nitProveedorIngresado, _emailProveedorIngresado)
                .Should().BeTrue($"El proveedor con nombre '{_nombreProveedorIngresado}' debería aparecer.");
        }

        // ========================================================================
        // UPDATE STEPS
        // ========================================================================
        [Given(@"que existe al menos (.*) proveedor creado previamente")]
        public void GivenExisteAlMenosProveedor(int cantidad)
        {
            if (!_supplierPage.ExisteTabla() || _supplierPage.ContarFilas() < cantidad)
            {
                GivenExisteProveedorPrevio("Proveedor Demo");
            }
        }

        [Given(@"que existe un proveedor creado previamente con nombre [""“](.*)[""“]")]
        [Given(@"existe un proveedor activo con nombre [""“](.*)[""“]")]
        public void GivenExisteProveedorPrevio(string nombre)
        {
            if (_supplierPage.ExisteProveedorEnTablaSimple(nombre)) return;

            _supplierPage.ClickAgregarProveedor();
            _supplierPage.EsperarQueElModalEsteAbierto();
            _supplierPage.LlenarNombre(nombre);
            _supplierPage.LlenarNit("12358420215");
            _supplierPage.LlenarTelefono("65221478");
            _supplierPage.LlenarEmail("contacto@gmail.com");
            _supplierPage.LlenarContacto("Juan Perez");
            _supplierPage.LlenarDireccion("Av. Siempre Viva 1234");
            _supplierPage.ClickBotonFormulario();
            _supplierPage.EsperarQueElModalSeCierre();
            Thread.Sleep(1000);
        }

        [When(@"hago click en el botón [""“](.*)[""“] del proveedor [""“](.*)[""“]")]
        public void WhenHagoClickEnBotonDelProveedor(string boton, string nombreProveedor)
        {
            _supplierPage.ClickOpcionEnTabla(nombreProveedor, boton);
        }

        [When(@"actualizo el nombre del proveedor a ""(.*)""")]
        public void WhenActualizoElNombreDelProveedorA(string nombre)
        {
            _nombreProveedorIngresado = nombre;
            _supplierPage.LlenarNombre(nombre);
        }

        [When(@"actualizo el nit a ""(.*)""")]
        public void WhenActualizoElNitA(string nit)
        {
            _nitProveedorIngresado = nit;
            _supplierPage.LlenarNit(nit);
        }

        [When(@"actualizo el telefono a ""(.*)""")]
        public void WhenActualizoElTelefonoA(string telefono)
        {
            _supplierPage.LlenarTelefono(telefono);
        }

        [When(@"actualizo el email a ""(.*)""")]
        public void WhenActualizoElEmailA(string email)
        {
            _emailProveedorIngresado = email;
            _supplierPage.LlenarEmail(email);
        }

        [When(@"actualizo el nombre de contacto a ""(.*)""")]
        public void WhenActualizoElNombreDeContactoA(string contacto)
        {
            _supplierPage.LlenarContacto(contacto);
        }

        [When(@"actualizo la direccion a ""(.*)""")]
        public void WhenActualizoLaDireccionA(string direccion)
        {
            _supplierPage.LlenarDireccion(direccion);
        }

        [Then(@"el proveedor se actualizo correctamente en la tabla")]
        public void ThenElProveedorSeActualizo()
        {
            Thread.Sleep(1000);

            bool datosCorrectos = _supplierPage.VerificarDatosProveedor(_nombreProveedorIngresado, _nitProveedorIngresado, _emailProveedorIngresado);
            datosCorrectos.Should().BeTrue($"Los datos del proveedor no se actualizaron correctamente.");
        }

        // ========================================================================
        // DELETE STEPS
        // ========================================================================
        [When(@"hago click en el botón Deshabilitar del proveedor [""“](.*)[""“]")]
        public void WhenHagoClickEnBotonDeshabilitarDelProveedor(string nombreProveedor)
        {
            _supplierPage.ClickOpcionEnTabla(nombreProveedor, "Deshabilitar");
        }

        [When(@"confirmo la eliminación del proveedor en el modal")]
        public void WhenConfirmoEliminacionDelProveedor()
        {
            _supplierPage.ClickConfirmarEliminacion();
        }

        [Then(@"el proveedor [""“](.*)[""“] ya aparece como [""“](.*)[""“] en la tabla")]
        public void ThenElProveedorApareceComoEnLaTabla(string nombreProveedor, string estado)
        {
            Thread.Sleep(1000);
            _supplierPage.VerificarEstadoProveedor(nombreProveedor, estado)
                .Should().BeTrue($"El proveedor '{nombreProveedor}' debería tener estado '{estado}'");
        }

        // ========================================================================
        // SELECT STEPS
        // ========================================================================
        [Then(@"debe mostrarse la tabla de proveedores")]
        public void ThenDebeMostrarseTablaProveedores()
        {
            _supplierPage.ExisteTabla().Should().BeTrue();
        }

        [Then(@"la tabla de proveedores debe contener al menos un registro")]
        public void ThenLaTablaProveedoresDebeContenerAlMenosUnRegistro()
        {
            _supplierPage.ContarFilas().Should().BeGreaterThan(0);
        }

        [Then(@"cada registro de proveedor debe mostrar enlaces ""(.*)"" y ""(.*)""")]
        public void ThenCadaRegistroProveedorDebeMostrarEnlaces(string enlace1, string enlace2)
        {
            _supplierPage.VerificarEnlacesEnFilas(enlace1, enlace2)
                .Should().BeTrue($"Se esperaban enlaces '{enlace1}' y '{enlace2}'");
        }
    }
}