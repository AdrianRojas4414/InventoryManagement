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
    [Scope(Feature = "Purchases Management")]
    public class PurchaseSteps
    {
        private readonly IWebDriver _driver;
        private readonly PurchasePage _purchasePage;
        private readonly LoginPage _loginPage;
        private static DbContextOptions<InventoryDbContext> _dbContextOptions;

        private int _stockInicial = 0;

        public PurchaseSteps(ScenarioContext context)
        {
            _driver = context["WebDriver"] as IWebDriver;
            _purchasePage = new PurchasePage(_driver);
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

        [Given(@"navego a la página Compras")]
        public void GivenNavegoALaPaginaCompras()
        {            
            _loginPage.NavegarDesdeMenu("Compras");
        }

        [Given(@"existe un proveedor activo para compras")]
        public async Task GivenExisteProveedorActivoParaCompras()
        {
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                var helper = new UiTestHelper(dbContext);
                var adminUser = await helper.EnsureUserExistsAsync("Admin");
                await helper.EnsureSupplierExistsAsync(adminUser.Id);
            }
        }

        [Given(@"existen productos activos para compras")]
        public async Task GivenExistenProductosActivosParaCompras()
        {
            using (var dbContext = new InventoryDbContext(_dbContextOptions))
            {
                var helper = new UiTestHelper(dbContext);
                var adminUser = await helper.EnsureUserExistsAsync("Admin");

                // 1. Asegura que la categoría por defecto exista (útil para crear nuevos productos manualmente)
                await helper.EnsureCategoryExistsAsync(adminUser.Id);

                // 2. Asegura que los productos específicos para la prueba existan
                await helper.EnsureProductExistsAsync("Laptop Dell", adminUser.Id);
                await helper.EnsureProductExistsAsync("Teclado Mecanico", adminUser.Id);
            }
        }

        [Given(@"el producto ""(.*)"" tiene stock inicial de (.*)")]
        public void GivenElProductoTieneStockInicialDe(string nombreProducto, int stockInicial)
        {
            _stockInicial = stockInicial;
            _driver.Navigate().GoToUrl("http://localhost:4200/products");
            Thread.Sleep(1000);
            _driver.Navigate().GoToUrl("http://localhost:4200/purchases");
        }

        // ========================================================================
        // CREATE STEPS
        // ========================================================================

        [When(@"hago click en el botón ""(.*)""")]
        public void WhenHagoClickEnElBoton(string boton)
        {
            switch (boton)
            {
                case "Agregar Compra":
                    _purchasePage.ClickAgregarCompra();
                    _purchasePage.EsperarQueElModalEsteAbierto();
                    break;

                case "Registrar Compra":
                    _purchasePage.ClickRegistrarCompra();
                    break;

                default:
                    throw new ArgumentException($"El botón '{boton}' no está definido en los pasos de PurchaseSteps.");
            }
        }

        [When(@"selecciono el proveedor disponible")]
        public void WhenSeleccionoElProveedorDisponible()
        {
            _purchasePage.SeleccionarPrimerProveedor();
        }

        [When(@"agrego el producto ""(.*)"" con cantidad (.*) y precio unitario (.*)")]
        public void WhenAgregoElProductoConCantidadYPrecioUnitario(string nombreProducto, int cantidad, decimal precioUnitario)
        {
            _purchasePage.AgregarProducto(nombreProducto, cantidad, precioUnitario);
        }

        // ========================================================================
        // VALIDACIONES STEPS
        // ========================================================================
        [Then(@"el modal de compra debe cerrarse automaticamente")]
        public void ThenElModalDeCompraDebeCerrarse()
        {
            Thread.Sleep(2000);
            bool cerrado = _purchasePage.EsperarQueElModalSeCierre();
            cerrado.Should().BeTrue("El modal debería haberse cerrado tras registrar la compra.");
        }

        [Then(@"la compra debe aparecer en la tabla con total ""(.*)""")]
        public void ThenLaCompraDebeAparecerEnLaTablaConTotal(string total)
        {
            Thread.Sleep(1000);
            bool compraVisible = _purchasePage.VerificarCompraEnTabla(total);
            compraVisible.Should().BeTrue($"La compra con total '{total}' debería aparecer en la tabla.");
        }

        [Then(@"el stock del producto ""(.*)"" debe ser (.*)")]
        public void ThenElStockDelProductoDebeSer(string nombreProducto, int stockEsperado)
        {
            _loginPage.NavegarDesdeMenu("Productos");

            var productPage = new ProductPage(_driver);
            
            Thread.Sleep(1000);
            
            bool stockCorrecto = productPage.VerificarStockProducto(nombreProducto, stockEsperado.ToString());
            
            stockCorrecto.Should().BeTrue($"El stock de '{nombreProducto}' debería ser {stockEsperado} en la tabla de productos.");
        }
    }
}