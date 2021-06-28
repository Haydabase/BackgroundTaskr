using System;
using System.Threading.Tasks;
using Haydabase.BackgroundTaskr;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackgroundTaskrBuilderExtensions
    {
        public static IBackgroundTaskrBuilder UsingMiddleware(
            this IBackgroundTaskrBuilder builder,
            Func<IServiceProvider, IBackgroundTaskrMiddleware> resolveMiddleware)
        {
            builder.Services.AddScoped<IBackgroundTaskrMiddleware>(resolveMiddleware);
            return builder;
        }

        public static IBackgroundTaskrBuilder UsingMiddleware<TMiddleware>(this IBackgroundTaskrBuilder builder)
            where TMiddleware : IBackgroundTaskrMiddleware =>
            builder.UsingMiddleware(x => x.GetRequiredService<TMiddleware>());

        public static IBackgroundTaskrBuilder UsingMiddleware(
            this IBackgroundTaskrBuilder builder,
            Func<string, Func<Task>, Task> onNext) => builder.UsingMiddleware(_ => new FuncMiddleware(onNext));

        public static IBackgroundTaskrBuilder UsingMiddleware(
            this IBackgroundTaskrBuilder builder,
            Func<IServiceProvider, string, Func<Task>, Task> onNext) =>
            builder.UsingMiddleware(s => new FuncMiddleware((name, next) => onNext(s, name, next)));

        internal class FuncMiddleware : IBackgroundTaskrMiddleware
        {
            private readonly Func<string, Func<Task>, Task> _onNext;

            public FuncMiddleware(Func<string, Func<Task>, Task> onNext) => _onNext = onNext;

            public Task OnNext(string name, Func<Task> next) => _onNext(name, next);
        }
    }
}