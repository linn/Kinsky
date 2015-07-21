using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceExakt : ServiceUpnp
    {


        public ServiceExakt(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceExakt(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("DeviceList");
            action.AddOutArgument(new Argument("List", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DeviceSettings");
            action.AddInArgument(new Argument("DeviceId", Argument.EType.eString));
            action.AddOutArgument(new Argument("Settings", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ConnectionStatus");
            action.AddOutArgument(new Argument("ConnectionStatus", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Set");
            action.AddInArgument(new Argument("DeviceId", Argument.EType.eString));
            action.AddInArgument(new Argument("BankId", Argument.EType.eUint));
            action.AddInArgument(new Argument("FileUri", Argument.EType.eString));
            action.AddInArgument(new Argument("Mute", Argument.EType.eBool));
            action.AddInArgument(new Argument("Persist", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Reprogram");
            action.AddInArgument(new Argument("DeviceId", Argument.EType.eString));
            action.AddInArgument(new Argument("FileUri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ReprogramFallback");
            action.AddInArgument(new Argument("DeviceId", Argument.EType.eString));
            action.AddInArgument(new Argument("FileUri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Version");
            action.AddOutArgument(new Argument("Version", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Exakt", 2));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Exakt", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionDeviceList CreateAsyncActionDeviceList()
        {
            return (new AsyncActionDeviceList(this));
        }

        public AsyncActionDeviceSettings CreateAsyncActionDeviceSettings()
        {
            return (new AsyncActionDeviceSettings(this));
        }

        public AsyncActionConnectionStatus CreateAsyncActionConnectionStatus()
        {
            return (new AsyncActionConnectionStatus(this));
        }

        public AsyncActionSet CreateAsyncActionSet()
        {
            return (new AsyncActionSet(this));
        }

        public AsyncActionReprogram CreateAsyncActionReprogram()
        {
            return (new AsyncActionReprogram(this));
        }

        public AsyncActionReprogramFallback CreateAsyncActionReprogramFallback()
        {
            return (new AsyncActionReprogramFallback(this));
        }

        public AsyncActionVersion CreateAsyncActionVersion()
        {
            return (new AsyncActionVersion(this));
        }


        // Synchronous actions
        
        public string DeviceListSync()
        {
            AsyncActionDeviceList action = CreateAsyncActionDeviceList();
            
            object result = action.DeviceListBeginSync();

            AsyncActionDeviceList.EventArgsResponse response = action.DeviceListEnd(result);
                
            return(response.List);
        }
        
        public string DeviceSettingsSync(string DeviceId)
        {
            AsyncActionDeviceSettings action = CreateAsyncActionDeviceSettings();
            
            object result = action.DeviceSettingsBeginSync(DeviceId);

            AsyncActionDeviceSettings.EventArgsResponse response = action.DeviceSettingsEnd(result);
                
            return(response.Settings);
        }
        
        public string ConnectionStatusSync()
        {
            AsyncActionConnectionStatus action = CreateAsyncActionConnectionStatus();
            
            object result = action.ConnectionStatusBeginSync();

            AsyncActionConnectionStatus.EventArgsResponse response = action.ConnectionStatusEnd(result);
                
            return(response.ConnectionStatus);
        }
        
        public void SetSync(string DeviceId, uint BankId, string FileUri, bool Mute, bool Persist)
        {
            AsyncActionSet action = CreateAsyncActionSet();
            
            object result = action.SetBeginSync(DeviceId, BankId, FileUri, Mute, Persist);

            action.SetEnd(result);
        }
        
        public void ReprogramSync(string DeviceId, string FileUri)
        {
            AsyncActionReprogram action = CreateAsyncActionReprogram();
            
            object result = action.ReprogramBeginSync(DeviceId, FileUri);

            action.ReprogramEnd(result);
        }
        
        public void ReprogramFallbackSync(string DeviceId, string FileUri)
        {
            AsyncActionReprogramFallback action = CreateAsyncActionReprogramFallback();
            
            object result = action.ReprogramFallbackBeginSync(DeviceId, FileUri);

            action.ReprogramFallbackEnd(result);
        }
        
        public string VersionSync()
        {
            AsyncActionVersion action = CreateAsyncActionVersion();
            
            object result = action.VersionBeginSync();

            AsyncActionVersion.EventArgsResponse response = action.VersionEnd(result);
                
            return(response.Version);
        }
        

        // AsyncActionDeviceList

        public class AsyncActionDeviceList
        {
            internal AsyncActionDeviceList(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object DeviceListBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DeviceListBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionDeviceList.DeviceListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeviceListEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionDeviceList.DeviceListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionDeviceList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    List = aHandler.ReadArgumentString("List");
                }
                
                public string List;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceExakt iService;
        }
        
        
        // AsyncActionDeviceSettings

        public class AsyncActionDeviceSettings
        {
            internal AsyncActionDeviceSettings(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object DeviceSettingsBeginSync(string DeviceId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DeviceSettingsBegin(string DeviceId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionDeviceSettings.DeviceSettingsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeviceSettingsEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionDeviceSettings.DeviceSettingsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionDeviceSettings.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Settings = aHandler.ReadArgumentString("Settings");
                }
                
                public string Settings;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceExakt iService;
        }
        
        
        // AsyncActionConnectionStatus

        public class AsyncActionConnectionStatus
        {
            internal AsyncActionConnectionStatus(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object ConnectionStatusBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ConnectionStatusBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionConnectionStatus.ConnectionStatusBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ConnectionStatusEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionConnectionStatus.ConnectionStatusEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionConnectionStatus.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ConnectionStatus = aHandler.ReadArgumentString("ConnectionStatus");
                }
                
                public string ConnectionStatus;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceExakt iService;
        }
        
        
        // AsyncActionSet

        public class AsyncActionSet
        {
            internal AsyncActionSet(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SetBeginSync(string DeviceId, uint BankId, string FileUri, bool Mute, bool Persist)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);           
                iHandler.WriteArgumentUint("BankId", BankId);           
                iHandler.WriteArgumentString("FileUri", FileUri);           
                iHandler.WriteArgumentBool("Mute", Mute);           
                iHandler.WriteArgumentBool("Persist", Persist);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBegin(string DeviceId, uint BankId, string FileUri, bool Mute, bool Persist)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);                
                iHandler.WriteArgumentUint("BankId", BankId);                
                iHandler.WriteArgumentString("FileUri", FileUri);                
                iHandler.WriteArgumentBool("Mute", Mute);                
                iHandler.WriteArgumentBool("Persist", Persist);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionSet.SetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionSet.SetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionSet.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceExakt iService;
        }
        
        
        // AsyncActionReprogram

        public class AsyncActionReprogram
        {
            internal AsyncActionReprogram(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object ReprogramBeginSync(string DeviceId, string FileUri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);           
                iHandler.WriteArgumentString("FileUri", FileUri);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReprogramBegin(string DeviceId, string FileUri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);                
                iHandler.WriteArgumentString("FileUri", FileUri);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionReprogram.ReprogramBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ReprogramEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionReprogram.ReprogramEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionReprogram.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceExakt iService;
        }
        
        
        // AsyncActionReprogramFallback

        public class AsyncActionReprogramFallback
        {
            internal AsyncActionReprogramFallback(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object ReprogramFallbackBeginSync(string DeviceId, string FileUri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);           
                iHandler.WriteArgumentString("FileUri", FileUri);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReprogramFallbackBegin(string DeviceId, string FileUri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DeviceId", DeviceId);                
                iHandler.WriteArgumentString("FileUri", FileUri);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionReprogramFallback.ReprogramFallbackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ReprogramFallbackEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionReprogramFallback.ReprogramFallbackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionReprogramFallback.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceExakt iService;
        }
        
        
        // AsyncActionVersion

        public class AsyncActionVersion
        {
            internal AsyncActionVersion(ServiceExakt aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object VersionBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void VersionBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Exakt.AsyncActionVersion.VersionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse VersionEnd(object aResult)
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
                    UserLog.WriteLine("Exakt.AsyncActionVersion.VersionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Exakt.AsyncActionVersion.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Version = aHandler.ReadArgumentString("Version");
                }
                
                public string Version;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceExakt iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceExakt): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventDeviceList = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DeviceList", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                DeviceList = value;

                eventDeviceList = true;
            }

            bool eventConnectionStatus = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ConnectionStatus", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ConnectionStatus = value;

                eventConnectionStatus = true;
            }

            bool eventVersion = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Version", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Version = value;

                eventVersion = true;
            }

          
            
            if(eventDeviceList)
            {
                if (EventStateDeviceList != null)
                {
					try
					{
						EventStateDeviceList(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDeviceList: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventConnectionStatus)
            {
                if (EventStateConnectionStatus != null)
                {
					try
					{
						EventStateConnectionStatus(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateConnectionStatus: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVersion)
            {
                if (EventStateVersion != null)
                {
					try
					{
						EventStateVersion(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVersion: " + ex);
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
        public event EventHandler<EventArgs> EventStateDeviceList;
        public event EventHandler<EventArgs> EventStateConnectionStatus;
        public event EventHandler<EventArgs> EventStateVersion;

        public string DeviceList;
        public string ConnectionStatus;
        public string Version;
    }
}
    
