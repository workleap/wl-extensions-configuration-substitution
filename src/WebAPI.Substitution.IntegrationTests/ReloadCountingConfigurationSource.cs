namespace WebAPI.Substitution.IntegrationTests;

/// <summary>
/// A configuration source that tracks reload count statistics.
/// </summary>
internal sealed class ReloadCountingConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ReloadCountingConfigurationProvider(this);
    }
}

/// <summary>
/// A simple configuration provider that counts the number of times it's asked to reload.
/// </summary>
internal sealed class ReloadCountingConfigurationProvider : ConfigurationProvider, IDisposable
{
    private int _reloadCount;
    private readonly ReloadCountingConfigurationSource _source;

    public int ReloadCount => this._reloadCount;

    public ReloadCountingConfigurationProvider(ReloadCountingConfigurationSource source)
    {
        this._source = source;
    }

    public override void Load()
    {
        Interlocked.Increment(ref this._reloadCount);
    }

    public void Dispose()
    {
    }
}

