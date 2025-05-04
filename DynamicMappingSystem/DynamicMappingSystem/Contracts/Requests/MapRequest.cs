using System.Text.Json;

namespace DynamicMappingSystem.Contracts.Requests
{
    /// <summary>
    /// Represents a request to map data from a source type to a target type.
    /// </summary>
    public class MapRequest
    {
        /// <summary>
        /// Gets or sets the source type of the data.
        /// </summary>
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target type of the data.
        /// </summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the mapping, if applicable.
        /// This is an optional field that allows support for versioned mappings.
        /// </summary>
        public string? Version { get; set; } // Optional, Making it extensible to support versioning.

        /// <summary>
        /// Gets or sets the raw data to be mapped.
        /// This data is in JSON format and is the actual content to be transformed during the mapping process.
        /// </summary>
        public JsonElement Data { get; set; } // Raw data to be mapped
    }
}
