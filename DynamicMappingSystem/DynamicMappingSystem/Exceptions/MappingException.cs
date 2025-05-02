namespace DynamicMappingSystem.Exceptions
{
    public class MappingException : Exception
    {
        public string Code { get; }

        public MappingException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
