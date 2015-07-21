using System;

namespace Linn
{
    public class OptionPagePrivacy : OptionPage
    {
        public OptionPagePrivacy(IHelper aHelper)
            : base("Privacy") {

            iOptionUsageData = new OptionBool("usagedata", "Send anonymous usage data", "Whether to send anonymous usage data for product improvement", true);
            Add(iOptionUsageData);
        }

        public bool UsageData
        {
            get { return iOptionUsageData.Native; }
            set { iOptionUsageData.Native = value; }
        }

        public event EventHandler<EventArgs> EventUsageDataChanged {
            add { iOptionUsageData.EventValueChanged += value; }
            remove { iOptionUsageData.EventValueChanged -= value; }
        }

        private OptionBool iOptionUsageData;
    }
}