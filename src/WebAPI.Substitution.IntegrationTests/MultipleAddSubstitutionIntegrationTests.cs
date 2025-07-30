namespace WebAPI.Substitution.IntegrationTests;

[Collection("Integration Tests")]
public class MultipleAddSubstitutionIntegrationTests : IClassFixture<MultipleAddSubstitutionIntegrationTests.MultipleAddSubstitutionApiFactory>
{
    private readonly MultipleAddSubstitutionApiFactory _factory;

    public MultipleAddSubstitutionIntegrationTests(MultipleAddSubstitutionApiFactory factory)
    {
        this._factory = factory;
    }

    [Fact]
    public async Task Multiple_AddSubstitution_Doesnt_Stack_Overflow()
    {
        using var client = this._factory.CreateClient();
        this._factory.UpdateAppSettings("SomeValue");

        var configuration = (this._factory.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot)!;
        var someValue = configuration.GetValue<string>("SomeKey");
        var substitutedValue = configuration.GetValue<string>("SubstitutedKey");

        Assert.Equal("SomeValue", someValue);
        Assert.Equal("SomeValue", substitutedValue);

        this._factory.UpdateAppSettings("UpdatedValue");

        var valueAfterReload = configuration.GetValue<string>("SomeKey");
        var substitutedValueAfterReload = configuration.GetValue<string>("SubstitutedKey");

        Assert.Equal("UpdatedValue", valueAfterReload);
        Assert.Equal("UpdatedValue", substitutedValueAfterReload);

    }

    public sealed class MultipleAddSubstitutionApiFactory : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddSubstitution();
                config.AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    { "SubstitutedKey", "${SomeKey}" },
                });
                config.AddSubstitution();
            });
        }
    }
}