using DynamicMappingSystem.Infrastructure.Providers;

namespace DynamicMappingSystem.Application.Interfaces
{
    /// <summary>
    /// Provides functionality to retrieve data format definitions based on a given type.
    /// </summary>
    public interface IFormatConfigProvider
    {
        /// <summary>
        /// Retrieves the <see cref="DataFormatDefinition"/> associated with the specified type.
        /// </summary>
        /// <param name="type">The identifier of the format type to retrieve.</param>
        /// <returns>The <see cref="DataFormatDefinition"/> corresponding to the specified type.</returns>
        DataFormatDefinition GetFormat(string type);
    }
}
