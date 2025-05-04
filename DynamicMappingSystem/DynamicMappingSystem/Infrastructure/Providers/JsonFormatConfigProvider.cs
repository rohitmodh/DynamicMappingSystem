using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using System.Text.Json;

namespace DynamicMappingSystem.Infrastructure.Providers
{
    /// <summary>
    /// Provides configuration for data formats by loading JSON-defined format configurations.
    /// Implements the <see cref="IFormatConfigProvider"/> interface.
    /// </summary>
    public class JsonFormatConfigProvider : IFormatConfigProvider
    {
        /// <summary>
        /// The list of data format definitions loaded from the JSON configuration.
        /// </summary>
        private readonly List<DataFormatDefinition> _formats;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatConfigProvider"/> class.
        /// Reads and deserializes the JSON file to load format configurations.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing format definitions.</param>
        public JsonFormatConfigProvider(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            _formats = JsonSerializer.Deserialize<List<DataFormatDefinition>>(json)!;
        }

        /// <summary>
        /// Retrieves the data format definition for a specific type.
        /// </summary>
        /// <param name="type">The type of the data format to retrieve.</param>
        /// <returns>The matching <see cref="DataFormatDefinition"/> for the specified type.</returns>
        /// <exception cref="MappingException">
        /// Thrown when no matching format definition is found for the specified type.
        /// </exception>
        public DataFormatDefinition GetFormat(string type)
        {
            return _formats.FirstOrDefault(f => f.Type == type)
                   ?? throw new MappingException(code: MappingErrorCodes.MissingFormat, message: $"Format definition not found for type '{type}'");
        }
    }
}
