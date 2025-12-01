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
public class CategoriesStepDefinitions
{
    private readonly IntegrationTestBase _testBase;
    private HttpResponseMessage? _response;
    private User? _adminUser;
    private Category? _currentCategory;
    private short _currentCategoryId;
    private List<Category> _createdCategories = new();

    public CategoriesStepDefinitions(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    // ========================================================================
    // BACKGROUND STEPS
    // ========================================================================

    [Given(@"La base de datos esta disponible")]
    public async Task GivenLaBaseDeDatosEstDisponible()
    {
        // La base de datos ya est� limpia por el DatabaseHooks
        // Solo verificamos que podemos conectarnos
        var canConnect = await _testBase.DbContext.Database.CanConnectAsync();
        Assert.True(canConnect, "No se puede conectar a la base de datos");
    }

    [Given(@"Existe un usuario administrador creado")]
    public async Task GivenExisteUnUsuarioAdministradorCreado()
    {
        _adminUser = await _testBase.CreateTestUserAsync("Admin");
        Assert.NotNull(_adminUser);
    }

    // ========================================================================
    // CREATE - STEPS
    // ========================================================================

    [When(@"Creo una categoria con nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task WhenCreoUnaCategoria(string nombre, string descripcion)
    {
        var dto = new CreateCategoryDto
        {
            Name = nombre,
            Description = descripcion
        };

        _response = await _testBase.PostAsync("/api/categories", dto, _adminUser?.Id ?? 1);

        // Si la respuesta es exitosa, guardamos la categor�a creada
        if (_response.IsSuccessStatusCode)
        {
            var category = await _response.Content.ReadFromJsonAsync<Category>();
            if (category != null)
            {
                _currentCategory = category;
                _currentCategoryId = category.Id;
            }
        }
    }

    [Then(@"La categoria debe estar guardada en la base de datos")]
    public async Task ThenLaCategoriaDebeEstarGuardadaEnLaBaseDeDatos()
    {
        Assert.NotNull(_currentCategory);

        var categoryFromDb = await _testBase.DbContext.Categories.FindAsync(_currentCategory.Id);
        Assert.NotNull(categoryFromDb);
        Assert.Equal(_currentCategory.Name, categoryFromDb.Name);
    }

    [Then(@"La categoria no debe estar guardada en la base de datos")]
    public async Task ThenLaCategoriaNoDebeEstarGuardadaEnLaBaseDeDatos()
    {
        // Verificamos que no se haya creado ninguna categor�a nueva
        var categoriesCount = await _testBase.DbContext.Categories.CountAsync();
        Assert.Equal(0, categoriesCount);
    }

    // ========================================================================
    // SELECT - STEPS
    // ========================================================================

    [Given(@"Existen (.*) categorias creadas previamente")]
    public async Task GivenExistenCategoriasCreadas(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            var category = await _testBase.CreateTestCategoryAsync(_adminUser!.Id);
            _createdCategories.Add(category);
        }
    }

    [When(@"Solicito la lista de categorias")]
    public async Task WhenSolicitoLaListaDeCategorias()
    {
        _response = await _testBase.Client.GetAsync("/api/categories");
    }

    [Then(@"La lista debe contener al menos (.*) categorias")]
    public async Task ThenLaListaDebeContenerAlMenos(int cantidad)
    {
        var categories = await _response!.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= cantidad);
    }

    // ========================================================================
    // UPDATE - STEPS
    // ========================================================================

    [Given(@"Existe una categoria creada previamente con nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task GivenExisteUnaCategoriaCreadaPreviamente(string nombre, string descripcion)
    {
        _currentCategory = await _testBase.CreateTestCategoryAsync(_adminUser!.Id, nombre, descripcion);
        _currentCategoryId = _currentCategory.Id;
    }

    [When(@"Actualizo la categoria con nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task WhenActualizoLaCategoria(string nuevoNombre, string nuevaDescripcion)
    {
        var dto = new CreateCategoryDto
        {
            Name = nuevoNombre,
            Description = nuevaDescripcion
        };

        _response = await _testBase.PutAsync($"/api/categories/{_currentCategoryId}", dto);
    }

    [Then(@"La categoria debe estar actualizada en la base de datos con nombre ""(.*)""")]
    public async Task ThenLaCategoriaDebeEstarActualizadaEnLaBaseDeDatos(string nombreEsperado)
    {
        var categoryFromDb = await _testBase.DbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == _currentCategoryId);

        Assert.NotNull(categoryFromDb);
        Assert.Equal(nombreEsperado, categoryFromDb.Name);
    }

    [Then(@"La categoria no debe estar actualizada en la base de datos")]
    public async Task ThenLaCategoriaNoDebeEstarActualizadaEnLaBaseDeDatos()
    {
        var categoryFromDb = await _testBase.DbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == _currentCategoryId);

        Assert.NotNull(categoryFromDb);
        // Verificamos que sigue teniendo el nombre original
        Assert.Equal("Tecnologia", categoryFromDb.Name);
    }

    // ========================================================================
    // DELETE - STEPS
    // ========================================================================

    [When(@"Creo una categoria para eliminar con nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task WhenCreoUnaCategoriaParaEliminar(string nombre, string descripcion)
    {
        _currentCategory = await _testBase.CreateTestCategoryAsync(_adminUser!.Id, nombre, descripcion);
        _currentCategoryId = _currentCategory.Id;
    }

    [Then(@"La categoria creada debe tener status (.*)")]
    public void ThenLaCategoriaCreadaDebeTenerStatus(int status)
    {
        Assert.NotNull(_currentCategory);
        Assert.Equal(status, _currentCategory.Status);
    }

    [When(@"Elimino la categoria como administrador")]
    public async Task WhenEliminoLaCategoriaComoAdministrador()
    {
        _response = await _testBase.DeleteAsync($"/api/categories/{_currentCategoryId}", "Admin");
    }

    [Then(@"La categoria debe tener status (.*) en la base de datos")]
    public async Task ThenLaCategoriaDebeTenerStatusEnLaBaseDeDatos(int statusEsperado)
    {
        await _testBase.DbContext.Entry(_currentCategory!).ReloadAsync();
        Assert.Equal(statusEsperado, _currentCategory.Status);
    }

    // ========================================================================
    // COMMON STEPS (Status HTTP)
    // ========================================================================

    [Then(@"La respuesta debe ser (\d+) (.*)")]
    public void ThenLaRespuestaDebeSer(int statusCode, string statusText)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }
}