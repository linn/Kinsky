using Linn;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
namespace KinskyDesktopWpf
{

    public class CrashLogDumperForm : ICrashLogDumper
    {
        private Thread iShowCrashLogDialog;
        private ManualResetEvent iWaitHandle;
        private string iReportText;
        public CrashLogDumperForm(Window aParent, string aTitle, string aProduct, string aVersion)
        {
            iParent = aParent;
            iTitle = aTitle;
            iProduct = aProduct;
            iVersion = aVersion;
            iShowCrashLogDialog = new Thread(new ThreadStart(ShowCrashLogDialog));
            iShowCrashLogDialog.SetApartmentState(ApartmentState.STA);
            iShowCrashLogDialog.IsBackground = true;
        }

        private void ShowCrashLogDialog()
        {
            // show crash form
            CrashLogDialog form = new CrashLogDialog(iTitle, iReportText, iProduct, iVersion);
            form.EventDialogClosed += new EventHandler<EventArgs>(form_EventDialogClosed);
            form.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        void form_EventDialogClosed(object sender, EventArgs e)
        {
            iWaitHandle.Set();
        }

        public void Dump(CrashLog aCrashLog)
        {
            try
            {
                iParent.Visibility = Visibility.Hidden;
                // include some system details in report
                iReportText = aCrashLog.ToString();
                iWaitHandle = new ManualResetEvent(false);
                iShowCrashLogDialog.Start();
                iWaitHandle.WaitOne();
            }catch(Exception ex) {
                UserLog.WriteLine("Unhandled exception in CrashLogDumper.Dump() " +  ex);
            }
        }

        private string iTitle;
        private string iProduct;
        private string iVersion;
        private Window iParent;
        //private Icon iIcon;
    }
}