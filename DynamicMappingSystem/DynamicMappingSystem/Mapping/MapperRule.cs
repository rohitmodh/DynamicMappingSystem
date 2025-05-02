namespace DynamicMappingSystem.Mapping;

public class MapperRule
{
    public string SourceType { get; set; }
    public string TargetType { get; set; }
    public string? Version { get; set; }
    public List<PropertyMapping> PropertyMappings { get; set; } = new();
}
