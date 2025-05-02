namespace DynamicMappingSystem.Converters
{
    public interface ICustomConverterFactory
    {
        ICustomConverter GetConverter(string? converterName);
    }
}
