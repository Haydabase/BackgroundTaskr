using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Haydabase.BackgroundTaskr
{
    internal class BackgroundTaskFactory : IBackgroundTaskr
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
                try
                {
                    // Must use newly resolved dependencies in our background task to avoid accessing disposed scoped services.
                    using var scope = _serviceProvider.CreateScope();
                    var invoker = scope.ServiceProvider.GetRequiredService<IMiddlewareInvoker>();
                    await invoker.InvokeAsync(name, () => runTask(scope.ServiceProvider));
                }
                catch
                {
                }
            });
        }
    }
}