using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Provides a contract for validating models represented as JSON objects.
    /// </summary>
    public interface IModelValidator
    {
        /// <summary>
        /// Validates the provided JSON object based on a specified property path and contextual data.
        /// </summary>
        /// <param name="data">The JSON object containing the data to validate.</param>
        /// <param name="propertyPath">The path to the property within the JSON object to validate.</param>
        /// <param name="dataContext">Additional context or metadata relevant to the validation process.</param>
        /// <param name="errorMessage">
        /// When this method returns, contains an error message if the validation failed; otherwise, it is <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the provided data meets the validation criteria; otherwise, <c>false</c>.
        /// </returns>
        bool Validate(JObject data, string propertyPath, string dataContext, out string? errorMessage);
    }
}
