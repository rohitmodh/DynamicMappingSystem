using DynamicMappingSystem.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DynamicMappingSystem.Mapping
{
    public abstract class BaseMapHandler : IMapHandler
    {
        public MappingResult Map(object data, string sourceType, string targetType, string? version = null)
        {
            var result = new MappingResult();
            try
            {
                ValidateInput(data, sourceType, targetType);

                var mapped = PerformMapping(data, sourceType, targetType, version);
                
                ValidateOutput(mapped, targetType);
             
                result.Success = true;
                result.MappedData = JsonSerializer.Deserialize<JsonElement>(mapped.ToString());
            }
            catch (ValidationMappingException ex)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.ValidationFailed, Message = ex.Message });
            }
            catch (MappingNotFoundException ex)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.MappingRuleNotFound, Message = ex.Message });
            }
            catch (MappingException ex)
            {
                result.Errors.Add(new MappingError { Code = ex.Code, Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.PropertyMappingFailed, Message = ex.Message });
            }
            catch (Exception ex)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.UnexpectedError, Message = ex.Message });
            }
            return result;
        }
        //protected abstract void ValidateData(object data, string sourceType, string targetType, bool isSourceValidation); // true = validate input, false = validate output

        protected abstract void ValidateInput(object data, string sourceType, string targetType);
        protected abstract object PerformMapping(object data, string sourceType, string targetType, string? version);
        protected abstract void ValidateOutput(object data, string targetType);
        protected abstract void HandleError(Exception ex, string sourceType, string targetType);
        protected virtual void Log(string message) => Console.WriteLine($"[LOG] {message}");
    }
}
