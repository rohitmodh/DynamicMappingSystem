using DynamicMappingSystem.Exceptions;
using DynamicMappingSystem.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text.Json;

namespace DynamicMappingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MappingsController : ControllerBase
    {
        private readonly IMapHandler _mapHandler;

        public MappingsController(IMapHandler mapHandler)
        {
            _mapHandler = mapHandler;
        }

        [HttpPost("map")]
        public IActionResult Map([FromBody] MapRequest request)
        {
            JToken sourceToken = JToken.Parse(request.Data.GetRawText());

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
            else if (sourceToken.Type == JTokenType.Object)
            {
                var result = _mapHandler.Map((JObject)sourceToken, request.SourceType, request.TargetType);
                return Ok(result);
            }
            
            throw new InvalidInpurFormatException("Data must be a JSON object or array of objects.");
        }
    }
}
