using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Haydabase.BackgroundTaskr.Testing
{
    public class SynchronousBackgroundTaskr : IBackgroundTaskr
    {
        private readonly List<Invocation> _invocations = new List<Invocation>();
        private readonly IServiceProvider _serviceProvider;

        private SynchronousBackgroundTaskr(Func<IServiceProvider> createServiceProvider)
        {
            _serviceProvider = createServiceProvider();
            Invocations = _invocations.AsReadOnly();
        }

        public SynchronousBackgroundTaskr(IServiceProvider serviceProvider) : this(() => serviceProvider)
        {
        }

        public SynchronousBackgroundTaskr(Action<IServiceCollection> registerServices)
            : this(() =>
            {
                var services = new ServiceCollection();
                registerServices(services);
                return services.BuildServiceProvider();
            })
        {
        }

        public IReadOnlyList<Invocation> Invocations { get; }

        void IBackgroundTaskr.CreateBackgroundTask(string name, Func<IServiceProvider, Task> runTask) =>
            RunBackgroundTask(name, runTask).Wait();

        private async Task RunBackgroundTask(string name, Func<IServiceProvider, Task> runTask)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await runTask(scope.ServiceProvider);
            }
            catch (Exception exception)
            {
                _invocations.Add(new Invocation(name, exception));
                return;
            }
            _invocations.Add(new Invocation(name, null));
        }

        public class Invocation
        {
            public Invocation(string taskName, Exception? exception)
            {
                TaskName = taskName;
                Exception = exception;
            }

            public string TaskName { get; }
            public Exception? Exception { get; }
        }
    }
}