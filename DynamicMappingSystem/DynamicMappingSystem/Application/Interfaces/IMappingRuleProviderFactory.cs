namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Factory interface for obtaining a mapping rule provider.
    /// </summary>
    public interface IMappingRuleProviderFactory
    {
        /// <summary>
        /// Gets an instance of a mapping rule provider based on the current configuration or context.
        /// </summary>
        /// <returns>An implementation of <see cref="IMappingRuleProvider"/>.</returns>
        IMappingRuleProvider GetProvider();
    }
}
