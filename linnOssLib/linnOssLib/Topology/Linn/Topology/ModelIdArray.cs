using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Net;

using Linn;
using Linn.ControlPoint;
using Upnp;

namespace Linn.Topology
{
    internal class ByteArray
    {
        internal static IList<uint> Unpack(byte[] aIdArray)
        {
            List<uint> ids = new List<uint>();
            // unpack id array into list of ids
            uint j = 0;
            for (int i = 0; i < aIdArray.Length; i += 4, ++j)
            {
                uint value = Linn.BigEndianConverter.BigEndianToUint32(aIdArray, i);
                ids.Add(value);
                Trace.WriteLine(Trace.kMediaRenderer, "IdArray.Unpack: idArray[" + j + "]=" + value);
            }

            return ids;
        }
    }

    public interface IIdArray
    {
        string Read(uint aId);
        string ReadList(string aIdList);

        IList<MrItem> ParseMetadataXml(string aXml);

        MrItem Default { get; }
    }

    public class ModelIdArray
    {
        internal class MetadataCollector
        {
            public MetadataCollector(IIdArray aIdArray)
            {
                iIdArray = aIdArray;
                ClearIds();
            }

            public void ClearIds()
            {
                iIdList = string.Empty;
                iCount = 0;
            }

            public void AddId(uint aId)
            {
                iIdList += ((iIdList.Length > 0) ? " " : "") + aId.ToString();
                ++iCount;
            }

            public IList<MrItem> Process()
            {
                return Process(false);
            }

            public IList<MrItem> Process(bool aForce)
            {
                if (iCount > kCountPerCall || (aForce && iCount > 0))
                {
                    Trace.WriteLine(Trace.kMediaRenderer, "MetadataCollector.Process: iCount=" + iCount + ", aForce=" + aForce);
                    string result = iIdArray.ReadList(iIdList);
                    
                    ClearIds();

                    return iIdArray.ParseMetadataXml(result);
                }

                return null;
            }

            private const uint kCountPerCall = 100;

            private string iIdList;
            private uint iCount;
            private IIdArray iIdArray;
        }

        public ModelIdArray(IIdArray aIdArray)
        {
            iIdArray = aIdArray;

            iMutex = new Mutex(false);

            iIds = new List<uint>();
            iCacheMetadata = new Dictionary<uint, MrItem>();
            iCacheUsage = new List<uint>();
            iEventIdArray = new AutoResetEvent(false);

            iMetadataCollector = new MetadataCollector(aIdArray);
        }

        public void Lock()
        {
            iMutex.WaitOne();
        }

        public void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public void Open()
        {
            Assert.Check(iThreadIdArray == null);

            iAbortThread = false;
            iRecollectMetadata = false;
            iThreadIdArray = new Thread(ProcessEventIdArray);
            iThreadIdArray.Name = "ProcessEventIdArray";
            iThreadIdArray.IsBackground = true;
            iThreadIdArray.Start();
        }

        public void Close()
        {
            Assert.Check(iThreadIdArray != null);

            Lock();
            iAbortThread = true;
            iEventIdArray.Set();
            Unlock();
            
            iThreadIdArray = null;
        }

        public void SetIdArray(IList<uint> aIdArray)
        {
            Lock();

            iRecollectMetadata = true;
            iIds = aIdArray;
            iEventIdArray.Set();

            Unlock();
        }

        public IList<uint> IdArray
        {
            get
            {
                Lock();
                List<uint> ids = new List<uint>(iIds);
                Unlock();

                return ids;
            }
        }

        public MrItem AtIndex(uint aIndex)
        {
            Lock();
            MrItem item;
            Assert.Check(aIndex >= 0);
            if (aIndex >= iIds.Count)
            {
                item = iIdArray.Default;
            }
            else
            {
                uint id = iIds[(int)aIndex];
                if (!iCacheMetadata.TryGetValue(id, out item))
                {
                    item = iIdArray.Default;
                    /*if(id > 0)
                    {
                        iRecollectMetadata = true;
                        iEventIdArray.Set();
                    }*/
                }
            }
            Unlock();

            return item;
        }

        public int Index(uint aId)
        {
            return iIds.IndexOf(aId);
        }

        public MrItem Metadata(uint aId)
        {
            MrItem item;
            if (!iCacheMetadata.TryGetValue(aId, out item))
            {
                item = iIdArray.Default;
                //iRecollectMetadata = true;
                //iEventIdArray.Set();
            }
            return item;
        }

        public void ClearCache()
        {
            Lock();

            iCacheMetadata.Clear();
            iCacheUsage.Clear();

            Unlock();
        }

        public void AddToCache(uint aNewId, MrItem aMrItem)
        {
            Lock();

            if (!iCacheMetadata.ContainsKey(aNewId))
            {
                iCacheMetadata.Add(aNewId, aMrItem);

                RemoveStaleCacheItems();

                iCacheUsage.Remove(aNewId);
                iCacheUsage.Add(aNewId);
            }

            Unlock();
        }

        public event EventHandler<EventArgs> EventIdArrayChanged;

        private void ProcessEventIdArray()
        {
            try
            {
                while (!iAbortThread)
                {
                    iEventIdArray.WaitOne();

                    try
                    {
                        Lock();

                        iRecollectMetadata = false;

                        Unlock();

                        if (!iAbortThread)
                        {
                            UpdateMetadata();
                        }

                        Lock();

                        if (!iRecollectMetadata)
                        {
                            iEventIdArray.Reset();
                        }

                        Unlock();
                    }
                    catch (ServiceException e) // do nothing with exception, topology will clean this source up
                    {
                        Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.ProcessEventIdArray: " + e.Message);
                    }
                    catch (WebException e)
                    {
                        Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.ProcessEventIdArray: " + e.Message);
                    }
                    catch (System.IO.IOException e)
                    {
                        Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.ProcessEventIdArray: " + e.Message);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.ProcessEventIdArray: Aborted");
            }
        }

        private void UpdateMetadata()
        {
            Lock();
            IList<uint> idArray = new List<uint>(iIds);
            Unlock();

            iMetadataCollector.ClearIds();

            IList<MrItem> list;
            foreach (uint id in idArray)
            {
                if (iRecollectMetadata || iAbortThread)
                {
                    return;
                }

                ItemById(id);

                list = iMetadataCollector.Process();
                if (list != null)
                {
                    UpdateCache(list);
                }

                Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.UpdateMetadata: id=" + id);
            }

            list = iMetadataCollector.Process(true);
            if (list != null)
            {
                UpdateCache(list);
            }

            if (EventIdArrayChanged != null)
            {
                EventIdArrayChanged(this, EventArgs.Empty);
            }
        }

        private MrItem ItemById(uint aId)
        {
            Lock();

            MrItem value;
            if (iCacheMetadata.TryGetValue(aId, out value))
            {
                Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.ItemById: Reading " + aId + " from cache");

                Unlock();

                return value;
            }
            else
            {
                // metadata only exists for non zero ids
                if (aId > 0)
                {
                    iMetadataCollector.AddId(aId);
                }

                Unlock();

                return null;
            }
        }

        private void UpdateCache(IList<MrItem> aItems)
        {
            foreach (MrItem item in aItems)
            {
                Lock();

                try
                {
                    iCacheMetadata.Add(item.Id, item);
                }
                catch (ArgumentException e)
                {
                    Trace.WriteLine(Trace.kTopology, e.Message + ": entry.Id=" + item.Id); 
                }

                RemoveStaleCacheItems();

                // refresh metadata usage
                iCacheUsage.Remove(item.Id);
                iCacheUsage.Add(item.Id);

                Unlock();
            }
        }

        private void RemoveStaleCacheItems()
        {
            // remove stale items from cache
            if (iCacheMetadata.Count > kMaxCacheSize)
            {
                uint idToRemove = iCacheUsage[0];
                iCacheMetadata.Remove(idToRemove);
                iCacheUsage.RemoveAt(0);

                Trace.WriteLine(Trace.kMediaRenderer, "ModelIdArray.RemoveStaleCacheItems: Removed id " + idToRemove);
            }

            Assert.Check(iCacheUsage.Count <= kMaxCacheSize);
            Assert.Check(iCacheMetadata.Count <= kMaxCacheSize);
        }

        private const uint kMaxCacheSize = 1000;

        private IIdArray iIdArray;

        private Mutex iMutex;
        private bool iAbortThread;
        private bool iRecollectMetadata;
        private Thread iThreadIdArray;
        private AutoResetEvent iEventIdArray;

        private MetadataCollector iMetadataCollector;
        private IList<uint> iIds;
        private Dictionary<uint, MrItem> iCacheMetadata;
        private List<uint> iCacheUsage;
    }

} // Linn.Topology
