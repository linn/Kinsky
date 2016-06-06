using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceNetworkMonitor : ServiceUpnp
    {


        public ServiceNetworkMonitor(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceNetworkMonitor(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Name");
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Ports");
            action.AddOutArgument(new Argument("Sender", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Receiver", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Results", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "NetworkMonitor", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "NetworkMonitor", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionName CreateAsyncActionName()
        {
            return (new AsyncActionName(this));
        }

        public AsyncActionPorts CreateAsyncActionPorts()
        {
            return (new AsyncActionPorts(this));
        }


        // Synchronous actions
        
        public string NameSync()
        {
            AsyncActionName action = CreateAsyncActionName();
            
            object result = action.NameBeginSync();

            AsyncActionName.EventArgsResponse response = action.NameEnd(result);
                
            return(response.Name);
        }
        
        public void PortsSync(out uint Sender, out uint Receiver, out uint Results)
        {
            AsyncActionPorts action = CreateAsyncActionPorts();
            
            object result = action.PortsBeginSync();

            AsyncActionPorts.EventArgsResponse response = action.PortsEnd(result);
                
            Sender = response.Sender;
            Receiver = response.Receiver;
            Results = response.Results;
        }
        

        // AsyncActionName

        public class AsyncActionName
        {
            internal AsyncActionName(ServiceNetworkMonitor aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object NameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void NameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionName.NameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse NameEnd(object aResult)
            {
                try
                {
                    iHandler.ReadBegin(aResult);
                    EventArgsResponse response = new EventArgsResponse(iHandler);
                    iHandler.ReadEnd();
                    
                    return (response);
                }
                catch (SoapException e)
                {
                    throw (iService.CreateServiceException(ref e));
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionName.NameEnd(" + iService.ControlUri + "): " + e.Message);

                    throw (iService.CreateServiceException(ref e)); 
                }
            }

            private void Callback(object aResult)
            {
                try
                {
                    iHandler.ReadBegin(aResult);
                    EventArgsResponse result = new EventArgsResponse(iHandler);
                    iHandler.ReadEnd();
                    
                    if (EventResponse != null)
                    {
                        EventResponse(iService, result);
                    }
                }
                catch (SoapException e)
                {
                    EventArgsError error = iService.CreateEventArgsError(ref e);

                    if (EventError != null)
                    {
                        EventError(this, error);
                    }
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Name = aHandler.ReadArgumentString("Name");
                }
                
                public string Name;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceNetworkMonitor iService;
        }
        
        
        // AsyncActionPorts

        public class AsyncActionPorts
        {
            internal AsyncActionPorts(ServiceNetworkMonitor aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object PortsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PortsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionPorts.PortsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PortsEnd(object aResult)
            {
                try
                {
                    iHandler.ReadBegin(aResult);
                    EventArgsResponse response = new EventArgsResponse(iHandler);
                    iHandler.ReadEnd();
                    
                    return (response);
                }
                catch (SoapException e)
                {
                    throw (iService.CreateServiceException(ref e));
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionPorts.PortsEnd(" + iService.ControlUri + "): " + e.Message);

                    throw (iService.CreateServiceException(ref e)); 
                }
            }

            private void Callback(object aResult)
            {
                try
                {
                    iHandler.ReadBegin(aResult);
                    EventArgsResponse result = new EventArgsResponse(iHandler);
                    iHandler.ReadEnd();
                    
                    if (EventResponse != null)
                    {
                        EventResponse(iService, result);
                    }
                }
                catch (SoapException e)
                {
                    EventArgsError error = iService.CreateEventArgsError(ref e);

                    if (EventError != null)
                    {
                        EventError(this, error);
                    }
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("NetworkMonitor.AsyncActionPorts.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Sender = aHandler.ReadArgumentUint("Sender");
                    Receiver = aHandler.ReadArgumentUint("Receiver");
                    Results = aHandler.ReadArgumentUint("Results");
                }
                
                public uint Sender;
                public uint Receiver;
                public uint Results;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceNetworkMonitor iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
            if (e.SubscriptionId != SubscriptionId)
            {
                // This event is for a different subscription than the current. This can happen as follows:
                //
                // 1. Events A & B are received and queued up in the event server
                // 2. Event A is processed and is out of sequence - an unsubscribe/resubscribe is triggered
                // 3. By the time B is processed, the unsubscribe/resubscribe has completed and the SID is now different
                //
                // The upshot is that this event is ignored
                return;
            }

            lock (iLock)
            {
                if (e.SequenceNo != iExpectedSequenceNumber)
			    {
                    // An out of sequence event is being processed - log, resubscribe and discard
				    UserLog.WriteLine("EventServerEvent(ServiceNetworkMonitor): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

                    // resubscribing means that the initial event will be resent
                    iExpectedSequenceNumber = 0;

                    Unsubscribe();
                    Subscribe();
                    return;
			    }
                else
                {
                    iExpectedSequenceNumber++;
                }
            }
			
            XmlNode variable;

            XmlNamespaceManager nsmanager = new XmlNamespaceManager(e.Xml.NameTable);

            nsmanager.AddNamespace("e", kNamespaceUpnpService);

            bool eventName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Name", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Name = value;

                eventName = true;
            }

            bool eventSender = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Sender", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				try
				{
					Sender = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Sender with value {1}", DateTime.Now, value));
				}

                eventSender = true;
            }

            bool eventReceiver = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Receiver", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				try
				{
					Receiver = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Receiver with value {1}", DateTime.Now, value));
				}

                eventReceiver = true;
            }

            bool eventResults = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Results", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				try
				{
					Results = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Results with value {1}", DateTime.Now, value));
				}

                eventResults = true;
            }

          
            
            if(eventName)
            {
                if (EventStateName != null)
                {
					try
					{
						EventStateName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSender)
            {
                if (EventStateSender != null)
                {
					try
					{
						EventStateSender(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSender: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventReceiver)
            {
                if (EventStateReceiver != null)
                {
					try
					{
						EventStateReceiver(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateReceiver: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventResults)
            {
                if (EventStateResults != null)
                {
					try
					{
						EventStateResults(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateResults: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if (EventState != null)
            {
                EventState(this, EventArgs.Empty);
            }

            EventHandler<EventArgs> eventInitial = null;
            lock (iLock)
            {
                if (!iInitialEventReceived && e.SequenceNo == 0)
                {
                    iInitialEventReceived = true;
                    eventInitial = iEventInitial;
                }
            }

            if (eventInitial != null)
            {
                eventInitial(this, EventArgs.Empty);
            }
        }

        private EventHandler<EventArgs> iEventInitial;

        private bool iInitialEventReceived = false;
		private uint iExpectedSequenceNumber = 0;
        private object iLock = new object();

        public event EventHandler<EventArgs> EventInitial
        {
            add
            {
                bool doNotify = false;

                lock (iLock)
                {
                    if (iEventInitial == null)
                    {
                        iExpectedSequenceNumber = 0;
                        iInitialEventReceived = false;
                        iEventInitial += value;
                        Subscribe();
                    }
                    else
                    {
                        doNotify = iInitialEventReceived;
                        iEventInitial += value;
                    }
                }

                if (doNotify) {
                    value(this, EventArgs.Empty);
                }
            }

            remove
            {
                lock (iLock)
                {
                    iEventInitial -= value;

                    if (iEventInitial == null)
                    {
                        Unsubscribe();
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventState;
        public event EventHandler<EventArgs> EventStateName;
        public event EventHandler<EventArgs> EventStateSender;
        public event EventHandler<EventArgs> EventStateReceiver;
        public event EventHandler<EventArgs> EventStateResults;

        public string Name;
        public uint Sender;
        public uint Receiver;
        public uint Results;
    }
}
    
