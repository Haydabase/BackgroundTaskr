using System;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTaskr.SlackErrorNotifier
{
    internal class SlackNotificationMiddleware : IBackgroundTaskrMiddleware
    {
        private readonly ISlackNotifier _notifier;

        public SlackNotificationMiddleware(ISlackNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task OnNext(string name, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception exception)
            {
                await _notifier.NotifyTaskFailure(name, exception);
                throw;
            }
        }
    }
}