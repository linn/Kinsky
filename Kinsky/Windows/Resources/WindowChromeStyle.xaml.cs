using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Linn;
using System.Windows.Media;
using System;
namespace KinskyDesktopWpf
{
    public partial class WindowChromeStyle : ResourceDictionary
    {

        public WindowChromeStyle() : base() { }

        public void CloseWindow(object sender, RoutedEventArgs args)
        {
            Window.GetWindow(sender as DependencyObject).Close();
        }

        public void ToggleMiniMode(object sender, RoutedEventArgs args)
        {
            WindowChrome source = (sender as FrameworkElement).FindVisualAncestor<WindowChrome>();
            if (!source.IsAnimating)
            {
                WindowChrome.SetIsMiniModeActive(source, !WindowChrome.GetIsMiniModeActive(source));
            }
        }
        public void MinimizeWindow(object sender, RoutedEventArgs args)
        {
            Window w = Window.GetWindow(sender as DependencyObject);
            w.WindowState = WindowState.Minimized;
        }
        public void RestoreWindow(object sender, RoutedEventArgs args)
        {
            Window w = Window.GetWindow(sender as DependencyObject);
            w.WindowState = WindowState.Normal;
        }
        public void MaximizeWindow(object sender, RoutedEventArgs args)
        {
            Window w = Window.GetWindow(sender as DependencyObject);
            w.WindowState = WindowState.Maximized;
        }
    }
}