using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Workleap.Extensions.Configuration.Substitution.Tests;

var builder = WebApplication.CreateBuilder(args);
var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.test.json");

builder.Configuration.AddJsonFile(path, optional: false, reloadOnChange: true);
builder.Configuration.Add<ReloadCountingConfigurationSource>(_ => { });
builder.Configuration.AddSubstitution();

var app = builder.Build();
app.Run();
