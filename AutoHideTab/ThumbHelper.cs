using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AutoHideTab
{
    internal class ThumbHelper : DependencyObject
    {
        public static FrameworkElement GetResizeTarget(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ResizeTargetProperty);
        }

        public static void SetResizeTarget(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ResizeTargetProperty, value);
        }

        // Using a DependencyProperty as the backing store for ResizeTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeTargetProperty =
            DependencyProperty.RegisterAttached("ResizeTarget", typeof(FrameworkElement), typeof(ThumbHelper), new PropertyMetadata(null, ResizeTargetChanged));

        private static void ResizeTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thumb = (Thumb)d;
            if (e.NewValue is FrameworkElement)
            {
                thumb.DragDelta += Thumb_DragDelta;
            }
        }

        private static void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var target = GetResizeTarget(thumb);
            target.Height = Math.Max(0, target.ActualHeight - e.VerticalChange);
        }
    }
}
