namespace DynamicMappingSystem.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a mapping cannot be found.
    /// Initializes a new instance of the <see cref="MappingNotFoundException"/> class.
    /// </summary>
    /// <param name="code">The code associated with the mapping error.</param>
    /// <param name="message">The message explaining the error.</param>
    public class MappingNotFoundException(string code, string message) : Exception(message)
    {
        /// <summary>
        /// Gets the code associated with the mapping error.
        /// </summary>
        public string Code { get; } = code;
    }
}
