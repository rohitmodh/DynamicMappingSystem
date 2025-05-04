namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Factory interface for creating instances of <see cref="IFormatConfigProvider"/>.
    /// Used to abstract the logic of selecting the appropriate configuration provider.
    /// </summary>
    public interface IFormatConfigProviderFactory
    {
        /// <summary>
        /// Retrieves an instance of <see cref="IFormatConfigProvider"/> based on current application context or configuration.
        /// </summary>
        /// <returns>An implementation of <see cref="IFormatConfigProvider"/>.</returns>
        IFormatConfigProvider GetProvider();
    }
}
