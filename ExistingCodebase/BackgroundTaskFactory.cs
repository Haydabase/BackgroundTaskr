using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Haydabase.BackgroundTasks.ExistingCodebase
{
    public class BackgroundTaskFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTaskFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void CreateBackgroundTask(string name, Func<IServiceProvider, Task> runTask)
        {
            Task.Run(async () =>
            {
                // Must use newly resolved dependencies in our background task to avoid accessing disposed scoped services.
                using var scope = _serviceProvider.CreateScope();
                try
                {
                    await runTask(scope.ServiceProvider);
                }
                catch (Exception exception)
                {
                    await scope.ServiceProvider.GetRequiredService<ISlackNotifier>().NotifyTaskFailure(name, exception);
                }
            });
        }
    }
}