using System;
using System.Threading.Tasks;
using Haydabase.BackgroundTasks.ExistingCodebase;
using Microsoft.Extensions.Logging;

namespace DemoApp
{
    public class DummySlackNotifier : ISlackNotifier
    {
        private readonly ILogger<DummySlackNotifier> _logger;

        public DummySlackNotifier(ILogger<DummySlackNotifier> logger)
        {
            _logger = logger;
        }

        public async Task NotifyTaskFailure(string name, Exception exception)
        {
            await Task.Delay(500);
            _logger.LogError(exception, "Simulated notifying slack for '{TaskName}' task error", name);
        }
    }
}