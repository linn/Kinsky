using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using System.Net;
using Linn;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace KinskyDesktopWpf
{
    /// <summary>
    /// Interaction logic for KinskyDesktop.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {

        private AutoUpdate iAutoUpdate;
        private AutoUpdate.AutoUpdateInfo iInfo;
        private Thread iUpdateThread;
        private Thread iUpdateCheckThread;
        private bool iClosed;

        public UpdateDialog(AutoUpdate aAutoUpdate)
        {
            iAutoUpdate = aAutoUpdate;

            InitializeComponent();

            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;
            iUpdateCheckThread = new Thread(new ThreadStart(() =>
            {
                iInfo = iAutoUpdate.CheckForUpdate();
                PromptForInstall();
            }));
            iUpdateCheckThread.Name = "UpdateCheck";
            iUpdateCheckThread.IsBackground = true;
            iUpdateCheckThread.Start();
        }

        private void PromptForInstall()
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                if (!iClosed)
                {
                    progressBar.IsIndeterminate = false;
                    progressBar.Visibility = Visibility.Collapsed;
                    if (iInfo != null)
                    {
                        txtStatus.Text = string.Format("There is a new version of {0} ({1}) available. Click here for details.", iInfo.Name, iInfo.Version);
                        btnInstall.Visibility = Visibility.Visible;
                        btnStatus.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        txtStatus.Text = string.Format("There are no updates available.");
                    }
                    iUpdateCheckThread = null;
                }
            });
        }

        private void InstallUpdates()
        {
            iUpdateThread = new Thread(new ThreadStart(() =>
            {
                iAutoUpdate.DownloadUpdate(iInfo);
                Dispatcher.Invoke((Action)delegate()
                {
                    if (!iClosed)
                    {
                        progressBar.Visibility = Visibility.Visible;
                        btnClose.IsEnabled = false;
                    }
                });

                if (iAutoUpdate.ApplyUpdate(iInfo))
                {
                    Dispatcher.Invoke((Action)delegate()
                    {
                        if (!iClosed)
                        {
                            DialogResult = true;
                            Close();
                        }
                    });
                }
                else
                {
                    Dispatcher.Invoke((Action)delegate()
                    {
                        if (!iClosed)
                        {
                            txtStatus.Text = "Failed to apply update.  Check user log for further info.";
                            btnClose.IsEnabled = true;
                        }
                    });
                }
                iUpdateThread = null;
            }));
            iUpdateThread.Name = "Update";
            iUpdateThread.IsBackground = true;

            iAutoUpdate.EventUpdateProgress += UpdateProgress;
            iAutoUpdate.EventUpdateFailed += UpdateFailed;
            iUpdateThread.Start();
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                if (!iClosed)
                {
                    progressBar.Value = iAutoUpdate.UpdateProgress;
                }
            });
        }

        private void UpdateFailed(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                if (!iClosed)
                {
                    txtStatus.Text = "Update failed.";
                }
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (iUpdateCheckThread != null)
            {
                iUpdateCheckThread.Abort();
            }
            if (iUpdateThread != null)
            {
                iUpdateThread.Abort();
            }
            Close();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            bool update = true;
            if (iInfo.IsCompatibilityFamilyUpgrade)
            {
                MessageBoxResult result = MessageBox.Show(this, 
                                            "This update is a compatibility family upgrade and therefore requires an update to devices as well. \nDo you still wish to update?", 
                                            "Confirm update", 
                                            MessageBoxButton.YesNo, 
                                            MessageBoxImage.Question, 
                                            MessageBoxResult.Yes,
                                            MessageBoxOptions.None);
                if (result == MessageBoxResult.No)
                {
                    update = false;
                }
            }
            if (update)
            {
                btnInstall.IsEnabled = false;
                InstallUpdates();
            }
            else
            {
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            iClosed = true;
            if (iUpdateThread != null)
            {
                iAutoUpdate.EventUpdateProgress -= UpdateProgress;
                iAutoUpdate.EventUpdateFailed -= UpdateFailed;
            }
        }

        #region Command Bindings
        private void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion

        private void txtStatus_Click(object sender, RoutedEventArgs e)
        {
            if (iInfo != null)
            {
                try
                {
                    System.Diagnostics.Process.Start(iInfo.History.ToString());
                }
                catch (Exception ex)
                {
                    UserLog.WriteLine("Could not open status link: " + iInfo.History.ToString() + ", " + ex);
                }
            }
        }

    }
}
