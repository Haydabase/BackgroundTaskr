using System;
using System.Diagnostics;

namespace Haydabase.BackgroundTaskr
{
    public class BackgroundTaskrOptions
    {
        public Action<Activity, string, object>? EnrichCreate { get; set; }
        public Action<Activity, string, object>? EnrichRun { get; set; }
    }
}