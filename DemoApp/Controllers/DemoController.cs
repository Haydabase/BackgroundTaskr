using System;
using System.Threading.Tasks;
using Haydabase.BackgroundTaskr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Controllers
{
    [ApiController]
    [Route("demo")]
    public class DemoController : ControllerBase
    {
        private readonly IBackgroundTaskr _backgroundTaskr;

        public DemoController(IBackgroundTaskr backgroundTaskr)
        {
            _backgroundTaskr = backgroundTaskr;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
        
        [HttpPost("background-tasks/{name}")]
        public IActionResult CreateBackgroundTask(string name)
        {
            _backgroundTaskr.CreateBackgroundTask(name,
                x => x.GetRequiredService<IDelayer>().DelayAsync(TimeSpan.FromSeconds(2)));
            return Ok();
        }
        
        [HttpPost("erroring-background-tasks/{name}")]
        public IActionResult CreateErroringBackgroundTask(string name, string? message = null)
        {
            _backgroundTaskr.CreateBackgroundTask(name, async x =>
            {
                await x.GetRequiredService<IDelayer>().DelayAsync(TimeSpan.FromSeconds(1));
                throw new Exception(message ?? "Test Erroring Background Task");
            });
            return Ok();
        }
    }
}