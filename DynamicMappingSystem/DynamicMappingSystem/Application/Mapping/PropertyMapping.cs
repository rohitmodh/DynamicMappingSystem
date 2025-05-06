namespace DynamicMappingSystem.Application.Mapping
{
    /// <summary>
    /// Represents a mapping between a source property and a target property.
    /// Optionally includes a custom converter for transforming the data between the source and target.
    /// </summary>
    public class PropertyMapping
    {
        /// <summary>
        /// Gets or sets the name of the source property.
        /// </summary>
        public string SourceProperty { get; set; }

        /// <summary>
        /// Gets or sets the name of the target property.
        /// </summary>
        public string TargetProperty { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom converter to be used for the property mapping. This is for future use case to make it more robust and extensible.
        /// If null, no custom converter is applied.
        /// </summary>
        public string? CustomConverter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source property is a collection.
        /// </summary>
        public bool? IsCollection { get; set; }
    }
}
