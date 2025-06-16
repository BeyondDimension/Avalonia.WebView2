
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Avalonia.Controls;

//public static partial class ColorExtensions
//{
//#if ANDROID
//    public static global::Android.Graphics.Color ToPlatform(this Color self)
//    {
//        return new global::Android.Graphics.Color((byte)(byte.MaxValue * self.R), (byte)(byte.MaxValue * self.G), (byte)(byte.MaxValue * self.B), (byte)(byte.MaxValue * self.A));
//    }

//    public static Color AsColor(this global::Android.Graphics.Color target)
//    {
//        var r = (int)target.R;
//        var g = (int)target.G;
//        var b = (int)target.B;
//        var a = (int)target.A;
//        return Color.FromArgb(a, r, g, b);
//    }
//#endif

//#if IOS
//    public static UIColor ToPlatform(this Color color)
//    {
//        if (color != default)
//        {
//            return UIColor.FromRGBA(color.R, color.G, color.B, color.A);
//        }

//        return UIColor.White;
//    }

//    public static Color AsColor(this UIColor? color)
//    {
//        if (color != null)
//        {
//            color.GetRGBA(out var red, out var green, out var blue, out var alpha);
//            return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
//        }

//        return Color.White;
//    }
//#endif

//#if MACOS || MACCATALYST
//    public static AppKit.NSColor ToPlatform(this Color color)
//    {
//        if (color != default)
//        {
//            return AppKit.NSColor.FromRgba(color.R, color.G, color.B, color.A);
//        }
//        return AppKit.NSColor.White;
//    }

//    public static Color AsColor(this NSColor? color)
//    {
//        if (color is null)
//            return Color.White;

//        var convertedColorspace = color.UsingColorSpace(NSColorSpace.GenericRGBColorSpace);
//        convertedColorspace.GetRgba(out var red, out var green, out var blue, out var alpha);
//        return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
//    }
//#endif
//}