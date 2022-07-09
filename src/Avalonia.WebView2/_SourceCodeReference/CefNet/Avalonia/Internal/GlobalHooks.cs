// MIT License Copyright(c) 2020 CefNet
// https://github.com/CefNet/CefNet/blob/103.0.22181.155/CefNet.Avalonia/Internal/GlobalHooks.cs

namespace CefNet.Internal;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using System.Windows.Interop;

sealed class GlobalHooks
{
    static bool IsInitialized;

    static readonly Dictionary<IntPtr, GlobalHooks> _HookedWindows = new();
    static readonly List<WeakReference<WebView2>> _Views = new();

    internal static void Initialize(WebView2 view)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        lock (_Views)
        {
            _Views.Add(new WeakReference<WebView2>(view));
        }

        if (IsInitialized)
            return;

        IsInitialized = true;

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
            return;

        Window.WindowOpenedEvent.AddClassHandler(typeof(Window), TryAddGlobalHook, handledEventsToo: true);
        //EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(TryAddGlobalHook));

        foreach (Window window in lifetime.Windows)
        {
            TryAddGlobalHook(window);
        }
    }

    WindowsHwndSource _source;
    WeakReference<Window> _windowRef;

    GlobalHooks(WindowsHwndSource source, Window window)
    {
        _source = source;
        _windowRef = new WeakReference<Window>(window);
        source.WndProcCallback = WndProc;
    }

    IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        foreach ((WebView2 webView, Window _) in GetViews(hwnd))
        {
            ((IHwndHost)webView).WndProc(hwnd, message, wParam, lParam, ref handled);
        }
        return IntPtr.Zero;
    }

    IEnumerable<(WebView2 webView, Window window)> GetViews(IntPtr hwnd)
    {
        if (hwnd != _source.Handle)
            yield break;

        if (!_windowRef.TryGetTarget(out var window))
            yield break;

        lock (_Views)
        {
            for (int i = 0; i < _Views.Count; i++)
            {
                WeakReference<WebView2> viewRef = _Views[i];
                if (viewRef.TryGetTarget(out WebView2? view))
                {
                    if (view.GetVisualRoot() == window)
                    {
                        yield return (view, window);
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
        if (e.Source is Window window) TryAddGlobalHook(window);
    }

    static void TryAddGlobalHook(Window window)
    {
        if (window == null)
            return;

        IntPtr hwnd = window.PlatformImpl.Handle.Handle;

        if (_HookedWindows.ContainsKey(hwnd))
            return;

        WindowsHwndSource source = WindowsHwndSource.FromHwnd(hwnd);
        if (source == null)
            return;

        _HookedWindows.Add(hwnd, new GlobalHooks(source, window));
    }

}