using DynamicMappingSystem.Application.Mapping;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Represents a strategy interface for applying specific types of property mappings
    /// (e.g., flat-to-flat, flat-to-array<object>, array<object>-to-flat, array<object>-to-array<object>).
    /// </summary>
    public interface IMappingStrategy
    {
        /// <summary>
        /// Determines whether this strategy can handle the provided set of property mappings and source token.
        /// </summary>
        /// <param name="mappings">A collection of property mappings to evaluate.</param>
        /// <param name="sourceToken">The source JSON token being mapped.</param>
        /// <returns>True if the strategy is applicable; otherwise, false.</returns>
        bool CanHandle(IEnumerable<PropertyMapping> mappings, JToken sourceToken);

        /// <summary>
        /// Applies the mapping strategy to transform the source data into the target format.
        /// </summary>
        /// <param name="mappings">The collection of property mappings to apply.</param>
        /// <param name="sourceData">The source JSON data to map from.</param>
        /// <param name="result">The target JSON object where mapped data should be written.</param>
        /// <param name="inputData">Optional: the full original input data, useful for context if needed by certain strategies.</param>
        void Apply(IEnumerable<PropertyMapping> mappings, JToken sourceData, JObject result, JObject? inputData = null);
    }

}
