using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using ReqnrollIntegrationTests.Support;
using Xunit;

namespace ReqnrollIntegrationTests.StepDefinitions;

[Binding]
public class ProductsStepDefinitions
{
    private readonly IntegrationTestBase _testBase;
    private HttpResponseMessage? _response;
    private User? _adminUser;
    private Category? _currentCategory;
    private Product? _currentProduct;
    private short _currentProductId;
    private List<Product> _createdProducts = new();

    public ProductsStepDefinitions(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    // ========================================================================
    // BACKGROUND STEPS
    // ========================================================================

    [Given(@"Existe una categoria creada previamente")]
    public async Task GivenExisteUnaCategoriaCreadaPreviamente()
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        _currentCategory = await _testBase.CreateTestCategoryAsync(_adminUser.Id);
        Assert.NotNull(_currentCategory);
    }

    // ========================================================================
    // CREATE - STEPS
    // ========================================================================

    [When(@"Creo un producto con serialCode (.*), nombre ""(.*)"", descripcion ""(.*)"" y stock (.*)")]
    public async Task WhenCreoUnProducto(short serialCode, string nombre, string descripcion, short stock)
    {
        var dto = new CreateProductDto
        {
            SerialCode = serialCode,
            Name = nombre,
            Description = descripcion,
            CategoryId = _currentCategory?.Id ?? -1,
            TotalStock = stock
        };

        _response = await _testBase.PostAsync("/api/products", dto, _adminUser?.Id ?? 1);

        if (_response.IsSuccessStatusCode)
        {
            var product = await _response.Content.ReadFromJsonAsync<Product>();
            if (product != null)
            {
                _currentProduct = product;
                _currentProductId = product.Id;
                _createdProducts.Add(product);
            }
        }
    }

    [Then(@"El producto debe estar guardado en la base de datos")]
    public async Task ThenElProductoDebeEstarGuardadoEnLaBaseDeDatos()
    {
        Assert.NotNull(_currentProduct);

        var productFromDb = await _testBase.DbContext.Products.FindAsync(_currentProduct.Id);
        Assert.NotNull(productFromDb);
        Assert.Equal(_currentProduct.Name, productFromDb.Name);
        Assert.Equal(_currentProduct.SerialCode, productFromDb.SerialCode);
    }

    [Then(@"El producto no debe estar guardado en la base de datos")]
    public async Task ThenElProductoNoDebeEstarGuardadoEnLaBaseDeDatos()
    {
        var response = await _testBase.Client.GetAsync($"/api/products/{_currentProductId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ========================================================================
    // SELECT - STEPS
    // ========================================================================

    [Given(@"Existen (.*) productos creados previamente")]
    public async Task GivenExistenProductosCreados(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            var product = new Product
            {
                SerialCode = (short)(2000 + i),
                Name = $"Producto Test {i}",
                Description = $"Descripcion {i}",
                CategoryId = _currentCategory!.Id,
                TotalStock = (short)(10 + i),
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = _adminUser!.Id
            };
            
            _testBase.DbContext.Products.Add(product);
            _createdProducts.Add(product);
        }
        
        await _testBase.DbContext.SaveChangesAsync();
    }

    [When(@"Solicito la lista de productos")]
    public async Task WhenSolicitoLaListaDeProductos()
    {
        _response = await _testBase.Client.GetAsync("/api/products");
    }

    [Then(@"La lista de productos debe contener al menos (.*) productos")]
    public async Task ThenLaListaDeProductosDebeContenerAlMenos(int cantidad)
    {
        var products = await _response!.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.True(products.Count >= cantidad);
    }

    // ========================================================================
    // UPDATE - STEPS
    // ========================================================================

    [Given(@"Existe un producto creado previamente con serialCode (.*) y nombre ""(.*)""")]
    public async Task GivenExisteUnProductoCreadoPreviamente(short serialCode, string nombre)
    {
        _currentProduct = new Product
        {
            SerialCode = serialCode,
            Name = nombre,
            Description = "Descripcion original",
            CategoryId = _currentCategory!.Id,
            TotalStock = 10,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = _adminUser!.Id
        };

        _testBase.DbContext.Products.Add(_currentProduct);
        await _testBase.DbContext.SaveChangesAsync();
        _currentProductId = _currentProduct.Id;
    }

    [When(@"Actualizo el producto con serialCode (.*), nombre ""(.*)"", descripcion ""(.*)"" y stock (.*)")]
    public async Task WhenActualizoElProducto(short serialCode, string nombre, string descripcion, short stock)
    {
        var dto = new CreateProductDto
        {
            SerialCode = serialCode,
            Name = nombre,
            Description = descripcion,
            CategoryId = _currentCategory!.Id,
            TotalStock = stock
        };

        _response = await _testBase.PutAsync($"/api/products/{_currentProductId}", dto);
    }

    [Then(@"El producto debe estar actualizado en la base de datos con nombre ""(.*)""")]
    public async Task ThenElProductoDebeEstarActualizadoEnLaBaseDeDatos(string nombreEsperado)
    {
        var productFromDb = await _testBase.DbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == _currentProductId);

        Assert.NotNull(productFromDb);
        Assert.Equal(nombreEsperado, productFromDb.Name);
    }

    [Then(@"El producto no debe estar actualizado en la base de datos")]
    public async Task ThenElProductoNoDebeEstarActualizadoEnLaBaseDeDatos()
    {
        var productFromDb = await _testBase.DbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == _currentProductId);

        Assert.NotNull(productFromDb);
        Assert.Equal("Laptop Dell", productFromDb.Name);
    }

    // ========================================================================
    // DELETE - STEPS
    // ========================================================================

    [When(@"Creo un producto para eliminar con serialCode (.*), nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task WhenCreoUnProductoParaEliminar(short serialCode, string nombre, string descripcion)
    {
        _currentProduct = new Product
        {
            SerialCode = serialCode,
            Name = nombre,
            Description = descripcion,
            CategoryId = _currentCategory!.Id,
            TotalStock = 5,
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = _adminUser!.Id
        };

        _testBase.DbContext.Products.Add(_currentProduct);
        await _testBase.DbContext.SaveChangesAsync();
        _currentProductId = _currentProduct.Id;
    }

    [Then(@"El producto creado debe tener status (.*)")]
    public void ThenElProductoCreadoDebeTenerStatus(int status)
    {
        Assert.NotNull(_currentProduct);
        Assert.Equal(status, _currentProduct.Status);
    }

    [When(@"Elimino el producto como administrador")]
    public async Task WhenEliminoElProductoComoAdministrador()
    {
        _response = await _testBase.DeleteAsync($"/api/products/{_currentProductId}", "Admin");
    }

    [Then(@"El producto debe tener status (.*) en la base de datos")]
    public async Task ThenElProductoDebeTenerStatusEnLaBaseDeDatos(int statusEsperado)
    {
        await _testBase.DbContext.Entry(_currentProduct!).ReloadAsync();
        Assert.Equal(statusEsperado, _currentProduct.Status);
    }

    // ========================================================================
    // COMMON STEPS (Status HTTP)
    // ========================================================================

    [Then(@"La respuesta producto debe ser (\d+) (.*)")]
    public void ThenLaRespuestaProductoDebeSer(int statusCode, string statusText)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }
}