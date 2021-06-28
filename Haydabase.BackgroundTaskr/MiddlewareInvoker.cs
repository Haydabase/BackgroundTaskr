using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Haydabase.BackgroundTaskr
{
    internal interface IMiddlewareInvoker
    {
        Task InvokeAsync(string name, Func<Task> runTask);
    }

    internal sealed class MiddlewareInvoker : IMiddlewareInvoker
    {
        private readonly IReadOnlyList<IBackgroundTaskrMiddleware> _middlewaresInReverse;

        public MiddlewareInvoker(IEnumerable<IBackgroundTaskrMiddleware> middlewares) =>
            _middlewaresInReverse = middlewares.Reverse().ToList();

        public Task InvokeAsync(string name, Func<Task> runTask) =>
            _middlewaresInReverse.Aggregate(runTask, (next, middleware) => () => middleware.OnNext(name, next))();
    }
}