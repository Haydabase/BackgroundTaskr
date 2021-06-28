// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public interface IBackgroundTaskrBuilder
    {
        public IServiceCollection Services { get; }
    }
}