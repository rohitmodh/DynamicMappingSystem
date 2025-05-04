using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Application.Mapping
{
    /// <summary>
    /// Handles dynamic mapping between source and target JSON data using configured mapping rules and validation logic.
    /// </summary>
    public class MapHandler : BaseMapHandler
    {
        private readonly IMappingRuleProvider _mappingRuleProvider;
        private readonly IEnumerable<IModelValidator> _validators;
        private readonly IFormatConfigProvider _formatConfigProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapHandler"/> class.
        /// </summary>
        /// <param name="mappingRuleProvider">Provider for mapping rules between source and target types.</param>
        /// <param name="validators">Collection of validators for input/output data.</param>
        /// <param name="formatConfigProvider">Provider for data format configuration.</param>
        public MapHandler(IMappingRuleProvider mappingRuleProvider,
            IEnumerable<IModelValidator> validators,
            IFormatConfigProvider formatConfigProvider)
        {
            _mappingRuleProvider = mappingRuleProvider;
            _validators = validators;
            _formatConfigProvider = formatConfigProvider;
        }

        /// <summary>
        /// Performs the mapping operation from source data to target format using defined mapping rules.
        /// </summary>
        /// <param name="data">Source data object (expected to be a JObject).</param>
        /// <param name="sourceType">Source data type identifier.</param>
        /// <param name="targetType">Target data type identifier.</param>
        /// <param name="version">Optional version identifier (not used in this implementation).</param>
        /// <returns>A JObject representing the mapped target data.</returns>
        /// <exception cref="MappingException">Thrown when data is invalid or mapping fails.</exception>
        protected override object PerformMapping(object data, string sourceType, string targetType, string? version)
        {
            if (data is not JObject inputData)
            {
                throw new MappingException(code: MappingErrorCodes.InvalidData, message: "Input data is not a valid JSON object.");
            }

            var mappings = _mappingRuleProvider.GetPropertyMappings(sourceType, targetType);

            if (mappings == null || !mappings.Any())
            {
                throw new MappingNotFoundException(code: MappingErrorCodes.MappingRuleNotFound, message: $"No mappings found for {sourceType} → {targetType}");
            }

            var result = new JObject();

            foreach (var mapping in mappings)
            {
                var value = GetPropertyValue(inputData, mapping.SourceProperty)
                    ?? throw new MappingException(
                        code: MappingErrorCodes.MissingSourceProperty, message: $"Source property '{mapping.SourceProperty}' is missing in source.");

                try
                {
                    SetPropertyValue(result, mapping.TargetProperty, value);
                }
                catch (Exception ex)
                {
                    throw new MappingException(
                        code: MappingErrorCodes.SetTargetPropertyFailed, message: $"Failed to set target property '{mapping.TargetProperty}': {ex.Message}"
                    );
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the input data against the expected format for the specified source type.
        /// </summary>
        /// <param name="data">Input data to validate.</param>
        /// <param name="sourceType">Source data type identifier.</param>
        /// <param name="targetType">Target data type identifier (not used here).</param>
        /// <exception cref="ValidationMappingException">Thrown when validation fails.</exception>
        protected override void ValidateInput(object data, string sourceType, string targetType)
        {
            if (data is not JObject inputData)
                throw new ValidationMappingException(MappingErrorCodes.ValidationFailed, "Invalid source data format. The source data does not match the required format.");

            ValidateDataFormat(inputData, sourceType, isInput: true);
        }

        /// <summary>
        /// Validates the output data against the expected format for the specified target type.
        /// </summary>
        /// <param name="result">Mapped output data to validate.</param>
        /// <param name="targetType">Target data type identifier.</param>
        /// <exception cref="ValidationMappingException">Thrown when validation fails.</exception>
        protected override void ValidateOutput(object result, string targetType)
        {
            if (result is not JObject outputData)
                throw new ValidationMappingException(MappingErrorCodes.ValidationFailed, "Invalid target data format. The target data does not match the required format.");

            ValidateDataFormat(outputData, targetType, isInput: false);
        }

        /// <summary>
        /// Validates a JObject's structure and content based on format configuration.
        /// </summary>
        /// <param name="data">The JObject to validate.</param>
        /// <param name="type">Type identifier for format lookup.</param>
        /// <param name="isInput">True if validating input data, false if output.</param>
        /// <exception cref="MappingException">Thrown when format configuration is missing.</exception>
        /// <exception cref="ValidationMappingException">Thrown when validation fails.</exception>
        private void ValidateDataFormat(JObject data, string type, bool isInput)
        {
            var format = _formatConfigProvider.GetFormat(type)
                ?? throw new MappingException(MappingErrorCodes.MissingFormat, $"Format definition not found for type '{type}'");

            var dataContext = isInput ? "source" : "target";

            foreach (var propertyPath in format.Properties)
            {
                foreach (var validator in _validators)
                {
                    if (!validator.Validate(data, propertyPath, dataContext, out var errorMessage))
                    {
                        throw new ValidationMappingException(MappingErrorCodes.ValidationFailed,
                            errorMessage ?? $"Property missing: {propertyPath} not found in {dataContext} data.");
                    }
                }
            }

            var allowedPaths = format.Properties.ToHashSet();
            ValidateExtraProperties(data, allowedPaths, dataContext);
        }

        /// <summary>
        /// Handles exceptions thrown during mapping and logs errors to the console.
        /// </summary>
        /// <param name="ex">The exception thrown during mapping.</param>
        /// <param name="sourceType">Source type involved in the mapping.</param>
        /// <param name="targetType">Target type involved in the mapping.</param>
        protected override void HandleError(Exception ex, string sourceType, string targetType)
        {
            Console.Error.WriteLine($"[ERROR] Mapping {sourceType} → {targetType} failed: {ex.Message}");
        }

        /// <summary>
        /// Retrieves a property value from a JObject using a dot-separated path.
        /// </summary>
        /// <param name="obj">JObject to query.</param>
        /// <param name="path">Dot-separated path to the property.</param>
        /// <returns>The property value as a JToken, or null if not found.</returns>
        private static JToken? GetPropertyValue(JObject obj, string path)
        {
            var token = obj.SelectToken(path);
            return token;
        }

        /// <summary>
        /// Sets a property value in a JObject using a dot-separated path. Creates nested objects if needed.
        /// </summary>
        /// <param name="obj">The JObject to modify.</param>
        /// <param name="path">Dot-separated path where the value should be set.</param>
        /// <param name="value">The value to set.</param>
        private static void SetPropertyValue(JObject obj, string path, JToken value)
        {
            var parts = path.Split('.');
            JObject current = obj;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (current[parts[i]] == null)
                {
                    current[parts[i]] = new JObject();
                }
                current = (JObject)current[parts[i]]!;
            }

            current[parts.Last()] = value;
        }

        /// <summary>
        /// Validates that there are no unexpected properties in the provided data.
        /// </summary>
        /// <param name="root">The JObject to check for unexpected properties.</param>
        /// <param name="allowedPaths">Set of allowed property paths.</param>
        /// <param name="context">Context string for error messages ("source" or "target").</param>
        /// <exception cref="ValidationMappingException">Thrown when unexpected properties are found.</exception>
        private static void ValidateExtraProperties(JObject root, HashSet<string> allowedPaths, string context)
        {
            var stack = new Stack<(JToken token, string path)>();
            stack.Push((root, ""));

            while (stack.Count > 0)
            {
                var (token, path) = stack.Pop();

                if (token.Type == JTokenType.Object)
                {
                    foreach (var prop in ((JObject)token).Properties())
                    {
                        var fullPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                        stack.Push((prop.Value, fullPath));
                    }
                }
                else
                {
                    if (!allowedPaths.Contains(path))
                    {
                        throw new ValidationMappingException(
                            MappingErrorCodes.ValidationFailed,
                            $"Unexpected property: {path} found in {context} data.");
                    }
                }
            }
        }
    }
}
