using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Linn;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using Linn.Topology;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using Upnp;
using System.Drawing;

namespace KinskyDesktopWpf
{

    public partial class ThreekArray : UserControl
    {
        public event EventHandler EventClickLeft;
        public event EventHandler EventClickMiddle;
        public event EventHandler EventClickRight;
        public event EventHandler<DragEventArgs> EventDragOverLeft;
        public event EventHandler<DragEventArgs> EventDragOverMiddle;
        public event EventHandler<DragEventArgs> EventDragOverRight;
        public event EventHandler<DragEventArgs> EventDragDropLeft;
        public event EventHandler<DragEventArgs> EventDragDropMiddle;
        public event EventHandler<DragEventArgs> EventDragDropRight;

        public ThreekArray()
        {
            InitializeComponent();
            this.IsEnabled = false;
        }

        public bool ControlLeftEnabled
        {
            get { return (bool)GetValue(ControlLeftEnabledProperty); }
            set { SetValue(ControlLeftEnabledProperty, value); }
        }

        public static readonly DependencyProperty ControlLeftEnabledProperty =
            DependencyProperty.Register("ControlLeftEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));



        public bool ControlMiddleEnabled
        {
            get { return (bool)GetValue(ControlMiddleEnabledProperty); }
            set { SetValue(ControlMiddleEnabledProperty, value); }
        }

        public static readonly DependencyProperty ControlMiddleEnabledProperty =
            DependencyProperty.Register("ControlMiddleEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));


        public bool ControlRightEnabled
        {
            get { return (bool)GetValue(ControlRightEnabledProperty); }
            set { SetValue(ControlRightEnabledProperty, value); }
        }

        public static readonly DependencyProperty ControlRightEnabledProperty =
            DependencyProperty.Register("ControlRightEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));


        public bool PlaylistLeftEnabled
        {
            get { return (bool)GetValue(PlaylistLeftEnabledProperty); }
            set { SetValue(PlaylistLeftEnabledProperty, value); }
        }

        public static readonly DependencyProperty PlaylistLeftEnabledProperty =
            DependencyProperty.Register("PlaylistLeftEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));



        public bool PlaylistMiddleEnabled
        {
            get { return (bool)GetValue(PlaylistMiddleEnabledProperty); }
            set { SetValue(PlaylistMiddleEnabledProperty, value); }
        }

        public static readonly DependencyProperty PlaylistMiddleEnabledProperty =
            DependencyProperty.Register("PlaylistMiddleEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));


        public bool PlaylistRightEnabled
        {
            get { return (bool)GetValue(PlaylistRightEnabledProperty); }
            set { SetValue(PlaylistRightEnabledProperty, value); }
        }

        public static readonly DependencyProperty PlaylistRightEnabledProperty =
            DependencyProperty.Register("PlaylistRightEnabled", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));

        public static bool GetIsDraggedOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDraggedOverProperty);
        }

        public static void SetIsDraggedOver(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDraggedOverProperty, value);
        }

        public static readonly DependencyProperty IsDraggedOverProperty =
            DependencyProperty.RegisterAttached("IsDraggedOver", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));



        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));


        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));



        public bool IsUsingPauseButton
        {
            get { return (bool)GetValue(IsUsingPauseButtonProperty); }
            set { SetValue(IsUsingPauseButtonProperty, value); }
        }

        public static readonly DependencyProperty IsUsingPauseButtonProperty =
            DependencyProperty.Register("IsUsingPauseButton", typeof(bool), typeof(ThreekArray), new UIPropertyMetadata(false));




        void hitBoxLeftButton_Click(object sender, RoutedEventArgs args)
        {
            if (EventClickLeft != null && ControlLeftEnabled)
            {
                EventClickLeft(this, new EventArgs());
            }
        }

        void hitBoxMiddleButton_Click(object sender, RoutedEventArgs args)
        {
            if (EventClickMiddle != null && ControlMiddleEnabled)
            {
                EventClickMiddle(this, new EventArgs());
            }
        }

        void hitBoxRightButton_Click(object sender, RoutedEventArgs args)
        {
            if (EventClickRight != null && ControlRightEnabled)
            {
                EventClickRight(this, new EventArgs());
            }
        }

        void ThreekArray_Drop(object sender, DragEventArgs args)
        {
            try{
            UserControl parent = GetEventSourceElement(args);
            ThreekArray.SetIsDraggedOver(btnLeft, false);
            ThreekArray.SetIsDraggedOver(btnMiddle, false);
            ThreekArray.SetIsDraggedOver(btnRight, false);
            if (parent == btnLeft)
            {
                if (EventDragDropLeft != null)
                {
                    EventDragDropLeft(this, args);
                }

            }
            else if (parent == btnMiddle)
            {
                if (EventDragDropMiddle != null)
                {
                    EventDragDropMiddle(this, args);
                }
            }
            else if (parent == btnRight)
            {
                if (EventDragDropRight != null)
                {
                    EventDragDropRight(this, args);
                }
            }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in ThreeKArray.DragDrop: " + ex);
            }
        }

        void ThreekArray_DragOver(object sender, DragEventArgs args)
        {
            try{
            UserControl parent = GetEventSourceElement(args);
            ThreekArray.SetIsDraggedOver(btnLeft, false);
            ThreekArray.SetIsDraggedOver(btnMiddle, false);
            ThreekArray.SetIsDraggedOver(btnRight, false);

            ThreekArray.SetIsDraggedOver(parent, true);
            if (parent == btnLeft && PlaylistLeftEnabled)
            {
                if (EventDragOverLeft != null)
                {
                    EventDragOverLeft(this, args);
                    args.Handled = true;
                }

            }
            else if (parent == btnMiddle && PlaylistMiddleEnabled)
            {
                if (EventDragOverMiddle != null)
                {
                    EventDragOverMiddle(this, args);
                    args.Handled = true;
                }
            }
            else if (parent == btnRight && PlaylistRightEnabled)
            {
                if (EventDragOverRight != null)
                {
                    EventDragOverRight(this, args);
                    args.Handled = true;
                }
            }

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in ThreeKArray.DragOver: " + ex);
            }

        }

        private UserControl GetEventSourceElement(RoutedEventArgs args)
        {
            DependencyObject dep = (DependencyObject)args.OriginalSource;
            while ((dep != null) && !(dep is UserControl))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            return dep as UserControl;
        }
    }
}
