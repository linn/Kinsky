
using System;
using Linn;

namespace Linn.Toolkit
{
    public partial class OpenFileDialog
    {
        public OpenFileDialog(string aTitle, string aFilterLabel, string aFilterExtension)
        {
            iTitle = aTitle;
            iFilterLabel = aFilterLabel;
            iFilterExtension = aFilterExtension;
            iReturnValue = false;
            iFilename = string.Empty;
        }

        public bool Show()
        {
            iReturnValue = false;
            iFilename = string.Empty;

            ShowDialog();

            return iReturnValue;
        }

        public string Filename
        {
            get { return iFilename; }
        }

        partial void ShowDialog();

        private string iTitle;
        private string iFilterLabel;
        private string iFilterExtension;
        private bool iReturnValue;
        private string iFilename;
    }

    public partial class SaveFileDialog
    {
        public SaveFileDialog(string aTitle, string aDefaultFilename, string aExtension, string aFilterLabel, string aFilterExtension)
        {
            iTitle = aTitle;
            iDefaultFilename = aDefaultFilename;
            iExtension = aExtension;
            iFilterLabel = aFilterLabel;
            iFilterExtension = aFilterExtension;
            iReturnValue = false;
            iFilename = string.Empty;
        }

        public bool Show()
        {
            iReturnValue = false;
            iFilename = string.Empty;

            ShowDialog();

            return iReturnValue;
        }

        public string Filename
        {
            get { return iFilename; }
        }

        partial void ShowDialog();

        private string iTitle;
        private string iDefaultFilename;
        private string iExtension;
        private string iFilterLabel;
        private string iFilterExtension;
        private bool iReturnValue;
        private string iFilename;
    }
}

