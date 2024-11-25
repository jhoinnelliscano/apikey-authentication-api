using Microsoft.AspNetCore.Mvc;

namespace APIKeyAuthentication.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController(ILogger<CityController> logger) : ControllerBase
    {
        private static readonly string[] _cities =
        [
            "Nueva York", "Londres", "Tokio", "Par�s", "S�dney", "Madrid", "Roma", "Toronto", "Berl�n", "Ciudad de M�xico"
        ];

        private readonly ILogger<CityController> _logger = logger;

        [HttpGet(Name = "GetCities")]
        public IEnumerable<string> Get()
        {
            return _cities;
        }
    }
}
