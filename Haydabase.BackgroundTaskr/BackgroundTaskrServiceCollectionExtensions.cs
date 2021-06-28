using System;
using Haydabase.BackgroundTaskr;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackgroundTaskrServiceCollectionExtensions
    {
        public static IBackgroundTaskrBuilder AddBackgroundTaskr(this IServiceCollection services,
            Action<IServiceProvider, BackgroundTaskrOptions> configure)
        {
            services.AddSingleton<IConfigureOptions<BackgroundTaskrOptions>>(serviceProvider =>
                new ConfigureOptions<BackgroundTaskrOptions>(options => configure(serviceProvider, options)));
            services.TryAddSingleton<IBackgroundTaskr, BackgroundTaskFactory>();
            services.TryAddScoped<IMiddlewareInvoker, MiddlewareInvoker>();
            return new BackgroundTaskrBuilder(services);
        }

        public static IBackgroundTaskrBuilder AddBackgroundTaskr(this IServiceCollection services,
            Action<BackgroundTaskrOptions> configure) =>
            services.AddBackgroundTaskr((_, options) => configure(options));

        public static IBackgroundTaskrBuilder AddBackgroundTaskr(this IServiceCollection services) =>
            services.AddBackgroundTaskr(_ => { });

        internal class BackgroundTaskrBuilder : IBackgroundTaskrBuilder
        {
            public BackgroundTaskrBuilder(IServiceCollection services) => Services = services;

            public IServiceCollection Services { get; }
        }
    }
}