namespace DynamicMappingSystem.Configuration
{
    /// <summary>
    /// Represents the configuration settings for mapping rules, including the provider type and file paths for JSON and MongoDB mappings.
    /// </summary>
    public class MappingRulesSettings
    {
        /// <summary>
        /// Gets or sets the provider type for the mapping rules (e.g., "Json").
        /// Defaults to "Json".
        /// </summary>
        public string Provider { get; set; } = "Json";

        /// <summary>
        /// Gets or sets the file path for the JSON mapping rule configuration.
        /// </summary>
        public string JsonMappingRuleFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file path for the JSON data format configuration.
        /// </summary>
        public string JsonDataFormatFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MongoDB-specific settings for the mapping.
        /// </summary>
        public MongoSettings Mongo { get; set; } = new MongoSettings();
    }
}
