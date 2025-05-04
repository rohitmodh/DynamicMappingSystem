using DynamicMappingSystem.Application.Interfaces;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Factory class that provides the appropriate <see cref="IMappingRuleProvider"/> based on the configuration.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MappingRuleProviderFactory"/> class.
    /// </remarks>
    /// <param name="jsonMappingRuleProvider">The provider for JSON mapping rules.</param>
    /// <param name="mongoMappingRuleProvider">The provider for MongoDB mapping rules.</param>
    /// <param name="configuration">The configuration used to determine the mapping rule source.</param>
    public class MappingRuleProviderFactory(
        JsonMappingRuleProvider jsonMappingRuleProvider,
        MongoMappingRuleProvider mongoMappingRuleProvider,
        IConfiguration configuration) : IMappingRuleProviderFactory
    {
        private readonly JsonMappingRuleProvider _jsonMappingRuleProvider = jsonMappingRuleProvider;
        private readonly MongoMappingRuleProvider _mongoMappingRuleProvider = mongoMappingRuleProvider;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Gets the appropriate <see cref="IMappingRuleProvider"/> based on the mapping rule source defined in the configuration.
        /// </summary>
        /// <returns>An instance of <see cref="IMappingRuleProvider"/> corresponding to the configured rule source.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the rule source is unknown or unsupported.</exception>
        public IMappingRuleProvider GetProvider()
        {
            var ruleSource = _configuration.GetValue<string>("MappingRuleSource");

            return ruleSource switch
            {
                "json" => _jsonMappingRuleProvider,
                "mongo" => _mongoMappingRuleProvider,
                _ => throw new InvalidOperationException("Unknown mapping rule source")
            };
        }
    }
}
