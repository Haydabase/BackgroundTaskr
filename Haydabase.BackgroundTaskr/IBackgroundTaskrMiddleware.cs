using System;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTaskr
{
    public interface IBackgroundTaskrMiddleware
    {
        public Task OnNext(string name, Func<Task> next);
    }
}