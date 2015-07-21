using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceVolkano : ServiceUpnp
    {

        public const string kBootModeMain = "Main";
        public const string kBootModeFallback = "Fallback";
        public const string kBootModeRam = "Ram";
        public const string kRebootModeMain = "Main";
        public const string kRebootModeFallback = "Fallback";

        public ServiceVolkano(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceVolkano(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Reboot");
            iActions.Add(action);
            
            action = new Action("BootMode");
            action.AddOutArgument(new Argument("aMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetBootMode");
            action.AddInArgument(new Argument("aMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("BspType");
            action.AddOutArgument(new Argument("aBspType", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("UglyName");
            action.AddOutArgument(new Argument("aUglyName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("MacAddress");
            action.AddOutArgument(new Argument("aMacAddress", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ProductId");
            action.AddOutArgument(new Argument("aProductNumber", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("BoardId");
            action.AddInArgument(new Argument("aIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBoardNumber", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("BoardType");
            action.AddInArgument(new Argument("aIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBoardNumber", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("MaxBoards");
            action.AddOutArgument(new Argument("aMaxBoards", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SoftwareVersion");
            action.AddOutArgument(new Argument("aSoftwareVersion", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SoftwareUpdate");
            action.AddOutArgument(new Argument("aAvailable", Argument.EType.eBool));
            action.AddOutArgument(new Argument("aSoftwareVersion", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DeviceInfo");
            action.AddOutArgument(new Argument("aDeviceInfoXml", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Volkano", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Volkano", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionReboot CreateAsyncActionReboot()
        {
            return (new AsyncActionReboot(this));
        }

        public AsyncActionBootMode CreateAsyncActionBootMode()
        {
            return (new AsyncActionBootMode(this));
        }

        public AsyncActionSetBootMode CreateAsyncActionSetBootMode()
        {
            return (new AsyncActionSetBootMode(this));
        }

        public AsyncActionBspType CreateAsyncActionBspType()
        {
            return (new AsyncActionBspType(this));
        }

        public AsyncActionUglyName CreateAsyncActionUglyName()
        {
            return (new AsyncActionUglyName(this));
        }

        public AsyncActionMacAddress CreateAsyncActionMacAddress()
        {
            return (new AsyncActionMacAddress(this));
        }

        public AsyncActionProductId CreateAsyncActionProductId()
        {
            return (new AsyncActionProductId(this));
        }

        public AsyncActionBoardId CreateAsyncActionBoardId()
        {
            return (new AsyncActionBoardId(this));
        }

        public AsyncActionBoardType CreateAsyncActionBoardType()
        {
            return (new AsyncActionBoardType(this));
        }

        public AsyncActionMaxBoards CreateAsyncActionMaxBoards()
        {
            return (new AsyncActionMaxBoards(this));
        }

        public AsyncActionSoftwareVersion CreateAsyncActionSoftwareVersion()
        {
            return (new AsyncActionSoftwareVersion(this));
        }

        public AsyncActionSoftwareUpdate CreateAsyncActionSoftwareUpdate()
        {
            return (new AsyncActionSoftwareUpdate(this));
        }

        public AsyncActionDeviceInfo CreateAsyncActionDeviceInfo()
        {
            return (new AsyncActionDeviceInfo(this));
        }


        // Synchronous actions
        
        public void RebootSync()
        {
            AsyncActionReboot action = CreateAsyncActionReboot();
            
            object result = action.RebootBeginSync();

            action.RebootEnd(result);
        }
        
        public string BootModeSync()
        {
            AsyncActionBootMode action = CreateAsyncActionBootMode();
            
            object result = action.BootModeBeginSync();

            AsyncActionBootMode.EventArgsResponse response = action.BootModeEnd(result);
                
            return(response.aMode);
        }
        
        public void SetBootModeSync(string aMode)
        {
            AsyncActionSetBootMode action = CreateAsyncActionSetBootMode();
            
            object result = action.SetBootModeBeginSync(aMode);

            action.SetBootModeEnd(result);
        }
        
        public string BspTypeSync()
        {
            AsyncActionBspType action = CreateAsyncActionBspType();
            
            object result = action.BspTypeBeginSync();

            AsyncActionBspType.EventArgsResponse response = action.BspTypeEnd(result);
                
            return(response.aBspType);
        }
        
        public string UglyNameSync()
        {
            AsyncActionUglyName action = CreateAsyncActionUglyName();
            
            object result = action.UglyNameBeginSync();

            AsyncActionUglyName.EventArgsResponse response = action.UglyNameEnd(result);
                
            return(response.aUglyName);
        }
        
        public string MacAddressSync()
        {
            AsyncActionMacAddress action = CreateAsyncActionMacAddress();
            
            object result = action.MacAddressBeginSync();

            AsyncActionMacAddress.EventArgsResponse response = action.MacAddressEnd(result);
                
            return(response.aMacAddress);
        }
        
        public string ProductIdSync()
        {
            AsyncActionProductId action = CreateAsyncActionProductId();
            
            object result = action.ProductIdBeginSync();

            AsyncActionProductId.EventArgsResponse response = action.ProductIdEnd(result);
                
            return(response.aProductNumber);
        }
        
        public string BoardIdSync(uint aIndex)
        {
            AsyncActionBoardId action = CreateAsyncActionBoardId();
            
            object result = action.BoardIdBeginSync(aIndex);

            AsyncActionBoardId.EventArgsResponse response = action.BoardIdEnd(result);
                
            return(response.aBoardNumber);
        }
        
        public string BoardTypeSync(uint aIndex)
        {
            AsyncActionBoardType action = CreateAsyncActionBoardType();
            
            object result = action.BoardTypeBeginSync(aIndex);

            AsyncActionBoardType.EventArgsResponse response = action.BoardTypeEnd(result);
                
            return(response.aBoardNumber);
        }
        
        public uint MaxBoardsSync()
        {
            AsyncActionMaxBoards action = CreateAsyncActionMaxBoards();
            
            object result = action.MaxBoardsBeginSync();

            AsyncActionMaxBoards.EventArgsResponse response = action.MaxBoardsEnd(result);
                
            return(response.aMaxBoards);
        }
        
        public string SoftwareVersionSync()
        {
            AsyncActionSoftwareVersion action = CreateAsyncActionSoftwareVersion();
            
            object result = action.SoftwareVersionBeginSync();

            AsyncActionSoftwareVersion.EventArgsResponse response = action.SoftwareVersionEnd(result);
                
            return(response.aSoftwareVersion);
        }
        
        public void SoftwareUpdateSync(out bool aAvailable, out string aSoftwareVersion)
        {
            AsyncActionSoftwareUpdate action = CreateAsyncActionSoftwareUpdate();
            
            object result = action.SoftwareUpdateBeginSync();

            AsyncActionSoftwareUpdate.EventArgsResponse response = action.SoftwareUpdateEnd(result);
                
            aAvailable = response.aAvailable;
            aSoftwareVersion = response.aSoftwareVersion;
        }
        
        public string DeviceInfoSync()
        {
            AsyncActionDeviceInfo action = CreateAsyncActionDeviceInfo();
            
            object result = action.DeviceInfoBeginSync();

            AsyncActionDeviceInfo.EventArgsResponse response = action.DeviceInfoEnd(result);
                
            return(response.aDeviceInfoXml);
        }
        

        // AsyncActionReboot

        public class AsyncActionReboot
        {
            internal AsyncActionReboot(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object RebootBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RebootBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionReboot.RebootBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RebootEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionReboot.RebootEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionReboot.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionBootMode

        public class AsyncActionBootMode
        {
            internal AsyncActionBootMode(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object BootModeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void BootModeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionBootMode.BootModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BootModeEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionBootMode.BootModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionBootMode.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMode = aHandler.ReadArgumentString("aMode");
                }
                
                public string aMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionSetBootMode

        public class AsyncActionSetBootMode
        {
            internal AsyncActionSetBootMode(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetBootModeBeginSync(string aMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aMode", aMode);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBootModeBegin(string aMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aMode", aMode);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionSetBootMode.SetBootModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetBootModeEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionSetBootMode.SetBootModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionSetBootMode.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionBspType

        public class AsyncActionBspType
        {
            internal AsyncActionBspType(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object BspTypeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void BspTypeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionBspType.BspTypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BspTypeEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionBspType.BspTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionBspType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBspType = aHandler.ReadArgumentString("aBspType");
                }
                
                public string aBspType;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionUglyName

        public class AsyncActionUglyName
        {
            internal AsyncActionUglyName(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object UglyNameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void UglyNameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionUglyName.UglyNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse UglyNameEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionUglyName.UglyNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionUglyName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aUglyName = aHandler.ReadArgumentString("aUglyName");
                }
                
                public string aUglyName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionMacAddress

        public class AsyncActionMacAddress
        {
            internal AsyncActionMacAddress(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object MacAddressBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MacAddressBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionMacAddress.MacAddressBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MacAddressEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionMacAddress.MacAddressEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionMacAddress.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMacAddress = aHandler.ReadArgumentString("aMacAddress");
                }
                
                public string aMacAddress;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionProductId

        public class AsyncActionProductId
        {
            internal AsyncActionProductId(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object ProductIdBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ProductIdBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionProductId.ProductIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ProductIdEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionProductId.ProductIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionProductId.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aProductNumber = aHandler.ReadArgumentString("aProductNumber");
                }
                
                public string aProductNumber;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionBoardId

        public class AsyncActionBoardId
        {
            internal AsyncActionBoardId(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object BoardIdBeginSync(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void BoardIdBegin(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionBoardId.BoardIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BoardIdEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionBoardId.BoardIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionBoardId.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBoardNumber = aHandler.ReadArgumentString("aBoardNumber");
                }
                
                public string aBoardNumber;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionBoardType

        public class AsyncActionBoardType
        {
            internal AsyncActionBoardType(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object BoardTypeBeginSync(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void BoardTypeBegin(uint aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionBoardType.BoardTypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BoardTypeEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionBoardType.BoardTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionBoardType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBoardNumber = aHandler.ReadArgumentString("aBoardNumber");
                }
                
                public string aBoardNumber;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionMaxBoards

        public class AsyncActionMaxBoards
        {
            internal AsyncActionMaxBoards(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object MaxBoardsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MaxBoardsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionMaxBoards.MaxBoardsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MaxBoardsEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionMaxBoards.MaxBoardsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionMaxBoards.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMaxBoards = aHandler.ReadArgumentUint("aMaxBoards");
                }
                
                public uint aMaxBoards;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionSoftwareVersion

        public class AsyncActionSoftwareVersion
        {
            internal AsyncActionSoftwareVersion(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object SoftwareVersionBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SoftwareVersionBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareVersion.SoftwareVersionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SoftwareVersionEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareVersion.SoftwareVersionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareVersion.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSoftwareVersion = aHandler.ReadArgumentString("aSoftwareVersion");
                }
                
                public string aSoftwareVersion;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionSoftwareUpdate

        public class AsyncActionSoftwareUpdate
        {
            internal AsyncActionSoftwareUpdate(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object SoftwareUpdateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SoftwareUpdateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareUpdate.SoftwareUpdateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SoftwareUpdateEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareUpdate.SoftwareUpdateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionSoftwareUpdate.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aAvailable = aHandler.ReadArgumentBool("aAvailable");
                    aSoftwareVersion = aHandler.ReadArgumentString("aSoftwareVersion");
                }
                
                public bool aAvailable;
                public string aSoftwareVersion;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        
        // AsyncActionDeviceInfo

        public class AsyncActionDeviceInfo
        {
            internal AsyncActionDeviceInfo(ServiceVolkano aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object DeviceInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DeviceInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volkano.AsyncActionDeviceInfo.DeviceInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeviceInfoEnd(object aResult)
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
                    UserLog.WriteLine("Volkano.AsyncActionDeviceInfo.DeviceInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volkano.AsyncActionDeviceInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDeviceInfoXml = aHandler.ReadArgumentString("aDeviceInfoXml");
                }
                
                public string aDeviceInfoXml;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolkano iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
        }

    }
}
    
