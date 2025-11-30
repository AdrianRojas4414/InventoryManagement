using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;
using Reqnroll;
using ReqnrollIntegrationTests.Support;
using Xunit;

namespace ReqnrollIntegrationTests.StepDefinitions;

[Binding]
public class CategoriesStepDefinitions
{
    private readonly IntegrationTestBase _testBase;
    private HttpResponseMessage? _response;
    private User? _currentUser;
    private Category? _currentCategory;

    public CategoriesStepDefinitions(IntegrationTestBase testBase)
    {
        _testBase = testBase;
    }

    [Given(@"Que soy un usuario autenticado")]
    public async Task GivenQueSoyUnUsuarioAutenticado()
    {
        _currentUser = await _testBase.CreateTestUserAsync("Admin");
    }

    [When(@"Intento crear una categoria con nombre ""(.*)"" y descripcion ""(.*)""")]
    public async Task WhenIntentoCrearUnaCategoriaConNombreYDescripcion(string name, string description)
    {
        var dto = new CreateCategoryDto
        {
            Name = name,
            Description = description
        };

        _response = await _testBase.PostAsync("/api/categories", dto, _currentUser?.Id ?? 1);
    }

    [Then(@"La respuesta debe ser (\d+) (.*)")]
    public void ThenLaRespuestaDebeSer(int statusCode, string statusText)
    {
        Assert.NotNull(_response);
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }

    [Given(@"Existe una categoria con ID (\d+) llamada ""(.*)""")]
    public async Task GivenExisteUnaCategoriaConIDLlamada(int id, string name)
    {
        _currentUser = await _testBase.CreateTestUserAsync("Admin");
        _currentCategory = await _testBase.CreateTestCategoryAsync(_currentUser.Id, name, "Descripción de prueba");
    }

    [Given(@"Existe una categoria con ID (\d+)")]
    public async Task GivenExisteUnaCategoriaConID(int id)
    {
        _currentUser = await _testBase.CreateTestUserAsync("Admin");
        _currentCategory = await _testBase.CreateTestCategoryAsync(_currentUser.Id);
    }

    [When(@"Actualizo la categoria (\d+) con nombre ""(.*)""")]
    public async Task WhenActualizoLaCategoriaConNombre(int id, string newName)
    {
        var dto = new CreateCategoryDto
        {
            Name = newName,
            Description = "Descripción actualizada"
        };

        _response = await _testBase.PutAsync($"/api/categories/{_currentCategory?.Id ?? id}", dto);
    }

    [When(@"Solicito la lista de categorias")]
    public async Task WhenSolicitoLaListaDeCategorias()
    {
        // Primero crear algunas categorías para que la lista no esté vacía
        var user = await _testBase.CreateTestUserAsync("Admin");
        await _testBase.CreateTestCategoryAsync(user.Id, "Cat1", "Desc1");
        await _testBase.CreateTestCategoryAsync(user.Id, "Cat2", "Desc2");

        _response = await _testBase.Client.GetAsync("/api/categories");
    }

    [Then(@"La lista no debe estar vacia")]
    public async Task ThenLaListaNoDebeEstarVacia()
    {
        Assert.NotNull(_response);
        var categories = await _response.Content.ReadFromJsonAsync<List<Category>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Given(@"Existe una categoria para eliminar")]
    public async Task GivenExisteUnaCategoriaParaEliminar()
    {
        _currentUser = await _testBase.CreateTestUserAsync("Admin");
        _currentCategory = await _testBase.CreateTestCategoryAsync(_currentUser.Id);
    }

    [When(@"Elimino la categoria")]
    public async Task WhenEliminoLaCategoria()
    {
        Assert.NotNull(_currentCategory);
        _response = await _testBase.DeleteAsync($"/api/categories/{_currentCategory.Id}", "Admin");
    }
}