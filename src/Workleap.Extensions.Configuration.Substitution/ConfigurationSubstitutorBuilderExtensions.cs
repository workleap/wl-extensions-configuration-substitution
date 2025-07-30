using Workleap.Extensions.Configuration.Substitution;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationSubstitutorBuilderExtensions
{
    /// <summary>
    /// Substitutes referenced configuration values in other configuration values, using the format <c>${MySection:MyValue}</c>.
    /// In that example, <c>${MySection:MyValue}</c> will be replaced by the actual string value.
    /// If a referenced configuration value does not exist, an exception will be thrown.
    /// You can escape values that must not be substituted using double curly braces, such as <c>${{Foo}}</c>.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="eagerValidation">Whether or not all the configuration must be validated to ensure there are no referenced configuration keys that does not exist.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddSubstitution(this IConfigurationBuilder configurationBuilder, bool eagerValidation = false)
    {
        if (configurationBuilder.Sources.LastOrDefault() is ChainedSubstitutedConfigurationSource)
        {
            return configurationBuilder;
        }

        // We clear the list of sources and re-add them as cached sources. We used to just replace them in the sources list, but that triggers a configuration reload
        // every time a source is changed. Adding them doesn't trigger a reload.
        var clone = configurationBuilder.Sources.ToArray();
        configurationBuilder.Sources.Clear();

        foreach (var configurationSource in clone)
        {
            switch (configurationSource)
            {
                case CachedConfigurationSource:
                    configurationBuilder.Sources.Add(configurationSource);
                    continue;
                case ChainedSubstitutedConfigurationSource:
                    // Don't do anything here as we want the ChainedSubstitutedConfigurationSource to be the last one in the list.
                    continue;
                default:
                    configurationBuilder.Sources.Add(new CachedConfigurationSource(configurationSource));
                    break;
            }
        }

        configurationBuilder.Sources.Add(new ChainedSubstitutedConfigurationSource(eagerValidation));

        return configurationBuilder;
    }
}