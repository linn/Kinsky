using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
namespace KinskyDesktopWpf
{
    public class BreadcrumbPanel : Panel
    {
        protected override Size MeasureOverride(Size aAvailableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                try { child.Measure(aAvailableSize); }
                catch { }
                resultSize.Width += child.DesiredSize.Width;
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            resultSize.Width = double.IsPositiveInfinity(aAvailableSize.Width) ? resultSize.Width : aAvailableSize.Width;
            resultSize.Width = Math.Min(resultSize.Width, aAvailableSize.Width);

            double[] constrainedWidths = CalculateConstrainedWidths(resultSize);


            double x = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Measure(new Size(constrainedWidths[i], resultSize.Height));
                x += constrainedWidths[i];
            }

            return new Size(x, resultSize.Height);
        }

        protected override Size ArrangeOverride(Size aFinalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
            {
                return aFinalSize;
            }

            double[] constrainedWidths = CalculateConstrainedWidths(aFinalSize);

            double x = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                //Children[i].Measure(new Size(constrainedWidths[i], aFinalSize.Height));
                Children[i].Arrange(new Rect(x, 0, constrainedWidths[i], aFinalSize.Height));
                x += constrainedWidths[i];
            }

            return new Size(x, aFinalSize.Height);
        }

        private double[] CalculateConstrainedWidths(Size aAvailableSize)
        {
            double finalWidth = aAvailableSize.Width;
            double desiredWidth = 0;

            double[] constrainedWidths = new double[Children.Count];

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                constrainedWidths[i] = Children[i].DesiredSize.Width;
                desiredWidth += constrainedWidths[i];
            }

            if (desiredWidth > aAvailableSize.Width)
            {
                // first constrain the first and last items to be no greater than half the total available width 
                if (constrainedWidths[0] > (aAvailableSize.Width / 2))
                {
                    double gain = constrainedWidths[0] - (aAvailableSize.Width / 2);
                    constrainedWidths[0] = aAvailableSize.Width / 2;
                    desiredWidth -= gain;
                }
                if (constrainedWidths[constrainedWidths.Length - 1] > (aAvailableSize.Width / 2))
                {
                    double gain = constrainedWidths[constrainedWidths.Length - 1] - (aAvailableSize.Width / 2);
                    constrainedWidths[constrainedWidths.Length - 1] = aAvailableSize.Width / 2;
                    desiredWidth -= gain;
                }

                // next constrain internal children's widths if that is not enough
                if (desiredWidth > aAvailableSize.Width && Children.Count > 2)
                {
                    double spaceLeft = aAvailableSize.Width - constrainedWidths[0] - constrainedWidths[constrainedWidths.Length - 1];
                    int maxDisplayable = (int)(spaceLeft / kMinWidthForDisplay);
                    int startIndex = 1;
                    if (maxDisplayable < Children.Count - 2)
                    {
                        for (int i = 1; i <= Children.Count - 2 - maxDisplayable; i++)
                        {
                            desiredWidth -= constrainedWidths[i];
                            constrainedWidths[i] = 0;
                            startIndex = i + 1;
                        }
                    }

                    if (desiredWidth > aAvailableSize.Width)
                    {
                        double available = aAvailableSize.Width - (constrainedWidths[0] + constrainedWidths[constrainedWidths.Length - 1]);
                        double required = 0;
                        for (int i = startIndex; i < constrainedWidths.Length - 1; i++)
                        {
                            required += constrainedWidths[i];
                        }



                        for (int i = startIndex; i < constrainedWidths.Length - 1; i++)
                        {
                            double trim = (required - available) / (constrainedWidths.Length - 1 - i);
                            constrainedWidths[i] = Math.Max(constrainedWidths[i] - trim, kMinWidthForDisplay);
                            required -= (constrainedWidths[i]);
                            available -= (constrainedWidths[i]);
                        }
                    }
                }

            }

            return constrainedWidths;
        }

        private const double kMinWidthForDisplay = 50;
    }
}