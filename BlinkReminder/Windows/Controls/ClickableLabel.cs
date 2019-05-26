using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BlinkReminder.Windows.Controls
{
    /// <summary>
    /// Custom label implementation from: https://social.msdn.microsoft.com/Forums/vstudio/en-US/6225f1ce-8c62-4f08-89ed-aa89e0c8fc41/label-mouseclick
    /// </summary>
    public class ClickableLabel : Label
    {
        public static readonly RoutedEvent ClickEvent;

        static ClickableLabel()
        {
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(ClickableLabel));
        }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }

            remove { RemoveHandler(ClickEvent, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            CaptureMouse();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();

                if (IsMouseOver)

                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }
    }
}
