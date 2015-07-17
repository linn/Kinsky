using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceSoundcard : ServiceUpnp
    {

        public const string kTransportStateStopped = "Stopped";
        public const string kTransportStatePlaying = "Playing";
        public const string kTransportStateBuffering = "Buffering";

        public ServiceSoundcard(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceSoundcard(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetChannel");
            action.AddInArgument(new Argument("Uri", Argument.EType.eString));
            action.AddInArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Stop");
            iActions.Add(action);
            
            action = new Action("Play");
            iActions.Add(action);
            
            action = new Action("Channel");
            action.AddOutArgument(new Argument("Uri", Argument.EType.eString));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ProtocolInfo");
            action.AddOutArgument(new Argument("Info", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TransportState");
            action.AddOutArgument(new Argument("State", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Soundcard", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Soundcard", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetChannel CreateAsyncActionSetChannel()
        {
            return (new AsyncActionSetChannel(this));
        }

        public AsyncActionStop CreateAsyncActionStop()
        {
            return (new AsyncActionStop(this));
        }

        public AsyncActionPlay CreateAsyncActionPlay()
        {
            return (new AsyncActionPlay(this));
        }

        public AsyncActionChannel CreateAsyncActionChannel()
        {
            return (new AsyncActionChannel(this));
        }

        public AsyncActionProtocolInfo CreateAsyncActionProtocolInfo()
        {
            return (new AsyncActionProtocolInfo(this));
        }

        public AsyncActionTransportState CreateAsyncActionTransportState()
        {
            return (new AsyncActionTransportState(this));
        }


        // Synchronous actions
        
        public void SetChannelSync(string Uri, string Metadata)
        {
            AsyncActionSetChannel action = CreateAsyncActionSetChannel();
            
            object result = action.SetChannelBeginSync(Uri, Metadata);

            action.SetChannelEnd(result);
        }
        
        public void StopSync()
        {
            AsyncActionStop action = CreateAsyncActionStop();
            
            object result = action.StopBeginSync();

            action.StopEnd(result);
        }
        
        public void PlaySync()
        {
            AsyncActionPlay action = CreateAsyncActionPlay();
            
            object result = action.PlayBeginSync();

            action.PlayEnd(result);
        }
        
        public void ChannelSync(out string Uri, out string Metadata)
        {
            AsyncActionChannel action = CreateAsyncActionChannel();
            
            object result = action.ChannelBeginSync();

            AsyncActionChannel.EventArgsResponse response = action.ChannelEnd(result);
                
            Uri = response.Uri;
            Metadata = response.Metadata;
        }
        
        public string ProtocolInfoSync()
        {
            AsyncActionProtocolInfo action = CreateAsyncActionProtocolInfo();
            
            object result = action.ProtocolInfoBeginSync();

            AsyncActionProtocolInfo.EventArgsResponse response = action.ProtocolInfoEnd(result);
                
            return(response.Info);
        }
        
        public string TransportStateSync()
        {
            AsyncActionTransportState action = CreateAsyncActionTransportState();
            
            object result = action.TransportStateBeginSync();

            AsyncActionTransportState.EventArgsResponse response = action.TransportStateEnd(result);
                
            return(response.State);
        }
        

        // AsyncActionSetChannel

        public class AsyncActionSetChannel
        {
            internal AsyncActionSetChannel(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetChannelBeginSync(string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Uri", Uri);           
                iHandler.WriteArgumentString("Metadata", Metadata);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetChannelBegin(string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Uri", Uri);                
                iHandler.WriteArgumentString("Metadata", Metadata);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionSetChannel.SetChannelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetChannelEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionSetChannel.SetChannelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionSetChannel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSoundcard iService;
        }
        
        
        // AsyncActionStop

        public class AsyncActionStop
        {
            internal AsyncActionStop(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object StopBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StopBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionStop.StopBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StopEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionStop.StopEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionStop.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSoundcard iService;
        }
        
        
        // AsyncActionPlay

        public class AsyncActionPlay
        {
            internal AsyncActionPlay(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object PlayBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlayBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionPlay.PlayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlayEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionPlay.PlayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionPlay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSoundcard iService;
        }
        
        
        // AsyncActionChannel

        public class AsyncActionChannel
        {
            internal AsyncActionChannel(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object ChannelBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ChannelBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionChannel.ChannelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ChannelEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionChannel.ChannelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionChannel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Uri = aHandler.ReadArgumentString("Uri");
                    Metadata = aHandler.ReadArgumentString("Metadata");
                }
                
                public string Uri;
                public string Metadata;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSoundcard iService;
        }
        
        
        // AsyncActionProtocolInfo

        public class AsyncActionProtocolInfo
        {
            internal AsyncActionProtocolInfo(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object ProtocolInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ProtocolInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionProtocolInfo.ProtocolInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ProtocolInfoEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionProtocolInfo.ProtocolInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionProtocolInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Info = aHandler.ReadArgumentString("Info");
                }
                
                public string Info;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSoundcard iService;
        }
        
        
        // AsyncActionTransportState

        public class AsyncActionTransportState
        {
            internal AsyncActionTransportState(ServiceSoundcard aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object TransportStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TransportStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Soundcard.AsyncActionTransportState.TransportStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TransportStateEnd(object aResult)
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
                    UserLog.WriteLine("Soundcard.AsyncActionTransportState.TransportStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Soundcard.AsyncActionTransportState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    State = aHandler.ReadArgumentString("State");
                }
                
                public string State;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSoundcard iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceSoundcard): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventChannelUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ChannelUri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ChannelUri = value;

                eventChannelUri = true;
            }

            bool eventChannelMetadata = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ChannelMetadata", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ChannelMetadata = value;

                eventChannelMetadata = true;
            }

            bool eventTransportState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TransportState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TransportState = value;

                eventTransportState = true;
            }

            bool eventProtocolInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProtocolInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProtocolInfo = value;

                eventProtocolInfo = true;
            }

          
            
            if(eventChannelUri)
            {
                if (EventStateChannelUri != null)
                {
					try
					{
						EventStateChannelUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateChannelUri: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventChannelMetadata)
            {
                if (EventStateChannelMetadata != null)
                {
					try
					{
						EventStateChannelMetadata(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateChannelMetadata: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTransportState)
            {
                if (EventStateTransportState != null)
                {
					try
					{
						EventStateTransportState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTransportState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProtocolInfo)
            {
                if (EventStateProtocolInfo != null)
                {
					try
					{
						EventStateProtocolInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProtocolInfo: " + ex);
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
        public event EventHandler<EventArgs> EventStateChannelUri;
        public event EventHandler<EventArgs> EventStateChannelMetadata;
        public event EventHandler<EventArgs> EventStateTransportState;
        public event EventHandler<EventArgs> EventStateProtocolInfo;

        public string ChannelUri;
        public string ChannelMetadata;
        public string TransportState;
        public string ProtocolInfo;
    }
}
    
