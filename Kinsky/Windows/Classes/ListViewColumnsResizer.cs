
using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace KinskyDesktopWPF
{

    public class ListViewColumnsResizer : DependencyObject
    {
        private ListView iParentListView = null;

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.RegisterAttached("Stretch",
            typeof(bool),
            typeof(ListViewColumnsResizer),
            new UIPropertyMetadata(true, null, OnCoerceStretch));


        public static bool GetStretch(DependencyObject obj)
        {
            return (bool)obj.GetValue(StretchProperty);
        }

        public static void SetStretch(DependencyObject obj, bool value)
        {
            obj.SetValue(StretchProperty, value);
        }

        public static object OnCoerceStretch(DependencyObject source, object value)
        {
            ListView lv = (source as ListView);

            if (lv == null)
                throw new ArgumentException("This property may only be used on ListViews");

            lv.Loaded += new RoutedEventHandler(lv_Loaded);
            lv.SizeChanged += new SizeChangedEventHandler(lv_SizeChanged);
            return value;
        }

        private static void lv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView lv = (sender as ListView);
            if (lv.IsLoaded)
            {
                SetColumnWidths(lv);
            }
        }
        private static void lv_Loaded(object sender, RoutedEventArgs e)
        {
            ListView lv = (sender as ListView);
            SetColumnWidths(lv);
        }

        public static void SetColumnWidths(ListView listView)
        {
            List<GridViewColumn> columns = (listView.Tag as List<GridViewColumn>);
            double specifiedWidth = 0;
            GridView gridView = listView.View as GridView;
            if (gridView != null)
            {
                if (columns == null)
                {
                    columns = new List<GridViewColumn>();
                    foreach (GridViewColumn column in gridView.Columns)
                    {
                        if (!(column.Width >= 0))
                            columns.Add(column);
                        specifiedWidth += column.ActualWidth;
                    }
                }
                else
                {
                    foreach (GridViewColumn column in gridView.Columns)
                        specifiedWidth += column.ActualWidth;
                }

                double viewWidth = listView.ActualWidth - listView.Margin.Left - listView.Margin.Right;

                if (specifiedWidth < viewWidth)
                {
                    double newWidth = (viewWidth - specifiedWidth) / columns.Count;
                    if (newWidth >= 0)
                    {
                        foreach (GridViewColumn column in columns)
                        {
                            var newColumnWidth = column.ActualWidth + newWidth - 10;
                            if (newColumnWidth > 0)
                            {
                                column.Width = newColumnWidth;
                            }
                        }
                    }
                }
                else
                {
                    double newWidth = (specifiedWidth - viewWidth) / columns.Count;
                    if (newWidth >= 0)
                    {
                        foreach (GridViewColumn column in columns)
                        {
                            var newColumnWidth = column.ActualWidth - newWidth - 10;
                            if (newColumnWidth > 0)
                            {
                                column.Width = newColumnWidth;
                            }
                        }
                    }
                }
                listView.Tag = columns;
            }
        }


    }
}