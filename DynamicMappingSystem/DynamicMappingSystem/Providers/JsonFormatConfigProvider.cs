using DynamicMappingSystem.Exceptions;
using System.Text.Json;

namespace DynamicMappingSystem.Providers
{
    public class JsonFormatConfigProvider: IFormatConfigProvider
    {
        private readonly List<DataFormatDefinition> _formats;

        public JsonFormatConfigProvider(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            _formats = JsonSerializer.Deserialize<List<DataFormatDefinition>>(json)!;
        }

        public DataFormatDefinition GetFormat(string type)
        {
            return _formats.FirstOrDefault(f => f.Type == type)
                   ?? throw new MappingException("InvalidFormat", $"Format definition not found for type '{type}'");
        }
    }
}
