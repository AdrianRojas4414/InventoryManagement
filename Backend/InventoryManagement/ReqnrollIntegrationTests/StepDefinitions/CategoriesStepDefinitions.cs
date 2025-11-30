using Reqnroll;
using System.Net.Http.Json;
using System.Net;
using InventoryManagement.Application.DTOs; //
using Xunit;

namespace ReqnrollIntegrationTests.StepDefinitions
{
    [Binding]
    public class CategoriesStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HttpClient _client;
        private HttpResponseMessage _response;
        private int _categoryIdToDelete;

        public CategoriesStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _client = scenarioContext.Get<HttpClient>("HttpClient");
        }

        [Given("Que soy un usuario autenticado")]
        public void GivenQueSoyUnUsuarioAutenticado()
        {
            // Aquí idealmente llamarías al endpoint de login si tienes Auth activado
            // O configurarías el WebApplicationFactory para bypassear auth.
        }

        // --- CREATE ---
        [When("Intento crear una categoria con nombre {string} y descripcion {string}")]
        public async Task WhenIntentoCrearUnaCategoria(string name, string description)
        {
            var command = new CreateCategoryDto { Name = name, Description = description }; //
            _response = await _client.PostAsJsonAsync("/api/Category", command); //
        }

        // --- UPDATE ---
        [Given("Existe una categoria con ID {int} llamada {string}")]
        public async Task GivenExisteUnaCategoria(int id, string name)
        {
            // Aseguramos que exista para el test de update (Seed rápido)
            var command = new CreateCategoryDto { Name = name, Description = "Pre-created" };
            await _client.PostAsJsonAsync("/api/Category", command);
        }

        [Given("Existe una categoria con ID {int}")]
        public void GivenExisteCategoriaSimple(int id) { /* Asumimos existencia por staging o paso previo */ }

        [When("Actualizo la categoria {int} con nombre {string}")]
        public async Task WhenActualizoLaCategoria(int id, string name)
        {
            // Usamos un DTO, suponiendo que tienes UpdateCategoryDto o reutilizas Create
            var command = new CreateCategoryDto { Name = name, Description = "Updated Desc" };
            _response = await _client.PutAsJsonAsync($"/api/Category/{id}", command); //
        }

        // --- DELETE ---
        [Given("Existe una categoria para eliminar")]
        public async Task GivenExisteUnaCategoriaParaEliminar()
        {
            var command = new CreateCategoryDto { Name = "To Delete", Description = "Temp" };
            var res = await _client.PostAsJsonAsync("/api/Category", command);
            var created = await res.Content.ReadFromJsonAsync<CategoryResponseStub>(); // Necesitas un DTO de respuesta o leer ID
            _categoryIdToDelete = created.Id;
        }

        [When("Elimino la categoria")]
        public async Task WhenEliminoLaCategoria()
        {
            _response = await _client.DeleteAsync($"/api/Category/{_categoryIdToDelete}"); //
        }

        // --- GET ---
        [When("Solicito la lista de categorias")]
        public async Task WhenSolicitoLaListaDeCategorias()
        {
            _response = await _client.GetAsync("/api/Category"); //
        }

        // --- ASSERTIONS (Reusable) ---
        [Then("La respuesta debe ser {int} {word}")]
        public void ThenLaRespuestaDebeSer(int statusCode, string statusDescription)
        {
            var expectedCode = (HttpStatusCode)statusCode;
            Assert.Equal(expectedCode, _response.StatusCode);
        }

        [Then("La respuesta debe ser {int} Bad Request")]
        public void ThenBadRequest(int statusCode)
        {
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        }

        [Then("La lista no debe estar vacia")]
        public async Task ThenLaListaNoDebeEstarVacia()
        {
            var content = await _response.Content.ReadFromJsonAsync<List<object>>();
            Assert.NotEmpty(content);
        }

        // Stub rápido para leer respuesta, ajusta a tus DTOs reales
        private class CategoryResponseStub { public int Id { get; set; } }
    }
}