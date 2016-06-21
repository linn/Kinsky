

using Android.Widget;
using Android.Views.Animations;
using Android.Content;
using Android.Views;
using Linn;
using System;
namespace OssToolkitDroid
{
    public class MasterDetailView : ViewFlipper, Animation.IAnimationListener
    {
        public MasterDetailView(Context aContext, Android.Graphics.Color aBackgroundColor)
            : base(aContext)
        {
            iBackButtonLayoutId = kNoButtonLayoutId;
            iEditButtonLayoutId = kNoButtonLayoutId;
            iSaveButtonLayoutId = kNoButtonLayoutId;
            iCancelButtonLayoutId = kNoButtonLayoutId;
            iEditMode = false;
            iIsShowingMasterView = true;
            CreateMasterContainer();
            CreateDetailContainer();
            SetBackgroundColor(aBackgroundColor);
        }

        public event EventHandler<EventArgs> EventDetailClosed;
        public event EventHandler<EventArgs> EventEditButtonClicked;
        public event EventHandler<EventArgs> EventSaveButtonClicked;
        public event EventHandler<EventArgs> EventCancelButtonClicked;

        public ViewGroup MasterContainer
        {
            get
            {
                return iMasterContainer;
            }
        }

        public View MasterView
        {
            get
            {
                return iMasterView;
            }
            set
            {
                if (iMasterView != null)
                {
                    iMasterContainer.RemoveView(iMasterView);
                }
                iMasterView = value;
                if (iMasterView != null)
                {
                    RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                    layoutParams.AddRule(LayoutRules.Below, kMasterTitleId);
                    iMasterView.LayoutParameters = layoutParams;
                    iMasterContainer.AddView(iMasterView);
                }
            }
        }

        public View DetailView
        {
            get
            {
                return iDetailView;
            }
            set
            {
                if (!iIsShowingMasterView)
                {
                    ToggleView();
                }
                if (iDetailView != null)
                {
                    iDetailContainer.RemoveView(iDetailView);
                }
                iDetailView = value;
                if (iDetailView != null)
                {
                    RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                    layoutParams.AddRule(LayoutRules.Below, kBackButtonId);
                    iDetailView.LayoutParameters = layoutParams;
                    iDetailContainer.AddView(iDetailView);
                }
            }
        }

        public bool EditMode
        {
            set
            {
                iEditMode = value;
                iEditButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                iEditButton.Checked = false;
            }
        }

        public string CancelButtonText
        {
            set
            {
                iCancelButtonText = value;
                RefreshCancelButton();
            }
        }

        public bool ShowSaveButton
        {
            set
            {
                iShowSaveButton = value;
                iSaveButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        public bool IsEditing
        {
            get
            {
                return iEditMode && iEditButton.Checked;
            }
        }

        public string MasterTitle
        {
            set
            {
                iMasterContainer.FindViewById<TextView>(kMasterTitleId).Text = value;
            }
        }

        public View MasterTitleView
        {
            get
            {
                return iMasterContainer.FindViewById<TextView>(kMasterTitleId);
            }
        }

        public string DetailTitle
        {
            set
            {
                iDetailContainer.FindViewById<TextView>(kDetailTitleId).Text = value;
            }
        }

        public bool IsShowingMasterView
        {
            get
            {
                return iIsShowingMasterView;
            }
        }

        public bool ShowCancelButton
        {
            set
            {
                iShowCancelButton = value;
                iCancelButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void CreateMasterContainer()
        {
            iMasterContainer = new RelativeLayout(Context);
            RelativeLayout.LayoutParams containerLayoutParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);
            iMasterContainer.LayoutParameters = containerLayoutParams;
            RefreshCancelButton();
            RefreshSaveButton();
            TextView title = new TextView(Context, null, Android.Resource.Attribute.TextAppearanceLarge);
            title.Id = kMasterTitleId;
            title.Typeface = Android.Graphics.Typeface.DefaultBold;
            RelativeLayout.LayoutParams titleLayoutParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            titleLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            titleLayoutParams.AddRule(LayoutRules.AlignParentTop);
            titleLayoutParams.BottomMargin = 10;
            title.LayoutParameters = titleLayoutParams;
            iMasterContainer.AddView(title);
            AddView(iMasterContainer);
        }

        private void CreateDetailContainer()
        {
            iDetailContainer = new RelativeLayout(Context);
            RelativeLayout.LayoutParams containerLayoutParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);
            iDetailContainer.LayoutParameters = containerLayoutParams;
            RefreshBackButton();
            RefreshEditButton();
            TextView title = new TextView(Context, null, Android.Resource.Attribute.TextAppearanceLarge);
            title.Id = kDetailTitleId;
            title.Typeface = Android.Graphics.Typeface.DefaultBold;
            RelativeLayout.LayoutParams titleLayoutParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            titleLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            titleLayoutParams.AddRule(LayoutRules.AlignParentTop);
            titleLayoutParams.BottomMargin = 10;
            title.LayoutParameters = titleLayoutParams;
            iDetailContainer.AddView(title);
            AddView(iDetailContainer);
        }

        private void RefreshCancelButton()
        {
            if (iCancelButton != null)
            {
                iMasterContainer.RemoveView(iCancelButton);
                iCancelButton.Click -= CancelButtonClickHandler;
                iCancelButton.Dispose();
            }
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            iCancelButton = CancelButtonLayoutId == kNoButtonLayoutId ? new Button(Context) : inflater.Inflate(CancelButtonLayoutId, null) as Button;
            iCancelButton.Text = iCancelButtonText;
            iCancelButton.Id = kCancelButtonId;
            iCancelButton.Click += CancelButtonClickHandler;
            iCancelButton.Visibility = iShowCancelButton ? ViewStates.Visible : ViewStates.Gone;

            RelativeLayout.LayoutParams buttonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentTop);
            buttonLayoutParams.BottomMargin = 10;
            iCancelButton.LayoutParameters = buttonLayoutParams;

            iMasterContainer.AddView(iCancelButton);
        }

        private void RefreshBackButton()
        {
            if (iBackButton != null)
            {
                iDetailContainer.RemoveView(iBackButton);
                iBackButton.Click -= BackButtonClickHandler;
                iBackButton.Dispose();
            }
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            iBackButton = BackButtonLayoutId == kNoButtonLayoutId ? new Button(Context) : inflater.Inflate(BackButtonLayoutId, null) as Button;
            iBackButton.Text = "Back";
            iBackButton.Id = kBackButtonId;
            iBackButton.Click += BackButtonClickHandler;

            RelativeLayout.LayoutParams buttonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentTop);
            buttonLayoutParams.BottomMargin = 10;
            iBackButton.LayoutParameters = buttonLayoutParams;

            iDetailContainer.AddView(iBackButton);
        }

        private void RefreshEditButton()
        {
            if (iEditButton != null)
            {
                iDetailContainer.RemoveView(iEditButton);
                iEditButton.Click -= EditButtonClickHandler;
                iEditButton.Dispose();
            }
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            iEditButton = BackButtonLayoutId == kNoButtonLayoutId ? new ToggleButton(Context) : inflater.Inflate(EditButtonLayoutId, null) as ToggleButton;
            iEditButton.TextOn = "Done";
            iEditButton.TextOff = "Edit";
            iEditButton.Id = kEditButtonId;
            iEditButton.Click += EditButtonClickHandler;
            iEditButton.Visibility = iEditMode ? ViewStates.Visible : ViewStates.Gone;

            RelativeLayout.LayoutParams buttonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentRight);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentTop);
            buttonLayoutParams.BottomMargin = 10;
            iEditButton.LayoutParameters = buttonLayoutParams;

            iDetailContainer.AddView(iEditButton);
        }

        private void RefreshSaveButton()
        {
            if (iSaveButton != null)
            {
                iMasterContainer.RemoveView(iSaveButton);
                iSaveButton.Click -= SaveButtonClickHandler;
                iSaveButton.Dispose();
            }
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            iSaveButton = SaveButtonLayoutId == kNoButtonLayoutId ? new Button(Context) : inflater.Inflate(SaveButtonLayoutId, null) as Button;
            iSaveButton.Text = "Save";
            iSaveButton.Id = kSaveButtonId;
            iSaveButton.Click += SaveButtonClickHandler;
            iSaveButton.Visibility = iShowSaveButton ? ViewStates.Visible : ViewStates.Gone;

            RelativeLayout.LayoutParams buttonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentRight);
            buttonLayoutParams.AddRule(LayoutRules.AlignParentTop);
            buttonLayoutParams.BottomMargin = 10;
            iSaveButton.LayoutParameters = buttonLayoutParams;

            iMasterContainer.AddView(iSaveButton);
        }

        public void ToggleView()
        {
            if (iAnimating)
            {
                return;
            }
            iAnimating = true;
            if (iIsShowingMasterView)
            {
                Assert.Check(iDetailView != null);                
                InAnimation = CreateTranslateAnimation(1, 0);
                OutAnimation = CreateTranslateAnimation(0, -1);
                OutAnimation.SetAnimationListener(this);
                iIsShowingMasterView = false;
            }
            else
            {
                Assert.Check(iMasterView != null);
                iIsShowingMasterView = true;
                InAnimation = CreateTranslateAnimation(-1, 0);
                OutAnimation = CreateTranslateAnimation(0, 1);
                OutAnimation.SetAnimationListener(this);
            }
            ShowNext();
        }

        public int BackButtonLayoutId
        {
            get
            {
                return iBackButtonLayoutId;
            }

            set
            {
                iBackButtonLayoutId = value;
                RefreshBackButton();
            }
        }

        public int EditButtonLayoutId
        {
            get
            {
                return iEditButtonLayoutId;
            }

            set
            {
                iEditButtonLayoutId = value;
                RefreshEditButton();
            }
        }

        public int CancelButtonLayoutId
        {
            get
            {
                return iCancelButtonLayoutId;
            }

            set
            {
                iCancelButtonLayoutId = value;
                RefreshCancelButton();
            }
        }

        public int SaveButtonLayoutId
        {
            get
            {
                return iSaveButtonLayoutId;
            }

            set
            {
                iSaveButtonLayoutId = value;
                RefreshSaveButton();
            }
        }

        private void BackButtonClickHandler(object sender, EventArgs e)
        {
            ToggleView();
        }

        private void EditButtonClickHandler(object sender, EventArgs e)
        {
            OnEventEditButtonClicked();
        }

        private void SaveButtonClickHandler(object sender, EventArgs e)
        {
            OnEventSaveButtonClicked();
        }

        private void CancelButtonClickHandler(object sender, EventArgs e)
        {
            OnEventCancelButtonClicked();
        }

        private Animation CreateTranslateAnimation(float fromX, float toX)
        {
            TranslateAnimation anim = new TranslateAnimation(Dimension.RelativeToParent, fromX, Dimension.RelativeToParent, toX, Dimension.RelativeToParent, 0f, Dimension.RelativeToParent, 0f);
            anim.Duration = 500;
            anim.Interpolator = new Android.Views.Animations.DecelerateInterpolator();
            return anim;
        }

        #region IAnimationListener Members

        public virtual void OnAnimationEnd(Animation animation)
        {
            iAnimating = false;
            animation.SetAnimationListener(null);
            Animation inAnim = InAnimation;
            InAnimation = null;
            inAnim.Dispose();
            inAnim = null;
            Animation outAnim = OutAnimation;
            OutAnimation = null;
            outAnim.Dispose();
            outAnim = null;
            if (iIsShowingMasterView)
            {
                OnEventDetailClosed();
            }
        }

        public virtual void OnAnimationRepeat(Animation animation)
        {
        }

        public virtual void OnAnimationStart(Animation animation)
        {
        }

        #endregion

        protected void OnEventDetailClosed()
        {
            EventHandler<EventArgs> del = EventDetailClosed;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected void OnEventEditButtonClicked()
        {
            EventHandler<EventArgs> del = EventEditButtonClicked;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected void OnEventSaveButtonClicked()
        {
            EventHandler<EventArgs> del = EventSaveButtonClicked;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected void OnEventCancelButtonClicked()
        {
            EventHandler<EventArgs> del = EventCancelButtonClicked;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        public override void SetBackgroundColor(Android.Graphics.Color color)
        {
            iBackgroundColor = color;
            FindViewById(kMasterTitleId).SetBackgroundColor(iBackgroundColor);
            FindViewById(kDetailTitleId).SetBackgroundColor(iBackgroundColor);
            iDetailContainer.SetBackgroundColor(iBackgroundColor);
            iMasterContainer.SetBackgroundColor(iBackgroundColor);
            base.SetBackgroundColor(color);
        }

        private int iBackButtonLayoutId;
        private int iEditButtonLayoutId;
        private int iSaveButtonLayoutId;
        private int iCancelButtonLayoutId;
        private const int kBackButtonId = 1001;
        private const int kMasterTitleId = 1002;
        private const int kDetailTitleId = 1003;
        private const int kEditButtonId = 1004;
        private const int kSaveButtonId = 1005;
        private const int kCancelButtonId = 1006;
        private View iMasterView;
        private View iDetailView;
        private bool iIsShowingMasterView;
        private ViewGroup iMasterContainer;
        private ViewGroup iDetailContainer;
        private const int kNoButtonLayoutId = ~0;
        private bool iAnimating;
        private Button iBackButton;
        private ToggleButton iEditButton;
        private Button iSaveButton;
        private Button iCancelButton;
        private bool iEditMode;
        private bool iShowSaveButton;
        private bool iShowCancelButton;
        private Android.Graphics.Color iBackgroundColor;
        private string iCancelButtonText = "Cancel";
    }

}