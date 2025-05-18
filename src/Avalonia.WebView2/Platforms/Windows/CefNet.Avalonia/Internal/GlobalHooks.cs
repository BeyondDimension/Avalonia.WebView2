// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/Internal/GlobalHooks.cs
#if WINDOWS || NETFRAMEWORK
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platforms.Windows.Interop;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CefNet.Internal;

sealed class GlobalHooks
{
    static bool IsInitialized;

    internal static readonly Dictionary<IntPtr, GlobalHooks> _HookedWindows = [];
    static readonly List<WeakReference<WebView2>> _Views = [];

    internal static void Initialize(WebView2 view)
    {
        //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    return;

        lock (_Views)
        {
            _Views.Add(new WeakReference<WebView2>(view));
        }

        if (IsInitialized)
            return;

        IsInitialized = true;

        if (global::Avalonia.Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
            return;

        Window.WindowOpenedEvent.AddClassHandler(typeof(Window), TryAddGlobalHook, handledEventsToo: true);
        //EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(TryAddGlobalHook));

        foreach (Window window in lifetime.Windows)
        {
            TryAddGlobalHook(window);
        }
    }

    readonly WindowsHwndSource _source;
    readonly WeakReference<Window> _windowRef;

    GlobalHooks(WindowsHwndSource source, Window window)
    {
        _source = source;
        _windowRef = new WeakReference<Window>(window);
        source.WndProcCallback = WndProc;
    }

    nint WndProc(nint hwnd, uint message, nint wParam, nint lParam, ref bool handled)
    {
        foreach (var webView in GetViews(hwnd))
        {
            ((IHwndHost)webView).WndProc(hwnd, message, wParam, lParam, ref handled);
        }
        return default;
    }

    internal IEnumerable<WebView2> GetViews(nint hwnd)
    {
        if (hwnd != _source.Handle)
        {
            yield break;
        }

        if (!_windowRef.TryGetTarget(out var window))
        {
            yield break;
        }

        lock (_Views)
        {
            for (int i = 0; i < _Views.Count; i++)
            {
                WeakReference<WebView2> viewRef = _Views[i];
                if (viewRef.TryGetTarget(out WebView2? view))
                {
                    if (view.GetVisualRoot() == window)
                    {
                        yield return view;
                    }
                }
                else
                {
                    _Views.RemoveAt(i--);
                }
            }
        }
    }

    static void TryAddGlobalHook(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Window window)
        {
            TryAddGlobalHook(window);
        }
    }

    static void TryAddGlobalHook(Window window)
    {
        if (window == null)
            return;

        var handle = window.TryGetPlatformHandle();
        if (handle == null)
            return;

        var hwnd = handle.Handle;

        if (_HookedWindows.ContainsKey(hwnd))
            return;

        WindowsHwndSource source = WindowsHwndSource.FromHwnd(hwnd);
        if (source == null)
            return;

        _HookedWindows.Add(hwnd, new GlobalHooks(source, window));
    }

}
#endif