using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

#pragma warning disable CA1068 // CancellationToken 参数必须最后出现

internal static class TaskExtensions
{
    #region Public 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DisposeAfterTask(this Task task,
                                        IDisposable disposableObject,
                                        CancellationToken cancellationToken = default,
                                        TaskContinuationOptions continuationOptions = TaskContinuationOptions.None,
                                        TaskScheduler? scheduler = null)
    {
        task.ContinueWith(static (_, state) => ((IDisposable)state!).Dispose(), disposableObject, cancellationToken, continuationOptions, scheduler ?? TaskScheduler.Current);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DisposeAfterTask(this Task task,
                                        IDisposable disposableObject,
                                        IDisposable disposableObject2,
                                        CancellationToken cancellationToken = default,
                                        TaskContinuationOptions continuationOptions = TaskContinuationOptions.None,
                                        TaskScheduler? scheduler = null)
    {
        task.ContinueWith(static (_, state) =>
        {
            var array = (IDisposable[])state!;
            array[0].Dispose();
            array[1].Dispose();
        }, new IDisposable[] { disposableObject, disposableObject2 }, cancellationToken, continuationOptions, scheduler ?? TaskScheduler.Current);
    }

    #endregion Public 方法
}
