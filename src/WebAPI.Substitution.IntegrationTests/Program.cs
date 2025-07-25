using WebAPI.Substitution.IntegrationTests;

var builder = WebApplication.CreateBuilder(args);
var path = Path.Combine(Directory.GetCurrentDirectory(), "appSettings.test.json");

builder.Configuration.AddJsonFile(path, optional: false, reloadOnChange: true);
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>()
{
    { "SampleKey", "SampleValue" }
});
builder.Configuration.Add<ReloadCountingConfigurationSource>(_ => { });
builder.Configuration.AddSubstitution();

var app = builder.Build();
app.Run();

public partial class Program { }
