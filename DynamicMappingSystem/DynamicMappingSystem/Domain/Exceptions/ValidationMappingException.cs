namespace DynamicMappingSystem.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when there is a validation error during the mapping process.
    /// Initializes a new instance of the <see cref="ValidationMappingException"/> class with the specified error code and message.
    /// </summary>
    /// <param name="code">The error code associated with the validation failure.</param>
    /// <param name="message">The error message describing the validation failure.</param>
    public class ValidationMappingException(string code, string message) : Exception(message)
    {

        /// <summary>
        /// Gets the error code associated with the validation failure.
        /// </summary>
        public string Code { get; } = code;
    }
}
