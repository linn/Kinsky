using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceConnectionManager : ServiceUpnp
    {

        public const string kConnectionStatusOk = "OK";
        public const string kConnectionStatusContentFormatMismatch = "ContentFormatMismatch";
        public const string kConnectionStatusInsufficientBandwidth = "InsufficientBandwidth";
        public const string kConnectionStatusUnreliableChannel = "UnreliableChannel";
        public const string kConnectionStatusUnknown = "Unknown";
        public const string kDirectionInput = "Input";
        public const string kDirectionOutput = "Output";

        public ServiceConnectionManager(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceConnectionManager(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("GetProtocolInfo");
            action.AddOutArgument(new Argument("Source", Argument.EType.eString));
            action.AddOutArgument(new Argument("Sink", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PrepareForConnection");
            action.AddInArgument(new Argument("RemoteProtocolInfo", Argument.EType.eString));
            action.AddInArgument(new Argument("PeerConnectionManager", Argument.EType.eString));
            action.AddInArgument(new Argument("PeerConnectionID", Argument.EType.eInt));
            action.AddInArgument(new Argument("Direction", Argument.EType.eString));
            action.AddOutArgument(new Argument("ConnectionID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("AVTransportID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("RcsID", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("ConnectionComplete");
            action.AddInArgument(new Argument("ConnectionID", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("GetCurrentConnectionIDs");
            action.AddOutArgument(new Argument("ConnectionIDs", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetCurrentConnectionInfo");
            action.AddInArgument(new Argument("ConnectionID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("RcsID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("AVTransportID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("ProtocolInfo", Argument.EType.eString));
            action.AddOutArgument(new Argument("PeerConnectionManager", Argument.EType.eString));
            action.AddOutArgument(new Argument("PeerConnectionID", Argument.EType.eInt));
            action.AddOutArgument(new Argument("Direction", Argument.EType.eString));
            action.AddOutArgument(new Argument("Status", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("upnp.org", "ConnectionManager", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("upnp.org", "ConnectionManager", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionGetProtocolInfo CreateAsyncActionGetProtocolInfo()
        {
            return (new AsyncActionGetProtocolInfo(this));
        }

        public AsyncActionPrepareForConnection CreateAsyncActionPrepareForConnection()
        {
            return (new AsyncActionPrepareForConnection(this));
        }

        public AsyncActionConnectionComplete CreateAsyncActionConnectionComplete()
        {
            return (new AsyncActionConnectionComplete(this));
        }

        public AsyncActionGetCurrentConnectionIDs CreateAsyncActionGetCurrentConnectionIDs()
        {
            return (new AsyncActionGetCurrentConnectionIDs(this));
        }

        public AsyncActionGetCurrentConnectionInfo CreateAsyncActionGetCurrentConnectionInfo()
        {
            return (new AsyncActionGetCurrentConnectionInfo(this));
        }


        // Synchronous actions
        
        public void GetProtocolInfoSync(out string Source, out string Sink)
        {
            AsyncActionGetProtocolInfo action = CreateAsyncActionGetProtocolInfo();
            
            object result = action.GetProtocolInfoBeginSync();

            AsyncActionGetProtocolInfo.EventArgsResponse response = action.GetProtocolInfoEnd(result);
                
            Source = response.Source;
            Sink = response.Sink;
        }
        
        public void PrepareForConnectionSync(string RemoteProtocolInfo, string PeerConnectionManager, int PeerConnectionID, string Direction, out int ConnectionID, out int AVTransportID, out int RcsID)
        {
            AsyncActionPrepareForConnection action = CreateAsyncActionPrepareForConnection();
            
            object result = action.PrepareForConnectionBeginSync(RemoteProtocolInfo, PeerConnectionManager, PeerConnectionID, Direction);

            AsyncActionPrepareForConnection.EventArgsResponse response = action.PrepareForConnectionEnd(result);
                
            ConnectionID = response.ConnectionID;
            AVTransportID = response.AVTransportID;
            RcsID = response.RcsID;
        }
        
        public void ConnectionCompleteSync(int ConnectionID)
        {
            AsyncActionConnectionComplete action = CreateAsyncActionConnectionComplete();
            
            object result = action.ConnectionCompleteBeginSync(ConnectionID);

            action.ConnectionCompleteEnd(result);
        }
        
        public string GetCurrentConnectionIDsSync()
        {
            AsyncActionGetCurrentConnectionIDs action = CreateAsyncActionGetCurrentConnectionIDs();
            
            object result = action.GetCurrentConnectionIDsBeginSync();

            AsyncActionGetCurrentConnectionIDs.EventArgsResponse response = action.GetCurrentConnectionIDsEnd(result);
                
            return(response.ConnectionIDs);
        }
        
        public void GetCurrentConnectionInfoSync(int ConnectionID, out int RcsID, out int AVTransportID, out string ProtocolInfo, out string PeerConnectionManager, out int PeerConnectionID, out string Direction, out string Status)
        {
            AsyncActionGetCurrentConnectionInfo action = CreateAsyncActionGetCurrentConnectionInfo();
            
            object result = action.GetCurrentConnectionInfoBeginSync(ConnectionID);

            AsyncActionGetCurrentConnectionInfo.EventArgsResponse response = action.GetCurrentConnectionInfoEnd(result);
                
            RcsID = response.RcsID;
            AVTransportID = response.AVTransportID;
            ProtocolInfo = response.ProtocolInfo;
            PeerConnectionManager = response.PeerConnectionManager;
            PeerConnectionID = response.PeerConnectionID;
            Direction = response.Direction;
            Status = response.Status;
        }
        

        // AsyncActionGetProtocolInfo

        public class AsyncActionGetProtocolInfo
        {
            internal AsyncActionGetProtocolInfo(ServiceConnectionManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object GetProtocolInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetProtocolInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetProtocolInfo.GetProtocolInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetProtocolInfoEnd(object aResult)
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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetProtocolInfo.GetProtocolInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetProtocolInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Source = aHandler.ReadArgumentString("Source");
                    Sink = aHandler.ReadArgumentString("Sink");
                }
                
                public string Source;
                public string Sink;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConnectionManager iService;
        }
        
        
        // AsyncActionPrepareForConnection

        public class AsyncActionPrepareForConnection
        {
            internal AsyncActionPrepareForConnection(ServiceConnectionManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object PrepareForConnectionBeginSync(string RemoteProtocolInfo, string PeerConnectionManager, int PeerConnectionID, string Direction)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("RemoteProtocolInfo", RemoteProtocolInfo);           
                iHandler.WriteArgumentString("PeerConnectionManager", PeerConnectionManager);           
                iHandler.WriteArgumentInt("PeerConnectionID", PeerConnectionID);           
                iHandler.WriteArgumentString("Direction", Direction);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PrepareForConnectionBegin(string RemoteProtocolInfo, string PeerConnectionManager, int PeerConnectionID, string Direction)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("RemoteProtocolInfo", RemoteProtocolInfo);                
                iHandler.WriteArgumentString("PeerConnectionManager", PeerConnectionManager);                
                iHandler.WriteArgumentInt("PeerConnectionID", PeerConnectionID);                
                iHandler.WriteArgumentString("Direction", Direction);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ConnectionManager.AsyncActionPrepareForConnection.PrepareForConnectionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PrepareForConnectionEnd(object aResult)
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
                    UserLog.WriteLine("ConnectionManager.AsyncActionPrepareForConnection.PrepareForConnectionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ConnectionManager.AsyncActionPrepareForConnection.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ConnectionID = aHandler.ReadArgumentInt("ConnectionID");
                    AVTransportID = aHandler.ReadArgumentInt("AVTransportID");
                    RcsID = aHandler.ReadArgumentInt("RcsID");
                }
                
                public int ConnectionID;
                public int AVTransportID;
                public int RcsID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConnectionManager iService;
        }
        
        
        // AsyncActionConnectionComplete

        public class AsyncActionConnectionComplete
        {
            internal AsyncActionConnectionComplete(ServiceConnectionManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object ConnectionCompleteBeginSync(int ConnectionID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("ConnectionID", ConnectionID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ConnectionCompleteBegin(int ConnectionID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("ConnectionID", ConnectionID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ConnectionManager.AsyncActionConnectionComplete.ConnectionCompleteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ConnectionCompleteEnd(object aResult)
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
                    UserLog.WriteLine("ConnectionManager.AsyncActionConnectionComplete.ConnectionCompleteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ConnectionManager.AsyncActionConnectionComplete.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceConnectionManager iService;
        }
        
        
        // AsyncActionGetCurrentConnectionIDs

        public class AsyncActionGetCurrentConnectionIDs
        {
            internal AsyncActionGetCurrentConnectionIDs(ServiceConnectionManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object GetCurrentConnectionIDsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetCurrentConnectionIDsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionIDs.GetCurrentConnectionIDsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetCurrentConnectionIDsEnd(object aResult)
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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionIDs.GetCurrentConnectionIDsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionIDs.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ConnectionIDs = aHandler.ReadArgumentString("ConnectionIDs");
                }
                
                public string ConnectionIDs;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConnectionManager iService;
        }
        
        
        // AsyncActionGetCurrentConnectionInfo

        public class AsyncActionGetCurrentConnectionInfo
        {
            internal AsyncActionGetCurrentConnectionInfo(ServiceConnectionManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object GetCurrentConnectionInfoBeginSync(int ConnectionID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("ConnectionID", ConnectionID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetCurrentConnectionInfoBegin(int ConnectionID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("ConnectionID", ConnectionID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionInfo.GetCurrentConnectionInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetCurrentConnectionInfoEnd(object aResult)
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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionInfo.GetCurrentConnectionInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ConnectionManager.AsyncActionGetCurrentConnectionInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    RcsID = aHandler.ReadArgumentInt("RcsID");
                    AVTransportID = aHandler.ReadArgumentInt("AVTransportID");
                    ProtocolInfo = aHandler.ReadArgumentString("ProtocolInfo");
                    PeerConnectionManager = aHandler.ReadArgumentString("PeerConnectionManager");
                    PeerConnectionID = aHandler.ReadArgumentInt("PeerConnectionID");
                    Direction = aHandler.ReadArgumentString("Direction");
                    Status = aHandler.ReadArgumentString("Status");
                }
                
                public int RcsID;
                public int AVTransportID;
                public string ProtocolInfo;
                public string PeerConnectionManager;
                public int PeerConnectionID;
                public string Direction;
                public string Status;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConnectionManager iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceConnectionManager): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventSourceProtocolInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SourceProtocolInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SourceProtocolInfo = value;

                eventSourceProtocolInfo = true;
            }

            bool eventSinkProtocolInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SinkProtocolInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SinkProtocolInfo = value;

                eventSinkProtocolInfo = true;
            }

            bool eventCurrentConnectionIDs = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "CurrentConnectionIDs", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                CurrentConnectionIDs = value;

                eventCurrentConnectionIDs = true;
            }

          
            
            if(eventSourceProtocolInfo)
            {
                if (EventStateSourceProtocolInfo != null)
                {
					try
					{
						EventStateSourceProtocolInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSourceProtocolInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSinkProtocolInfo)
            {
                if (EventStateSinkProtocolInfo != null)
                {
					try
					{
						EventStateSinkProtocolInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSinkProtocolInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventCurrentConnectionIDs)
            {
                if (EventStateCurrentConnectionIDs != null)
                {
					try
					{
						EventStateCurrentConnectionIDs(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateCurrentConnectionIDs: " + ex);
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
        public event EventHandler<EventArgs> EventStateSourceProtocolInfo;
        public event EventHandler<EventArgs> EventStateSinkProtocolInfo;
        public event EventHandler<EventArgs> EventStateCurrentConnectionIDs;

        public string SourceProtocolInfo;
        public string SinkProtocolInfo;
        public string CurrentConnectionIDs;
    }
}
    
