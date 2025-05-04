using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Application.Mapping;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using Newtonsoft.Json;
using System.Text.Json;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Provides mapping rules loaded from a JSON configuration file.
    /// Implements the <see cref="IMappingRuleProvider"/> interface to retrieve property mappings between source and target types.
    /// </summary>
    public class JsonMappingRuleProvider : IMappingRuleProvider
    {
        private readonly string _filePath;
        private readonly Dictionary<string, List<PropertyMapping>> _mappingConfigurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMappingRuleProvider"/> class.
        /// Loads mapping configurations from the specified JSON file.
        /// </summary>
        /// <param name="configFilePath">The file path to the JSON configuration containing the mapping rules.</param>
        public JsonMappingRuleProvider(string configFilePath)
        {
            // Load the mappings from the external JSON configuration file
            var json = File.ReadAllText(configFilePath);
            var mappings = JsonConvert.DeserializeObject<List<MapperRule>>(json);

            // Group by SourceType and TargetType for easier access
            _mappingConfigurations = mappings
                .SelectMany(m => m.PropertyMappings
                    .Select(p => new { m.SourceType, m.TargetType, PropertyMapping = p }))
                .GroupBy(x => $"{x.SourceType}:{x.TargetType}")
                .ToDictionary(g => g.Key, g => g.Select(x => x.PropertyMapping).ToList());
        }

        /// <summary>
        /// Retrieves the property mappings for a given source and target type.
        /// </summary>
        /// <param name="sourceType">The source type for the mapping.</param>
        /// <param name="targetType">The target type for the mapping.</param>
        /// <returns>A list of <see cref="PropertyMapping"/> for the specified source and target types.</returns>
        /// <exception cref="MappingNotFoundException">
        /// Thrown when no mapping rules are found for the given source and target types.
        /// </exception>
        public List<PropertyMapping> GetPropertyMappings(string sourceType, string targetType)
        {
            var key = $"{sourceType}:{targetType}";
            if (_mappingConfigurations.TryGetValue(key, out List<PropertyMapping>? value))
            {
                return value;
            }

            throw new MappingNotFoundException(code: MappingErrorCodes.MappingRuleNotFound, message: $"Mappings for {sourceType} -> {targetType} not found.");
        }
    }
}
