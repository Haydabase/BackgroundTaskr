using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

namespace Haydabase.BackgroundTaskr
{
    internal class BackgroundTaskFactory : IBackgroundTaskr
    {
        private static ActivitySource ActivitySource { get; } = new ActivitySource("Haydabase.BackgroundTaskr");
        
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackgroundTaskrOptions> _options;

        public BackgroundTaskFactory(IServiceProvider serviceProvider, IOptions<BackgroundTaskrOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public void CreateBackgroundTask(string name, Func<IServiceProvider, Task> runTask)
        {
            var options = _options.Value;
            using var createActivity = ActivitySource.StartActivity(
                $"Create BackgroundTask: {name}",
                ActivityKind.Internal,
                default(ActivityContext),
                new Dictionary<string, object?> {["backgroundtaskr.name"] = name});
            options.EnrichCreate?.InvokeSafe(createActivity, "OnStartActivity", name);
            var propagationContext = createActivity?.Context ?? Activity.Current?.Context ?? default(ActivityContext);
            Task.Run(async () =>
            {
                using var runActivity = ActivitySource.StartActivity(
                    $"Run BackgroundTask {name}",
                    ActivityKind.Internal,
                    propagationContext,
                    new Dictionary<string, object?> {["backgroundtaskr.name"] = name});
                options.EnrichRun?.InvokeSafe(runActivity, "OnStartActivity", name);
                try
                {
                    // Must use newly resolved dependencies in our background task to avoid accessing disposed scoped services.
                    using var scope = _serviceProvider.CreateScope();
                    var invoker = scope.ServiceProvider.GetRequiredService<IMiddlewareInvoker>();
                    await invoker.InvokeAsync(name, () => runTask(scope.ServiceProvider));
                    runActivity?.SetStatus(Status.Ok);
                }
                catch (Exception exception)
                {
                    runActivity?.RecordException(exception);
                    runActivity?.SetStatus(Status.Error);
                    options.EnrichRun?.InvokeSafe(runActivity, "OnException", exception);
                }
                finally
                {
                    runActivity?.SetEndTime(DateTime.UtcNow);
                    options.EnrichRun?.InvokeSafe(runActivity, "OnStopActivity", name);
                }
            });
            createActivity?.SetStatus(Status.Ok);
            createActivity?.SetEndTime(DateTime.UtcNow);
            options.EnrichCreate?.InvokeSafe(createActivity, "OnStopActivity", name);
        }
    }
}