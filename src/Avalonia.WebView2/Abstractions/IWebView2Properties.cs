using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;

public interface IWebView2Properties
{
    bool AllowExternalDrop { get; set; }

    bool CanGoBack { get; }

    bool CanGoForward { get; }

    Color DefaultBackgroundColor { get; set; }

    string? HtmlSource { get; set; }

    Uri? Source { get; set; }

    double ZoomFactor { get; set; }
}
