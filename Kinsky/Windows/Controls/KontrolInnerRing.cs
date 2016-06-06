using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
namespace KinskyDesktopWpf
{

    public class KontrolInnerRing : FrameworkElement
    {

        public Brush ValueBrush
        {
            get { return (Brush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register("ValueBrush", typeof(Brush), typeof(KontrolInnerRing), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        public Brush UpdatingValueBrush
        {
            get { return (Brush)GetValue(UpdatingValueBrushProperty); }
            set { SetValue(UpdatingValueBrushProperty, value); }
        }

        public static readonly DependencyProperty UpdatingValueBrushProperty =
            DependencyProperty.Register("UpdatingValueBrush", typeof(Brush), typeof(KontrolInnerRing), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(KontrolInnerRing), new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));



        protected override void OnRender(DrawingContext aDrawingContext)
        {
            base.OnRender(aDrawingContext);

            double value = Kontrol.GetValue(this);
            double maxValue = Kontrol.GetMaxValue(this);
            double updatingValue = Kontrol.GetUpdatingValue(this);

            if (maxValue > 0)
            {
                if (value >= maxValue)
                {
                    value = maxValue - 0.1;
                }
                double sweepAngle = 360d * (value / maxValue);
                double startAngle = 90d;

                Rect rect = new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height);
               
                Pen pen = new Pen();
                pen.Brush = ValueBrush;
                pen.Thickness = Thickness;
                DrawArc(aDrawingContext, rect, startAngle, sweepAngle, pen);
                if (updatingValue != value && updatingValue != 0)
                {
                    startAngle = startAngle + sweepAngle;
                    sweepAngle = 360d * ((updatingValue - value) / maxValue);
                    pen = new Pen();
                    pen.Brush = UpdatingValueBrush;
                    pen.Thickness = Thickness;
                    DrawArc(aDrawingContext, rect, startAngle, sweepAngle, pen);
                }
            }
        }

        private void DrawArc(DrawingContext aDrawingContext, Rect aRect, double aStartAngle, double aSweepAngle, Pen aPen)
        {
            double degToRad = 180d / Math.PI;
            bool isLargeArc = (Math.Abs(aSweepAngle) >= 180);
            SweepDirection sweepDir = aSweepAngle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

            double sweepAngleRad = aSweepAngle / degToRad;
            double startAngleRad = aStartAngle / degToRad;

            double centerX = aRect.Width / 2d;
            double centerY = aRect.Height / 2d;
            //Calculate start point  
            double x1 = aRect.X + centerX + (Math.Cos(startAngleRad) * centerX);
            double y1 = aRect.Y + centerY + (Math.Sin(startAngleRad) * centerY);
            //Calculate end point  
            double x2 = aRect.X + centerX + (Math.Cos(startAngleRad + sweepAngleRad) * centerX);
            double y2 = aRect.Y + centerY + (Math.Sin(startAngleRad + sweepAngleRad) * centerY);


            Point startPoint = new Point(x1, y1);
            Point endPoint = new Point(x2, y2);
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = startPoint;

            Size size = new Size(aRect.Width / 2d, aRect.Height / 2d);

            // add the arc to the geometry
            figure.Segments.Add(new ArcSegment(endPoint, size, 0, isLargeArc, sweepDir, true));
            figure.IsClosed = false;
            figure.IsFilled = false;

            aDrawingContext.DrawGeometry(aPen.Brush, aPen, geometry);
        }
    }





}