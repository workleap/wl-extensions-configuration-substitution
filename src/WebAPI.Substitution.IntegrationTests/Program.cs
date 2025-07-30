var builder = WebApplication.CreateBuilder(args);
var path = Path.Combine(Directory.GetCurrentDirectory(), "appSettings.test.json");

builder.Configuration.AddJsonFile(path, optional: false, reloadOnChange: true);
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
{
    { "SampleKey", "SampleValue" },
});

var app = builder.Build();
app.Run();

public partial class Program { }
