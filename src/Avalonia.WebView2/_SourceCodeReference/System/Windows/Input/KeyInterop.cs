// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// https://github.com/dotnet/wpf/blob/v6.0.6/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Input/KeyInterop.cs

namespace System.Windows.Input;

/// <summary>
///     Provides static methods to convert between Win32 VirtualKeys
///     and our Key enum.
/// </summary>
static class KeyInterop
{
    /// <summary>
    ///     Convert a Win32 VirtualKey into our Key enum.
    /// </summary>
    public static Key KeyFromVirtualKey(int virtualKey) => virtualKey switch
    {
        NativeMethods.VK_CANCEL => Key.Cancel,
        NativeMethods.VK_BACK => Key.Back,
        NativeMethods.VK_TAB => Key.Tab,
        NativeMethods.VK_CLEAR => Key.Clear,
        NativeMethods.VK_RETURN => Key.Return,
        NativeMethods.VK_PAUSE => Key.Pause,
        NativeMethods.VK_CAPITAL => Key.Capital,
        NativeMethods.VK_KANA => Key.KanaMode,
        NativeMethods.VK_JUNJA => Key.JunjaMode,
        NativeMethods.VK_FINAL => Key.FinalMode,
        NativeMethods.VK_KANJI => Key.KanjiMode,
        NativeMethods.VK_ESCAPE => Key.Escape,
        NativeMethods.VK_CONVERT => Key.ImeConvert,
        NativeMethods.VK_NONCONVERT => Key.ImeNonConvert,
        NativeMethods.VK_ACCEPT => Key.ImeAccept,
        NativeMethods.VK_MODECHANGE => Key.ImeModeChange,
        NativeMethods.VK_SPACE => Key.Space,
        NativeMethods.VK_PRIOR => Key.Prior,
        NativeMethods.VK_NEXT => Key.Next,
        NativeMethods.VK_END => Key.End,
        NativeMethods.VK_HOME => Key.Home,
        NativeMethods.VK_LEFT => Key.Left,
        NativeMethods.VK_UP => Key.Up,
        NativeMethods.VK_RIGHT => Key.Right,
        NativeMethods.VK_DOWN => Key.Down,
        NativeMethods.VK_SELECT => Key.Select,
        NativeMethods.VK_PRINT => Key.Print,
        NativeMethods.VK_EXECUTE => Key.Execute,
        NativeMethods.VK_SNAPSHOT => Key.Snapshot,
        NativeMethods.VK_INSERT => Key.Insert,
        NativeMethods.VK_DELETE => Key.Delete,
        NativeMethods.VK_HELP => Key.Help,
        NativeMethods.VK_0 => Key.D0,
        NativeMethods.VK_1 => Key.D1,
        NativeMethods.VK_2 => Key.D2,
        NativeMethods.VK_3 => Key.D3,
        NativeMethods.VK_4 => Key.D4,
        NativeMethods.VK_5 => Key.D5,
        NativeMethods.VK_6 => Key.D6,
        NativeMethods.VK_7 => Key.D7,
        NativeMethods.VK_8 => Key.D8,
        NativeMethods.VK_9 => Key.D9,
        NativeMethods.VK_A => Key.A,
        NativeMethods.VK_B => Key.B,
        NativeMethods.VK_C => Key.C,
        NativeMethods.VK_D => Key.D,
        NativeMethods.VK_E => Key.E,
        NativeMethods.VK_F => Key.F,
        NativeMethods.VK_G => Key.G,
        NativeMethods.VK_H => Key.H,
        NativeMethods.VK_I => Key.I,
        NativeMethods.VK_J => Key.J,
        NativeMethods.VK_K => Key.K,
        NativeMethods.VK_L => Key.L,
        NativeMethods.VK_M => Key.M,
        NativeMethods.VK_N => Key.N,
        NativeMethods.VK_O => Key.O,
        NativeMethods.VK_P => Key.P,
        NativeMethods.VK_Q => Key.Q,
        NativeMethods.VK_R => Key.R,
        NativeMethods.VK_S => Key.S,
        NativeMethods.VK_T => Key.T,
        NativeMethods.VK_U => Key.U,
        NativeMethods.VK_V => Key.V,
        NativeMethods.VK_W => Key.W,
        NativeMethods.VK_X => Key.X,
        NativeMethods.VK_Y => Key.Y,
        NativeMethods.VK_Z => Key.Z,
        NativeMethods.VK_LWIN => Key.LWin,
        NativeMethods.VK_RWIN => Key.RWin,
        NativeMethods.VK_APPS => Key.Apps,
        NativeMethods.VK_SLEEP => Key.Sleep,
        NativeMethods.VK_NUMPAD0 => Key.NumPad0,
        NativeMethods.VK_NUMPAD1 => Key.NumPad1,
        NativeMethods.VK_NUMPAD2 => Key.NumPad2,
        NativeMethods.VK_NUMPAD3 => Key.NumPad3,
        NativeMethods.VK_NUMPAD4 => Key.NumPad4,
        NativeMethods.VK_NUMPAD5 => Key.NumPad5,
        NativeMethods.VK_NUMPAD6 => Key.NumPad6,
        NativeMethods.VK_NUMPAD7 => Key.NumPad7,
        NativeMethods.VK_NUMPAD8 => Key.NumPad8,
        NativeMethods.VK_NUMPAD9 => Key.NumPad9,
        NativeMethods.VK_MULTIPLY => Key.Multiply,
        NativeMethods.VK_ADD => Key.Add,
        NativeMethods.VK_SEPARATOR => Key.Separator,
        NativeMethods.VK_SUBTRACT => Key.Subtract,
        NativeMethods.VK_DECIMAL => Key.Decimal,
        NativeMethods.VK_DIVIDE => Key.Divide,
        NativeMethods.VK_F1 => Key.F1,
        NativeMethods.VK_F2 => Key.F2,
        NativeMethods.VK_F3 => Key.F3,
        NativeMethods.VK_F4 => Key.F4,
        NativeMethods.VK_F5 => Key.F5,
        NativeMethods.VK_F6 => Key.F6,
        NativeMethods.VK_F7 => Key.F7,
        NativeMethods.VK_F8 => Key.F8,
        NativeMethods.VK_F9 => Key.F9,
        NativeMethods.VK_F10 => Key.F10,
        NativeMethods.VK_F11 => Key.F11,
        NativeMethods.VK_F12 => Key.F12,
        NativeMethods.VK_F13 => Key.F13,
        NativeMethods.VK_F14 => Key.F14,
        NativeMethods.VK_F15 => Key.F15,
        NativeMethods.VK_F16 => Key.F16,
        NativeMethods.VK_F17 => Key.F17,
        NativeMethods.VK_F18 => Key.F18,
        NativeMethods.VK_F19 => Key.F19,
        NativeMethods.VK_F20 => Key.F20,
        NativeMethods.VK_F21 => Key.F21,
        NativeMethods.VK_F22 => Key.F22,
        NativeMethods.VK_F23 => Key.F23,
        NativeMethods.VK_F24 => Key.F24,
        NativeMethods.VK_NUMLOCK => Key.NumLock,
        NativeMethods.VK_SCROLL => Key.Scroll,
        NativeMethods.VK_SHIFT or NativeMethods.VK_LSHIFT => Key.LeftShift,
        NativeMethods.VK_RSHIFT => Key.RightShift,
        NativeMethods.VK_CONTROL or NativeMethods.VK_LCONTROL => Key.LeftCtrl,
        NativeMethods.VK_RCONTROL => Key.RightCtrl,
        NativeMethods.VK_MENU or NativeMethods.VK_LMENU => Key.LeftAlt,
        NativeMethods.VK_RMENU => Key.RightAlt,
        NativeMethods.VK_BROWSER_BACK => Key.BrowserBack,
        NativeMethods.VK_BROWSER_FORWARD => Key.BrowserForward,
        NativeMethods.VK_BROWSER_REFRESH => Key.BrowserRefresh,
        NativeMethods.VK_BROWSER_STOP => Key.BrowserStop,
        NativeMethods.VK_BROWSER_SEARCH => Key.BrowserSearch,
        NativeMethods.VK_BROWSER_FAVORITES => Key.BrowserFavorites,
        NativeMethods.VK_BROWSER_HOME => Key.BrowserHome,
        NativeMethods.VK_VOLUME_MUTE => Key.VolumeMute,
        NativeMethods.VK_VOLUME_DOWN => Key.VolumeDown,
        NativeMethods.VK_VOLUME_UP => Key.VolumeUp,
        NativeMethods.VK_MEDIA_NEXT_TRACK => Key.MediaNextTrack,
        NativeMethods.VK_MEDIA_PREV_TRACK => Key.MediaPreviousTrack,
        NativeMethods.VK_MEDIA_STOP => Key.MediaStop,
        NativeMethods.VK_MEDIA_PLAY_PAUSE => Key.MediaPlayPause,
        NativeMethods.VK_LAUNCH_MAIL => Key.LaunchMail,
        NativeMethods.VK_LAUNCH_MEDIA_SELECT => Key.SelectMedia,
        NativeMethods.VK_LAUNCH_APP1 => Key.LaunchApplication1,
        NativeMethods.VK_LAUNCH_APP2 => Key.LaunchApplication2,
        NativeMethods.VK_OEM_1 => Key.OemSemicolon,
        NativeMethods.VK_OEM_PLUS => Key.OemPlus,
        NativeMethods.VK_OEM_COMMA => Key.OemComma,
        NativeMethods.VK_OEM_MINUS => Key.OemMinus,
        NativeMethods.VK_OEM_PERIOD => Key.OemPeriod,
        NativeMethods.VK_OEM_2 => Key.OemQuestion,
        NativeMethods.VK_OEM_3 => Key.OemTilde,
        NativeMethods.VK_C1 => Key.AbntC1,
        NativeMethods.VK_C2 => Key.AbntC2,
        NativeMethods.VK_OEM_4 => Key.OemOpenBrackets,
        NativeMethods.VK_OEM_5 => Key.OemPipe,
        NativeMethods.VK_OEM_6 => Key.OemCloseBrackets,
        NativeMethods.VK_OEM_7 => Key.OemQuotes,
        NativeMethods.VK_OEM_8 => Key.Oem8,
        NativeMethods.VK_OEM_102 => Key.OemBackslash,
        NativeMethods.VK_PROCESSKEY => Key.ImeProcessed,
        // VK_DBE_ALPHANUMERIC
        NativeMethods.VK_OEM_ATTN => Key.OemAttn, // DbeAlphanumeric
                                                  // VK_DBE_KATAKANA
        NativeMethods.VK_OEM_FINISH => Key.OemFinish, // DbeKatakana
                                                      // VK_DBE_HIRAGANA
        NativeMethods.VK_OEM_COPY => Key.OemCopy, // DbeHiragana
                                                  // VK_DBE_SBCSCHAR
        NativeMethods.VK_OEM_AUTO => Key.OemAuto, // DbeSbcsChar
                                                  // VK_DBE_DBCSCHAR
        NativeMethods.VK_OEM_ENLW => Key.OemEnlw, // DbeDbcsChar
                                                  // VK_DBE_ROMAN
        NativeMethods.VK_OEM_BACKTAB => Key.OemBackTab, // DbeRoman
                                                        // VK_DBE_NOROMAN
        NativeMethods.VK_ATTN => Key.Attn, // DbeNoRoman
                                           // VK_DBE_ENTERWORDREGISTERMODE
        NativeMethods.VK_CRSEL => Key.CrSel, // DbeEnterWordRegisterMode
                                             // VK_DBE_ENTERIMECONFIGMODE
        NativeMethods.VK_EXSEL => Key.ExSel, // DbeEnterImeConfigMode
                                             // VK_DBE_FLUSHSTRING
        NativeMethods.VK_EREOF => Key.EraseEof, // DbeFlushString
                                                // VK_DBE_CODEINPUT
        NativeMethods.VK_PLAY => Key.Play, // DbeCodeInput
                                           // VK_DBE_NOCODEINPUT
        NativeMethods.VK_ZOOM => Key.Zoom, // DbeNoCodeInput
                                           // VK_DBE_DETERMINESTRING
        NativeMethods.VK_NONAME => Key.NoName, // DbeDetermineString
                                               // VK_DBE_ENTERDLGCONVERSIONMODE
        NativeMethods.VK_PA1 => Key.Pa1, // DbeEnterDlgConversionMode
        NativeMethods.VK_OEM_CLEAR => Key.OemClear,
        _ => Key.None,
    };
}
