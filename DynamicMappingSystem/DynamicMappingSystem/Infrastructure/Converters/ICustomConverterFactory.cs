namespace DynamicMappingSystem.Infrastructure.Converters
{
    public interface ICustomConverterFactory
    {
        ICustomConverter GetConverter(string? converterName);
    }
}
