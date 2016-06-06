using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceSender : ServiceUpnp
    {

        public const string kStatusEnabled = "Enabled";
        public const string kStatusDisabled = "Disabled";
        public const string kStatusBlocked = "Blocked";

        public ServiceSender(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceSender(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("PresentationUrl");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Metadata");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Audio");
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Status");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Attributes");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Sender", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Sender", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionPresentationUrl CreateAsyncActionPresentationUrl()
        {
            return (new AsyncActionPresentationUrl(this));
        }

        public AsyncActionMetadata CreateAsyncActionMetadata()
        {
            return (new AsyncActionMetadata(this));
        }

        public AsyncActionAudio CreateAsyncActionAudio()
        {
            return (new AsyncActionAudio(this));
        }

        public AsyncActionStatus CreateAsyncActionStatus()
        {
            return (new AsyncActionStatus(this));
        }

        public AsyncActionAttributes CreateAsyncActionAttributes()
        {
            return (new AsyncActionAttributes(this));
        }


        // Synchronous actions
        
        public string PresentationUrlSync()
        {
            AsyncActionPresentationUrl action = CreateAsyncActionPresentationUrl();
            
            object result = action.PresentationUrlBeginSync();

            AsyncActionPresentationUrl.EventArgsResponse response = action.PresentationUrlEnd(result);
                
            return(response.Value);
        }
        
        public string MetadataSync()
        {
            AsyncActionMetadata action = CreateAsyncActionMetadata();
            
            object result = action.MetadataBeginSync();

            AsyncActionMetadata.EventArgsResponse response = action.MetadataEnd(result);
                
            return(response.Value);
        }
        
        public bool AudioSync()
        {
            AsyncActionAudio action = CreateAsyncActionAudio();
            
            object result = action.AudioBeginSync();

            AsyncActionAudio.EventArgsResponse response = action.AudioEnd(result);
                
            return(response.Value);
        }
        
        public string StatusSync()
        {
            AsyncActionStatus action = CreateAsyncActionStatus();
            
            object result = action.StatusBeginSync();

            AsyncActionStatus.EventArgsResponse response = action.StatusEnd(result);
                
            return(response.Value);
        }
        
        public string AttributesSync()
        {
            AsyncActionAttributes action = CreateAsyncActionAttributes();
            
            object result = action.AttributesBeginSync();

            AsyncActionAttributes.EventArgsResponse response = action.AttributesEnd(result);
                
            return(response.Value);
        }
        

        // AsyncActionPresentationUrl

        public class AsyncActionPresentationUrl
        {
            internal AsyncActionPresentationUrl(ServiceSender aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object PresentationUrlBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PresentationUrlBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sender.AsyncActionPresentationUrl.PresentationUrlBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PresentationUrlEnd(object aResult)
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
                    UserLog.WriteLine("Sender.AsyncActionPresentationUrl.PresentationUrlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sender.AsyncActionPresentationUrl.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentString("Value");
                }
                
                public string Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSender iService;
        }
        
        
        // AsyncActionMetadata

        public class AsyncActionMetadata
        {
            internal AsyncActionMetadata(ServiceSender aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object MetadataBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MetadataBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sender.AsyncActionMetadata.MetadataBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MetadataEnd(object aResult)
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
                    UserLog.WriteLine("Sender.AsyncActionMetadata.MetadataEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sender.AsyncActionMetadata.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentString("Value");
                }
                
                public string Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSender iService;
        }
        
        
        // AsyncActionAudio

        public class AsyncActionAudio
        {
            internal AsyncActionAudio(ServiceSender aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object AudioBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AudioBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sender.AsyncActionAudio.AudioBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AudioEnd(object aResult)
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
                    UserLog.WriteLine("Sender.AsyncActionAudio.AudioEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sender.AsyncActionAudio.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentBool("Value");
                }
                
                public bool Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSender iService;
        }
        
        
        // AsyncActionStatus

        public class AsyncActionStatus
        {
            internal AsyncActionStatus(ServiceSender aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object StatusBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StatusBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sender.AsyncActionStatus.StatusBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StatusEnd(object aResult)
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
                    UserLog.WriteLine("Sender.AsyncActionStatus.StatusEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sender.AsyncActionStatus.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentString("Value");
                }
                
                public string Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSender iService;
        }
        
        
        // AsyncActionAttributes

        public class AsyncActionAttributes
        {
            internal AsyncActionAttributes(ServiceSender aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object AttributesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AttributesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sender.AsyncActionAttributes.AttributesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AttributesEnd(object aResult)
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
                    UserLog.WriteLine("Sender.AsyncActionAttributes.AttributesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sender.AsyncActionAttributes.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentString("Value");
                }
                
                public string Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSender iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceSender): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventPresentationUrl = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PresentationUrl", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                PresentationUrl = value;

                eventPresentationUrl = true;
            }

            bool eventMetadata = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Metadata", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Metadata = value;

                eventMetadata = true;
            }

            bool eventAudio = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Audio", nsmanager);

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
	                Audio = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Audio = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Audio = false; 
    	            }
                }

                eventAudio = true;
            }

            bool eventStatus = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Status", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Status = value;

                eventStatus = true;
            }

            bool eventAttributes = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Attributes", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Attributes = value;

                eventAttributes = true;
            }

          
            
            if(eventPresentationUrl)
            {
                if (EventStatePresentationUrl != null)
                {
					try
					{
						EventStatePresentationUrl(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePresentationUrl: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventMetadata)
            {
                if (EventStateMetadata != null)
                {
					try
					{
						EventStateMetadata(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateMetadata: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAudio)
            {
                if (EventStateAudio != null)
                {
					try
					{
						EventStateAudio(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAudio: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventStatus)
            {
                if (EventStateStatus != null)
                {
					try
					{
						EventStateStatus(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateStatus: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAttributes)
            {
                if (EventStateAttributes != null)
                {
					try
					{
						EventStateAttributes(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAttributes: " + ex);
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
        public event EventHandler<EventArgs> EventStatePresentationUrl;
        public event EventHandler<EventArgs> EventStateMetadata;
        public event EventHandler<EventArgs> EventStateAudio;
        public event EventHandler<EventArgs> EventStateStatus;
        public event EventHandler<EventArgs> EventStateAttributes;

        public string PresentationUrl;
        public string Metadata;
        public bool Audio;
        public string Status;
        public string Attributes;
    }
}
    
