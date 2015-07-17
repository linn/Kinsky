using System;
using System.Collections.Generic;

using Linn.Topology;

namespace Linn.Kinsky
{
    public class ReceiverSourceList
    {
        public ReceiverSourceList(HelperKinsky aHelperKinsky)
        {
            iPendingModelSourceReceiverList = new SortedList<string, ModelSourceReceiver>();
            iModelSourceReceiverList = new SortedList<string, ModelSourceReceiver>();

            iSourceList = new SourceList(aHelperKinsky.TopologyHouse, delegate(Linn.Topology.Source aSource) { return (aSource.Type == "Receiver"); });

            iSourceList.EventSourceAdded += SourceAdded;
            iSourceList.EventSourceRemoved += SourceRemoved;
        }

        public void Start()
        {
            iSourceList.Start();
        }

        public void Stop()
        {
            lock (this)
            {
                iSourceList.Stop();

                foreach (ModelSourceReceiver model in iPendingModelSourceReceiverList.Values)
                {
                    model.Close();
                    model.EventChannelChanged -= ChannelChanged;
                    model.EventTransportStateChanged -= TransportStateChanged;
                    model.EventControlInitialised -= ControlInitialised;
                }
                iPendingModelSourceReceiverList.Clear();

                foreach (ModelSourceReceiver model in iModelSourceReceiverList.Values)
                {
                    model.Close();
                    model.EventChannelChanged -= ChannelChanged;
                    model.EventTransportStateChanged -= TransportStateChanged;
                    model.EventControlInitialised -= ControlInitialised;
                }
                iModelSourceReceiverList.Clear();
            }
        }

        public IList<ModelSourceReceiver> Sources
        {
            get
            {
                lock (this)
                {
                    return new List<ModelSourceReceiver>(iModelSourceReceiverList.Values).AsReadOnly();
                }
            }
        }

        public event EventHandler<EventArgsReceiverSource> EventReceiverSourceAdded;
        public event EventHandler<EventArgsReceiverSource> EventReceiverSourceRemoved;
        public event EventHandler<EventArgsReceiverSource> EventReceiverSourceChanged;

        private void SourceAdded(object sender, SourceList.EventArgsSource e)
        {
            ModelSourceReceiver model = new ModelSourceReceiver(e.Source);
            model.EventChannelChanged += ChannelChanged;
            model.EventTransportStateChanged += TransportStateChanged;
            model.EventControlInitialised += ControlInitialised;

            lock (this)
            {
                iPendingModelSourceReceiverList.Add(e.Source.Device.Udn, model);
            }

            model.Open();
        }

        private void SourceRemoved(object sender, SourceList.EventArgsSource e)
        {
            bool changed = false;

            ModelSourceReceiver model;
            lock (this)
            {
                if (iPendingModelSourceReceiverList.TryGetValue(e.Source.Device.Udn, out model))
                {
                    iPendingModelSourceReceiverList.Remove(e.Source.Device.Udn);
                }
                else if (iModelSourceReceiverList.TryGetValue(e.Source.Device.Udn, out model))
                {
                    iModelSourceReceiverList.Remove(e.Source.Device.Udn);
                    changed = true;
                }

                if (model != null)
                {
                    model.Close();
                    model.EventChannelChanged -= ChannelChanged;
                    model.EventTransportStateChanged -= TransportStateChanged;
                    model.EventControlInitialised -= ControlInitialised;
                }
            }

            if (changed && EventReceiverSourceRemoved != null)
            {
                EventReceiverSourceRemoved(this, new EventArgsReceiverSource(model));
            }
        }

        private void ControlInitialised(object sender, EventArgs e)
        {
            ModelSourceReceiver model = sender as ModelSourceReceiver;

            lock (this)
            {
                if (iPendingModelSourceReceiverList.Remove(model.Source.Device.Udn))
                {
                    iModelSourceReceiverList.Add(model.Source.Device.Udn, model);
                }
            }

            if (EventReceiverSourceAdded != null)
            {
                EventReceiverSourceAdded(this, new EventArgsReceiverSource(model));
            }
        }

        private void ChannelChanged(object sender, EventArgs e)
        {
            if (EventReceiverSourceChanged != null)
            {
                EventReceiverSourceChanged(this, new EventArgsReceiverSource(sender as ModelSourceReceiver));
            }
        }

        private void TransportStateChanged(object sender, EventArgs e)
        {
            if (EventReceiverSourceChanged != null)
            {
                EventReceiverSourceChanged(this, new EventArgsReceiverSource(sender as ModelSourceReceiver));
            }
        }


        

        private SourceList iSourceList;
        private SortedList<string, ModelSourceReceiver> iPendingModelSourceReceiverList;
        private SortedList<string, ModelSourceReceiver> iModelSourceReceiverList;
    }

    public class EventArgsReceiverSource : EventArgs
    {
        public EventArgsReceiverSource(ModelSourceReceiver aReceiverSource)
        {
            ReceiverSource = aReceiverSource;
        }

        public ModelSourceReceiver ReceiverSource { get; set; }
    }
}