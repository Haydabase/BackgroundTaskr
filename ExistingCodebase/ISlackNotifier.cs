using System;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTasks.ExistingCodebase
{
    public interface ISlackNotifier
    {
        public Task NotifyTaskFailure(string name, Exception exception);
    }
}