using DynamicMappingSystem.Exceptions;
using DynamicMappingSystem.Providers;
using DynamicMappingSystem.Validation;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Mapping
{
    public class MapHandler : BaseMapHandler
    {
        private readonly MappingConfigService _mappingConfigService;
        private readonly IMappingRuleProvider _ruleProvider;
        private readonly IEnumerable<IModelValidator> _validators;
        private readonly IFormatConfigProvider _formatConfigProvider;

        public MapHandler(IMappingRuleProvider ruleProvider,
            MappingConfigService mappingConfigService,
            IEnumerable<IModelValidator> validators,
            IFormatConfigProvider formatConfigProvider
            )
        {
            _ruleProvider = ruleProvider;
            _mappingConfigService = mappingConfigService;
            _validators = validators;
            _formatConfigProvider = formatConfigProvider;

        }

        //private object GetPropertyValue(JObject inputData, string sourceProperty)
        //{
        //    var propertyParts = sourceProperty.Split('.'); // Split the source property into parts for nested access
        //    JToken currentToken = inputData;

        //    foreach (var part in propertyParts)
        //    {
        //        // Navigate through nested properties
        //        if (currentToken is JObject obj && obj[part] != null)
        //        {
        //            currentToken = obj[part];
        //        }
        //        else if (currentToken is JArray array)
        //        {
        //            // Handle array if needed, for now we'll just return the first element as an example
        //            currentToken = array.First;
        //        }
        //        else
        //        {
        //            return null; // If property is not found, return null
        //        }
        //    }

        //    // Return the final value after navigating through the nested structure
        //    return currentToken?.ToObject<object>();
        //}

        //private bool IsValid(object sourceValue, string sourceProperty)
        //{
        //    // Convert sourceValue to JObject to easily navigate through properties
        //    var sourceData = sourceValue as JObject;

        //    if (sourceData == null)
        //    {
        //        return false;
        //    }

        //    // Split the sourceProperty string (e.g., "Customer.Name") into individual property names
        //    var properties = sourceProperty.Split('.');

        //    JToken currentToken = sourceData;
        //    foreach (var property in properties)
        //    {
        //        // If the property is not found in the current token, return false
        //        if (currentToken is JObject currentObject && currentObject[property] != null)
        //        {
        //            currentToken = currentObject[property];
        //        }
        //        else if (currentToken is JArray currentArray && int.TryParse(property, out var index) && currentArray.Count > index)
        //        {
        //            currentToken = currentArray[index];
        //        }
        //        else
        //        {
        //            return false; // Property doesn't exist at the specified path
        //        }
        //    }

        //    return true; // If we traverse through all properties and find the final value, it's valid
        //}

        //protected override void ValidateData(object data, string sourceType, string targetType, bool isSourceValidation) // true = validate input, false = validate output
        //{
        //    var jObj = data as JObject ?? JObject.FromObject(data);
        //    if (jObj == null)
        //    {
        //        throw new ValidationMappingException("Invalid data format.");
        //    }

        //    var mappings = _mappingConfigService.GetPropertyMappings(sourceType, targetType);
        //    var dataContext = isSourceValidation ? "source" : "target";

        //    foreach (var mapping in mappings)
        //    {
        //        var propertyPath = isSourceValidation ? mapping.SourceProperty : mapping.TargetProperty;

        //        foreach (var validator in _validators)
        //        {
        //                if (!validator.Validate(jObj, propertyPath, dataContext, out var errorMessage))
        //            {
        //                throw new ValidationMappingException(errorMessage ?? $"Invalid property: {propertyPath} not found in {dataContext} data.");
        //            }
        //        }
        //    }
        //}

        //protected override void ValidateInput(object data, string sourceType, string targetType)
        //{
        //    var inputData = data as JObject;
        //    if (inputData == null)
        //    {
        //        throw new FluentValidation.ValidationException("Invalid input data format.");
        //    }

        //    // Get property mappings dynamically from the configuration service
        //    var mappings = _mappingConfigService.GetPropertyMappings(sourceType, targetType);

        //    foreach (var mapping in mappings)
        //    {
        //        var sourceValue = GetPropertyValue(inputData, mapping.SourceProperty);
        //        if (sourceValue == null)
        //        {
        //            throw new FluentValidation.ValidationException($"Missing required source property: {mapping.SourceProperty}");
        //        }

        //        // Apply custom converter if needed
        //        //if (mapping.CustomConverter != null)
        //        //{
        //        //    sourceValue = ApplyCustomConverter(sourceValue, mapping.CustomConverter);
        //        //}

        //        if (!IsValid(inputData, mapping.SourceProperty))
        //        {
        //            throw new FluentValidation.ValidationException($"Invalid value for source property: {mapping.SourceProperty}");
        //        }
        //    }
        //}

        //protected override object PerformMapping(object data, string sourceType, string targetType, string? version)
        //{
        //    var rule = _ruleProvider.GetRules()
        //        .FirstOrDefault(r => r.SourceType == sourceType && r.TargetType == targetType && (version == null || r.Version == version))
        //        ?? throw new MappingNotFoundException($"No mapping rule found for {sourceType} -> {targetType} (version: {version ?? "latest"})");

        //    var sourceObj = data is JObject jObj ? jObj : JObject.FromObject(data);
        //    var flatSource = sourceObj.Flatten(); // Use the flattening method
        //    var result = new JObject();

        //    foreach (var map in rule.PropertyMappings)
        //    {
        //        if (!flatSource.TryGetValue(map.SourceProperty, out var sourceValue))
        //        {
        //            Console.WriteLine($"Property '{map.SourceProperty}' not found.");
        //            continue;
        //        }
        //        result.SetNestedValue(map.TargetProperty, sourceValue);
        //    }

        //    return result!;
        //}
        protected override object PerformMapping(object data, string sourceType, string targetType, string? version)
        {
            if (data is not JObject inputData)
            {
                throw new MappingException(code: "InvalidInput", message: "Input data is not a valid JSON object.");
            }

            var mappings = _mappingConfigService.GetPropertyMappings(sourceType, targetType);

            if (mappings == null || !mappings.Any())
            {
                throw new MappingNotFoundException($"No mappings found for {sourceType} → {targetType}");
            }

            var result = new JObject();

            foreach (var mapping in mappings)
            {
                var value = GetPropertyValue(inputData, mapping.SourceProperty)
                    ?? throw new MappingException(
                        code: MappingErrorCodes.MissingSourceProperty, message: $"Source property '{mapping.SourceProperty}' is missing in input.");

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

        protected override void ValidateInput(object data, string sourceType, string targetType)
        {
            if (data is not JObject inputData)
                throw new ValidationMappingException("Invalid source data format. The source data does not match the required format.");

            ValidateDataFormat(inputData, sourceType, isInput: true);
        }

        protected override void ValidateOutput(object result, string targetType)
        {
            if (result is not JObject outputData)
                throw new ValidationMappingException("Invalid target data format. The target data does not match the required format.");

            ValidateDataFormat(outputData, targetType, isInput: false);
        }

        private void ValidateDataFormat(JObject data, string type, bool isInput)
        {
            var format = _formatConfigProvider.GetFormat(type);

            var dataContext = isInput ? "source" : "target";

            foreach (var propertyPath in format.Properties)
            {
                foreach (var validator in _validators)
                {
                    if (!validator.Validate(data, propertyPath, dataContext, out var errorMessage))
                    {
                        throw new ValidationMappingException(
                            errorMessage ?? $"Invalid property: {propertyPath} not found in {dataContext} data.");
                    }
                }
            }

        }

        protected override void HandleError(Exception ex, string sourceType, string targetType)
        {
            Console.Error.WriteLine($"[ERROR] Mapping {sourceType} → {targetType} failed: {ex.Message}");
        }

        private static JToken? GetPropertyValue(JObject obj, string path)
        {
            var token = obj.SelectToken(path);
            return token;
        }
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

    }
}