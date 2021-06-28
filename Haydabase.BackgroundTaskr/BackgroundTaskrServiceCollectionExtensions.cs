using Haydabase.BackgroundTaskr;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackgroundTaskrServiceCollectionExtensions
    {
        public static void AddBackgroundTaskr(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundTaskr, BackgroundTaskFactory>();
            services.TryAddScoped<IMiddlewareInvoker, MiddlewareInvoker>();
        }
    }
}