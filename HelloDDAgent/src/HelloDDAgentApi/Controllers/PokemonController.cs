using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HellDDAgentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PokemonController> _logger;

        public PokemonController(ILogger<PokemonController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{name}")]
        public string Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var response = new HttpClient().GetStringAsync($"https://pokeapi.co/api/v2/pokemon/{name}").GetAwaiter().GetResult();
            _logger.LogDebug(response);

            return response;
        }
    }
}