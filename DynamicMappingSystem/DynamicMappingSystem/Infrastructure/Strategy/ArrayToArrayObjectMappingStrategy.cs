using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Infrastructure.Helpers;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Strategy
{
    /// <summary>
    /// Strategy for handling mappings from a JSON array of objects to another JSON array of objects,
    /// e.g., mapping `Guests[*].Name` to `Attendees[*].FullName`.
    /// </summary>
    public class ArrayToArrayObjectMappingStrategy : IMappingStrategy
    {
        /// <summary>
        /// Determines whether this strategy can handle the given property mappings and source token.
        /// This strategy is applicable when:
        /// - The source token is a JSON array.
        /// - All elements in the array are objects.
        /// - All target property paths contain the wildcard token "[*].".
        /// </summary>
        /// <param name="mappings">The property mappings to evaluate.</param>
        /// <param name="sourceToken">The JSON token from the source data.</param>
        /// <returns>True if this strategy can handle the given data; otherwise, false.</returns>
        public bool CanHandle(IEnumerable<PropertyMapping> mappings, JToken sourceToken)
        {
            return sourceToken is JArray array && array.All(t => t.Type == JTokenType.Object)
                   && mappings.All(m => m.TargetProperty.Contains("[*]."));
        }

        /// <summary>
        /// Applies the mapping logic to transform a JSON array of objects to another array of objects
        /// based on the specified property mappings.
        /// </summary>
        /// <param name="mappings">A collection of property mappings defining how to transform data.</param>
        /// <param name="sourceData">The source JSON array of objects.</param>
        /// <param name="result">The resulting JSON object to populate with the mapped array.</param>
        /// <param name="inputData">Optional full input JSON data, used for broader context if needed.</param>
        public void Apply(IEnumerable<PropertyMapping> mappings, JToken sourceData, JObject result, JObject? inputData = null)
        {
            var sourceArray = (JArray)sourceData;
            var outputArray = new JArray();

            foreach (var item in sourceArray.OfType<JObject>())
            {
                var outputItem = new JObject();
                foreach (var mapping in mappings)
                {
                    var subPath = mapping.SourceProperty.Split(new[] { "[*]." }, StringSplitOptions.None).Last();
                    var targetSubPath = mapping.TargetProperty.Split(new[] { "[*]." }, StringSplitOptions.None).Last();
                    var value = item.SelectToken(subPath);
                    if (value != null)
                        MappingHelper.SetNestedProperty(outputItem, targetSubPath, value);
                }

                outputArray.Add(outputItem);
            }

            var arrayPath = mappings.First().TargetProperty.Split("[*].")[0];
            MappingHelper.SetNestedProperty(result, arrayPath, outputArray);
        }
    }
}
