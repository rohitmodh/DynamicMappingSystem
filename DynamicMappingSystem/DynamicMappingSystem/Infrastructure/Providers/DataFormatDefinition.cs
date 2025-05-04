namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Represents the definition of a data format, including its type and associated properties.
    /// </summary>
    public class DataFormatDefinition
    {
        /// <summary>
        /// Gets or sets the type of the data format.
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// Gets or sets the list of properties associated with the data format.
        /// </summary>
        public List<string> Properties { get; set; } = new();
    }
}
