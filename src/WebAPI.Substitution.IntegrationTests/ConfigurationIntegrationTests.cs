using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.Substitution.IntegrationTests;

public class ConfigurationIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [Fact]
    public async Task AddSubstitution_Does_Not_Trigger_Config_Reload_Foreach_Provider()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        UpdateAppSettings("SomeValue");

        var configuration = (factory.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot)!;
        var someValue = configuration.GetValue<string>("SomeKey");
        var provider = configuration.Providers.First(provider => provider is ReloadCountingConfigurationProvider) as ReloadCountingConfigurationProvider;

        Assert.Equal("SomeValue", someValue);

        UpdateAppSettings("UpdatedValue");
        configuration.Reload();

        var valueAfterReload = configuration.GetValue<string>("SomeKey");

        Assert.Equal("UpdatedValue", valueAfterReload);
        Assert.NotNull(provider);
        Assert.Equal(3, provider.ReloadCount);

        static void UpdateAppSettings(string value)
        {
            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appSettings.test.json");
            var newContent = new
            {
                SomeKey = value
            };

            var jsonContent = JsonSerializer.Serialize(newContent, JsonOptions);
            File.WriteAllText(appSettingsPath, jsonContent);
        }
    }
}