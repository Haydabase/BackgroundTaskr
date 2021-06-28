using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haydabase.BackgroundTaskr.SlackErrorNotifier
{
    internal class DummySlackNotifier : ISlackNotifier
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