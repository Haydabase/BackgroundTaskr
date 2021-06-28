// ReSharper disable once CheckNamespace
namespace OpenTelemetry.Trace
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddBackgroundTaskrInstrumentation(this TracerProviderBuilder builder) =>
            builder.AddSource("Haydabase.BackgroundTaskr");
    }
}