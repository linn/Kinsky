using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Linn.Topology;
using Linn.Kinsky;

namespace KinskyTouch
{
    internal interface IControllerSenders
    {
        void Select(ModelSender aSender);
    }

    internal class ViewWidgetSenderReceiver : UITableViewController, IControllerSenders
    {
        private class DataSourceReceivers : UITableViewDataSource
        {
            public DataSourceReceivers(UITableView aTableView, ReceiverSourceList aReceiversList, ModelSender aSender)
            {
                iTableView = aTableView;
                iReceiversList = aReceiversList;
                iSender = aSender;

                iList = new List<ModelSourceReceiver>();
                
                CreateList();

                iReceiversList.EventReceiverSourceAdded += SourceAdded;
                iReceiversList.EventReceiverSourceRemoved += SourceRemoved;
                iReceiversList.EventReceiverSourceChanged += SourceChanged;
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iList.Count;
            }

            public ModelSourceReceiver ReceiverAt(NSIndexPath aIndexPath)
            {
                return iList[aIndexPath.Row];
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                UITableViewCell cell = aTableView.DequeueReusableCell(kCellIdentifierReceivers);
                if(cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Subtitle, kCellIdentifierReceivers);
                }

                cell.BackgroundColor = UIColor.Black;
                cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
                cell.TextLabel.TextColor = UIColor.White;
                cell.TextLabel.Text = string.Format("{0} ({1})", iList[aIndexPath.Row].Source.Room.Name, iList[aIndexPath.Row].Source.Group.Name);
                cell.ImageView.Image = iList[aIndexPath.Row].IsPlayingSender(iSender) ? KinskyTouch.Properties.ResourceManager.SourceSongcastNotSending : KinskyTouch.Properties.ResourceManager.SourceSongcast;

                return cell;
            }

            private void CreateList()
            {
                iList.Clear();

                IList<ModelSourceReceiver> list = new List<ModelSourceReceiver>(iReceiversList.Sources);
                foreach(ModelSourceReceiver s in list)
                {
                    if(iSender != null && s.Source.Device.Udn != iSender.Udn)
                    {
                        string name = string.Format("{0} ({1})", s.Source.Room.Name, s.Source.Group.Name);

                        int index = 0;
                        for(int i = 0; i < iList.Count; ++i, ++index)
                        {
                            string testName = string.Format("{0} ({1})", iList[i].Source.Room.Name, iList[i].Source.Group.Name);
                            if(testName.CompareTo(name) > 0)
                            {
                                break;
                            }
                        }

                        iList.Insert(index, s);
                        //iTableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                    }
                }

                iTableView.ReloadData();
            }

            private void SourceAdded(object sender, EventArgs e)
            {
                CreateList();
            }

            private void SourceRemoved(object sender, EventArgs e)
            {
                CreateList();
            }

            private void SourceChanged(object sender, EventArgs e)
            {
                CreateList();
            }

            private UITableView iTableView;

            private ReceiverSourceList iReceiversList;
            private ModelSender iSender;
            private List<ModelSourceReceiver> iList;
        }

        private class DelegateReceivers : UITableViewDelegate
        {
            public DelegateReceivers(ModelSender aSender, DataSourceReceivers aDataSource)
            {
                iSender = aSender;
                iDataSource = aDataSource;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                ModelSourceReceiver receiver = iDataSource.ReceiverAt(aIndexPath);
                if(receiver.IsPlayingSender(iSender))
                {
                    receiver.Stop();
                }
                else
                {
                    receiver.PlayNow(iSender.Metadata);
                }
            }

            private ModelSender iSender;
            private DataSourceReceivers iDataSource;
        }

        public ViewWidgetSenderReceiver(ModelSenders aSendersList, ReceiverSourceList aReceiversList)
        {
            iReceiversList = aReceiversList;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = 73.0f;
            TableView.BackgroundColor = UIColor.Black;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.ShowsHorizontalScrollIndicator = false;
            TableView.IndicatorStyle = UIScrollViewIndicatorStyle.White;

            DataSourceReceivers dataSource = new DataSourceReceivers(TableView, iReceiversList, iSender);
            TableView.DataSource = dataSource;
            TableView.Delegate = new DelegateReceivers(iSender, dataSource);
        }

        public void Select( ModelSender aSender)
        {
            iSender = aSender;

            if(IsViewLoaded)
            {
                Title = aSender.Name;

                DataSourceReceivers dataSource = new DataSourceReceivers(TableView, iReceiversList, aSender);
                TableView.DataSource = dataSource;
                TableView.Delegate = new DelegateReceivers(aSender, dataSource);
            }
        }

        private static NSString kCellIdentifierReceivers = new NSString("Receivers");

        private ModelSender iSender;
        private ReceiverSourceList iReceiversList;
    }
}