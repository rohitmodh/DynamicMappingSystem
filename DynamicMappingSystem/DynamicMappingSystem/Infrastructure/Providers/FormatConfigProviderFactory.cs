using DynamicMappingSystem.Application.Interfaces;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Factory for providing the correct format configuration provider based on the data format source.
    /// </summary>
    public class FormatConfigProviderFactory : IFormatConfigProviderFactory
    {
        private readonly JsonFormatConfigProvider _jsonFormatConfigProvider;
        private readonly MongoFormatConfigProvider _mongoFormatConfigProvider;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConfigProviderFactory"/> class.
        /// </summary>
        /// <param name="jsonFormatConfigProvider">The provider for JSON format configurations.</param>
        /// <param name="mongoFormatConfigProvider">The provider for MongoDB format configurations.</param>
        /// <param name="configuration">The configuration used to determine the data format source.</param>
        public FormatConfigProviderFactory(
            JsonFormatConfigProvider jsonFormatConfigProvider,
            MongoFormatConfigProvider mongoFormatConfigProvider,
            IConfiguration configuration)
        {
            _jsonFormatConfigProvider = jsonFormatConfigProvider;
            _mongoFormatConfigProvider = mongoFormatConfigProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the appropriate format configuration provider based on the data format source from the configuration.
        /// </summary>
        /// <returns>The selected format configuration provider.</returns>
        /// <exception cref="InvalidOperationException">Thrown if an unknown data format source is provided.</exception>
        public IFormatConfigProvider GetProvider()
        {
            var ruleSource = _configuration.GetValue<string>("DataFormatSource");

            return ruleSource switch
            {
                "json" => _jsonFormatConfigProvider,
                "mongo" => _mongoFormatConfigProvider,
                _ => throw new InvalidOperationException("Unknown mapping rule source")
            };
        }
    }
}
