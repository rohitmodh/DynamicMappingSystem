using DynamicMappingSystem.Mapping;
using System.Text.Json;

namespace DynamicMappingSystem.Providers
{
    public class JsonMappingRuleProvider : IMappingRuleProvider
    {
        private readonly string _filePath;

        public JsonMappingRuleProvider(string filePath)
        {
            _filePath = filePath;
        }

        public List<MapperRule> GetRules()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<MapperRule>>(json)!;
        }
    }
}
