using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using Reqnroll.Assist;
using ReqnrollIntegrationTests.Support;
using Xunit;

namespace ReqnrollIntegrationTests.StepDefinitions;

[Binding]
public class PurchasesStepDefinitions
{
    private readonly IntegrationTestBase _testBase;
    private HttpResponseMessage? _response;
    private User? _adminUser;
    private Category? _currentCategory;
    private Supplier? _currentSupplier;
    private Purchase? _currentPurchase;
    private Dictionary<short, Product> _testProducts = new();

    public PurchasesStepDefinitions(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    // ========================================================================
    // BACKGROUND STEPS
    // ========================================================================

    [Given(@"Existe una categoria para compras")]
    public async Task GivenExisteUnaCategoriaParaCompras()
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        _currentCategory = await _testBase.CreateTestCategoryAsync(_adminUser.Id);
        Assert.NotNull(_currentCategory);
    }

    [Given(@"Existe un proveedor para compras")]
    public async Task GivenExisteUnProveedorParaCompras()
    {
        _currentSupplier = await _testBase.CreateTestSupplierAsync(_adminUser!.Id);
        Assert.NotNull(_currentSupplier);
    }

    // ========================================================================
    // SETUP PRODUCTS
    // ========================================================================

    [Given(@"Existen (.*) productos para comprar con stock inicial")]
    public async Task GivenExistenProductosParaComprarConStockInicial(int cantidad, Table table)
    {
        _testProducts.Clear();

        foreach (var row in table.Rows)
        {
            var serialCode = short.Parse(row["SerialCode"]);
            var name = row["Name"];
            var stock = short.Parse(row["Stock"]);

            var product = new Product
            {
                SerialCode = serialCode,
                Name = name,
                Description = $"Descripcion de {name}",
                CategoryId = _currentCategory!.Id,
                TotalStock = stock,
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = _adminUser!.Id
            };

            _testBase.DbContext.Products.Add(product);
            _testProducts[serialCode] = product;
        }

        await _testBase.DbContext.SaveChangesAsync();
    }

    // ========================================================================
    // CREATE PURCHASE - HAPPY PATH
    // ========================================================================

    [When(@"Creo una compra con los siguientes detalles")]
    public async Task WhenCreoUnaCompraConLosSiguientesDetalles(Table table)
    {
        var purchaseDetails = new List<CreatePurchaseDetailDto>();

        foreach (var row in table.Rows)
        {
            var serialCode = short.Parse(row["ProductSerialCode"]);
            var quantity = short.Parse(row["Quantity"]);
            var unitPrice = decimal.Parse(row["UnitPrice"]);

            var product = _testProducts[serialCode];

            purchaseDetails.Add(new CreatePurchaseDetailDto
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
        }

        var purchaseDto = new CreatePurchaseDto
        {
            SupplierId = _currentSupplier!.Id,
            PurchaseDetails = purchaseDetails
        };

        _response = await _testBase.PostAsync("/api/purchases", purchaseDto, _adminUser!.Id);

        if (_response.IsSuccessStatusCode)
        {
            _currentPurchase = await _testBase.DbContext.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.SupplierId == _currentSupplier.Id);
        }
    }

    [Then(@"La compra debe estar guardada en la base de datos")]
    public void ThenLaCompraDebeEstarGuardadaEnLaBaseDeDatos()
    {
        Assert.NotNull(_currentPurchase);
    }

    [Then(@"El total de la compra debe ser (.*)")]
    public void ThenElTotalDeLaCompraDebeSer(decimal expectedTotal)
    {
        Assert.NotNull(_currentPurchase);
        Assert.Equal(expectedTotal, _currentPurchase.TotalPurchase);
    }

    [Then(@"El stock del producto con serialCode (.*) debe ser (.*)")]
    public async Task ThenElStockDelProductoDebeSer(short serialCode, short expectedStock)
    {
        var product = _testProducts[serialCode];
        await _testBase.DbContext.Entry(product).ReloadAsync();
        Assert.Equal(expectedStock, product.TotalStock);
    }

    // ========================================================================
    // COMMON STEPS
    // ========================================================================

    [Then(@"La respuesta compra debe ser (\d+) (.*)")]
    public void ThenLaRespuestaCompraDebeSer(int statusCode, string statusText)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }
}