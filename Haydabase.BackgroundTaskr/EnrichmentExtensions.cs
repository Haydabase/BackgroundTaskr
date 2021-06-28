using System;
using System.Diagnostics;

namespace Haydabase.BackgroundTaskr
{
    internal static class EnrichmentExtensions
    {
        public static void InvokeSafe(
            this Action<Activity, string, object> enrich,
            Activity? activity,
            string eventName,
            object rawObject)
        {
            if (activity == null)
            {
                return;
            }
            try
            {
                enrich(activity, eventName, rawObject);
            }
            catch
            {
            }
        }
    }
}