namespace DynamicMappingSystem.Configuration
{
    /// <summary>
    /// Represents the settings required to connect to a MongoDB database.
    /// </summary>
    public class MongoSettings
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string for read operations.
        /// </summary>
        public string MongoDbConnectionStringR { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the MongoDB database.
        /// </summary>
        public string MongoDatabase { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the collection in the MongoDB database.
        /// </summary>
        public string CollectionName { get; set; } = string.Empty;
    }
}
