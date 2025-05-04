using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Contracts.Responses;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using System.Text.Json;

namespace DynamicMappingSystem.Application.Mapping
{
    /// <summary>
    /// Provides a base implementation for handling dynamic mapping operations,
    /// including input/output validation, mapping execution, and error handling.
    /// </summary>
    public abstract class BaseMapHandler : IMapHandler
    {
        /// <summary>
        /// Executes the complete mapping workflow including input validation,
        /// data transformation, output validation, and error handling.
        /// </summary>
        /// <param name="data">The source data to be mapped.</param>
        /// <param name="sourceType">The source type identifier.</param>
        /// <param name="targetType">The target type identifier.</param>
        /// <param name="version">Optional mapping version.</param>
        /// <returns>A <see cref="MappingResult"/> containing the mapped data or errors if any.</returns>
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
            catch (ValidationMappingException validationException)
            {
                result.Errors.Add(new MappingError { Code = validationException.Code, Message = validationException.Message });
                HandleError(validationException, sourceType, targetType);
            }
            catch (MappingNotFoundException mappingNotFoundException)
            {
                result.Errors.Add(new MappingError { Code = mappingNotFoundException.Code, Message = mappingNotFoundException.Message });
                HandleError(mappingNotFoundException, sourceType, targetType);
            }
            catch (MappingException mappingException)
            {
                result.Errors.Add(new MappingError { Code = mappingException.Code, Message = mappingException.Message });
                HandleError(mappingException, sourceType, targetType);
            }
            catch (ApplicationException applicationException)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.PropertyMappingFailed, Message = applicationException.Message });
                HandleError(applicationException, sourceType, targetType);
            }
            catch (Exception exception)
            {
                result.Errors.Add(new MappingError { Code = MappingErrorCodes.UnexpectedError, Message = exception.Message });
                HandleError(exception, sourceType, targetType);
            }
            return result;
        }

        /// <summary>
        /// Validates the input data before mapping.
        /// </summary>
        /// <param name="data">The source data to validate.</param>
        /// <param name="sourceType">The source type identifier.</param>
        /// <param name="targetType">The target type identifier.</param>
        protected abstract void ValidateInput(object data, string sourceType, string targetType);

        /// <summary>
        /// Performs the mapping transformation between source and target types.
        /// </summary>
        /// <param name="data">The input data to map.</param>
        /// <param name="sourceType">The source type identifier.</param>
        /// <param name="targetType">The target type identifier.</param>
        /// <param name="version">Optional mapping version.</param>
        /// <returns>The mapped object.</returns>
        protected abstract object PerformMapping(object data, string sourceType, string targetType, string? version);

        /// <summary>
        /// Validates the mapped output to ensure correctness.
        /// </summary>
        /// <param name="data">The mapped data to validate.</param>
        /// <param name="targetType">The target type identifier.</param>
        protected abstract void ValidateOutput(object data, string targetType);

        /// <summary>
        /// Handles unexpected or custom errors that occur during mapping.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <param name="sourceType">The source type identifier.</param>
        /// <param name="targetType">The target type identifier.</param>
        protected abstract void HandleError(Exception ex, string sourceType, string targetType);

        /// <summary>
        /// Logs messages to the console or any custom logging mechanism.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected virtual void Log(string message) => Console.WriteLine($"[LOG] {message}");
    }
}
