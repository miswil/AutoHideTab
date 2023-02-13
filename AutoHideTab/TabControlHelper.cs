using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoHideTab
{
    internal class TabControlHelper : DependencyObject
    {
        private static Thickness GetOriginalMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(OriginalMarginProperty);
        }

        private static void SetOriginalMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(OriginalMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for OriginalMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalMarginProperty =
            DependencyProperty.RegisterAttached("OriginalMargin", typeof(Thickness), typeof(TabControlHelper), new PropertyMetadata(default));

        public static bool GetFloatTabContentEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(FloatTabContentEnabledProperty);
        }

        public static void SetFloatTabContentEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(FloatTabContentEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for FloatTabContentEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FloatTabContentEnabledProperty =
            DependencyProperty.RegisterAttached("FloatTabContentEnabled", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(false, FloatTabContentEnabledChanged));

        private static void FloatTabContentEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = (TabControl)d;
            if ((bool)e.NewValue)
            {
                SetOriginalMargin(tabControl, tabControl.Margin);
                tabControl.GotFocus += TabControl_GotFocus;
                tabControl.LostFocus += TabControl_LostFocus;
                tabControl.SelectionChanged += TabControl_SelectionChanged;
                tabControl.SizeChanged += TabControl_SizeChanged;
                tabControl.CommandBindings.Add(new CommandBinding(CloseTabCommand, CloseTabCommaneExecuted));
            }
            else
            {

            }
        }

        private static void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (tabControl.ItemsSource.GetEnumerator().MoveNext())
            {
                tabControl.Visibility = Visibility.Visible;
                ShowTabContent(tabControl);
                if (GetIsTabContentPinned(tabControl))
                {
                    PinTabContent(tabControl);
                }
                else
                {
                    FloatTabContent(tabControl);
                }
            }
            else
            {
                tabControl.Visibility = Visibility.Collapsed;
            }
        }

        private static void TabControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            ShowTabContent(tabControl);
            if (GetIsTabContentPinned(tabControl))
            {
                PinTabContent(tabControl);
            }
            else
            {
                FloatTabContent(tabControl);
            }
        }

        private static void TabControl_LostFocus(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (GetIsTabContentPinned(tabControl))
            {
                PinTabContent(tabControl);
            }
            else
            {
                HideTabContent(tabControl);
            }
        }

        private static void TabControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (GetIsTabContentPinned(tabControl))
            {
                PinTabContent(tabControl);
            }
            else
            {
                FloatTabContent(tabControl);
            }
        }

        public static bool GetIsTabContentPinned(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTabContentPinnedProperty);
        }

        public static void SetIsTabContentPinned(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTabContentPinnedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsTabContentPinned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTabContentPinnedProperty =
            DependencyProperty.RegisterAttached("IsTabContentPinned", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(false, IsTabContentPinnedChanged));

        private static void IsTabContentPinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = (TabControl)d;
            if ((bool)e.NewValue)
            {
                tabControl.Margin = (Thickness)GetOriginalMargin(tabControl)!;
                var contentArea = (FrameworkElement)tabControl.Template.FindName("ContentArea", tabControl);
                contentArea.SetValue(Panel.ZIndexProperty, 0);
            }
            else
            {
                FloatTabContent(tabControl);
            }
        }

        private static void ShowTabContent(TabControl tabControl)
        {
            tabControl.Focus();
            var contentArea = GetTabContentArea(tabControl);
            contentArea.Visibility = Visibility.Visible;
        }

        private static void HideTabContent(TabControl tabControl)
        {
            var contentArea = GetTabContentArea(tabControl);
            contentArea.Visibility = Visibility.Collapsed;
        }

        private static void FloatTabContent(TabControl tabControl)
        {
            var contentArea = GetTabContentArea(tabControl);
            var originalMargin = (Thickness)GetOriginalMargin(tabControl)!;
            var floatMargin = new Thickness(originalMargin.Left, originalMargin.Top - contentArea.ActualHeight, originalMargin.Right, originalMargin.Bottom);
            tabControl.Margin = floatMargin;
            contentArea.SetValue(Panel.ZIndexProperty, 100);
        }

        private static void PinTabContent(TabControl tabControl)
        {
            tabControl.Margin = (Thickness)GetOriginalMargin(tabControl);
            var contentArea = GetTabContentArea(tabControl);
            contentArea.SetValue(Panel.ZIndexProperty, 0);
        }

        private static FrameworkElement GetTabContentArea(TabControl tabControl)
        {
            tabControl.ApplyTemplate();
            return (FrameworkElement)tabControl.Template.FindName("ContentArea", tabControl);
        }

        public static ICommand CloseTabCommand = new RoutedCommand();

        private static void CloseTabCommaneExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (tabControl.ItemsSource is IList list)
            {
                list.Remove(tabControl.SelectedItem);
            }
        }
    }
}
