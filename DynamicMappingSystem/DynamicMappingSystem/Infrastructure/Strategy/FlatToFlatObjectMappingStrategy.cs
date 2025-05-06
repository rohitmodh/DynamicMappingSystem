using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using DynamicMappingSystem.Infrastructure.Helpers;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Strategy
{
    /// <summary>
    /// Mapping strategy for transforming flat source data to flat target data.
    /// </summary>
    public class FlatToFlatObjectMappingStrategy : IMappingStrategy
    {
        /// <summary>
        /// Determines if the strategy can handle the given property mappings and source token.
        /// This strategy is valid for mappings where neither source nor target contains array elements (i.e., flat-to-flat).
        /// </summary>
        /// <param name="mappings">A collection of property mappings.</param>
        /// <param name="sourceToken">The source data to be mapped.</param>
        /// <returns>True if the strategy can handle the mappings, otherwise false.</returns>
        public bool CanHandle(IEnumerable<PropertyMapping> mappings, JToken sourceToken)
            => mappings.All(m => !m.SourceProperty.Contains("[*]") && !m.TargetProperty.Contains("[*]"));

        /// <summary>
        /// Applies the flat-to-flat mapping strategy, mapping source data to the target object.
        /// This method will map each source property to the corresponding target property.
        /// </summary>
        /// <param name="mappings">A collection of property mappings.</param>
        /// <param name="sourceData">The source data to be mapped.</param>
        /// <param name="result">The target object that will hold the mapped data.</param>
        /// <param name="inputData">Optional: The full input data, which can be useful for context or validation.</param>
        /// <exception cref="MappingException">Thrown if a source property is missing or if setting the target property fails.</exception>
        public void Apply(IEnumerable<PropertyMapping> mappings, JToken sourceData, JObject result, JObject? inputData = null)
        {
            foreach (var mapping in mappings)
            {
                var value = GetPropertyValue(sourceData, mapping.SourceProperty);
                if (value == null)
                {
                    throw new MappingException(MappingErrorCodes.MissingSourceProperty,
                        $"Source property '{mapping.SourceProperty}' is missing.");
                }

                try
                {
                    SetPropertyValue(result, mapping.TargetProperty, value);
                }
                catch (Exception ex)
                {
                    throw new MappingException(
                        MappingErrorCodes.SetTargetPropertyFailed,
                        $"Failed to set target property '{mapping.TargetProperty}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Retrieves the value from the source data based on the specified path.
        /// </summary>
        /// <param name="source">The source data token.</param>
        /// <param name="path">The path to the property to retrieve.</param>
        /// <returns>The value of the property, or null if not found.</returns>
        private JToken? GetPropertyValue(JToken source, string path)
        {
            return source.SelectToken(path);
        }

        /// <summary>
        /// Sets a value on the target object based on the provided path. Handles both flat and array mappings.
        /// </summary>
        /// <param name="obj">The target object to set the value on.</param>
        /// <param name="path">The path to the property on the target object.</param>
        /// <param name="value">The value to set on the target object.</param>
        /// <exception cref="MappingException">Thrown if the path is invalid or if the value cannot be set.</exception>
        private static void SetPropertyValue(JObject obj, string path, JToken value)
        {
            if (path.Contains("[*]."))
            {
                var segments = path.Split(new[] { "[*]." }, StringSplitOptions.None);

                if (segments.Length == 2 && value is JArray valueArray)
                {
                    var arrayName = segments[0]; // e.g., "Guests"
                    var subPropertyPath = segments[1]; // e.g., "Name"

                    if (obj[arrayName] is not JArray targetArray)
                    {
                        targetArray = new JArray();
                        obj[arrayName] = targetArray;
                    }

                    for (int i = 0; i < valueArray.Count; i++)
                    {
                        // Ensure item exists in target array
                        if (targetArray.Count <= i)
                        {
                            targetArray.Add(new JObject());
                        }

                        if (targetArray[i] is not JObject item)
                        {
                            item = new JObject();
                            targetArray[i] = item;
                        }

                        MappingHelper.SetNestedProperty(item, subPropertyPath, valueArray[i]);
                    }

                    return;
                }

                throw new MappingException(MappingErrorCodes.InvalidData, $"Unsupported collection path '{path}' or expected an array.");
            }
            else
            {
                MappingHelper.SetNestedProperty(obj, path, value);
            }
        }
    }
}