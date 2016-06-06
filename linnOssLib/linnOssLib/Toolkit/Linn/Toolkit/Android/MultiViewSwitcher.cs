using System;
using Android.Content;
using Android.Views;
using Android.Graphics;
using Android.Views.Animations;
using Android.Widget;
using System.Collections.Generic;
using Android.Runtime;
using Android.Util;
using Linn;
namespace OssToolkitDroid
{
    
    public class MultiViewSwitcher : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public event EventHandler<EventArgsSwipe> EventSwipe;

        public MultiViewSwitcher(IInvoker aInvoker, ViewGroup aHiddenContainer, ViewGroup aVisibleContainer, IList<View> aViews, View aInitialView, MultiViewPageIndicator aViewPageIndicator)
            : base()
        {
            iInvoker = aInvoker;
            iVisibleContainer = aVisibleContainer;
            iHiddenContainer = aHiddenContainer;
            iViewPageIndicator = aViewPageIndicator;
            iViewPageIndicator.EventPagePrevious += EventPagePreviousHandler;
            iViewPageIndicator.EventPageNext += EventPageNextHandler;
            iViews = aViews;
            iViewPageIndicator.Count = iViews.Count;
            iViewPageIndicator.SelectedIndex = aViews.IndexOf(aInitialView);
            iSwipeGestureDetector = new SwipeGestureDetector(iVisibleContainer.Context);
            iSwipeGestureDetector.EventSwipe += EventSwipeHandler;
            iGestureDetector = new GestureDetector(iSwipeGestureDetector);
            foreach (View v in iViews)
            {
                v.SetOnTouchListener(this);
                if (v != aInitialView)
                {
                    iHiddenContainer.AddView(v);
                }
                else
                {
                    iVisibleContainer.AddView(v);
                    iCurrentView = v;
                }
            }
        }

        private void EventPagePreviousHandler(object sender, EventArgs e)
        {
            MovePrevious();
        }

        private void EventPageNextHandler(object sender, EventArgs e)
        {
            MoveNext();
        }

        public void Close()
        {
            iSwipeGestureDetector.EventSwipe -= EventSwipeHandler;
            foreach (View v in iViews)
            {
                v.SetOnTouchListener(null);
            }
            iVisibleContainer.RemoveAllViews();
            iHiddenContainer.RemoveAllViews();
        }

        public void AddListener(View aView)
        {
            aView.SetOnTouchListener(this);
        }

        public void RemoveListener(View aView)
        {
            aView.SetOnTouchListener(null);
        }


        #region IOnTouchListener Members

        public bool OnTouch(View v, MotionEvent e)
        {
            iSwipeGestureDetector.CallBaseDown = v is ListView;
            return iGestureDetector.OnTouchEvent(e);
        }

        #endregion

        private void EventSwipeHandler(object sender, EventArgsSwipe e)
        {
            if (e.Direction == ESwipeDirection.Left)
            {
                MoveNext();
            }
            else if (e.Direction == ESwipeDirection.Right)
            {
                MovePrevious();
            }
            OnEventSwipe(e.Direction);
        }

        public void MoveNext()
        {
            if (iCurrentView != iViews[iViews.Count - 1])
            {
                int index = iViews.IndexOf(iCurrentView);
                iVisibleContainer.RemoveView(iCurrentView);
                iHiddenContainer.AddView(iCurrentView);
                iCurrentView = iViews[index + 1];
                iHiddenContainer.RemoveView(iCurrentView);
                iVisibleContainer.AddView(iCurrentView);
                iViewPageIndicator.SelectedIndex = index + 1;
            }
        }

        public void MovePrevious()
        {
            if (iCurrentView != iViews[0])
            {
                int index = iViews.IndexOf(iCurrentView);
                iVisibleContainer.RemoveView(iCurrentView);
                iHiddenContainer.AddView(iCurrentView);
                iCurrentView = iViews[index - 1];
                iHiddenContainer.RemoveView(iCurrentView);
                iVisibleContainer.AddView(iCurrentView);
                iViewPageIndicator.SelectedIndex = index - 1;
            }
        }

        private void OnEventSwipe(ESwipeDirection aDirection)
        {
            EventHandler<EventArgsSwipe> del = EventSwipe;
            if (del != null)
            {
                del(this, new EventArgsSwipe(aDirection));
            }
        }

        private IList<View> iViews;
        private ViewGroup iVisibleContainer;
        private SwipeGestureDetector iSwipeGestureDetector;
        private GestureDetector iGestureDetector;
        private MultiViewPageIndicator iViewPageIndicator;
        private ViewGroup iHiddenContainer;
        private View iCurrentView;
        private IInvoker iInvoker;

    }
    
    public class MultiViewPageIndicator : View
    {
        public event EventHandler<EventArgs> EventPagePrevious;
        public event EventHandler<EventArgs> EventPageNext;

        public MultiViewPageIndicator(Context aContext)
            : base(aContext)
        {
            Init();
        }

        public MultiViewPageIndicator(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            Init();
        }

        public MultiViewPageIndicator(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            Init();
        }

        private void Init()
        {
            iPaintSelected = new Paint() { Color = Android.Graphics.Color.White, StrokeWidth = 1, AntiAlias = true };
            iPaintSelected.SetStyle(Paint.Style.Fill);
            iPaintUnselected = new Paint() { Color = Android.Graphics.Color.Gray, StrokeWidth = 1, AntiAlias = true };
            iPaintUnselected.SetStyle(Paint.Style.Fill);
            this.Touch += TouchHandler;
        }

        private void TouchHandler(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                int x = (int)e.Event.GetX();
                if (x < this.Width / 2)
                {
                    OnEventPagePrevious();
                }
                else
                {
                    OnEventPageNext();
                }
            }
        }

        public int Count
        {
            get
            {
                return iCount;
            }
            set
            {
                iCount = value;
                Invalidate();
            }
        }

        public int SelectedIndex
        {
            get
            {
                return iSelectedIndex;
            }
            set
            {
                iSelectedIndex = value;
                Invalidate();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int height = (kOuterPaddingVertical * 2) + (kSelectedRadius * 2);
            int width = Math.Max(0, (kInterItemPadding * (iCount - 1))) + Math.Max(0, (kDeselectedRadius * (iCount - 1) * 2)) + (kOuterPaddingHorizontal * 2) + (kSelectedRadius * 2);
            this.SetMeasuredDimension(width, height);
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
            int x = kOuterPaddingHorizontal;
            int y = (this.Height / 2);
            for (int i = 0; i < Count; i++)
            {
                int radius = i == iSelectedIndex ? kSelectedRadius : kDeselectedRadius;
                x += radius;
                canvas.DrawCircle(x, y, radius, i == iSelectedIndex ? iPaintSelected : iPaintUnselected);
                x += radius + kInterItemPadding;
            }
        }

        private void OnEventPagePrevious()
        {
            EventHandler<EventArgs> del = EventPagePrevious;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventPageNext()
        {
            EventHandler<EventArgs> del = EventPageNext;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private Paint iPaintSelected;
        private Paint iPaintUnselected;
        private const int kSelectedRadius = 5;
        private const int kDeselectedRadius = 5;
        private const int kInterItemPadding = 10;
        private const int kOuterPaddingHorizontal = 150;
        private const int kOuterPaddingVertical = 15;
        private int iCount;
        private int iSelectedIndex;
    }

    
    public class SwipeGestureDetector : Android.Views.GestureDetector.SimpleOnGestureListener
    {

        public event EventHandler<EventArgsSwipe> EventSwipe;

        public SwipeGestureDetector(Context aContext)
        {
            iContext = aContext;
        }

        public bool CallBaseDown { get; set; }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            try
            {
                if (e1 != null && e2 != null)
                {
                    ViewConfiguration vc = ViewConfiguration.Get(iContext);
                    int swipeMinDistance = vc.ScaledPagingTouchSlop; 
                    int swipeThresholdVelocity = vc.ScaledMinimumFlingVelocity;
                    //int swipeMaxOffPath = vc.ScaledTouchSlop;
                    float length = Math.Abs(e1.GetX() - e2.GetX());
                    float height = Math.Abs(e1.GetY() - e2.GetY());

                    if (height / length > 0.5f)
                    {
                        return false;
                    }
                    // right to left swipe
                    if (e1.GetX() - e2.GetX() > swipeMinDistance && Math.Abs(velocityX) > swipeThresholdVelocity)
                    {
                        OnEventSwipe(ESwipeDirection.Left);
                        return true;
                    }
                    else if (e2.GetX() - e1.GetX() > swipeMinDistance && Math.Abs(velocityX) > swipeThresholdVelocity)
                    {
                        OnEventSwipe(ESwipeDirection.Right);
                        return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        public override bool OnDown(MotionEvent e)
        {
            return CallBaseDown ? base.OnDown(e) : true;
        }


        private void OnEventSwipe(ESwipeDirection aDirection)
        {
            EventHandler<EventArgsSwipe> del = EventSwipe;
            if (del != null)
            {
                del(this, new EventArgsSwipe(aDirection));
            }
        }

        private Context iContext;
    }

    public class EventArgsSwipe : EventArgs
    {
        public EventArgsSwipe(ESwipeDirection aDirection)
        {
            Direction = aDirection;
        }
        public ESwipeDirection Direction { get; set; }
    }

    public enum ESwipeDirection
    {
        Left,
        Right
    }
    
}