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

        // --- BACKGROUND ---
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
            _driver.Navigate().GoToUrl("http://localhost:4200/purchases");
            Thread.Sleep(1000);
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
                var category = await helper.EnsureCategoryExistsAsync(adminUser.Id);

                // Crear productos específicos para las pruebas
                var productosNecesarios = new[] { "Laptop Dell", "Teclado Mecanico" };
                
                foreach (var nombreProducto in productosNecesarios)
                {
                    var productoExistente = await dbContext.Products
                        .FirstOrDefaultAsync(p => p.Name == nombreProducto && p.Status == 1);

                    if (productoExistente == null)
                    {
                        var nuevoProducto = new Domain.Entities.Product
                        {
                            Name = nombreProducto,
                            Description = $"Descripción de {nombreProducto}",
                            SerialCode = (short)(10000 + productosNecesarios.ToList().IndexOf(nombreProducto)),
                            CategoryId = category.Id,
                            TotalStock = 10,
                            Status = 1,
                            CreationDate = DateTime.UtcNow,
                            ModificationDate = DateTime.UtcNow,
                            CreatedByUserId = adminUser.Id
                        };
                        dbContext.Products.Add(nuevoProducto);
                    }
                }
                
                await dbContext.SaveChangesAsync();
            }
        }

        [Given(@"el producto ""(.*)"" tiene stock inicial de (.*)")]
        public void GivenElProductoTieneStockInicialDe(string nombreProducto, int stockInicial)
        {
            _stockInicial = stockInicial;
            // Verificar el stock actual en la interfaz
            _driver.Navigate().GoToUrl("http://localhost:4200/products");
            Thread.Sleep(1000);
            _driver.Navigate().GoToUrl("http://localhost:4200/purchases");
        }

        // --- CREATE STEPS ---

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

        // --- VALIDACIONES ---
        [Then(@"el modal de compra debe cerrarse automaticamente")]
        public void ThenElModalDeCompraDebeCerrarse()
        {
            Thread.Sleep(2000); // Dar tiempo para que se procese
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
            Thread.Sleep(1000);
            int stockActual = _purchasePage.ObtenerStockProducto(nombreProducto);
            stockActual.Should().Be(stockEsperado, $"El stock de '{nombreProducto}' debería ser {stockEsperado}");
        }
    }
}