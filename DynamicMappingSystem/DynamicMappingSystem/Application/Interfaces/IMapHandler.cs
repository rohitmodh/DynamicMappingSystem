using DynamicMappingSystem.Contracts.Responses;

namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Defines a contract for handling the mapping of data between different model types.
    /// </summary>
    public interface IMapHandler
    {
        /// <summary>
        /// Maps the given input data from a source type to a target type, optionally considering a specific version.
        /// </summary>
        /// <param name="data">The input data to be mapped.</param>
        /// <param name="sourceType">The type identifier of the source model.</param>
        /// <param name="targetType">The type identifier of the target model.</param>
        /// <param name="version">An optional version string to apply version-specific mapping logic.</param>
        /// <returns>A <see cref="MappingResult"/> containing the mapped data or error details.</returns>
        MappingResult Map(object data, string sourceType, string targetType, string? version = null);
    }
}
