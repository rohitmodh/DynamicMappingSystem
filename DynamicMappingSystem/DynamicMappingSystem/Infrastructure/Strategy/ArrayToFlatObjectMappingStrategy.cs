using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Infrastructure.Helpers;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Strategy
{
    /// <summary>
    /// A mapping strategy that handles conversion from an array of objects to flat arrays.
    /// For example: Guests[*].Name → Guest.Names
    /// </summary>
    public class ArrayToFlatObjectMappingStrategy : IMappingStrategy
    {
        /// <summary>
        /// Determines whether this strategy can handle the given mappings and source token.
        /// This strategy applies when the source is an array of objects and the target properties are flat (i.e., do not contain wildcards).
        /// </summary>
        /// <param name="mappings">The collection of property mappings.</param>
        /// <param name="sourceToken">The source JSON token (should be a JArray of objects).</param>
        /// <returns>True if the strategy can handle the given inputs; otherwise, false.</returns>
        public bool CanHandle(IEnumerable<PropertyMapping> mappings, JToken sourceToken)
            => sourceToken is JArray array && array.All(t => t.Type == JTokenType.Object)
               && mappings.All(m => !m.TargetProperty.Contains("[*]"));

        /// <summary>
        /// Applies the array-to-flat mapping strategy.
        /// Extracts values from each object in the array and flattens them into separate target arrays.
        /// </summary>
        /// <param name="mappings">The list of property mappings to apply.</param>
        /// <param name="sourceData">The source JSON array to map from.</param>
        /// <param name="result">The target JSON object where the mapped data will be added.</param>
        /// <param name="inputData">Optional: full input data for context, not used in this strategy.</param>
        public void Apply(IEnumerable<PropertyMapping> mappings, JToken sourceData, JObject result, JObject? inputData = null)
        {
            var sourceArray = (JArray)sourceData;

            foreach (var mapping in mappings)
            {
                var subPath = mapping.SourceProperty.Split(new[] { "[*]." }, StringSplitOptions.None).Last();
                var values = sourceArray
                    .OfType<JObject>()
                    .Select(o => o.SelectToken(subPath))
                    .Where(v => v != null)
                    .ToList();

                MappingHelper.SetNestedProperty(result, mapping.TargetProperty, new JArray(values));
            }
        }
    }
}