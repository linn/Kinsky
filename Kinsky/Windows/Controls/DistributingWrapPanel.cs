using System.Windows.Controls;
using System.Windows;
using System;

namespace KinskyDesktopWpf
{
    /* A simple, nonvirtualized wrap panel that distributes its content evenly instead of bunching them up to the left hand side */
    public class DistributingWrapPanel : Panel
    {

        public DistributingWrapPanel() : base() { }

        protected override Size MeasureOverride(Size aSize)
        {
            Size rowSize = new Size();
            Size result = new Size();

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i] as UIElement;
                child.Measure(aSize);
                Size desiredSize = child.DesiredSize;

                if (rowSize.Width + desiredSize.Width > aSize.Width)
                {
                    result.Width = Math.Max(rowSize.Width, result.Width);
                    result.Height += rowSize.Height;
                    rowSize = desiredSize;

                    if (desiredSize.Width > aSize.Width)
                    {
                        result.Width = Math.Max(desiredSize.Width, result.Width);
                        result.Height += desiredSize.Height;
                        rowSize = new Size();
                    }
                }
                else
                {
                    rowSize.Width += desiredSize.Width;
                    rowSize.Height = Math.Max(desiredSize.Height, rowSize.Height);
                }
            }

            result.Width = Math.Max(rowSize.Width, result.Width);
            result.Height += rowSize.Height;

            return result;
        }


        protected override Size ArrangeOverride(Size aSize)
        {
            int rowStartIndex = 0;
            Size rowSize = new Size();
            double totalHeight = 0;
            double rowWidth = 0.0;
            double itemPadding = 0.0;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                Size desiredSize = InternalChildren[i].DesiredSize;
                if (rowSize.Width + desiredSize.Width > aSize.Width)
                {
                    if (itemPadding == 0.0)
                    {
                        itemPadding = (aSize.Width - rowWidth) / (i - rowStartIndex);
                    }
                    ArrangeRow(totalHeight, rowSize.Height, rowStartIndex, i, itemPadding);
                    rowWidth = 0.0;

                    totalHeight += rowSize.Height;
                    rowSize = desiredSize;

                    if (desiredSize.Width > aSize.Width)
                    {
                        ArrangeRow(totalHeight, desiredSize.Height, i, ++i, itemPadding);
                        rowWidth = 0.0;
                        totalHeight += desiredSize.Height;
                        rowSize = new Size();
                    }
                    rowStartIndex = i;
                }
                else
                {
                    rowSize.Width += desiredSize.Width;
                    rowSize.Height = Math.Max(desiredSize.Height, rowSize.Height);
                }
                rowWidth += desiredSize.Width;
            }

            if (rowStartIndex < InternalChildren.Count)
            {
                ArrangeRow(totalHeight, rowSize.Height, rowStartIndex, InternalChildren.Count, itemPadding);
            }

            return aSize;
        }

        private void ArrangeRow(double y, double aRowHeight, int aStart, int aEnd, double aItemPadding)
        {
            double x = 0;
            for (int i = aStart; i < aEnd; i++)
            {
                UIElement child = InternalChildren[i];
                child.Arrange(new Rect(x + (aItemPadding / 2), y, child.DesiredSize.Width, aRowHeight));
                if (child.DesiredSize.Width > 0)
                {
                    x += child.DesiredSize.Width + aItemPadding;
                }
            }
        }

    }


}