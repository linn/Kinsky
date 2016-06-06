using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Linn.Kinsky;
using System.Windows;
using System.Reflection;
using Linn;

namespace KinskyDesktopWpf
{
    public partial class SystrayForm : Form
    {

        public event EventHandler<EventArgs> EventClosed;

        private ToolStripSeparator iSeparator1;
        private ToolStripSeparator iSeparator2;
        private ToolStripSeparator iSeparator3;
        private ToolStripSeparator iSeparator4;

        public ToolStripMenuItem ShowToolStripMenuItem { get; set; }
        public ToolStripMenuItem RepeatToolStripMenuItem { get; set; }
        public ToolStripMenuItem ShuffleToolStripMenuItem { get; set; }
        public ToolStripMenuItem PreviousToolStripMenuItem { get; set; }
        public ToolStripMenuItem PlayToolStripMenuItem { get; set; }
        public ToolStripMenuItem NextToolStripMenuItem { get; set; }
        public ToolStripMenuItem MuteToolStripMenuItem { get; set; }
        public ToolStripMenuItem ExitToolStripMenuItem { get; set; }
        public NotifyIcon NotifyIcon { get; set; }

        private ViewSysTrayContextMenu iViewSysTrayContextMenu;

        private ViewMaster iViewMaster;
        private KinskyDesktop iParent;

        public SystrayForm(KinskyDesktop aParent)
        {
            InitializeComponent();
            iParent = aParent;

            ShowToolStripMenuItem = new ToolStripMenuItem();
            ShowToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            ShowToolStripMenuItem.Text = "Hide Kinsky";
            ShowToolStripMenuItem.Click += (d, e) =>
            {
                if (aParent.WindowState == System.Windows.WindowState.Minimized)
                {
                    aParent.WindowState = System.Windows.WindowState.Normal;
                }
                else
                {
                    aParent.WindowState = System.Windows.WindowState.Minimized;
                }
            };

            aParent.StateChanged += (d, e) =>
            {
                if (aParent.WindowState == System.Windows.WindowState.Minimized)
                {
                    ShowToolStripMenuItem.Text = "Show Kinsky";
                }
                else
                {
                    ShowToolStripMenuItem.Text = "Hide Kinsky";
                }
            };


            iSeparator1 = new ToolStripSeparator();
            iSeparator1.Size = new System.Drawing.Size(174, 6);


            RepeatToolStripMenuItem = new ToolStripMenuItem();
            RepeatToolStripMenuItem.Enabled = false;
            RepeatToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            RepeatToolStripMenuItem.Text = "Repeat";


            ShuffleToolStripMenuItem = new ToolStripMenuItem();
            ShuffleToolStripMenuItem.Enabled = false;
            ShuffleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            ShuffleToolStripMenuItem.Text = "Shuffle";


            iSeparator2 = new ToolStripSeparator();
            iSeparator2.Size = new System.Drawing.Size(174, 6);


            PreviousToolStripMenuItem = new ToolStripMenuItem();
            PreviousToolStripMenuItem.Enabled = false;
            PreviousToolStripMenuItem.Image = StaticImages.SysTrayPrevious;
            PreviousToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            PreviousToolStripMenuItem.Text = "Previous";


            PlayToolStripMenuItem = new ToolStripMenuItem();
            PlayToolStripMenuItem.Enabled = false;
            PlayToolStripMenuItem.Image = StaticImages.SysTrayPlay;
            PlayToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            PlayToolStripMenuItem.Text = "Play";


            NextToolStripMenuItem = new ToolStripMenuItem();
            NextToolStripMenuItem.Enabled = false;
            NextToolStripMenuItem.Image = StaticImages.SysTrayNext;
            NextToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            NextToolStripMenuItem.Text = "Next";


            iSeparator3 = new ToolStripSeparator();
            iSeparator3.Size = new System.Drawing.Size(174, 6);


            MuteToolStripMenuItem = new ToolStripMenuItem();
            MuteToolStripMenuItem.Enabled = false;
            MuteToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            MuteToolStripMenuItem.Text = "Mute";


            iSeparator4 = new ToolStripSeparator();
            iSeparator4.Size = new System.Drawing.Size(174, 6);


            ExitToolStripMenuItem = new ToolStripMenuItem();
            ExitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            ExitToolStripMenuItem.Text = "Exit";
            ExitToolStripMenuItem.Click += (d, e) =>
            {
                if (EventClosed != null)
                {
                    EventClosed(this, EventArgs.Empty);
                }
            };

            NotifyIcon = new NotifyIcon();
            NotifyIcon.ContextMenuStrip = new ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                ShowToolStripMenuItem,
                iSeparator1,
                RepeatToolStripMenuItem,
                ShuffleToolStripMenuItem,
                iSeparator2,
                PreviousToolStripMenuItem,
                PlayToolStripMenuItem,
                NextToolStripMenuItem,
                iSeparator3,
                MuteToolStripMenuItem,
                iSeparator4,
                ExitToolStripMenuItem
            });
            NotifyIcon.MouseDoubleClick += new MouseEventHandler(NotifyIcon_MouseDoubleClick);
            Bitmap bmp = KinskyDesktopWpf.StaticImages.KinskyIcon;
            try
            {
                NotifyIcon.Icon = System.Drawing.Icon.FromHandle((bmp).GetHicon());
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                UserLog.WriteLine("COM error caught creating icon for SystrayForm: " + ex);
            }
            NotifyIcon.Visible = true;
            NotifyIcon.Text = aParent.Title;
            // hide the form on startup
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            Show();
            Hide();
        }

        void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (iParent.WindowState == System.Windows.WindowState.Minimized)
            {
                iParent.WindowState = System.Windows.WindowState.Normal;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            NotifyIcon.Dispose();
            base.OnClosed(e);
        }

        public void Initialise(ViewMaster aViewMaster)
        {
            iViewMaster = aViewMaster;
            iViewSysTrayContextMenu = new ViewSysTrayContextMenu(this);
        }

        internal void Start()
        {
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetTransportControlRadio.Add(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetPlayMode.Add(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewSysTrayContextMenu);
        }

        internal void Stop()
        {
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Remove(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Remove(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetTransportControlRadio.Remove(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetPlayMode.Remove(iViewSysTrayContextMenu);
            iViewMaster.ViewWidgetVolumeControl.Remove(iViewSysTrayContextMenu);
        }
    }



}
