using System;

using Foundation;
using UIKit;

using Linn.Kinsky;

namespace KinskyTouch
{
    abstract class ViewWidgetButton : IViewWidgetButton
    {
        protected ViewWidgetButton(UIButton aButton)
        {
            iButton = aButton;
            iButton.TouchUpInside += TouchUpInside;
        }

        public void Open()
        {
            lock(this)
            {
                OnOpen();
                iButton.Enabled = true;
            }
        }

        public void Close()
        {
            lock(this)
            {
                OnClose();
                iButton.Enabled = false;
            }
        }

        public event EventHandler<EventArgs> EventClick;

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        private void TouchUpInside(object aSender, EventArgs e)
        {
            if(EventClick != null)
            {
                EventClick(this, EventArgs.Empty);
            }
        }

        protected UIButton iButton;
    }

    abstract class ViewWidgetBarButtonItem : IViewWidgetButton
    {
        protected ViewWidgetBarButtonItem(UIBarButtonItem aButton)
        {
            iButton = aButton;
            iButton.Clicked += Clicked;
        }

        public void Open()
        {
            lock(this)
            {
                OnOpen();
                iButton.Enabled = true;
            }
        }

        public void Close()
        {
            lock(this)
            {
                OnClose();
                iButton.Enabled = false;
            }
        }

        public event EventHandler<EventArgs> EventClick;

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        private void Clicked(object aSender, EventArgs e)
        {
            if(EventClick != null)
            {
                EventClick(this, EventArgs.Empty);
            }
        }

        protected UIBarButtonItem iButton;
    }

    internal class ViewWidgetButtonWasteBin : ViewWidgetBarButtonItem
    {
        public ViewWidgetButtonWasteBin(UIBarButtonItem aButton)
            : base(aButton)
        {
        }
    }

    internal class ViewWidgetButtonStandby : ViewWidgetButton
    {
        public ViewWidgetButtonStandby(UIButton aButton)
            : base(aButton)
        {
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            iButton.Selected = true;
        }

        protected override void OnClose()
        {
            base.OnClose();
            iButton.Selected = false;
        }
    }

    internal class ViewWidgetButtonSave : ViewWidgetBarButtonItem
    {
        public ViewWidgetButtonSave(UIBarButtonItem aButton)
            : base(aButton)
        {
        }
    }
}