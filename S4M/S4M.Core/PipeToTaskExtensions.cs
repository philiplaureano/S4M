using System;
using System.Threading;
using System.Threading.Tasks;

namespace S4M.Core
{
    public static class PipeToTaskExtensions
    {
        public static void PipeTo<T>(this Task<T> currentTask, ICanTellAsync receiver, bool waitForTaskCompletion = false)
        {
            currentTask.PipeTo(receiver, waitForTaskCompletion, CancellationToken.None);
        }

        public static void PipeTo<T>(this Task<T> currentTask, ICanTellAsync receiver,
            bool waitForTaskCompletion,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (!waitForTaskCompletion)
            {
                currentTask.ContinueWith(t => { Task.WaitAny(receiver.TellAsync(t.Result, cancellationToken)); },
                    TaskContinuationOptions.OnlyOnRanToCompletion);

                return;
            }

            // Wait for the task to complete
            Task.WaitAny(new[] {currentTask}, cancellationToken);

            // Pass the results to the receiver
            var result = currentTask.Result;
            Task.WaitAny(new[] {receiver.TellAsync(result, cancellationToken)}, cancellationToken);
        }
    }
}