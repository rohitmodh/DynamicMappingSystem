using DynamicMappingSystem.Application.Interfaces;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Application.Validation
{
    /// <summary>
    /// Validator for ensuring that data in a specific format is valid based on property paths and data context.
    /// Implements the <see cref="IModelValidator"/> interface.
    /// </summary>
    public class DataFormatValidator : IModelValidator
    {
        /// <summary>
        /// Validates if a specific property exists within the provided JSON data based on the given property path.
        /// </summary>
        /// <param name="data">The JSON data to validate against.</param>
        /// <param name="propertyPath">The property path to search for within the JSON data.</param>
        /// <param name="dataContext">A string representing source or target data</param>
        /// <param name="errorMessage">An output parameter that will hold the error message if validation fails.</param>
        /// <returns>A boolean indicating whether the property was found and the data is valid.</returns>
        public bool Validate(JObject data, string propertyPath, string dataContext, out string? errorMessage)
        {
            var properties = propertyPath.Split('.');
            JToken currentToken = data;

            foreach (var property in properties)
            {
                if (currentToken is JObject currentObject)
                {
                    if (currentObject.TryGetValue(property, out var nextToken))
                    {
                        currentToken = nextToken;
                    }
                    else
                    {
                        errorMessage = $"The property: {propertyPath} is missing in the {dataContext} data.";
                        return false;
                    }
                }
                else if (currentToken is JArray currentArray && int.TryParse(property, out var index) && currentArray.Count > index)
                {
                    currentToken = currentArray[index];
                }
                else
                {
                    errorMessage = $"The property: {propertyPath} is missing in the {dataContext} data";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}
