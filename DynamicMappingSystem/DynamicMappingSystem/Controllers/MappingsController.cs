using DynamicMappingSystem.Application.Interfaces;
using DynamicMappingSystem.Contracts.Requests;
using DynamicMappingSystem.Domain.Exceptions;
using DynamicMappingSystem.Domain.Rules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DynamicMappingSystem.Controllers
{
    /// <summary>
    /// Controller that handles mapping operations for source and target data types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MappingsController"/> class.
    /// </remarks>
    /// <param name="mapHandler">The map handler used to perform the mapping operations.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class MappingsController(IMapHandler mapHandler) : ControllerBase
    {
        private readonly IMapHandler _mapHandler = mapHandler;

        /// <summary>
        /// Maps the input data from the source type to the target type.
        /// </summary>
        /// <param name="request">The map request containing the data and source/target types.</param>
        /// <returns>An IActionResult representing the mapped result or an error.</returns>
        [HttpPost("map")]
        public IActionResult Map([FromBody] MapRequest request)
        {
            // Parse the raw JSON data in the request to a JToken
            JToken sourceToken = JToken.Parse(request.Data.GetRawText());

            // Check if the data is an array of objects
            if (sourceToken.Type == JTokenType.Array)
            {
                var results = new List<object>();
                foreach (var item in sourceToken as JArray)
                {
                    var mapped = _mapHandler.Map(item, request.SourceType, request.TargetType);
                    results.Add(mapped);
                }
                return Ok(results);
            }
            // Check if the data is a single JSON object
            else if (sourceToken.Type == JTokenType.Object)
            {
                var result = _mapHandler.Map((JObject)sourceToken, request.SourceType, request.TargetType);
                return Ok(result);
            }

            // Throw an exception if the data is not in the expected format
            throw new InvalidInpurFormatException(code: MappingErrorCodes.InvalidData, message: "Data must be a JSON object or array of objects.");
        }
    }
}
