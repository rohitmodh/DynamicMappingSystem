using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Domain.Rules;
using DynamicMappingSystem.Domain.Exceptions;
using MongoDB.Driver;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Provides mapping rules from a MongoDB collection for dynamic mapping operations.
    /// Implements the <see cref="IMappingRuleProvider"/> interface.
    /// </summary>
    public class MongoMappingRuleProvider : IMappingRuleProvider
    {
        private readonly IMongoCollection<MapperRule> _rulesCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMappingRuleProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration used to initialize the MongoDB client and collection.</param>
        public MongoMappingRuleProvider(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnectionStringR"));
            var database = client.GetDatabase(configuration["MappingRules:Mongo:MongoDatabase"]);
            _rulesCollection = database.GetCollection<MapperRule>("MappingRules");
        }

        /// <summary>
        /// Retrieves property mappings from the MongoDB collection based on source and target types.
        /// </summary>
        /// <param name="sourceType">The source type for the mapping.</param>
        /// <param name="targetType">The target type for the mapping.</param>
        /// <returns>A list of <see cref="PropertyMapping"/> objects.</returns>
        /// <exception cref="MappingException">Thrown when no mapping rule is found for the given types.</exception>
        public List<PropertyMapping> GetPropertyMappings(string sourceType, string targetType)
        {
            var rule = _rulesCollection
                .Find(r => r.SourceType == sourceType && r.TargetType == targetType)
                .FirstOrDefault();

            if (rule == null)
            {
                throw new MappingException(
                    MappingErrorCodes.MappingRuleNotFound,
                    $"Mapping rule not found for types '{sourceType}' -> '{targetType}'"
                );
            }

            return rule.PropertyMappings;
        }
    }
}
