using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Extensions
{
    public static class JObjectExtensions
    {
        public static Dictionary<string, JToken> Flatten(this JObject obj, string? parentPath = null)
        {
            var result = new Dictionary<string, JToken>();

            foreach (var prop in obj.Properties())
            {
                var currentPath = string.IsNullOrEmpty(parentPath) ? prop.Name : $"{parentPath}.{prop.Name}";

                if (prop.Value.Type == JTokenType.Object)
                {
                    var childObj = (JObject)prop.Value;
                    foreach (var child in childObj.Flatten(currentPath))
                    {
                        result[child.Key] = child.Value;
                    }
                }
                else
                {
                    result[currentPath] = prop.Value;
                }
            }

            return result;
        }

        // Check if the target property is nested (e.g., "Customer.Name")
        public static void SetNestedValue(this JObject target, string path, object? value)
        {
            var targetProperties = path.Split('.');

            if (targetProperties.Length > 1)
            {
                JObject currentTarget = target;

                for (int i = 0; i < targetProperties.Length - 1; i++)
                {
                    var nestedProperty = targetProperties[i];

                    if (currentTarget[nestedProperty] == null || currentTarget[nestedProperty]!.Type != JTokenType.Object)
                    {
                        currentTarget[nestedProperty] = new JObject();
                    }

                    currentTarget = (JObject)currentTarget[nestedProperty]!;
                }

                currentTarget[targetProperties.Last()] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
            }
            else
            {
                target[path] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
            }
        }
    }
}


// Check if the target property is nested (e.g., "Customer.Name")
//var targetProperties = map.TargetProperty.Split('.');

//if (targetProperties.Length > 1)
//{
//    // Create a nested object if necessary
//    var currentTarget = result as JObject;

//    // Iterate over the properties to create nested structures
//    for (int i = 0; i < targetProperties.Length - 1; i++)
//    {
//        var nestedProperty = targetProperties[i];
//        // If the nested property doesn't exist, create a JObject for it
//        if (currentTarget[nestedProperty] == null)
//        {
//            currentTarget[nestedProperty] = new JObject();
//        }
//        currentTarget = (JObject)currentTarget[nestedProperty];  // Move to the next level
//    }

//    // Assign the value to the last property
//    currentTarget[targetProperties.Last()] = JToken.FromObject(sourceValue);
//}
//else
//{
//    // If it's a simple property, just assign directly
//    result[map.TargetProperty] = JToken.FromObject(sourceValue);
//}
