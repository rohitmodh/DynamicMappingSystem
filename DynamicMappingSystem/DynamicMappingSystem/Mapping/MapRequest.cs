using System.Text.Json;

namespace DynamicMappingSystem.Mapping
{
    public class MapRequest
    {
        public string SourceType { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string? Version { get; set; } // Optional, in case you support versioned mappings
        public JsonElement Data { get; set; } // Raw data to be mapped
    }
}
