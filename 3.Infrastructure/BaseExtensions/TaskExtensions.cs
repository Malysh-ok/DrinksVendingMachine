using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.BaseExtensions
{
    /// <summary>
    /// Методы расширения для <see cref="Task"/>.
    /// </summary>
    /// TODO: Разобраться с TaskExtensions, прокомментировать и протестировать.
    public static class TaskExtensions
    {
       /*
       var t = Task.Factory
          .StartNew(() => Console.WriteLine("Started"))
          .Finally(() => Console.WriteLine("Finished"));
      */
        
        /// <summary>
        /// Handles any exceptions on this task, and executes action on specified scheduler.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="finalAction">The final action.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>The passed-in task.</returns>
        public static Task Finally(this Task task, Action finalAction,
            Action<Exception> exceptionHandler = null, TaskScheduler scheduler = null)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (scheduler == null) scheduler = TaskScheduler.Default;
            task.ContinueWith(t =>
            {
                finalAction?.Invoke();
                if (t.IsCanceled || !t.IsFaulted) return;
                var innerException = t.Exception?.Flatten().InnerExceptions.FirstOrDefault();
                exceptionHandler?.Invoke(innerException ?? t.Exception);
            }, scheduler);
            return task;
        }

        /// <summary>
        /// Ожидание окончания задачи, с блокировкой исключения OperationCanceledException.
        /// </summary>
        /// <param name="task">Задача, окончание которой будем ждать.</param>
        public static Task Waiting(this Task task)
        {
            task.ContinueWith(t => { }).Wait();
            return task;
        }

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (!cancellationToken.CanBeCanceled || task.IsCompleted)
                return task;
            if (cancellationToken.IsCancellationRequested)
                return SingletonTask<T>.CanceledTask;
            return WithCancellationSlow(task, cancellationToken);
        }

        public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (!cancellationToken.CanBeCanceled || task.IsCompleted)
                return task;
            if (cancellationToken.IsCancellationRequested)
                return SingletonTask.CanceledTask;
            return task.WithCancellationSlow(cancellationToken);
        }

        /// <summary>
        /// Убирает предупреждение, что метод awaitable.
        /// </summary>
        // TODO: проверить!
        public static void DisableAsyncWarning(this Task task)
        {
            ;
        }

        private static async Task<T> WithCancellationSlow<T>(Task<T> task, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    cancellationToken.ThrowIfCancellationRequested();
            }
            return await task.ConfigureAwait(false);
        }

        private static async Task WithCancellationSlow(this Task task, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    cancellationToken.ThrowIfCancellationRequested();
            }
            await task.ConfigureAwait(false);
        }

        private static class SingletonTask<T>
        {
            internal static readonly Task<T> CanceledTask = CreateCanceledTask();

            private static Task<T> CreateCanceledTask()
            {
                var completionSource = new TaskCompletionSource<T>();
                completionSource.SetCanceled();
                return completionSource.Task;
            }
        }

        private static class SingletonTask
        {
            internal static readonly Task CanceledTask = CreateCanceledTask();

            private static Task CreateCanceledTask()
            {
                var completionSource = new TaskCompletionSource<EmptyStruct>();
                completionSource.SetCanceled();
                return completionSource.Task;
            }
        }

        private struct EmptyStruct
        {
        }
    }   
}
