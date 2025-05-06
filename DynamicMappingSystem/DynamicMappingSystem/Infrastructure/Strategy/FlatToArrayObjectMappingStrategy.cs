using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Infrastructure.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Strategy
{
    /// <summary>
    /// Strategy for mapping flat arrays into an array of objects structure
    /// (e.g., "Names" → "Guests[*].Name").
    /// </summary>
    public class FlatToArrayObjectMappingStrategy : IMappingStrategy
    {
        /// <summary>
        /// Determines whether this strategy can handle the specified mappings and source data.
        /// This strategy handles flat arrays where the target properties indicate an object array.
        /// </summary>
        /// <param name="mappings">The collection of property mappings.</param>
        /// <param name="sourceToken">The source token (expected to be a JArray of primitive values).</param>
        /// <returns>True if this strategy can handle the given mappings and data; otherwise, false.</returns>
        public bool CanHandle(IEnumerable<PropertyMapping> mappings, JToken sourceToken)
            => sourceToken is JArray array && !array.All(t => t.Type == JTokenType.Object)
                && mappings.All(m => m.TargetProperty.Contains("[*]"));

        /// <summary>
        /// Applies the flat-to-array-object mapping by transforming a flat array into an array of JSON objects,
        /// assigning each array value to the corresponding nested target property.
        /// </summary>
        /// <param name="mappings">The property mappings to apply.</param>
        /// <param name="sourceData">The actual source data token (should be a JArray of flat values).</param>
        /// <param name="result">The target object to populate with the transformed data.</param>
        /// <param name="inputData">Optional full input data context, used to resolve source paths correctly.</param>
        public void Apply(IEnumerable<PropertyMapping> mappings, JToken sourceData, JObject result, JObject? inputData = null)
        {
            var sampleArray = sourceData as JArray;
            if (sampleArray == null)
                return;

            int itemCount = sampleArray.Count;
            var outputArray = new JArray(Enumerable.Range(0, itemCount).Select(_ => new JObject()));

            foreach (var mapping in mappings)
            {
                var values = inputData?.SelectToken(MappingHelper.NormalizeSourcePath(inputData, mapping.SourceProperty)) as JArray;
                if (values == null)
                    continue;

                var subPath = mapping.TargetProperty.Split(new[] { "[*]." }, StringSplitOptions.None).Last();

                for (int i = 0; i < values.Count && i < outputArray.Count; i++)
                {
                    var obj = (JObject)outputArray[i];
                    MappingHelper.SetNestedProperty(obj, subPath, values[i]);
                }
            }

            var targetPath = mappings.First().TargetProperty.Split("[*].")[0];
            MappingHelper.SetNestedProperty(result, targetPath, outputArray);
        }
    }
}