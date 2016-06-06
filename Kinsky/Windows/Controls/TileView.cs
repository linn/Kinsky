using System.Windows.Controls;
using System.Windows;

namespace KinskyDesktopWpf
{
    public class TileView : ViewBase
    {

        public static readonly DependencyProperty
          ItemContainerStyleProperty =
          ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(TileView));

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(TileView));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        protected override object DefaultStyleKey
        {
            get
            {
                return typeof(TileView);
            }
        }

    }
}