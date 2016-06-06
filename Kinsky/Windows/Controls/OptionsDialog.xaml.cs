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
using System.Windows.Threading;
using System.Collections;

namespace KinskyDesktopWpf
{

    public partial class OptionsDialog : Window
    {
        IList<OptionPageViewModel> iOptionPages;
        private HelperKinsky iHelperKinsky;
        private UiOptions iUiOptions;
        public OptionsDialog(HelperKinsky aHelperKinsky, UiOptions aUiOptions)
        {
            InitializeComponent();
            iHelperKinsky = aHelperKinsky;
            iHelperKinsky.EventOptionPagesChanged += new EventHandler<EventArgs>(aHelperKinsky_EventOptionPagesChanged);
            LoadOptions();
            if (iOptionPages.Count > 0)
            {
                iOptionPages[0].IsSelected = true;
            }
            iUiOptions = aUiOptions;
            this.Loaded += LoadedHandler;
        }

        void LoadedHandler(object sender, RoutedEventArgs e)
        {
            iUiOptions.DialogSettings.Register(this, "Options");
            this.Loaded -= LoadedHandler;
        }

        void aHelperKinsky_EventOptionPagesChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                LoadOptions();
            }));
        }

        private void LoadOptions()
        {

            iOptionPages = (from p in iHelperKinsky.OptionPages
                            select new OptionPageViewModel(null, p, Dispatcher.CurrentDispatcher)).ToList();

            treeViewOptions.ItemsSource = iOptionPages;
        }

        public void SetPageByName(string aName)
        {
            foreach (OptionPageViewModel page in iOptionPages)
            {
                if (page.Name == aName)
                {
                    page.IsSelected = true;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (OptionPageViewModel model in iOptionPages)
            {
                foreach (OptionViewModel option in model.Options)
                {
                    option.Dispose();
                }
            }
            base.OnClosed(e);
        }

        private void ItemsControl_MouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            // take the focus off text fields allowing them to update their bound properties
            lstOptions.Focus();
        }

        private void ListOption_AddButtonClick(object sender, RoutedEventArgs args)
        {
            IListOptionViewModel model = GetEventSourceItem(args) as IListOptionViewModel;
            try
            {
                model.AddItem();
            }
            catch (ItemAlreadyExistsException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ListOption_RemoveButtonClick(object sender, RoutedEventArgs args)
        {
            ListBoxItem container = lstOptions.GetEventSourceElement<ListBoxItem>(args);


            IListOptionViewModel model = GetEventSourceItem(args) as IListOptionViewModel;

            DataTemplateKey key = new DataTemplateKey(model.GetType());
            DataTemplate template = FindResource(key) as DataTemplate;

            ContentPresenter presenter = container.FindVisualChild<ContentPresenter>();
            ListBox list = template.FindName("lstItems", presenter) as ListBox;

            IList foo = list.SelectedItems;
            object[] removeItems = (from object f in foo
                                    select f).ToArray();
            model.RemoveItems(removeItems);

        }

        private void FileOption_ChooseButtonClick(object sender, RoutedEventArgs args)
        {
            //need to use winforms dialogs here as unfortunately wpf does not ship with file browser dialog
            FileOptionViewModel model = GetEventSourceItem(args) as FileOptionViewModel;
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.FileName = model.Value;
            dlg.InitialDirectory = model.Value;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                model.Value = dlg.FileName;
            }
        }
        private void FolderOption_ChooseButtonClick(object sender, RoutedEventArgs args)
        {
            //need to use winforms dialogs here as unfortunately wpf does not ship with folder browser dialog
            FolderOptionViewModel model = GetEventSourceItem(args) as FolderOptionViewModel;
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = model.WrappedOption.Description;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                model.Value = dlg.SelectedPath;
            }
        }

        private void ColourOption_ChooseButtonClick(object sender, RoutedEventArgs args)
        {
            //need to use winforms dialogs here as unfortunately wpf does not ship with colour picker dialog
            ColourOptionViewModel model = GetEventSourceItem(args) as ColourOptionViewModel;
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(Int32.Parse(model.Value));
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                model.Value = dlg.Color.ToArgb().ToString();
            }
        }

        private OptionViewModel GetEventSourceItem(RoutedEventArgs args)
        {
            return lstOptions.GetEventSourceItem<OptionViewModel, ListBoxItem>(args);
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
        private void ResetCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = treeViewOptions.SelectedItem != null;
            e.Handled = true;
        }

        private void ResetExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (OptionViewModel opt in (treeViewOptions.SelectedItem as OptionPageViewModel).Options)
            {
                opt.ResetToDefault();
            }
        }
        #endregion
    }
}
