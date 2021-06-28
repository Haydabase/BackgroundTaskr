using System;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTaskr
{
    public interface IBackgroundTaskr
    {
        void CreateBackgroundTask(string name, Func<IServiceProvider, Task> runTask);
    }
}