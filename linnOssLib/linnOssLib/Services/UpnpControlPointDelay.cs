using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceDelay : ServiceUpnp
    {


        public ServiceDelay(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceDelay(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetDelay");
            action.AddInArgument(new Argument("aDelay", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Delay");
            action.AddOutArgument(new Argument("aDelay", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DelayMinimum");
            action.AddOutArgument(new Argument("aDelay", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DelayMaximum");
            action.AddOutArgument(new Argument("aDelay", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PresetCount");
            action.AddOutArgument(new Argument("aCount", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PresetIndex");
            action.AddOutArgument(new Argument("aIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetPresetIndex");
            action.AddInArgument(new Argument("aIndex", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Delay", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Delay", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetDelay CreateAsyncActionSetDelay()
        {
            return (new AsyncActionSetDelay(this));
        }

        public AsyncActionDelay CreateAsyncActionDelay()
        {
            return (new AsyncActionDelay(this));
        }

        public AsyncActionDelayMinimum CreateAsyncActionDelayMinimum()
        {
            return (new AsyncActionDelayMinimum(this));
        }

        public AsyncActionDelayMaximum CreateAsyncActionDelayMaximum()
        {
            return (new AsyncActionDelayMaximum(this));
        }

        public AsyncActionPresetCount CreateAsyncActionPresetCount()
        {
            return (new AsyncActionPresetCount(this));
        }

        public AsyncActionPresetIndex CreateAsyncActionPresetIndex()
        {
            return (new AsyncActionPresetIndex(this));
        }

        public AsyncActionSetPresetIndex CreateAsyncActionSetPresetIndex()
        {
            return (new AsyncActionSetPresetIndex(this));
        }


        // Synchronous actions
        
        public void SetDelaySync(uint aDelay)
        {
            AsyncActionSetDelay action = CreateAsyncActionSetDelay();
            
            object result = action.SetDelayBeginSync(aDelay);

            action.SetDelayEnd(result);
        }
        
        public uint DelaySync()
        {
            AsyncActionDelay action = CreateAsyncActionDelay();
            
            object result = action.DelayBeginSync();

            AsyncActionDelay.EventArgsResponse response = action.DelayEnd(result);
                
            return(response.aDelay);
        }
        
        public uint DelayMinimumSync()
        {
            AsyncActionDelayMinimum action = CreateAsyncActionDelayMinimum();
            
            object result = action.DelayMinimumBeginSync();

            AsyncActionDelayMinimum.EventArgsResponse response = action.DelayMinimumEnd(result);
                
            return(response.aDelay);
        }
        
        public uint DelayMaximumSync()
        {
            AsyncActionDelayMaximum action = CreateAsyncActionDelayMaximum();
            
            object result = action.DelayMaximumBeginSync();

            AsyncActionDelayMaximum.EventArgsResponse response = action.DelayMaximumEnd(result);
                
            return(response.aDelay);
        }
        
        public uint PresetCountSync()
        {
            AsyncActionPresetCount action = CreateAsyncActionPresetCount();
            
            object result = action.PresetCountBeginSync();

            AsyncActionPresetCount.EventArgsResponse response = action.PresetCountEnd(result);
                
            return(response.aCount);
        }
        
        public uint PresetIndexSync()
        {
            AsyncActionPresetIndex action = CreateAsyncActionPresetIndex();
            
            object result = action.PresetIndexBeginSync();

            AsyncActionPresetIndex.EventArgsResponse response = action.PresetIndexEnd(result);
                
            return(response.aIndex);
        }
        
        public void SetPresetIndexSync(uint aIndex)
        {
            AsyncActionSetPresetIndex action = CreateAsyncActionSetPresetIndex();
            
            object result = action.SetPresetIndexBeginSync(aIndex);

            action.SetPresetIndexEnd(result);
        }
        

        // AsyncActionSetDelay

        public class AsyncActionSetDelay
        {
            internal AsyncActionSetDelay(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetDelayBeginSync(uint aDelay)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDelay", aDelay);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetDelayBegin(uint aDelay)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDelay", aDelay);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionSetDelay.SetDelayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetDelayEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionSetDelay.SetDelayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionSetDelay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDelay iService;
        }
        
        
        // AsyncActionDelay

        public class AsyncActionDelay
        {
            internal AsyncActionDelay(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object DelayBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DelayBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionDelay.DelayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DelayEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionDelay.DelayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionDelay.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDelay = aHandler.ReadArgumentUint("aDelay");
                }
                
                public uint aDelay;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDelay iService;
        }
        
        
        // AsyncActionDelayMinimum

        public class AsyncActionDelayMinimum
        {
            internal AsyncActionDelayMinimum(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object DelayMinimumBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DelayMinimumBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionDelayMinimum.DelayMinimumBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DelayMinimumEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionDelayMinimum.DelayMinimumEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionDelayMinimum.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDelay = aHandler.ReadArgumentUint("aDelay");
                }
                
                public uint aDelay;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDelay iService;
        }
        
        
        // AsyncActionDelayMaximum

        public class AsyncActionDelayMaximum
        {
            internal AsyncActionDelayMaximum(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object DelayMaximumBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DelayMaximumBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionDelayMaximum.DelayMaximumBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DelayMaximumEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionDelayMaximum.DelayMaximumEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionDelayMaximum.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDelay = aHandler.ReadArgumentUint("aDelay");
                }
                
                public uint aDelay;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDelay iService;
        }
        
        
        // AsyncActionPresetCount

        public class AsyncActionPresetCount
        {
            internal AsyncActionPresetCount(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object PresetCountBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PresetCountBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionPresetCount.PresetCountBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PresetCountEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionPresetCount.PresetCountEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionPresetCount.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aCount = aHandler.ReadArgumentUint("aCount");
                }
                
                public uint aCount;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDelay iService;
        }
        
        
        // AsyncActionPresetIndex

        public class AsyncActionPresetIndex
        {
            internal AsyncActionPresetIndex(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object PresetIndexBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PresetIndexBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionPresetIndex.PresetIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PresetIndexEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionPresetIndex.PresetIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionPresetIndex.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aIndex = aHandler.ReadArgumentUint("aIndex");
                }
                
                public uint aIndex;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDelay iService;
        }
        
        
        // AsyncActionSetPresetIndex

        public class AsyncActionSetPresetIndex
        {
            internal AsyncActionSetPresetIndex(ServiceDelay aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SetPresetIndexBeginSync(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetPresetIndexBegin(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Delay.AsyncActionSetPresetIndex.SetPresetIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetPresetIndexEnd(object aResult)
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
                    UserLog.WriteLine("Delay.AsyncActionSetPresetIndex.SetPresetIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Delay.AsyncActionSetPresetIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDelay iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceDelay): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventDelay = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Delay", nsmanager);

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
					Delay = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Delay with value {1}", DateTime.Now, value));
				}

                eventDelay = true;
            }

            bool eventDelayMinimum = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DelayMinimum", nsmanager);

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
					DelayMinimum = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse DelayMinimum with value {1}", DateTime.Now, value));
				}

                eventDelayMinimum = true;
            }

            bool eventDelayMaximum = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DelayMaximum", nsmanager);

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
					DelayMaximum = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse DelayMaximum with value {1}", DateTime.Now, value));
				}

                eventDelayMaximum = true;
            }

            bool eventPresetCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PresetCount", nsmanager);

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
					PresetCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse PresetCount with value {1}", DateTime.Now, value));
				}

                eventPresetCount = true;
            }

            bool eventPresetIndex = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PresetIndex", nsmanager);

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
					PresetIndex = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse PresetIndex with value {1}", DateTime.Now, value));
				}

                eventPresetIndex = true;
            }

          
            
            if(eventDelay)
            {
                if (EventStateDelay != null)
                {
					try
					{
						EventStateDelay(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDelay: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDelayMinimum)
            {
                if (EventStateDelayMinimum != null)
                {
					try
					{
						EventStateDelayMinimum(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDelayMinimum: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDelayMaximum)
            {
                if (EventStateDelayMaximum != null)
                {
					try
					{
						EventStateDelayMaximum(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDelayMaximum: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventPresetCount)
            {
                if (EventStatePresetCount != null)
                {
					try
					{
						EventStatePresetCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePresetCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventPresetIndex)
            {
                if (EventStatePresetIndex != null)
                {
					try
					{
						EventStatePresetIndex(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePresetIndex: " + ex);
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
        public event EventHandler<EventArgs> EventStateDelay;
        public event EventHandler<EventArgs> EventStateDelayMinimum;
        public event EventHandler<EventArgs> EventStateDelayMaximum;
        public event EventHandler<EventArgs> EventStatePresetCount;
        public event EventHandler<EventArgs> EventStatePresetIndex;

        public uint Delay;
        public uint DelayMinimum;
        public uint DelayMaximum;
        public uint PresetCount;
        public uint PresetIndex;
    }
}
    
