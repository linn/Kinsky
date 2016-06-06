using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceDeviceProperties : ServiceUpnp
    {

        public const string kLEDStateOn = "On";
        public const string kLEDStateOff = "Off";

        public ServiceDeviceProperties(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceDeviceProperties(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetLEDState");
            action.AddInArgument(new Argument("DesiredLEDState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetLEDState");
            action.AddOutArgument(new Argument("CurrentLEDState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetInvisible");
            action.AddInArgument(new Argument("DesiredInvisible", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("GetInvisible");
            action.AddOutArgument(new Argument("CurrentInvisible", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetZoneAttributes");
            action.AddInArgument(new Argument("DesiredZoneName", Argument.EType.eString));
            action.AddInArgument(new Argument("DesiredIcon", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetZoneAttributes");
            action.AddOutArgument(new Argument("CurrentZoneName", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentIcon", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetHouseholdID");
            action.AddOutArgument(new Argument("CurrentHouseholdID", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetZoneInfo");
            action.AddOutArgument(new Argument("SerialNumber", Argument.EType.eString));
            action.AddOutArgument(new Argument("SoftwareVersion", Argument.EType.eString));
            action.AddOutArgument(new Argument("DisplaySoftwareVersion", Argument.EType.eString));
            action.AddOutArgument(new Argument("HardwareVersion", Argument.EType.eString));
            action.AddOutArgument(new Argument("IPAddress", Argument.EType.eString));
            action.AddOutArgument(new Argument("MACAddress", Argument.EType.eString));
            action.AddOutArgument(new Argument("CopyrightInfo", Argument.EType.eString));
            action.AddOutArgument(new Argument("ExtraInfo", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("upnp.org", "DeviceProperties", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("upnp.org", "DeviceProperties", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetLEDState CreateAsyncActionSetLEDState()
        {
            return (new AsyncActionSetLEDState(this));
        }

        public AsyncActionGetLEDState CreateAsyncActionGetLEDState()
        {
            return (new AsyncActionGetLEDState(this));
        }

        public AsyncActionSetInvisible CreateAsyncActionSetInvisible()
        {
            return (new AsyncActionSetInvisible(this));
        }

        public AsyncActionGetInvisible CreateAsyncActionGetInvisible()
        {
            return (new AsyncActionGetInvisible(this));
        }

        public AsyncActionSetZoneAttributes CreateAsyncActionSetZoneAttributes()
        {
            return (new AsyncActionSetZoneAttributes(this));
        }

        public AsyncActionGetZoneAttributes CreateAsyncActionGetZoneAttributes()
        {
            return (new AsyncActionGetZoneAttributes(this));
        }

        public AsyncActionGetHouseholdID CreateAsyncActionGetHouseholdID()
        {
            return (new AsyncActionGetHouseholdID(this));
        }

        public AsyncActionGetZoneInfo CreateAsyncActionGetZoneInfo()
        {
            return (new AsyncActionGetZoneInfo(this));
        }


        // Synchronous actions
        
        public void SetLEDStateSync(string DesiredLEDState)
        {
            AsyncActionSetLEDState action = CreateAsyncActionSetLEDState();
            
            object result = action.SetLEDStateBeginSync(DesiredLEDState);

            action.SetLEDStateEnd(result);
        }
        
        public string GetLEDStateSync()
        {
            AsyncActionGetLEDState action = CreateAsyncActionGetLEDState();
            
            object result = action.GetLEDStateBeginSync();

            AsyncActionGetLEDState.EventArgsResponse response = action.GetLEDStateEnd(result);
                
            return(response.CurrentLEDState);
        }
        
        public void SetInvisibleSync(bool DesiredInvisible)
        {
            AsyncActionSetInvisible action = CreateAsyncActionSetInvisible();
            
            object result = action.SetInvisibleBeginSync(DesiredInvisible);

            action.SetInvisibleEnd(result);
        }
        
        public bool GetInvisibleSync()
        {
            AsyncActionGetInvisible action = CreateAsyncActionGetInvisible();
            
            object result = action.GetInvisibleBeginSync();

            AsyncActionGetInvisible.EventArgsResponse response = action.GetInvisibleEnd(result);
                
            return(response.CurrentInvisible);
        }
        
        public void SetZoneAttributesSync(string DesiredZoneName, string DesiredIcon)
        {
            AsyncActionSetZoneAttributes action = CreateAsyncActionSetZoneAttributes();
            
            object result = action.SetZoneAttributesBeginSync(DesiredZoneName, DesiredIcon);

            action.SetZoneAttributesEnd(result);
        }
        
        public void GetZoneAttributesSync(out string CurrentZoneName, out string CurrentIcon)
        {
            AsyncActionGetZoneAttributes action = CreateAsyncActionGetZoneAttributes();
            
            object result = action.GetZoneAttributesBeginSync();

            AsyncActionGetZoneAttributes.EventArgsResponse response = action.GetZoneAttributesEnd(result);
                
            CurrentZoneName = response.CurrentZoneName;
            CurrentIcon = response.CurrentIcon;
        }
        
        public string GetHouseholdIDSync()
        {
            AsyncActionGetHouseholdID action = CreateAsyncActionGetHouseholdID();
            
            object result = action.GetHouseholdIDBeginSync();

            AsyncActionGetHouseholdID.EventArgsResponse response = action.GetHouseholdIDEnd(result);
                
            return(response.CurrentHouseholdID);
        }
        
        public void GetZoneInfoSync(out string SerialNumber, out string SoftwareVersion, out string DisplaySoftwareVersion, out string HardwareVersion, out string IPAddress, out string MACAddress, out string CopyrightInfo, out string ExtraInfo)
        {
            AsyncActionGetZoneInfo action = CreateAsyncActionGetZoneInfo();
            
            object result = action.GetZoneInfoBeginSync();

            AsyncActionGetZoneInfo.EventArgsResponse response = action.GetZoneInfoEnd(result);
                
            SerialNumber = response.SerialNumber;
            SoftwareVersion = response.SoftwareVersion;
            DisplaySoftwareVersion = response.DisplaySoftwareVersion;
            HardwareVersion = response.HardwareVersion;
            IPAddress = response.IPAddress;
            MACAddress = response.MACAddress;
            CopyrightInfo = response.CopyrightInfo;
            ExtraInfo = response.ExtraInfo;
        }
        

        // AsyncActionSetLEDState

        public class AsyncActionSetLEDState
        {
            internal AsyncActionSetLEDState(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetLEDStateBeginSync(string DesiredLEDState)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DesiredLEDState", DesiredLEDState);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetLEDStateBegin(string DesiredLEDState)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DesiredLEDState", DesiredLEDState);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetLEDState.SetLEDStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetLEDStateEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetLEDState.SetLEDStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetLEDState.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionGetLEDState

        public class AsyncActionGetLEDState
        {
            internal AsyncActionGetLEDState(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object GetLEDStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetLEDStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetLEDState.GetLEDStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetLEDStateEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetLEDState.GetLEDStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetLEDState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentLEDState = aHandler.ReadArgumentString("CurrentLEDState");
                }
                
                public string CurrentLEDState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionSetInvisible

        public class AsyncActionSetInvisible
        {
            internal AsyncActionSetInvisible(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetInvisibleBeginSync(bool DesiredInvisible)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("DesiredInvisible", DesiredInvisible);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetInvisibleBegin(bool DesiredInvisible)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("DesiredInvisible", DesiredInvisible);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetInvisible.SetInvisibleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetInvisibleEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetInvisible.SetInvisibleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetInvisible.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionGetInvisible

        public class AsyncActionGetInvisible
        {
            internal AsyncActionGetInvisible(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object GetInvisibleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetInvisibleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetInvisible.GetInvisibleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetInvisibleEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetInvisible.GetInvisibleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetInvisible.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentInvisible = aHandler.ReadArgumentBool("CurrentInvisible");
                }
                
                public bool CurrentInvisible;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionSetZoneAttributes

        public class AsyncActionSetZoneAttributes
        {
            internal AsyncActionSetZoneAttributes(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object SetZoneAttributesBeginSync(string DesiredZoneName, string DesiredIcon)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DesiredZoneName", DesiredZoneName);           
                iHandler.WriteArgumentString("DesiredIcon", DesiredIcon);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetZoneAttributesBegin(string DesiredZoneName, string DesiredIcon)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("DesiredZoneName", DesiredZoneName);                
                iHandler.WriteArgumentString("DesiredIcon", DesiredIcon);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetZoneAttributes.SetZoneAttributesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetZoneAttributesEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetZoneAttributes.SetZoneAttributesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionSetZoneAttributes.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionGetZoneAttributes

        public class AsyncActionGetZoneAttributes
        {
            internal AsyncActionGetZoneAttributes(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object GetZoneAttributesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetZoneAttributesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneAttributes.GetZoneAttributesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetZoneAttributesEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneAttributes.GetZoneAttributesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneAttributes.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentZoneName = aHandler.ReadArgumentString("CurrentZoneName");
                    CurrentIcon = aHandler.ReadArgumentString("CurrentIcon");
                }
                
                public string CurrentZoneName;
                public string CurrentIcon;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionGetHouseholdID

        public class AsyncActionGetHouseholdID
        {
            internal AsyncActionGetHouseholdID(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object GetHouseholdIDBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetHouseholdIDBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetHouseholdID.GetHouseholdIDBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetHouseholdIDEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetHouseholdID.GetHouseholdIDEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetHouseholdID.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentHouseholdID = aHandler.ReadArgumentString("CurrentHouseholdID");
                }
                
                public string CurrentHouseholdID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDeviceProperties iService;
        }
        
        
        // AsyncActionGetZoneInfo

        public class AsyncActionGetZoneInfo
        {
            internal AsyncActionGetZoneInfo(ServiceDeviceProperties aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object GetZoneInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetZoneInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneInfo.GetZoneInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetZoneInfoEnd(object aResult)
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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneInfo.GetZoneInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("DeviceProperties.AsyncActionGetZoneInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SerialNumber = aHandler.ReadArgumentString("SerialNumber");
                    SoftwareVersion = aHandler.ReadArgumentString("SoftwareVersion");
                    DisplaySoftwareVersion = aHandler.ReadArgumentString("DisplaySoftwareVersion");
                    HardwareVersion = aHandler.ReadArgumentString("HardwareVersion");
                    IPAddress = aHandler.ReadArgumentString("IPAddress");
                    MACAddress = aHandler.ReadArgumentString("MACAddress");
                    CopyrightInfo = aHandler.ReadArgumentString("CopyrightInfo");
                    ExtraInfo = aHandler.ReadArgumentString("ExtraInfo");
                }
                
                public string SerialNumber;
                public string SoftwareVersion;
                public string DisplaySoftwareVersion;
                public string HardwareVersion;
                public string IPAddress;
                public string MACAddress;
                public string CopyrightInfo;
                public string ExtraInfo;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDeviceProperties iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceDeviceProperties): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventSettingsReplicationState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SettingsReplicationState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SettingsReplicationState = value;

                eventSettingsReplicationState = true;
            }

            bool eventZoneName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ZoneName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ZoneName = value;

                eventZoneName = true;
            }

            bool eventIcon = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Icon", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Icon = value;

                eventIcon = true;
            }

            bool eventInvisible = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Invisible", nsmanager);

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
	                Invisible = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Invisible = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Invisible = false; 
    	            }
                }

                eventInvisible = true;
            }

          
            
            if(eventSettingsReplicationState)
            {
                if (EventStateSettingsReplicationState != null)
                {
					try
					{
						EventStateSettingsReplicationState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSettingsReplicationState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventZoneName)
            {
                if (EventStateZoneName != null)
                {
					try
					{
						EventStateZoneName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateZoneName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventIcon)
            {
                if (EventStateIcon != null)
                {
					try
					{
						EventStateIcon(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateIcon: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventInvisible)
            {
                if (EventStateInvisible != null)
                {
					try
					{
						EventStateInvisible(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateInvisible: " + ex);
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
        public event EventHandler<EventArgs> EventStateSettingsReplicationState;
        public event EventHandler<EventArgs> EventStateZoneName;
        public event EventHandler<EventArgs> EventStateIcon;
        public event EventHandler<EventArgs> EventStateInvisible;

        public string SettingsReplicationState;
        public string ZoneName;
        public string Icon;
        public bool Invisible;
    }
}
    
