
using System;
using System.Threading;


namespace Linn.Toolkit
{
    // Interface defining the view for the updating process
    public interface IViewAutoUpdate
    {
        void StateChanged(AutoUpdateModel aModel);
        bool ShowCompatibilityBreak(AutoUpdateModel aModel);
        void ProgressChanged(AutoUpdateModel aModel);

        bool ButtonAutoCheckOn { get; set; }

        event EventHandler EventClosed;
        event EventHandler EventButtonUpdateClicked;
        event EventHandler EventButtonAutoCheckClicked;
    }


    // Model class defining the data used in the update process
    public class AutoUpdateModel
    {
        public enum EState
        {
            eChecking,
            eAvailable,
            eUnavailable,
            eDownloading,
            ePreparingToUpdate,
            eUpdateStarted,
            eFailed
        }

        public AutoUpdateModel(string aProductName)
        {
            iProductName = aProductName;
            State = EState.eChecking;
            UpdateInfo = null;
            DownloadProgress = 0;
        }

        public string ProductName { get { return iProductName; } }
        public EState State { get; set; }        
        public AutoUpdate.AutoUpdateInfo UpdateInfo { get; set; }
        public int DownloadProgress { get; set; }
        
        private string iProductName;
    }


    // Controller defining the behaviour of the update process
    public class AutoUpdateController
    {
        public AutoUpdateController(IHelper aHelper, AutoUpdate aAutoUpdate, OptionPageUpdates aOptionPageUpdates, IViewAutoUpdate aView, IInvoker aInvoker)
        {
            iAutoUpdate = aAutoUpdate;
            iOptionPageUpdates = aOptionPageUpdates;
            iView = aView;
            iInvoker = aInvoker;

            iModel = new AutoUpdateModel(aHelper.Title);
            
            iAutoUpdate.EventUpdateProgress += UpdateProgress;
            iAutoUpdate.EventUpdateFailed += UpdateFailed;
            iAutoUpdate.EventUpdateFound += UpdateFound;

            iOptionPageUpdates.EventAutoUpdateChanged += AutoUpdateChanged;
            
            iView.EventClosed += ViewClosed;
            iView.EventButtonUpdateClicked += ButtonUpdateClicked;
            iView.EventButtonAutoCheckClicked += ButtonAutoCheckClicked;
            
            AutoUpdateChanged(this, EventArgs.Empty);
        }

        public void ManualCheck()
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                // initialise the check
                iModel.State = AutoUpdateModel.EState.eChecking;
                iModel.UpdateInfo = null;

                // show the view
                iView.StateChanged(iModel);

                // start a thread to do the update checking
                iThread = new Thread(ThreadFuncCheck);
                iThread.IsBackground = true;
                iThread.Name = "UpdateCheck";
                iThread.Start();
            }));
        }
        
        public event EventHandler<EventArgs> EventUpdateStarted;

        private void ViewClosed(object sender, EventArgs e)
        {
            if (iThread != null)
            {
                iThread.Abort();
                iThread.Join();
                iThread = null;
            }
        }

        private void ThreadFuncCheck()
        {
            // performs the check for updates in a separate thread
            iModel.UpdateInfo = iAutoUpdate.CheckForUpdate();
            
            // pass over to main thread to update the view
            iInvoker.BeginInvoke(new System.Action(CheckFinished));
        }
        
        private void CheckFinished()
        {
            iModel.State = (iModel.UpdateInfo != null) ? AutoUpdateModel.EState.eAvailable : AutoUpdateModel.EState.eUnavailable;
            iThread = null;
            iView.StateChanged(iModel);
        }

        private void ButtonUpdateClicked(object sender, EventArgs e)
        {
            // When an update is available, the "Update" button has been clicked
            Assert.Check(iModel.UpdateInfo != null);

            if (iModel.UpdateInfo.IsCompatibilityFamilyUpgrade)
            {
                if (!iView.ShowCompatibilityBreak(iModel))
                {
                    return;
                }
            }

            // start to download the update
            iModel.State = AutoUpdateModel.EState.eDownloading;
            iView.StateChanged(iModel);
            
            iThread = new Thread(ThreadFuncUpdate);
            iThread.IsBackground = true;
            iThread.Name = "Update";
            iThread.Start();
        }

        private void ThreadFuncUpdate()
        {
            // performs the download and update in a separate thread
            iAutoUpdate.DownloadUpdate(iModel.UpdateInfo);

            // update UI
            iInvoker.BeginInvoke(new System.Action(DownloadFinished));

            // start the update
            if (iAutoUpdate.ApplyUpdate(iModel.UpdateInfo))
            {
                // update started successfully - close the dialog
                iInvoker.BeginInvoke(new System.Action(UpdateStarted));
            }
        }
        
        private void DownloadFinished()
        {
            iModel.State = AutoUpdateModel.EState.ePreparingToUpdate;
            iView.StateChanged(iModel);
        }
        
        private void UpdateStarted()
        {
            iThread = null;
            iModel.State = AutoUpdateModel.EState.eUpdateStarted;
            iView.StateChanged(iModel);
            
            if (EventUpdateStarted != null) {
                EventUpdateStarted(this, EventArgs.Empty);
            }
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            if (iInvoker.TryBeginInvoke(new EventHandler<EventArgs>(UpdateProgress), sender, e))
                return;

            iModel.DownloadProgress = iAutoUpdate.UpdateProgress;
            iView.ProgressChanged(iModel);
        }

        private void UpdateFailed(object sender, EventArgs e)
        {
            if (iInvoker.TryBeginInvoke(new EventHandler<EventArgs>(UpdateFailed), sender, e))
                return;

            iModel.State = AutoUpdateModel.EState.eFailed;
            iView.StateChanged(iModel);
            iThread = null;
        }

        private void UpdateFound(object sender, AutoUpdate.EventArgsUpdateFound e)
        {
            if (iInvoker.TryBeginInvoke(new EventHandler<AutoUpdate.EventArgsUpdateFound>(UpdateFound), sender, e))
                return;

            if (iOptionPageUpdates.AutoUpdate)
            {
                iModel.State = AutoUpdateModel.EState.eAvailable;
                iModel.UpdateInfo = e.Info;
                iView.StateChanged(iModel);
            }
        }
        
        private void AutoUpdateChanged(object sender, EventArgs e)
        {
            if (iInvoker.TryBeginInvoke(new EventHandler<EventArgs>(AutoUpdateChanged), sender, e))
                return;

            iView.ButtonAutoCheckOn = iOptionPageUpdates.AutoUpdate;
        }
        
        private void ButtonAutoCheckClicked(object sender, EventArgs e)
        {
            iOptionPageUpdates.AutoUpdate = iView.ButtonAutoCheckOn;
        }

        private AutoUpdate iAutoUpdate;
        private OptionPageUpdates iOptionPageUpdates;
        private IViewAutoUpdate iView;
        private IInvoker iInvoker;
        private AutoUpdateModel iModel;
        private Thread iThread;
    }


    // Abstract class implementation of an IViewAutoUpdate that defines that the view must have specific
    // elements in its UI
    public abstract class ViewAutoUpdateStandard : IViewAutoUpdate
    {
        #region Abstract methods for the derived class to implement
        
        protected abstract string Text1 { set; }
        protected abstract string Text2 { set; }
        protected abstract string ButtonCloseText { set; }

        protected abstract void StartProgress(bool aIsIndeterminate);
        protected abstract void StopProgress();
        protected abstract bool ProgressHidden { set; }
        protected abstract int ProgressValue { set; }

        protected abstract bool ButtonDetailsEnabled { set; }
        protected abstract bool ButtonUpdateEnabled { set; }
        protected abstract bool ButtonCloseEnabled { set; }

        protected abstract void SetButtonUpdateAsDefault();
        protected abstract void SetButtonCloseAsDefault();

        protected abstract bool ButtonAutoCheckHidden { set; }
        protected abstract bool WindowHidden { set; }
        
        protected abstract string WebViewUri { set; }

        protected abstract bool ShowCompatibilityBreak(string aButtonUpdate, string aButtonCancel, string aMessage, string aInformation);

        public abstract bool ButtonAutoCheckOn { get; set; }
        public abstract event EventHandler EventClosed;
        public abstract event EventHandler EventButtonUpdateClicked;
        public abstract event EventHandler EventButtonAutoCheckClicked;
        
        #endregion

        #region IViewAutoUpdate implementation

        public void StateChanged(AutoUpdateModel aModel)
        {
            switch (aModel.State)
            {
            case AutoUpdateModel.EState.eChecking:
                Text1 = "Checking for updates...";
                Text2 = "";
                ButtonCloseText = "Cancel";

                StartProgress(true);
                ProgressHidden = false;

                ButtonDetailsEnabled = false;
                ButtonUpdateEnabled = false;
                ButtonCloseEnabled = true;
                SetButtonCloseAsDefault();
                ButtonAutoCheckHidden = true;
                WindowHidden = false;
                break;

            case AutoUpdateModel.EState.eUnavailable:
                StopProgress();
                ProgressHidden = true;

                Text1 = string.Format("{0} is up to date.", aModel.ProductName);
                Text2 = "";
                ButtonCloseText = "Close";

                ButtonDetailsEnabled = false;
                ButtonUpdateEnabled = false;
                ButtonCloseEnabled = true;
                SetButtonCloseAsDefault();
                ButtonAutoCheckHidden = false;
                break;

            case AutoUpdateModel.EState.eAvailable:
                StopProgress();
                ProgressHidden = true;

                if (aModel.UpdateInfo.Quality != EReleaseQuality.Stable)
                {
                    Text1 = string.Format("There is a new version of {0} ({1} {2}) available.", aModel.UpdateInfo.Name, aModel.UpdateInfo.Quality, aModel.UpdateInfo.Version);
                }
                else
                {
                    Text1 = string.Format("There is a new version of {0} ({1}) available.", aModel.UpdateInfo.Name, aModel.UpdateInfo.Version);
                }
                Text2 = "Do you wish to update?";
                ButtonCloseText = "Not Now";
                
                ButtonDetailsEnabled = true;
                ButtonUpdateEnabled = true;
                ButtonCloseEnabled = true;
                SetButtonUpdateAsDefault();
                ButtonAutoCheckHidden = false;
                WindowHidden = false;                

                WebViewUri = aModel.UpdateInfo.History.AbsoluteUri;
                break;

            case AutoUpdateModel.EState.eDownloading:
                StartProgress(false);
                ProgressHidden = false;

                Text1 = string.Format("Downloading {0} ({1}).", aModel.UpdateInfo.Name, aModel.UpdateInfo.Version);
                Text2 = "";
                ButtonCloseText = "Cancel";

                ButtonDetailsEnabled = true;
                ButtonCloseEnabled = true;
                ButtonUpdateEnabled = false;
                ButtonAutoCheckHidden = true;
                break;

            case AutoUpdateModel.EState.ePreparingToUpdate:
                StartProgress(true);
                ProgressHidden = false;

                Text1 = string.Format("Updating {0} to {1}.", aModel.UpdateInfo.Name, aModel.UpdateInfo.Version);
                Text2 = "";

                ButtonDetailsEnabled = false;
                ButtonUpdateEnabled = false;
                ButtonCloseEnabled = false;
                ButtonAutoCheckHidden = true;
                break;

            case AutoUpdateModel.EState.eFailed:
                StopProgress();
                ProgressHidden = false;
                ProgressValue = 0;
                
                Text1 = string.Format("The update failed.");
                Text2 = "";
                ButtonCloseText = "Close";

                ButtonDetailsEnabled = false;
                ButtonUpdateEnabled = false;
                ButtonCloseEnabled = true;
                SetButtonCloseAsDefault();
                ButtonAutoCheckHidden = true;
                break;

            case AutoUpdateModel.EState.eUpdateStarted:
                WindowHidden = true;
                break;
            }
        }

        public bool ShowCompatibilityBreak(AutoUpdateModel aModel)
        {
            return ShowCompatibilityBreak("Update", "Cancel",
                                          "This is a compatibility family upgrade. Do you wish to continue with the upgrade?",
                                          "Updating " + aModel.UpdateInfo.Name + " to a new compatibility family will also require updating Linn DS firmware.");
        }
                
        public void ProgressChanged(AutoUpdateModel aModel)
        {
            ProgressValue = aModel.DownloadProgress;
        }

        #endregion
    }
}



