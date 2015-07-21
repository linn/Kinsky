using System;

namespace Linn
{
    public class OptionPageCrashDumper : OptionPage
    {
        public OptionPageCrashDumper(string aTitle)
            : base(aTitle) {

                iAutoSend = new OptionBool("autosendcrashlog", "Auto send crash log", "Send crash reports automatically without prompting", false);
            Add(iAutoSend);
        }

        public bool AutoSend {
            get { return iAutoSend.Native; }
            set { iAutoSend.Native = value; }
        }

        public event EventHandler<EventArgs> EventAutoSendChanged
        {
            add { iAutoSend.EventValueChanged += value; }
            remove { iAutoSend.EventValueChanged -= value; }
        }

        private OptionBool iAutoSend;
    }
}