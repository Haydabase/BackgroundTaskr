using System;
using System.Threading.Tasks;

namespace DemoApp
{
    public interface IDelayer
    {
        Task DelayAsync(TimeSpan delay);
    }

    public class Delayer : IDelayer
    {
        public Task DelayAsync(TimeSpan delay) => Task.Delay(delay);
    }
}