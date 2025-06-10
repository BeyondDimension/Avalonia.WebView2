#if ANDROID
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;

internal class JavaScriptValueCallback : Java.Lang.Object, IValueCallback
{
    private readonly Action<Java.Lang.Object?> _callback;

    public JavaScriptValueCallback(Action<Java.Lang.Object?> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = callback;
    }

    public void OnReceiveValue(Java.Lang.Object? value)
    {
        _callback(value);
    }
}

#endif