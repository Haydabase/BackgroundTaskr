using System;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTaskr.SlackErrorNotifier
{
    internal interface ISlackNotifier
    {
        Task NotifyTaskFailure(string name, Exception exception);
    }
}