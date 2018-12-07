using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp.Structs;
using CefSharp.Wpf.Example.Handlers;

namespace CefSharp.Wpf.Example.Controls
{
    public class ChromiumWebBrowserIMESupport : ChromiumWebBrowser
    {
        public ChromiumWebBrowserIMESupport()
        {
            WpfKeyboardHandler = new IMEWpfKeyboardHandler(this);
        }

        static ChromiumWebBrowserIMESupport()
        {
            InputMethod.IsInputMethodEnabledProperty.OverrideMetadata(
                typeof(ChromiumWebBrowserIMESupport),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.Inherits,
                    (obj, e) =>
                    {
                        var browser = obj as ChromiumWebBrowserIMESupport;
                        if ((bool)e.NewValue && browser.GetBrowserHost() != null && Keyboard.FocusedElement == browser)
                        {
                            browser.GetBrowserHost().SendFocusEvent(true);
                            InputMethod.SetIsInputMethodSuspended(browser, true);
                        }
                    }));

            InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(
                typeof(ChromiumWebBrowserIMESupport),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.Inherits));
        }

        protected override void OnImeCompositionRangeChanged(Range selectedRange, Structs.Rect[] characterBounds)
        {
            var imeKeyboardHandler = WpfKeyboardHandler as IMEWpfKeyboardHandler;
            if (imeKeyboardHandler.IsActive)
            {
                var screenInfo = GetScreenInfo();
                var scaleFactor = screenInfo.HasValue ? screenInfo.Value.DeviceScaleFactor : 1.0f;

                UiThreadRunSync(() =>
                {
                    var parentWindow = GetParentWindow();
                    if (parentWindow != null)
                    {
                        var point = TransformToAncestor(parentWindow).Transform(new System.Windows.Point(0, 0));
                        var rects = new List<Structs.Rect>();

                        foreach (var item in characterBounds)
                            rects.Add(new Structs.Rect(
                                (int)((point.X + item.X) * scaleFactor),
                                (int)((point.Y + item.Y) * scaleFactor),
                                (int)(item.Width * scaleFactor),
                                (int)(item.Height * scaleFactor)));

                        imeKeyboardHandler.ChangeCompositionRange(selectedRange, rects);
                    }
                });
            }

            Visual GetParentWindow()
            {
                var current = VisualTreeHelper.GetParent(this);
                while (current != null && !(current is Window))
                    current = VisualTreeHelper.GetParent(current);

                return current as Window;
            }
        }
    }
}
