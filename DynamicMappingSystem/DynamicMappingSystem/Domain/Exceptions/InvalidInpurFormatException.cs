namespace DynamicMappingSystem.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when the input format is invalid.
    /// Initializes a new instance of the <see cref="InvalidInpurFormatException"/> class.
    /// </summary>
    /// <param name="code">The error code associated with the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public class InvalidInpurFormatException(string code, string message) : Exception(message)
    {
        /// <summary>
        /// Gets the error code associated with the exception.
        /// </summary>
        public string Code { get; } = code;
    }
}
