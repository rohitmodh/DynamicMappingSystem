namespace DynamicMappingSystem.Providers
{
    public class DataFormatDefinition
    {
        public string Type { get; set; } = default!;
        public List<string> Properties { get; set; } = new();
    }
}
