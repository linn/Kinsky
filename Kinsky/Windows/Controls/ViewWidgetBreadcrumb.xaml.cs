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
using Linn;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using Linn.Topology;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using Upnp;
using System.Windows.Media.Animation;

namespace KinskyDesktopWpf
{
    public partial class ViewWidgetBreadcrumb : BreadcrumbPanel
    {
        public event EventHandler<EventArgsBreadcrumbNavigation> EventBreadcrumbNavigate;
        private IViewWidgetButton iButtonUpDirectory;
        private IViewWidgetButton iButtonHome;
        private BreadcrumbTrail iLocation;
        private object iLockObject;

        public ViewWidgetBreadcrumb()
        {
            InitializeComponent();
            iLockObject = new object();
        }

        public void SetButtonUpDirectory(IViewWidgetButton aButtonUpDirectory)
        {
            iButtonUpDirectory = aButtonUpDirectory;
            iButtonUpDirectory.EventClick += iButtonUpDirectory_EventClick;
        }

        public void SetButtonHome(IViewWidgetButton aButtonHome)
        {
            iButtonHome = aButtonHome;
            iButtonHome.EventClick += iButtonHome_EventClick;
        }

        private void Up(int aCount)
        {
            if (EventBreadcrumbNavigate != null)
            {
                EventBreadcrumbNavigate(this, new EventArgsBreadcrumbNavigation(iLocation.TruncateEnd(aCount)));
            }
        }

        private void iButtonUpDirectory_EventClick(object sender, EventArgs e)
        {
            Up(1);
        }

        private void iButtonHome_EventClick(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iLocation != null)
                {
                    Up(iLocation.Count - 1);
                }
            }
        }

        public void SetLocation(BreadcrumbTrail aLocation)
        {
            lock (iLockObject)
            {
                iLocation = aLocation;
                if (aLocation.Count > 1)
                {
                    iButtonUpDirectory.Close();
                    iButtonUpDirectory.Open();
                    if (iButtonHome != null)
                    {
                        iButtonHome.Close();
                        iButtonHome.Open();
                    }
                }
                else
                {
                    iButtonUpDirectory.Close();
                    if (iButtonHome != null)
                    {
                        iButtonHome.Close();
                    }
                }
            }
            this.Dispatcher.BeginInvoke(new Action(delegate()
                      {
                          this.Children.Clear();
                          for (int i = 0; i < aLocation.Count; i++)
                          {
                              Button b = new Button();
                              b.Tag = i;
                              b.DataContext = aLocation[i].Title;
                              if (i != aLocation.Count - 1)
                              {
                                  b.Style = FindResource("BreadcrumbButton") as Style;
                                  b.Click += (sender, args) =>
                                  {
                                      int up = (int)(sender as Button).Tag;
                                      Up((aLocation.Count - up - 1));
                                  };
                              }
                              else
                              {
                                  b.Style = FindResource("BreadcrumbButtonNoClick") as Style;
                              }
                              this.Children.Add(b);
                          }
                      }));
        }

    }

    public class EventArgsBreadcrumbNavigation : EventArgs
    {
        public EventArgsBreadcrumbNavigation(BreadcrumbTrail aBreadcrumbTrail)
        {
            BreadcrumbTrail = aBreadcrumbTrail;
        }
        public BreadcrumbTrail BreadcrumbTrail { get; set; }
    }

}
