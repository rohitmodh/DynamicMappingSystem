using DynamicMappingSystem.Application.Mapping;

namespace DynamicMappingSystem.Domain.Rules
{
    /// <summary>
    /// Represents a mapping rule that defines how properties of a source type map to a target type.
    /// </summary>
    public class MapperRule
    {
        /// <summary>
        /// Gets or sets the source type for the mapping rule.
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Gets or sets the target type for the mapping rule.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// Gets or sets the version of the mapping rule. This can be used for versioning of the mapping logic.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the list of property mappings that define how properties from the source type map to the target type.
        /// </summary>
        public List<PropertyMapping> PropertyMappings { get; set; } = new();
    }
}
