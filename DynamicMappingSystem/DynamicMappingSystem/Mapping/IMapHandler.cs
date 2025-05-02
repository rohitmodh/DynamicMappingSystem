namespace DynamicMappingSystem.Mapping
{
    public interface IMapHandler
    {
        MappingResult Map(object data, string sourceType, string targetType, string? version = null);
    }
}
