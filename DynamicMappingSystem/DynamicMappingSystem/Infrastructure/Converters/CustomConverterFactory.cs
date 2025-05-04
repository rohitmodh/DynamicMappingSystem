namespace DynamicMappingSystem.Infrastructure.Converters
{
    public class CustomConverterFactory : ICustomConverterFactory
    {
        public ICustomConverter GetConverter(string? converterName)
        {
            return new DefaultConverter(); // Stub, plug custom logic or reflection here
        }
    }
}
