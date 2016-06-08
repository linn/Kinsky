using System;
using System.Threading;
using System.Collections.Generic;

using Linn.Topology;

namespace Linn.Kinsky
{

    public interface IControllerRoomSelector
    {
        void Select(Linn.Kinsky.Room aRoom);
    }

    public interface IControllerSourceSelector
    {
        void Select(Linn.Kinsky.Source aSource);
    }

    public interface IControllerVolumeControl
    {
        void IncrementVolume();
        void DecrementVolume();
        void ToggleMute();

        void SetVolume(uint aVolume);
        void SetMute(bool aMute);
    }

    internal class ControllerVolumeControl : IControllerVolumeControl
    {
        public ControllerVolumeControl()
        {
            iMutex = new Mutex(false);
        }

        public void SetModelVolumeControl(ModelVolumeControl aModelVolumeControl)
        {
            Lock();
            
            iModelVolumeControl = aModelVolumeControl;

            Unlock();
        }

        public void Lock()
        {
            iMutex.WaitOne();
        }

        public void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public void IncrementVolume()
        {
            Lock();

            if (iModelVolumeControl != null)
            {
                iModelVolumeControl.IncrementVolume();
            }

            Unlock();
        }

        public void DecrementVolume()
        {
            Lock();

            if (iModelVolumeControl != null)
            {
                iModelVolumeControl.DecrementVolume();
            }

            Unlock();
        }

        public void ToggleMute()
        {
            Lock();

            if (iModelVolumeControl != null)
            {
                iModelVolumeControl.ToggleMute();
            }

            Unlock();
        }

        public void SetVolume(uint aVolume)
        {
            Lock();

            if (iModelVolumeControl != null)
            {
                iModelVolumeControl.SetVolume(aVolume);
            }

            Unlock();
        }

        public void SetMute(bool aMute)
        {
            Lock();

            if (iModelVolumeControl != null)
            {
                iModelVolumeControl.SetMute(aMute);
            }

            Unlock();
        }

        private Mutex iMutex;

        private ModelVolumeControl iModelVolumeControl;
    }

    public interface IControllerPlaylistMediaRenderer
    {
        void SeekTrack(uint aIndex);
        void PlaylistInsert(uint aInsertAfterId, IMediaRetriever aMediaRetriever);
        void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems);
        void PlaylistDelete(IList<MrItem> aPlaylistItems);
        void PlaylistDeleteAll();
    }

    public interface IControllerPlaylistRadio
    {
        void SelectPreset(int aPresetIndex);
        void SelectChannel(Channel aChannel);
    }

    public interface IControllerPlaylistReceiver
    {
        void SelectSender(ModelSender aSender);
    }

    public interface IControllerLocation
    {
        void Up(uint aLevels);
        void SetBrowser(IBrowser aBrowser);
    }

    public class MediatorLocation : IControllerLocation
    {
        public MediatorLocation()
        {
            iMutex = new Mutex(false);
            iViewWidgetLocationList = new List<IViewWidgetLocation>();
        }

        public void Lock()
        {
            iMutex.WaitOne();
        }

        public void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public void Add(IViewWidgetLocation aViewWidgetLocation)
        {
            Lock();

            iViewWidgetLocationList.Add(aViewWidgetLocation);

            Unlock();

            aViewWidgetLocation.Open();
        }

        public void Remove(IViewWidgetLocation aViewWidgetLocation)
        {
            aViewWidgetLocation.Close();

            Lock();

            iViewWidgetLocationList.Remove(aViewWidgetLocation);

            Unlock();
        }

        public void Up(uint aLevels)
        {
            Lock();

            if (iBrowser != null)
            {
                iBrowser.Up(aLevels);
            }

            Unlock();
        }

        public void SetBrowser(IBrowser aBrowser)
        {
            Lock();

            if (iBrowser != null)
            {
                iBrowser.EventLocationChanged -= LocationChanged;
            }

            iBrowser = aBrowser;

            if (iBrowser != null)
            {
                iBrowser.EventLocationChanged += LocationChanged;

                SetLocation(new Location(iBrowser.Location.Containers));

            }
            Unlock();
        }

        private void LocationChanged(object sender, EventArgs e)
        {
            IBrowser browser = sender as IBrowser;
            SetLocation(new Location(browser.Location.Containers));
        }

        private void SetLocation(Location aLocation)
        {
            IList<string> titles = new List<string>();
            foreach (IContainer c in aLocation.Containers)
            {
                titles.Add(c.Metadata.Title);
            }

            Lock();

            foreach (IViewWidgetLocation l in iViewWidgetLocationList)
            {
                l.SetLocation(titles);
            }

            Unlock();
        }

        private IBrowser iBrowser;

        private Mutex iMutex;
        private List<IViewWidgetLocation> iViewWidgetLocationList;
    }
}
