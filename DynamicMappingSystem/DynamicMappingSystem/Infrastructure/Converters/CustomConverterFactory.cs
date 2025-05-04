namespace DynamicMappingSystem.Infrastructure.Converters
{
    public class CustomConverterFactory : ICustomConverterFactory
    {
        public ICustomConverter GetConverter(string? converterName)
        {
            return new DefaultConverter(); // We can add some functionlity here to return different converters based on the name
        }
    }
}
