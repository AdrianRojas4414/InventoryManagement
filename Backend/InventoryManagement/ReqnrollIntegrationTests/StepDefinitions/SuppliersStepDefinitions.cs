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
public class SuppliersStepDefinitions
{
    private readonly IntegrationTestBase _testBase;
    private HttpResponseMessage? _response;
    private User? _adminUser;
    private Supplier? _currentSupplier;
    private short _currentSupplierId;
    private List<Supplier> _createdSuppliers = new();

    public SuppliersStepDefinitions(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    // ========================================================================
    // CREATE - STEPS
    // ========================================================================

    [When(@"Creo un proveedor con nombre ""(.*)"", nit ""(.*)"", telefono ""(.*)"", email ""(.*)"", contacto ""(.*)"" y direccion ""(.*)""")]
    public async Task WhenCreoUnProveedor(string nombre, string nit, string telefono, string email, string contacto, string direccion)
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        // Para evitar problemas de unicidad en tests consecutivos
        var uniqueNit = $"{nit}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        var uniqueEmail = nit.Length >= 7 ? $"{email.Split('@')[0]}-{Guid.NewGuid().ToString().Substring(0, 4)}@{email.Split('@')[1]}" : email;

        var dto = new CreateSupplierDto
        {
            Name = nombre,
            Nit = uniqueNit,
            Phone = telefono,
            Email = uniqueEmail,
            ContactName = contacto,
            Address = direccion
        };

        _response = await _testBase.PostAsync("/api/suppliers", dto, _adminUser.Id);

        if (_response.IsSuccessStatusCode)
        {
            var supplier = await _response.Content.ReadFromJsonAsync<Supplier>();
            if (supplier != null)
            {
                _currentSupplier = supplier;
                _currentSupplierId = supplier.Id;
                _createdSuppliers.Add(supplier);
            }
        }
    }

    [Then(@"El proveedor debe estar guardado en la base de datos")]
    public async Task ThenElProveedorDebeEstarGuardadoEnLaBaseDeDatos()
    {
        Assert.NotNull(_currentSupplier);

        var supplierFromDb = await _testBase.DbContext.Suppliers.FindAsync(_currentSupplier.Id);
        Assert.NotNull(supplierFromDb);
        Assert.Equal(_currentSupplier.Name, supplierFromDb.Name);
    }

    [Then(@"El proveedor no debe estar guardado en la base de datos")]
    public async Task ThenElProveedorNoDebeEstarGuardadoEnLaBaseDeDatos()
    {
        var response = await _testBase.Client.GetAsync($"/api/suppliers/{_currentSupplierId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ========================================================================
    // SELECT - STEPS
    // ========================================================================

    [Given(@"Existen (.*) proveedores creados previamente")]
    public async Task GivenExistenProveedoresCreados(int cantidad)
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        for (int i = 0; i < cantidad; i++)
        {
            var supplier = new Supplier
            {
                Name = $"Proveedor Test {i}",
                Nit = $"NIT{i}{Guid.NewGuid().ToString().Substring(0, 6)}",
                Address = $"Direccion {i}",
                Phone = $"7700000{i}",
                Email = $"test{i}@supplier.com",
                ContactName = $"Contacto {i}",
                Status = 1,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedByUserId = _adminUser.Id
            };
            
            _testBase.DbContext.Suppliers.Add(supplier);
            _createdSuppliers.Add(supplier);
        }
        
        await _testBase.DbContext.SaveChangesAsync();
    }

    [When(@"Solicito la lista de proveedores")]
    public async Task WhenSolicitoLaListaDeProveedores()
    {
        _response = await _testBase.Client.GetAsync("/api/suppliers");
    }

    [Then(@"La lista de proveedores debe contener al menos (.*) proveedores")]
    public async Task ThenLaListaDeProveedoresDebeContenerAlMenos(int cantidad)
    {
        var suppliers = await _response!.Content.ReadFromJsonAsync<List<Supplier>>();
        Assert.NotNull(suppliers);
        Assert.True(suppliers.Count >= cantidad);
    }

    // ========================================================================
    // UPDATE - STEPS
    // ========================================================================

    [Given(@"Existe un proveedor creado previamente con nombre ""(.*)"" y nit ""(.*)""")]
    public async Task GivenExisteUnProveedorCreadoPreviamente(string nombre, string nit)
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        _currentSupplier = new Supplier
        {
            Name = nombre,
            Nit = nit,
            Address = "Direccion Original",
            Phone = "77123456",
            Email = "original@test.com",
            ContactName = "Contacto Original",
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = _adminUser.Id
        };

        _testBase.DbContext.Suppliers.Add(_currentSupplier);
        await _testBase.DbContext.SaveChangesAsync();
        _currentSupplierId = _currentSupplier.Id;
    }

    [When(@"Actualizo el proveedor con nombre ""(.*)"", nit ""(.*)"", telefono ""(.*)"", email ""(.*)"", contacto ""(.*)"" y direccion ""(.*)""")]
    public async Task WhenActualizoElProveedor(string nombre, string nit, string telefono, string email, string contacto, string direccion)
    {
        // Para evitar problemas de unicidad en updates
        var uniqueNit = nit.Length >= 7 ? $"{nit}-{Guid.NewGuid().ToString().Substring(0, 4)}" : nit;
        var uniqueEmail = email.Contains('@') && nit.Length >= 7 ? $"{email.Split('@')[0]}-{Guid.NewGuid().ToString().Substring(0, 4)}@{email.Split('@')[1]}" : email;

        var dto = new UpdateSupplierDto
        {
            Name = nombre,
            Nit = uniqueNit,
            Phone = telefono,
            Email = uniqueEmail,
            ContactName = contacto,
            Address = direccion
        };

        _response = await _testBase.PutAsync($"/api/suppliers/{_currentSupplierId}", dto);
    }

    [Then(@"El proveedor debe estar actualizado en la base de datos con nombre ""(.*)""")]
    public async Task ThenElProveedorDebeEstarActualizadoEnLaBaseDeDatos(string nombreEsperado)
    {
        var supplierFromDb = await _testBase.DbContext.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == _currentSupplierId);

        Assert.NotNull(supplierFromDb);
        Assert.Equal(nombreEsperado, supplierFromDb.Name);
    }

    [Then(@"El proveedor no debe estar actualizado en la base de datos")]
    public async Task ThenElProveedorNoDebeEstarActualizadoEnLaBaseDeDatos()
    {
        var supplierFromDb = await _testBase.DbContext.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == _currentSupplierId);

        Assert.NotNull(supplierFromDb);
        Assert.Equal("Proveedor ABC", supplierFromDb.Name);
    }

    // ========================================================================
    // DELETE - STEPS
    // ========================================================================

    [When(@"Creo un proveedor para eliminar con nombre ""(.*)"" y nit ""(.*)""")]
    public async Task WhenCreoUnProveedorParaEliminar(string nombre, string nit)
    {
        _adminUser = await _testBase.DbContext.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        
        if (_adminUser == null)
        {
            _adminUser = await _testBase.CreateTestUserAsync("Admin");
        }

        _currentSupplier = new Supplier
        {
            Name = nombre,
            Nit = nit,
            Address = "Direccion Temporal",
            Phone = "77999888",
            Email = "temporal@test.com",
            ContactName = "Contacto Temporal",
            Status = 1,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedByUserId = _adminUser.Id
        };

        _testBase.DbContext.Suppliers.Add(_currentSupplier);
        await _testBase.DbContext.SaveChangesAsync();
        _currentSupplierId = _currentSupplier.Id;
    }

    [Then(@"El proveedor creado debe tener status (.*)")]
    public void ThenElProveedorCreadoDebeTenerStatus(int status)
    {
        Assert.NotNull(_currentSupplier);
        Assert.Equal(status, _currentSupplier.Status);
    }

    [When(@"Elimino el proveedor como administrador")]
    public async Task WhenEliminoElProveedorComoAdministrador()
    {
        _response = await _testBase.DeleteAsync($"/api/suppliers/{_currentSupplierId}", "Admin");
    }

    [Then(@"El proveedor debe tener status (.*) en la base de datos")]
    public async Task ThenElProveedorDebeTenerStatusEnLaBaseDeDatos(int statusEsperado)
    {
        await _testBase.DbContext.Entry(_currentSupplier!).ReloadAsync();
        Assert.Equal(statusEsperado, _currentSupplier.Status);
    }

    // ========================================================================
    // COMMON STEPS
    // ========================================================================

    [Then(@"La respuesta proveedor debe ser (\d+) (.*)")]
    public void ThenLaRespuestaProveedorDebeSer(int statusCode, string statusText)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }
}