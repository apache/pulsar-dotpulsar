using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotPulsar.Stress.Tests
{
    public static class EnumerableValueTaskExtensions
    {
        [DebuggerStepThrough]
        public static async ValueTask<TResult[]> WhenAll<TResult>(this IEnumerable<ValueTask<TResult>> source)
        {
            // the volatile property IsCompleted must be accessed only once
            var tasks = source.Select(GetInfo).ToArray();

            // run incomplete tasks
            await tasks
                .Where(x => x.IsTask).Select(t => t.Task)
                .WhenAll()
                .ConfigureAwait(false);

            // return ordered mixed tasks \m/
            return tasks
                .Select(x => x.IsTask ? x.Task.Result : x.Result)
                .ToArray();
        }

        [DebuggerStepThrough]
        public static async Task<TResult[]> WhenAllAsTask<TResult>(this IEnumerable<ValueTask<TResult>> source)
            => await source.WhenAll().ConfigureAwait(false);

        [DebuggerStepThrough]
        public static async IAsyncEnumerable<TResult> Enumerate<TResult>(this IEnumerable<ValueTask<TResult>> source)
        {
            foreach (var operation in source.Select(GetInfo))
                yield return operation.IsTask
                    ? await operation.Task.ConfigureAwait(false)
                    : operation.Result;
        }

        private static ValueTaskInfo<TResult> GetInfo<TResult>(this ValueTask<TResult> source)
        {
            return source.IsCompleted
                ? new ValueTaskInfo<TResult>(source.Result)
                : new ValueTaskInfo<TResult>(source.AsTask());
        }

        private struct ValueTaskInfo<TResult>
        {
            public ValueTaskInfo(Task<TResult> task)
            {
                IsTask = true;
                Result = default;
                Task = task;
            }

            public ValueTaskInfo(TResult result)
            {
                IsTask = false;
                Result = result;
                Task = default;
            }

            public bool IsTask { get; }
            public TResult Result { get; }
            public Task<TResult> Task { get; }
        }
    }

    public static class EnumerableTaskExtensions
    {
        [DebuggerStepThrough]
        public static Task WhenAll(this IEnumerable<Task> source) => Task.WhenAll(source);

        [DebuggerStepThrough]
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> source) => Task.WhenAll(source);
    }
}