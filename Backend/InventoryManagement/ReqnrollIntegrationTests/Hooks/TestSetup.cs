using Reqnroll;
using ReqnrollIntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagement.Infrastructure.Persistence;

namespace ReqnrollIntegrationTests.Hooks
{
    [Binding]
    public class TestSetup
    {
        private static CustomWebApplicationFactory _factory;
        private readonly ScenarioContext _scenarioContext;

        public TestSetup(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void GlobalSetup()
        {
            _factory = new CustomWebApplicationFactory();
        }

        [BeforeScenario]
        public void ScenarioSetup()
        {
            // Cliente HTTP para el escenario
            var client = _factory.CreateClient();
            _scenarioContext.Set(client, "HttpClient");

            // Limpiar datos específicos antes del test
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        }

        [AfterTestRun]
        public static void GlobalTeardown()
        {
            _factory?.Dispose();
        }
    }
}