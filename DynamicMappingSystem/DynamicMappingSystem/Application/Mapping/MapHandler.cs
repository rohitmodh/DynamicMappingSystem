using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using DynamicMappingSystem.Infrastructure.Strategy;
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
        private readonly MappingEngine _mappingEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapHandler"/> class.
        /// </summary>
        /// <param name="mappingRuleProvider">Provider for mapping rules between source and target types.</param>
        /// <param name="validators">Collection of validators for input/output data.</param>
        /// <param name="formatConfigProvider">Provider for data format configuration.</param>
        /// <param name="mappingEngine">Mapping engine instance for choosing strategy.</param>
        public MapHandler(IMappingRuleProvider mappingRuleProvider,
                          IEnumerable<IModelValidator> validators,
                          IFormatConfigProvider formatConfigProvider,
                          MappingEngine mappingEngine)
        {
            _mappingRuleProvider = mappingRuleProvider;
            _validators = validators;
            _formatConfigProvider = formatConfigProvider;
            _mappingEngine = mappingEngine;
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

            try
            {
                     return _mappingEngine.Map(mappings, inputData);
            }
            catch (Exception ex)
            {
                throw new MappingException(MappingErrorCodes.PropertyMappingFailed, $"Mapping failed: {ex.Message}");
            }
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

            foreach (var propertyPath in format.Properties.Where(p => !p.Contains("[*]")))
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
            ValidateArrayStructure(data, format.Properties, dataContext);
            var allowedPaths = ExpandWildcards(format.Properties, data).ToHashSet();
        }

        /// <summary>
        /// Validates an array structure based on format configuration for missing / extra properties.
        /// </summary>
        /// <param name="data">The JObject to validate.</param>
        /// <param name="expectedPaths">path to check in JObject</param>
        /// <param name="context">True if validating input data, false if output.</param>
        /// <exception cref="ValidationMappingException">Thrown when validation fails.</exception>
        private static void ValidateArrayStructure(JObject data, IEnumerable<string> expectedPaths, string context)
        {
            var groupedPaths = expectedPaths
                .Where(p => p.Contains("[*]"))
                .GroupBy(p => p.Split(new[] { "[*]." }, StringSplitOptions.None)[0]); // e.g., Guests

            foreach (var group in groupedPaths)
            {
                var basePath = group.Key; // "Guests"
                var properties = group.Select(p => p.Split(new[] { "[*]." }, StringSplitOptions.None)[1]).ToList();

                var arrayToken = data.SelectToken(basePath);

                if (arrayToken is not JArray array)
                {
                    throw new ValidationMappingException(MappingErrorCodes.ValidationFailed,
                        $"Expected array at path '{basePath}' in {context} data.");
                }

                for (int i = 0; i < array.Count; i++)
                {
                    var item = array[i] as JObject;
                    if (item == null)
                    {
                        throw new ValidationMappingException(MappingErrorCodes.ValidationFailed,
                            $"Expected object at '{basePath}[{i}]' in {context} data.");
                    }

                    var itemProps = item.Properties().Select(p => p.Name).ToHashSet();

                    // Check for missing properties
                    foreach (var expectedProp in properties)
                    {
                        if (!itemProps.Contains(expectedProp))
                        {
                            throw new ValidationMappingException(MappingErrorCodes.ValidationFailed,
                                $"Missing property '{expectedProp}' in '{basePath}[{i}]' in {context} data.");
                        }
                    }

                    // Check for extra properties
                    foreach (var actualProp in itemProps)
                    {
                        if (!properties.Contains(actualProp))
                        {
                            throw new ValidationMappingException(MappingErrorCodes.ValidationFailed,
                                $"Unexpected property '{actualProp}' in '{basePath}[{i}]' in {context} data.");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Expands wildcard array paths (e.g., "Guests[*].Name") into explicit paths with indices
        /// (e.g., "Guests[0].Name", "Guests[1].Name") based on the actual data in the provided <paramref name="data"/>.
        /// </summary>
        /// <param name="paths">A collection of JSON paths, potentially containing wildcard array references.</param>
        /// <param name="data">The source <see cref="JObject"/> used to determine the array lengths for expansion.</param>
        /// <returns>
        /// A collection of expanded JSON paths with wildcards replaced by explicit indices,
        /// based on the structure of the input <paramref name="data"/>.
        /// </returns>
        private static IEnumerable<string> ExpandWildcards(IEnumerable<string> paths, JObject data)
        {
            var expanded = new List<string>();

            foreach (var path in paths)
            {
                if (!path.Contains("[*]"))
                {
                    expanded.Add(path);
                    continue;
                }

                var segments = path.Split('.');
                ExpandPath(data, segments, 0, "", expanded);
            }

            return expanded;
        }

        /// <summary>
        /// Recursively expands a JSON path containing wildcard segments (e.g., "[*]") into concrete paths
        /// with explicit array indices, based on the structure of the provided <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The current JSON token being examined in the recursion.</param>
        /// <param name="segments">An array of path segments split from the original wildcard path.</param>
        /// <param name="index">The current segment index being processed.</param>
        /// <param name="currentPath">The progressively built path as recursion proceeds.</param>
        /// <param name="results">A list of fully expanded paths accumulated during recursion.</param>
        private static void ExpandPath(JToken token, string[] segments, int index, string currentPath, List<string> results)
        {
            if (index >= segments.Length)
            {
                results.Add(currentPath.Trim('.'));
                return;
            }

            var segment = segments[index];

            if (segment == "[*]")
            {
                if (token is JArray array)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        ExpandPath(array[i], segments, index + 1, $"{currentPath}[{i}].", results);
                    }
                }
            }
            else if (segment.Contains("[*]"))
            {
                var collectionKey = segment.Split(new[] { "[*]" }, StringSplitOptions.None)[0];
                var childToken = token[collectionKey];

                if (childToken is JArray array)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        ExpandPath(array[i], segments, index + 1, $"{currentPath}{collectionKey}[{i}].", results);
                    }
                }
            }
            else
            {
                var nextToken = token[segment];
                if (nextToken != null)
                {
                    ExpandPath(nextToken, segments, index + 1, $"{currentPath}{segment}.", results);
                }
            }
        }
    }
}