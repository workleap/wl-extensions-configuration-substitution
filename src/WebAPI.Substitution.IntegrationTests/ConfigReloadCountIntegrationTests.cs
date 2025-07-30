namespace WebAPI.Substitution.IntegrationTests;

[Collection("Integration Tests")]
public class ConfigReloadCountIntegrationTests : IClassFixture<ConfigReloadCountIntegrationTests.ConfigReloadCountApiFactory>
{
    private readonly ConfigReloadCountApiFactory _factory;

    public ConfigReloadCountIntegrationTests(ConfigReloadCountApiFactory factory)
    {
        this._factory = factory;
    }

    [Fact]
    public async Task AddSubstitution_Does_Not_Trigger_Config_Reload_Foreach_Provider()
    {
        using var client = this._factory.CreateClient();
        this._factory.UpdateAppSettings("SomeValue");

        var configuration = (this._factory.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot)!;
        var someValue = configuration.GetValue<string>("SomeKey");
        var provider = configuration.Providers.First(provider => provider is ReloadCountingConfigurationProvider) as ReloadCountingConfigurationProvider;

        Assert.Equal("SomeValue", someValue);

        this._factory.UpdateAppSettings("UpdatedValue");

        var valueAfterReload = configuration.GetValue<string>("SomeKey");

        Assert.Equal("UpdatedValue", valueAfterReload);
        Assert.NotNull(provider);
        Assert.Equal(4, provider.ReloadCount);
    }

    public sealed class ConfigReloadCountApiFactory : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.Add<ReloadCountingConfigurationSource>(_ => { });
                config.AddSubstitution();
                config.AddSubstitution();
            });
        }
    }
}