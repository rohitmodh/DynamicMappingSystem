using DynamicMappingSystem.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicMappingSystem.Mapping
{

    public class MappingConfigService
    {
        private readonly Dictionary<string, List<PropertyMapping>> _mappingConfigurations;

        public MappingConfigService(string configFilePath)
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

        // Get property mappings for a given sourceType and targetType
        public List<PropertyMapping> GetPropertyMappings(string sourceType, string targetType)
        {
            var key = $"{sourceType}:{targetType}";
            if (_mappingConfigurations.TryGetValue(key, out List<PropertyMapping>? value))
            {
                return value;
            }

            throw new MappingNotFoundException($"Mappings for {sourceType} -> {targetType} not found.");
        }
    }
}
