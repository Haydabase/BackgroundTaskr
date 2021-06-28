using System;
using System.Threading.Tasks;

namespace DemoApp
{
    public class FakeDelayer : IDelayer
    {
        public Task DelayAsync(TimeSpan delay) => Task.CompletedTask;
    }
}