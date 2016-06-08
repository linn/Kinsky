using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;
using System.IO.IsolatedStorage;

namespace Linn.Kinsky
{
    public class WebFile : IDisposable
    {
        private bool iDisposed;

        internal WebFile(Uri aUri, string aLocalPath, uint aRefreshMinutes, WebFetcher aWebFetcher, WebScheduler aScheduler)
        {
            iUri = aUri;
            iLocalPath = aLocalPath;
            iRefreshMinutes = aRefreshMinutes;
            iWebFetcher = aWebFetcher;
            iScheduler = aScheduler;
            iDisposed = false;
        }

        public EventHandler<EventArgs> EventContentsChanged;

        public void Open()
        {
            /*iContents = iWebFetcher.Contents(iLocalPath);

            if (iContents != null)
            {
                iInitialContentsFromFile = true;
            }*/

            RefreshCallback();
        }

        public void Refresh()
        {
            lock (this)
            {
                if (!iDisposed)
                {
                    iScheduler.ScheduleNow(this.RefreshCallback);
                }
            }
        }

        public void Delete()
        {
            iWebFetcher.Delete(iLocalPath);
        }

        private void RefreshCallback()
        {
            /*if (iInitialContentsFromFile)
            {
                if (EventContentsChanged != null)
                {
                    EventContentsChanged(this, EventArgs.Empty);
                }

                iInitialContentsFromFile = false;

                FileInfo info = new FileInfo(iLocalPath);
                TimeSpan t = DateTime.Now.Subtract(info.CreationTime);
                if(t.TotalMinutes < iRefreshMinutes)
                {
                    return;
                }
            }*/

            string contents = iWebFetcher.Contents(iUri);

            if (contents != null)
            {
                if (contents != iContents)
                {
                    iWebFetcher.Update(iLocalPath, contents);

                    lock (this)
                    {
                        iContents = contents;
                    }

                    if (EventContentsChanged != null)
                    {
                        EventContentsChanged(this, EventArgs.Empty);
                    }
                }
            }

            /*lock (this)
            {
                if (!iDisposed)
                {
                    iScheduler.Schedule(iRefreshMinutes * 1000 * 60, this.RefreshCallback);
                }
            }*/
        }

        public string Contents
        {
            get
            {
                lock (this)
                {
                    return (iContents);
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                iDisposed = true;
                iScheduler.Unschedule(this.RefreshCallback);
            }
        }

        Uri iUri;
        string iLocalPath;
        uint iRefreshMinutes;
        WebFetcher iWebFetcher;
        WebScheduler iScheduler;
        string iContents;
        bool iInitialContentsFromFile;
    }

    public class WebFetcher
    {
        public WebFetcher(string aCacheFolder)
        {
            iCacheFolder = aCacheFolder;
            iScheduler = new WebScheduler();
        }

        public WebFile Create(Uri aUri, string aLocalPath, uint aRefreshMinutes)
        {
            return (new WebFile(aUri, aLocalPath, aRefreshMinutes, this, iScheduler));
        }

        internal string Contents(string aLocalPath)
        {
            string path = Path.Combine(iCacheFolder, aLocalPath);

            try
            {
                FileStream file = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(file);
                string contents = reader.ReadToEnd();
                reader.Close();
                file.Close();
                return (contents);
            }
            catch (IsolatedStorageException)
            {
                return (null);
            }
            catch (FileNotFoundException)
            {
                return (null);
            }
        }

        internal string Contents(Uri aUri)
        {
            try
            {
                WebRequest request = WebRequest.Create(aUri.AbsoluteUri);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                response.Close();
                return (contents);
            }
            catch (WebException)
            {
                return (null);
            }
        }

        internal void Update(string aLocalPath, string aContents)
        {
            string path = Path.Combine(iCacheFolder, aLocalPath);
            FileStream file = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);
            writer.Write(aContents);
            writer.Close();
            file.Close();
        }

        internal void Delete(string aLocalPath)
        {
            string path = Path.Combine(iCacheFolder, aLocalPath);

            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
            }
        }

        private string iCacheFolder;
        private WebScheduler iScheduler;
    }

    public class WebScheduler
    {
        public WebScheduler()
        {
            iLock = new object();
            iScheduledEvents = new Dictionary<Linn.Timer, List<Scheduler.DCallback>>();
            iScheduler = new Scheduler("Web Scheduler", 3);
        }

        public void ScheduleNow(Scheduler.DCallback aCallback)
        {
            iScheduler.Schedule(aCallback);
        }

        public void Schedule(uint aInterval, Scheduler.DCallback aCallback)
        {
            lock (iLock)
            {
                // check existing timers
	            foreach (KeyValuePair<Linn.Timer, List<Scheduler.DCallback>> item in iScheduledEvents)
	            {
	                if (item.Key.Interval == aInterval)
	                {
                        item.Value.Add(aCallback);
                        return;
	                }
	            }

                // add a new timer
                Linn.Timer t = new Linn.Timer();
                t.AutoReset = false;
                t.Interval = aInterval;
                t.Elapsed += TimerElapsed;
                t.Start();

                List<Scheduler.DCallback> list = new List<Scheduler.DCallback>();
                list.Add(aCallback);
                iScheduledEvents.Add(t, list);
            }
        }

        public void Unschedule(Scheduler.DCallback aCallback)
        {
            lock (iLock)
            {
                List<Linn.Timer> toRemove = new List<Timer>();

                foreach (KeyValuePair<Linn.Timer, List<Scheduler.DCallback>> item in iScheduledEvents)
                {
                    item.Value.Remove(aCallback);

                    if (item.Value.Count == 0)
                    {
                        toRemove.Add(item.Key);
                        item.Key.Dispose();
                    }
                }

                foreach (Linn.Timer t in toRemove)
                {
                    iScheduledEvents.Remove(t);
                }
            }
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            Linn.Timer t = sender as Linn.Timer;
            List<Scheduler.DCallback> callbacks = null;

            lock (iLock)
            {
                // get the timer
                foreach (KeyValuePair<Linn.Timer, List<Scheduler.DCallback>> item in iScheduledEvents)
                {
                    if (item.Key.Interval == t.Interval)
                    {
                        callbacks = item.Value;

                        iScheduledEvents.Remove(item.Key);
                        item.Key.Dispose();
                        break;
                    }
                }
            }

            if (callbacks != null)
            {
                foreach (Scheduler.DCallback callback in callbacks)
                {
                    iScheduler.Schedule(callback);
                }
            }
        }

        private object iLock;
        private Dictionary<Linn.Timer, List<Scheduler.DCallback>> iScheduledEvents;
        private Scheduler iScheduler;
    }
}


