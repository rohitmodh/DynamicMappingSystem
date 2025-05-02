namespace DynamicMappingSystem.Mapping
{
    public class PropertyMapping
    {
        public string SourceProperty { get; set; }
        public string TargetProperty { get; set; }
        public string? CustomConverter { get; set; }
    }
}
