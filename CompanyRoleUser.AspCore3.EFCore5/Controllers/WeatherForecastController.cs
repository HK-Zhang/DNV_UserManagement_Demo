using DNVGL.Authorization.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CompanyRoleUser.AspCore3.EFCore5.PermissionBook;

namespace CompanyRoleUser.AspCore3.EFCore5.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/company/{companyId}/weatherForecast")]
    [CompanyIdentityFieldNameFilter(companyIdInRoute: "companyId")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]

        //[PermissionAuthorize(NormalPermission.ViewWeatherForecast)]
        [Authorize(Roles = "ViewWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var user = HttpContext.User;
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
