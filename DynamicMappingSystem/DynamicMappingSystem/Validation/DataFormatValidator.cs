
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Validation
{
    public class DataFormatValidator : IModelValidator
    {
        public bool Validate(JObject data, string propertyPath, string dataContext, out string? errorMessage)
        {
            var properties = propertyPath.Split('.');
            JToken currentToken = data;

            foreach (var property in properties)
            {
                if (currentToken is JObject currentObject && currentObject.TryGetValue(property, out var nextToken))
                {
                    currentToken = nextToken;
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
