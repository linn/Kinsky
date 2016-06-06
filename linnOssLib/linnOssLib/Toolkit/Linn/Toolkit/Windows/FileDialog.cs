
using System;


namespace Linn.Toolkit
{
    public partial class OpenFileDialog
    {
        partial void ShowDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = iTitle;
            dlg.Filter = iFilterLabel + "|*" + iFilterExtension;

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                iReturnValue = true;
                iFilename = dlg.FileName;
            }
        }
    }

    public partial class SaveFileDialog
    {
        partial void ShowDialog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = iTitle;
            dlg.FileName = iDefaultFilename;
            dlg.DefaultExt = iExtension;
            dlg.Filter = iFilterLabel + "|*" + iFilterExtension;

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                iReturnValue = true;
                iFilename = dlg.FileName;
            }
        }
    }
}

