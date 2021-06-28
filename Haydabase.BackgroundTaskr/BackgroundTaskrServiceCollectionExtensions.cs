using Haydabase.BackgroundTaskr;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackgroundTaskrServiceCollectionExtensions
    {
        public static IBackgroundTaskrBuilder AddBackgroundTaskr(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundTaskr, BackgroundTaskFactory>();
            services.TryAddScoped<IMiddlewareInvoker, MiddlewareInvoker>();
            return new BackgroundTaskrBuilder(services);
        }

        internal class BackgroundTaskrBuilder : IBackgroundTaskrBuilder
        {
            public BackgroundTaskrBuilder(IServiceCollection services) => Services = services;

            public IServiceCollection Services { get; }
        }
    }
}