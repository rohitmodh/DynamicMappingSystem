using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using MongoDB.Driver;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Provides MongoDB-based format configurations for data mapping.
    /// Implements the <see cref="IFormatConfigProvider"/> interface.
    /// </summary>
    public class MongoFormatConfigProvider : IFormatConfigProvider
    {
        private readonly IMongoCollection<DataFormatDefinition> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoFormatConfigProvider"/> class.
        /// Sets up the MongoDB client, connects to the database, and retrieves the collection of data format definitions.
        /// </summary>
        /// <param name="configuration">The configuration object that contains the MongoDB connection string and database settings.</param>
        public MongoFormatConfigProvider(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnectionStringR"));
            var database = client.GetDatabase(configuration["MappingRules:Mongo:MongoDatabase"]);
            _collection = database.GetCollection<DataFormatDefinition>("DataFormat");
        }

        /// <summary>
        /// Retrieves the format definition for the specified type from the MongoDB collection.
        /// </summary>
        /// <param name="type">The type of the data format to retrieve.</param>
        /// <returns>A <see cref="DataFormatDefinition"/> object that contains the format information for the specified type.</returns>
        /// <exception cref="MappingException">Thrown when the format definition for the specified type is not found.</exception>
        public DataFormatDefinition GetFormat(string type)
        {
            var format = _collection.Find(f => f.Type == type).FirstOrDefault();

            return format ?? throw new MappingException(MappingErrorCodes.MissingFormat, $"Format definition not found for type '{type}'");
        }
    }
}
