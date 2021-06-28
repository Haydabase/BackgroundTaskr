using Haydabase.BackgroundTaskr.SlackErrorNotifier;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SlackNotifierBackgroundTaskrBuilderExtensions
    {
        public static IBackgroundTaskrBuilder UsingSlackErrorNotifications(this IBackgroundTaskrBuilder builder)
        {
            builder.Services.TryAddSingleton<ISlackNotifier, DummySlackNotifier>();
            builder.Services.TryAddSingleton<SlackNotificationMiddleware>();
            return builder.UsingMiddleware<SlackNotificationMiddleware>();
        }
    }
}