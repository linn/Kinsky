using System;

namespace Linn
{
    public class OptionPageUpdates : OptionPage
    {
        public OptionPageUpdates(IHelper aHelper)
            : base("Updates") {

            iAutoUpdate = new OptionBool("autoupdate", "Automatic update checks", "Perform automatic checks for updates", true);
            Add(iAutoUpdate);

            iBetaVersions = new OptionBool("beta", "Participate in beta program", "Include beta versions when checking for available updates", false);
            Add(iBetaVersions);

            iDevelopmentVersions = new OptionParser.OptionBool("-d", "--development", "Include development versions when checking for available updates");
            aHelper.OptionParser.AddOption(iDevelopmentVersions);

            iNightlyBuilds = new OptionParser.OptionBool("-n", "--nightly", "Include nightly builds when checking for available updates");
            aHelper.OptionParser.AddOption(iNightlyBuilds);
        }

        public bool BetaVersions {
            get { return iBetaVersions.Native; }
            set { iBetaVersions.Native = value; }
        }

        public bool DevelopmentVersions {
            get { return iDevelopmentVersions.Value; }
        }

        public bool NightlyBuilds {
            get { return iNightlyBuilds.Value; }
        }

        public bool AutoUpdate {
            get { return iAutoUpdate.Native; }
            set { iAutoUpdate.Native = value; }
        }

        public event EventHandler<EventArgs> EventBetaVersionsChanged {
            add { iBetaVersions.EventValueChanged += value; }
            remove { iBetaVersions.EventValueChanged -= value; }
        }

        public event EventHandler<EventArgs> EventAutoUpdateChanged {
            add { iAutoUpdate.EventValueChanged += value; }
            remove { iAutoUpdate.EventValueChanged -= value; }
        }

        private OptionBool iBetaVersions;
        private OptionParser.OptionBool iDevelopmentVersions;
        private OptionParser.OptionBool iNightlyBuilds;
        private OptionBool iAutoUpdate;
    }
}