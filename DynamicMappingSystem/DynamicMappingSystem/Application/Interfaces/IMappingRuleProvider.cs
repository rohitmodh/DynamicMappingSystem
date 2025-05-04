using DynamicMappingSystem.Application.Mapping;

namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Defines a provider interface for retrieving mapping rules between source and target types.
    /// </summary>
    public interface IMappingRuleProvider
    {
        /// <summary>
        /// Retrieves the list of property mappings between a given source type and target type.
        /// </summary>
        /// <param name="sourceType">The name of the source type to map from.</param>
        /// <param name="targetType">The name of the target type to map to.</param>
        /// <returns>A list of <see cref="PropertyMapping"/> objects that define the mapping rules.</returns>
        List<PropertyMapping> GetPropertyMappings(string sourceType, string targetType);
    }
}
