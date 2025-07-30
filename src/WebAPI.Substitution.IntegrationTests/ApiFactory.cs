using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.Substitution.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public void UpdateAppSettings(string value)
    {
        var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appSettings.test.json");
        var newContent = new
        {
            SomeKey = value,
        };

        var jsonContent = JsonSerializer.Serialize(newContent, JsonOptions);
        File.WriteAllText(appSettingsPath, jsonContent);

        (this.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot)!.Reload();
    }
}