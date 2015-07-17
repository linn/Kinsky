
using System;

using Linn;


namespace Linn.Toolkit
{
    public partial class HelperAutoUpdate
    {
        public HelperAutoUpdate(IHelper aHelper, IViewAutoUpdate aView, IInvoker aInvoker)
        {
            Initialise(aHelper, aView, aInvoker, UpdateStarted, AutoUpdate.DefaultFeedLocation(aHelper.Title));
        }

        public HelperAutoUpdate(IHelper aHelper, IViewAutoUpdate aView, IInvoker aInvoker, string aUpdateFeedUri)
		{
			Initialise(aHelper, aView, aInvoker, UpdateStarted, aUpdateFeedUri);
        }

        public HelperAutoUpdate(IHelper aHelper, IViewAutoUpdate aView, IInvoker aInvoker, EventHandler<EventArgs> aUpdateStarted)
        {
            Initialise(aHelper, aView, aInvoker, aUpdateStarted, AutoUpdate.DefaultFeedLocation(aHelper.Title));
        }

        public HelperAutoUpdate(IHelper aHelper, IViewAutoUpdate aView, IInvoker aInvoker, EventHandler<EventArgs> aUpdateStarted, string aUpdateFeedUri)
        {
            Initialise(aHelper, aView, aInvoker, aUpdateStarted, aUpdateFeedUri);
        }

        private void Initialise(IHelper aHelper, IViewAutoUpdate aView, IInvoker aInvoker, EventHandler<EventArgs> aUpdateStarted, string aUpdateFeedUri)
        {
            // create the auto updater
            // set it to look for stable releases to start with
            // option page will event what releases to look for once saved option has been parsed
            iAutoUpdate = new AutoUpdate(aHelper, aUpdateFeedUri, 1000 * 60 * 60, EReleaseQuality.Stable, 1);

            // create the option page
            iOptionPageUpdates = new OptionPageUpdates(aHelper);
            iOptionPageUpdates.EventChanged += OptionPageUpdatesChanged;
            aHelper.AddOptionPage(iOptionPageUpdates);
            
            // create the controller
            iAutoUpdateController = new AutoUpdateController(aHelper, iAutoUpdate, iOptionPageUpdates, aView, aInvoker);
            iAutoUpdateController.EventUpdateStarted += aUpdateStarted;
        }

        public void Start()
        {
            iAutoUpdate.Start();
        }

        public void SetUri(string aUpdateFeedUri)
        {
            iAutoUpdate.SetUri(aUpdateFeedUri);
        }
        
        public void Dispose()
        {
            iAutoUpdate.Stop();
            iAutoUpdate.Dispose();
        }

        public void CheckForUpdates()
        {
            iAutoUpdateController.ManualCheck();
        }
        
        public OptionPageUpdates OptionPageUpdates
        {
            get { return iOptionPageUpdates; }
        }
        
        private EReleaseQuality OptionUpdateQuality
        {
            get
            {
                EReleaseQuality updateQuality = EReleaseQuality.Stable;

                if (iOptionPageUpdates.BetaVersions)
                {
                    updateQuality |= EReleaseQuality.Beta;
                }
                
                if (iOptionPageUpdates.DevelopmentVersions)
                {
                    updateQuality |= EReleaseQuality.Development;
                }

                if (iOptionPageUpdates.NightlyBuilds)
                {
                    updateQuality |= EReleaseQuality.Nightly;
                }

                return updateQuality;
            }
        }

        private void OptionPageUpdatesChanged(object sender, EventArgs e)
        {
            iAutoUpdate.DesiredQuality = OptionUpdateQuality;
        }

        // to be implemented in toolkit specific areas of code
		public virtual void UpdateStarted(object sender, EventArgs e) { }
        
        private OptionPageUpdates iOptionPageUpdates;
        private AutoUpdate iAutoUpdate;
        private AutoUpdateController iAutoUpdateController;
    }
}

