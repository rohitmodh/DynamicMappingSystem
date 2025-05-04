namespace DynamicMappingSystem.Contracts.Responses
{
    /// <summary>
    /// Represents the result of a dynamic mapping operation.
    /// </summary>
    public class MappingResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the mapping operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the mapped data as an object. This will hold the result of the mapping if successful.
        /// </summary>
        public object? MappedData { get; set; }

        /// <summary>
        /// Gets or sets the list of errors that occurred during the mapping process.
        /// </summary>
        public List<MappingError> Errors { get; set; } = new();
    }
}
