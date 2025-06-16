using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;
internal static class TaskExtensions
{
    public static async void FireAndForget<TResult>(
       this Task<TResult> task,
       Action<Exception>? errorCallback = null)
    {
        TResult? result = default;
        try
        {
            result = await task.ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            errorCallback?.Invoke(exc);
#if DEBUG
            throw;
#endif
        }
    }

    public static async void FireAndForget(
        this Task task,
        Action<Exception>? errorCallback = null
        )
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            errorCallback?.Invoke(ex);
#if DEBUG
            throw;
#endif
        }
    }
}
