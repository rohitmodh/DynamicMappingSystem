using DynamicMappingSystem.Application.Interfaces;
using FluentValidation;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Application.Validation
{
    public class RequiredFieldValidator : IModelValidator
    {
        public bool Validate(JObject data, string propertyPath, string dataContext, out string? errorMessage)
        {
            errorMessage = null;

            var properties = propertyPath.Split('.');
            JToken currentToken = data;

            foreach (var property in properties)
            {
                if (currentToken is JObject currentObject && currentObject.TryGetValue(property, out var nextToken))
                {
                    currentToken = nextToken;
                }
                else
                {
                    errorMessage = $"Missing required property: {propertyPath} in {dataContext} data.";
                    return false;
                }
            }

            return true;
        }
    }

}
