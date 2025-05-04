namespace DynamicMappingSystem.Contracts.Responses
{
    /// <summary>
    /// Represents an error that occurred during the mapping process.
    /// </summary>
    public class MappingError
    {
        /// <summary>
        /// Gets or sets the error code associated with the mapping error.
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// Gets or sets the error message describing the mapping error.
        /// </summary>
        public string Message { get; set; } = default!;
    }
}
