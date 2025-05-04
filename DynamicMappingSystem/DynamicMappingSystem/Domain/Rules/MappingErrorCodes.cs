namespace DynamicMappingSystem.Domain.Rules
{
    /// <summary>
    /// A static class containing error codes used throughout the dynamic mapping system for consistency.
    /// These error codes are referenced when mapping operations fail due to validation, missing properties, or other unexpected issues.
    /// </summary>
    public static class MappingErrorCodes
    {
        /// <summary>
        /// Error code indicating that validation has failed.
        /// </summary>
        public const string ValidationFailed = "ERR_VALIDATION_FAILED";

        /// <summary>
        /// Error code indicating that the provided data is invalid.
        /// </summary>
        public const string InvalidData = "ERR_INVALID_DATA";

        /// <summary>
        /// Error code indicating that a required format is missing.
        /// </summary>
        public const string MissingFormat = "ERR_FORMAT_MISSING";

        /// <summary>
        /// Error code indicating that a source property is missing during mapping.
        /// </summary>
        public const string MissingSourceProperty = "ERR_SOURCE_PROPERTY_MISSING";

        /// <summary>
        /// Error code indicating that setting a target property has failed.
        /// </summary>
        public const string SetTargetPropertyFailed = "ERR_SET_TARGET_PROPERTY_FAILED";

        /// <summary>
        /// Error code indicating that no mapping rule was found for the given mapping request.
        /// </summary>
        public const string MappingRuleNotFound = "ERR_MAPPING_RULE_NOT_FOUND";

        /// <summary>
        /// Error code indicating that a property mapping operation has failed.
        /// </summary>
        public const string PropertyMappingFailed = "ERR_PROPERTY_MAPPING_FAILED";

        /// <summary>
        /// Error code indicating that an unexpected error has occurred.
        /// </summary>
        public const string UnexpectedError = "ERR_UNEXPECTED";
    }
}
