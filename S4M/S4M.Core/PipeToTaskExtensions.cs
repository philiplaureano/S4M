using System;
using System.Threading;
using System.Threading.Tasks;

namespace S4M.Core
{
    public static class PipeToTaskExtensions
    {
        public static void PipeTo<T>(this Task<T> currentTask, ICanTellAsync receiver)
        {
            currentTask.PipeTo(receiver, CancellationToken.None);
        }

        public static void PipeTo<T>(this Task<T> currentTask, ICanTellAsync receiver,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            // Wait for the task to complete
            Task.WaitAny(new[] {currentTask}, cancellationToken);

            // Pass the results to the receiver
            var result = currentTask.Result;
            Task.WaitAny(new[] {receiver.TellAsync(result, cancellationToken)}, cancellationToken);
        }
    }
}