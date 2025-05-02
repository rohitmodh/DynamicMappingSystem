namespace DynamicMappingSystem.Providers
{
    public interface IFormatConfigProvider
    {
        DataFormatDefinition GetFormat(string type);
    }
}
