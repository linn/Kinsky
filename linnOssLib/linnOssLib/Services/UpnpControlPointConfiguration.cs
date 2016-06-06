using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceConfiguration : ServiceUpnp
    {


        public ServiceConfiguration(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceConfiguration(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("ConfigurationXml");
            action.AddOutArgument(new Argument("aConfigurationXml", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ParameterXml");
            action.AddOutArgument(new Argument("aParameterXml", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetParameter");
            action.AddInArgument(new Argument("aTarget", Argument.EType.eString));
            action.AddInArgument(new Argument("aName", Argument.EType.eString));
            action.AddInArgument(new Argument("aValue", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Configuration", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Configuration", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionConfigurationXml CreateAsyncActionConfigurationXml()
        {
            return (new AsyncActionConfigurationXml(this));
        }

        public AsyncActionParameterXml CreateAsyncActionParameterXml()
        {
            return (new AsyncActionParameterXml(this));
        }

        public AsyncActionSetParameter CreateAsyncActionSetParameter()
        {
            return (new AsyncActionSetParameter(this));
        }


        // Synchronous actions
        
        public string ConfigurationXmlSync()
        {
            AsyncActionConfigurationXml action = CreateAsyncActionConfigurationXml();
            
            object result = action.ConfigurationXmlBeginSync();

            AsyncActionConfigurationXml.EventArgsResponse response = action.ConfigurationXmlEnd(result);
                
            return(response.aConfigurationXml);
        }
        
        public string ParameterXmlSync()
        {
            AsyncActionParameterXml action = CreateAsyncActionParameterXml();
            
            object result = action.ParameterXmlBeginSync();

            AsyncActionParameterXml.EventArgsResponse response = action.ParameterXmlEnd(result);
                
            return(response.aParameterXml);
        }
        
        public void SetParameterSync(string aTarget, string aName, string aValue)
        {
            AsyncActionSetParameter action = CreateAsyncActionSetParameter();
            
            object result = action.SetParameterBeginSync(aTarget, aName, aValue);

            action.SetParameterEnd(result);
        }
        

        // AsyncActionConfigurationXml

        public class AsyncActionConfigurationXml
        {
            internal AsyncActionConfigurationXml(ServiceConfiguration aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object ConfigurationXmlBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ConfigurationXmlBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Configuration.AsyncActionConfigurationXml.ConfigurationXmlBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ConfigurationXmlEnd(object aResult)
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
                    UserLog.WriteLine("Configuration.AsyncActionConfigurationXml.ConfigurationXmlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Configuration.AsyncActionConfigurationXml.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aConfigurationXml = aHandler.ReadArgumentString("aConfigurationXml");
                }
                
                public string aConfigurationXml;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConfiguration iService;
        }
        
        
        // AsyncActionParameterXml

        public class AsyncActionParameterXml
        {
            internal AsyncActionParameterXml(ServiceConfiguration aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object ParameterXmlBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ParameterXmlBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Configuration.AsyncActionParameterXml.ParameterXmlBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ParameterXmlEnd(object aResult)
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
                    UserLog.WriteLine("Configuration.AsyncActionParameterXml.ParameterXmlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Configuration.AsyncActionParameterXml.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aParameterXml = aHandler.ReadArgumentString("aParameterXml");
                }
                
                public string aParameterXml;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceConfiguration iService;
        }
        
        
        // AsyncActionSetParameter

        public class AsyncActionSetParameter
        {
            internal AsyncActionSetParameter(ServiceConfiguration aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetParameterBeginSync(string aTarget, string aName, string aValue)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aTarget", aTarget);           
                iHandler.WriteArgumentString("aName", aName);           
                iHandler.WriteArgumentString("aValue", aValue);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetParameterBegin(string aTarget, string aName, string aValue)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aTarget", aTarget);                
                iHandler.WriteArgumentString("aName", aName);                
                iHandler.WriteArgumentString("aValue", aValue);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Configuration.AsyncActionSetParameter.SetParameterBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetParameterEnd(object aResult)
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
                    UserLog.WriteLine("Configuration.AsyncActionSetParameter.SetParameterEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Configuration.AsyncActionSetParameter.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceConfiguration iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceConfiguration): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventConfigurationXml = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ConfigurationXml", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ConfigurationXml = value;

                eventConfigurationXml = true;
            }

            bool eventParameterXml = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ParameterXml", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ParameterXml = value;

                eventParameterXml = true;
            }

          
            
            if(eventConfigurationXml)
            {
                if (EventStateConfigurationXml != null)
                {
					try
					{
						EventStateConfigurationXml(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateConfigurationXml: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventParameterXml)
            {
                if (EventStateParameterXml != null)
                {
					try
					{
						EventStateParameterXml(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateParameterXml: " + ex);
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
        public event EventHandler<EventArgs> EventStateConfigurationXml;
        public event EventHandler<EventArgs> EventStateParameterXml;

        public string ConfigurationXml;
        public string ParameterXml;
    }
}
    
