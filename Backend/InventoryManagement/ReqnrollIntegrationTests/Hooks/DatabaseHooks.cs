using ReqnrollIntegrationTests.Fixtures;
using ReqnrollIntegrationTests.Support;
using Reqnroll;
using Reqnroll.BoDi;

namespace ReqnrollIntegrationTests.Hooks;

[Binding]
public class DatabaseHooks
{
    private static CustomWebApplicationFactory? _factory = null;

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        _factory = new CustomWebApplicationFactory();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _factory?.Dispose();
    }

    [BeforeScenario]
    public async Task BeforeScenario(IObjectContainer objectContainer)
    {
        if (_factory == null)
        {
            throw new InvalidOperationException("Factory no inicializada");
        }

        var testBase = new IntegrationTestBase(_factory);
        await testBase.CleanupDatabaseAsync();

        objectContainer.RegisterInstanceAs(testBase);
        objectContainer.RegisterInstanceAs(_factory);
    }

    [AfterScenario]
    public void AfterScenario(IntegrationTestBase testBase)
    {
        testBase?.Dispose();
    }
}