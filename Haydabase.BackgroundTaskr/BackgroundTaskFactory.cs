using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Haydabase.BackgroundTaskr
{
    internal class BackgroundTaskFactory : IBackgroundTaskr
    {
        private static ActivitySource ActivitySource { get; } = new ActivitySource("Haydabase.BackgroundTaskr");
        
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTaskFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void CreateBackgroundTask(string name, Func<IServiceProvider, Task> runTask)
        {
            using var createActivity = ActivitySource.StartActivity(
                $"Create BackgroundTask: {name}",
                ActivityKind.Internal,
                default(ActivityContext),
                new Dictionary<string, object?> {["backgroundtaskr.name"] = name});
            var propagationContext = createActivity?.Context ?? Activity.Current?.Context ?? default(ActivityContext);
            Task.Run(async () =>
            {
                using var runActivity = ActivitySource.StartActivity(
                    $"Run BackgroundTask {name}",
                    ActivityKind.Internal,
                    propagationContext,
                    new Dictionary<string, object?> {["backgroundtaskr.name"] = name});
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var invoker = scope.ServiceProvider.GetRequiredService<IMiddlewareInvoker>();
                    await invoker.InvokeAsync(name, () => runTask(scope.ServiceProvider));
                    runActivity?.SetStatus(Status.Ok);
                }
                catch (Exception exception)
                {
                    runActivity?.RecordException(exception);
                    runActivity?.SetStatus(Status.Error);
                }
            });
            createActivity?.SetStatus(Status.Ok);
        }
    }
}