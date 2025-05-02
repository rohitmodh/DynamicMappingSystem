using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Validation
{
    public interface IModelValidator
    {
        bool Validate(JObject data, string propertyPath, string dataContext, out string? errorMessage);
    }
}
