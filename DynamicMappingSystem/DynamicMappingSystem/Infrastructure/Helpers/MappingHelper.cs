using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Infrastructure.Helpers
{
    /// <summary>
    /// Provides helper methods for setting nested properties and normalizing source paths within JSON objects.
    /// </summary>
    public static class MappingHelper
    {
        /// <summary>
        /// Sets a nested property within a <see cref="JObject"/> using a dot-separated path.
        /// Creates any missing intermediate objects as needed.
        /// </summary>
        /// <param name="obj">The target <see cref="JObject"/> to modify.</param>
        /// <param name="path">The dot-separated path to the nested property (e.g., "Guest.Name.First").</param>
        /// <param name="value">The <see cref="JToken"/> value to set at the specified path.</param>
        public static void SetNestedProperty(JObject obj, string path, JToken value)
        {
            var parts = path.Split('.');
            JToken current = obj;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var part = parts[i];
                if (current[part] == null || current[part].Type != JTokenType.Object)
                {
                    current[part] = new JObject();
                }
                current = current[part];
            }

            var finalPart = parts.Last();

            // Merge logic for arrays and objects
            if (current[finalPart] is JArray existingArray && value is JArray newArray)
            {
                foreach (var item in newArray)
                    existingArray.Add(item);
            }
            else if (current[finalPart] is JObject existingObj && value is JObject newObj)
            {
                existingObj.Merge(newObj, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }
            else
            {
                current[finalPart] = value;
            }
        }

        /// <summary>
        /// Normalizes a source property path to include wildcard array access (e.g., "Guests.Name" → "Guests[*].Name")
        /// if the corresponding token in the input data is an array.
        /// </summary>
        /// <param name="inputData">The input <see cref="JObject"/> to inspect.</param>
        /// <param name="sourcePath">The source property path to normalize.</param>
        /// <returns>A normalized path with array wildcards where applicable.</returns>
        public static string NormalizeSourcePath(JObject inputData, string sourcePath)
        {
            if (sourcePath.Contains("[*]"))
                return sourcePath;

            var firstSegment = sourcePath.Split('.')[0];
            var token = inputData.SelectToken(firstSegment);

            if (token is JArray)
            {
                var rest = sourcePath.Substring(firstSegment.Length + 1); // skip dot
                return $"{firstSegment}[*].{rest}";
            }

            return sourcePath;
        }
    }
}
