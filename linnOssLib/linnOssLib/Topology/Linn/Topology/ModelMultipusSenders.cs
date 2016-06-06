using System;
using System.Net;
using System.Collections.Generic;

using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;

using Upnp;
using Linn.ControlPoint;

namespace Linn.Topology
{
    public interface IModelSenders
    {
        event EventHandler<ModelSenders.EventArgsSender> EventSenderAdded;
        event EventHandler<ModelSenders.EventArgsSender> EventSenderChanged;
        event EventHandler<ModelSenders.EventArgsSender> EventSenderRemoved;
        IList<IModelSender> SendersList { get; }
    }

    public class ModelSenders : IModelSenders
    {
        public class EventArgsSender : EventArgs
        {
            public EventArgsSender(IModelSender aSender)
            {
                iSender = aSender;
            }

            public IModelSender Sender
            {
                get
                {
                    return (iSender);
                }
            }

            private IModelSender iSender;
        }

        public ModelSenders(ISsdpNotifyProvider aListenerNotify, IEventUpnpProvider aEventServer)
        {
            iOpen = false;
            iEventServer = aEventServer;

            iPendingSenderList = new Dictionary<string, ModelSender>();
            iEnabledSenderList = new List<IModelSender>();
            iSenderList = new Dictionary<string, ModelSender>();
            iSenders = new Senders(aListenerNotify);

            iLock = new object();
        }

        public void Start(IPAddress aIpAddress)
        {
            iSenders.EventSenderAdded += SenderAdded;
            iSenders.EventSenderRemoved += SenderRemoved;
            lock (iLock)
            {
                iOpen = true;
            }
            iSenders.Start(aIpAddress);
        }

        public void Stop()
        {
            iSenders.EventSenderAdded -= SenderAdded;
            iSenders.EventSenderRemoved -= SenderRemoved;
            lock (iLock)
            {            
                foreach (ModelSender m in iPendingSenderList.Values)
                {
                    m.EventSenderInitialised -= SenderInitialised;
                    m.EventAudioChanged -= SenderChanged;
                    m.EventMetadataChanged -= SenderChanged;
                    m.EventStatusChanged -= SenderChanged;
                    m.EventSubscriptionError -= SubscriptionError;

                    m.Close();
                }
                foreach (ModelSender m in iSenderList.Values)
                {
                    m.EventSenderInitialised -= SenderInitialised;
                    m.EventAudioChanged -= SenderChanged;
                    m.EventMetadataChanged -= SenderChanged;
                    m.EventStatusChanged -= SenderChanged;
                    m.EventSubscriptionError -= SubscriptionError;

                    m.Close();
                }

                iPendingSenderList.Clear();
                iEnabledSenderList.Clear();
                iSenderList.Clear();

                iOpen = false;

                UserLog.WriteLine(DateTime.Now + ": ModelSenders stopped");
            }
            iSenders.Stop();
        }

        public void Rescan()
        {
            iSenders.Rescan();
        }

        public IList<IModelSender> SendersList
        {
            get
            {
                lock (iLock)
                {
                    return new List<IModelSender>(iEnabledSenderList).AsReadOnly();
                }
            }
        }

        private void SubscriptionError(object sender, EventArgs args)
        {
            iSenders.RemoveDevice((sender as ModelSender).Sender.Device);
        }

        public event EventHandler<EventArgsSender> EventSenderAdded;
        public event EventHandler<EventArgsSender> EventSenderRemoved;
        public event EventHandler<EventArgsSender> EventSenderChanged;

        private void SenderAdded(object sender, Senders.EventArgsSender e)
        {
            lock(iLock)
            {
                if(iOpen)
                {
                    try
                    {
                        ModelSender model = new ModelSender(e.Sender, iEventServer);
                        model.EventAudioChanged += SenderChanged;
                        model.EventMetadataChanged += SenderChanged;
                        model.EventStatusChanged += SenderChanged;
                        model.EventSenderInitialised += SenderInitialised;
                        model.EventSubscriptionError += SubscriptionError;

                        iPendingSenderList.Add(e.Sender.Device.Udn, model);

                        model.Open();
                    }
                    catch (ServiceException ex)
                    {
                        UserLog.WriteLine("Failed to create ModelSender: " + ex);
                        iSenders.RemoveDevice(e.Sender.Device);
                    }
                }
            }
        }

        private void SenderRemoved(object sender, Senders.EventArgsSender e)
        {
            ModelSender model;
            bool changed = false;

            lock (iLock)
            {
                if (iPendingSenderList.TryGetValue(e.Sender.Device.Udn, out model))
                {
                    iPendingSenderList.Remove(e.Sender.Device.Udn);
                }
                else
                {
                    if (iSenderList.TryGetValue(e.Sender.Device.Udn, out model))
                    {
                        iSenderList.Remove(e.Sender.Device.Udn);
                        iEnabledSenderList.Remove(model);
                    }
                    else
                    {
                        model = null;
                    }
                }

                if (model != null)
                {
                    model.EventSenderInitialised -= SenderInitialised;
                    model.EventAudioChanged -= SenderChanged;
                    model.EventMetadataChanged -= SenderChanged;
                    model.EventStatusChanged -= SenderChanged;
                    model.EventSubscriptionError -= SubscriptionError;

                    model.Close();

                    changed = true;
                }
            }

            if (changed && EventSenderRemoved != null)
            {
                EventSenderRemoved(this, new EventArgsSender(model));
            }
        }

        private void SenderInitialised(object sender, EventArgs e)
        {
            ModelSender modelSender = sender as ModelSender;
            bool changed = false;

            lock (iLock)
            {
                if (iPendingSenderList.Remove(modelSender.Udn))
                {
                    iSenderList.Add(modelSender.Udn, modelSender);
                    Assert.Check(modelSender.Metadata != null);
                    if (modelSender.Status != ModelSender.EStatus.eDisabled)
                    {
                        int index = 0;
                        foreach (ModelSender s in iEnabledSenderList)
                        {
                            if (modelSender.FullName.CompareTo(s.FullName) < 0)
                            {
                                break;
                            }
                            ++index;
                        }
                        iEnabledSenderList.Insert(index, modelSender);
                        changed = true;
                    }
                    else
                    {
                        UserLog.WriteLine("Disabled Sender: " + modelSender.Sender.Name);
                    }
                }
                else
                {
                    UserLog.WriteLine("Sender not found: " + modelSender.Sender.Name);
                }
            }

            if (changed && EventSenderAdded != null)
            {
                EventSenderAdded(this, new EventArgsSender(modelSender));
            }
        }

        private void SenderChanged(object sender, EventArgs e)
        {
            ModelSender multipus = sender as ModelSender;
            bool removed = false;
            bool added = false;

            lock (iLock)
            {
                if (iSenderList.ContainsKey(multipus.Udn))
                {
                    if (multipus.Status == ModelSender.EStatus.eDisabled)
                    {
                        removed = iEnabledSenderList.Remove(multipus);
                    }
                    else
                    {
                        if (!iEnabledSenderList.Contains(multipus))
                        {
                            iEnabledSenderList.Add(multipus);
                            added = true;
                        }
                        else
                        {
                            removed = true;
                            added = true;
                        }
                    }
                }
            }

            if (removed && added && EventSenderChanged != null)
            {
                EventSenderChanged(this, new EventArgsSender(multipus));
            }

            if (removed && !added && EventSenderRemoved != null)
            {
                EventSenderRemoved(this, new EventArgsSender(multipus));
            }

            if (added && !removed && EventSenderAdded != null)
            {
                EventSenderAdded(this, new EventArgsSender(multipus));
            }
        }

        private bool iOpen;

        private Senders iSenders;
        private IEventUpnpProvider iEventServer;

        private Dictionary<string, ModelSender> iPendingSenderList;
        private List<IModelSender> iEnabledSenderList;
        private Dictionary<string, ModelSender> iSenderList;
        private object iLock;
    }
}