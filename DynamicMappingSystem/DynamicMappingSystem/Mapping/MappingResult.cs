namespace DynamicMappingSystem.Mapping
{
    public class MappingResult
    {
        public bool Success { get; set; }
        public object? MappedData { get; set; }
        public List<MappingError> Errors { get; set; } = new();
    }
}
