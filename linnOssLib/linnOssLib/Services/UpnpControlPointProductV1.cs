using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceProductV1 : ServiceUpnp
    {


        public ServiceProductV1(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceProductV1(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Room");
            action.AddOutArgument(new Argument("aRoom", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetRoom");
            action.AddInArgument(new Argument("aRoom", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Standby");
            action.AddOutArgument(new Argument("aStandby", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetStandby");
            action.AddInArgument(new Argument("aStandby", Argument.EType.eBool));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Product", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Product", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionRoom CreateAsyncActionRoom()
        {
            return (new AsyncActionRoom(this));
        }

        public AsyncActionSetRoom CreateAsyncActionSetRoom()
        {
            return (new AsyncActionSetRoom(this));
        }

        public AsyncActionStandby CreateAsyncActionStandby()
        {
            return (new AsyncActionStandby(this));
        }

        public AsyncActionSetStandby CreateAsyncActionSetStandby()
        {
            return (new AsyncActionSetStandby(this));
        }


        // Synchronous actions
        
        public string RoomSync()
        {
            AsyncActionRoom action = CreateAsyncActionRoom();
            
            object result = action.RoomBeginSync();

            AsyncActionRoom.EventArgsResponse response = action.RoomEnd(result);
                
            return(response.aRoom);
        }
        
        public void SetRoomSync(string aRoom)
        {
            AsyncActionSetRoom action = CreateAsyncActionSetRoom();
            
            object result = action.SetRoomBeginSync(aRoom);

            action.SetRoomEnd(result);
        }
        
        public bool StandbySync()
        {
            AsyncActionStandby action = CreateAsyncActionStandby();
            
            object result = action.StandbyBeginSync();

            AsyncActionStandby.EventArgsResponse response = action.StandbyEnd(result);
                
            return(response.aStandby);
        }
        
        public void SetStandbySync(bool aStandby)
        {
            AsyncActionSetStandby action = CreateAsyncActionSetStandby();
            
            object result = action.SetStandbyBeginSync(aStandby);

            action.SetStandbyEnd(result);
        }
        

        // AsyncActionRoom

        public class AsyncActionRoom
        {
            internal AsyncActionRoom(ServiceProductV1 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object RoomBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RoomBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV1.AsyncActionRoom.RoomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RoomEnd(object aResult)
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
                    UserLog.WriteLine("ProductV1.AsyncActionRoom.RoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV1.AsyncActionRoom.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aRoom = aHandler.ReadArgumentString("aRoom");
                }
                
                public string aRoom;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV1 iService;
        }
        
        
        // AsyncActionSetRoom

        public class AsyncActionSetRoom
        {
            internal AsyncActionSetRoom(ServiceProductV1 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object SetRoomBeginSync(string aRoom)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aRoom", aRoom);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRoomBegin(string aRoom)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aRoom", aRoom);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV1.AsyncActionSetRoom.SetRoomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRoomEnd(object aResult)
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
                    UserLog.WriteLine("ProductV1.AsyncActionSetRoom.SetRoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV1.AsyncActionSetRoom.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                }
                
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV1 iService;
        }
        
        
        // AsyncActionStandby

        public class AsyncActionStandby
        {
            internal AsyncActionStandby(ServiceProductV1 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object StandbyBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StandbyBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV1.AsyncActionStandby.StandbyBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StandbyEnd(object aResult)
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
                    UserLog.WriteLine("ProductV1.AsyncActionStandby.StandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV1.AsyncActionStandby.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aStandby = aHandler.ReadArgumentBool("aStandby");
                }
                
                public bool aStandby;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV1 iService;
        }
        
        
        // AsyncActionSetStandby

        public class AsyncActionSetStandby
        {
            internal AsyncActionSetStandby(ServiceProductV1 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SetStandbyBeginSync(bool aStandby)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStandby", aStandby);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStandbyBegin(bool aStandby)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStandby", aStandby);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV1.AsyncActionSetStandby.SetStandbyBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStandbyEnd(object aResult)
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
                    UserLog.WriteLine("ProductV1.AsyncActionSetStandby.SetStandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV1.AsyncActionSetStandby.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                }
                
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV1 iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceProductV1): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventRoom = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Room", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Room = value;

                eventRoom = true;
            }

            bool eventStandby = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Standby", nsmanager);

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
	                Standby = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Standby = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Standby = false; 
    	            }
                }

                eventStandby = true;
            }

          
            
            if(eventRoom)
            {
                if (EventStateRoom != null)
                {
					try
					{
						EventStateRoom(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateRoom: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventStandby)
            {
                if (EventStateStandby != null)
                {
					try
					{
						EventStateStandby(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateStandby: " + ex);
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
        public event EventHandler<EventArgs> EventStateRoom;
        public event EventHandler<EventArgs> EventStateStandby;

        public string Room;
        public bool Standby;
    }
}
    
