namespace DynamicMappingSystem.Domain.Exceptions
{
    /// <summary>
    /// Represents errors that occur during the mapping process.
    /// Initializes a new instance of the <see cref="MappingException"/> class.
    /// </summary>
    /// <param name="code">The error code representing the specific mapping error.</param>
    /// <param name="message">The error message that explains the exception.</param>

    public class MappingException(string code, string message) : Exception(message)
    {
        /// <summary>
        /// Gets the code associated with the specific mapping error.
        /// </summary>
        public string Code { get; } = code;
    }
}
