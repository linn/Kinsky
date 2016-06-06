using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceProxy : ServiceUpnp
    {


        public ServiceProxy(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceProxy(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SoftwareVersion");
            action.AddOutArgument(new Argument("aSoftwareVersion", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("HardwareVersion");
            action.AddOutArgument(new Argument("aHardwareVersion", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Proxy", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Proxy", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSoftwareVersion CreateAsyncActionSoftwareVersion()
        {
            return (new AsyncActionSoftwareVersion(this));
        }

        public AsyncActionHardwareVersion CreateAsyncActionHardwareVersion()
        {
            return (new AsyncActionHardwareVersion(this));
        }


        // Synchronous actions
        
        public string SoftwareVersionSync()
        {
            AsyncActionSoftwareVersion action = CreateAsyncActionSoftwareVersion();
            
            object result = action.SoftwareVersionBeginSync();

            AsyncActionSoftwareVersion.EventArgsResponse response = action.SoftwareVersionEnd(result);
                
            return(response.aSoftwareVersion);
        }
        
        public string HardwareVersionSync()
        {
            AsyncActionHardwareVersion action = CreateAsyncActionHardwareVersion();
            
            object result = action.HardwareVersionBeginSync();

            AsyncActionHardwareVersion.EventArgsResponse response = action.HardwareVersionEnd(result);
                
            return(response.aHardwareVersion);
        }
        

        // AsyncActionSoftwareVersion

        public class AsyncActionSoftwareVersion
        {
            internal AsyncActionSoftwareVersion(ServiceProxy aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
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
                    UserLog.WriteLine("Proxy.AsyncActionSoftwareVersion.SoftwareVersionBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Proxy.AsyncActionSoftwareVersion.SoftwareVersionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Proxy.AsyncActionSoftwareVersion.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProxy iService;
        }
        
        
        // AsyncActionHardwareVersion

        public class AsyncActionHardwareVersion
        {
            internal AsyncActionHardwareVersion(ServiceProxy aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object HardwareVersionBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void HardwareVersionBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Proxy.AsyncActionHardwareVersion.HardwareVersionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse HardwareVersionEnd(object aResult)
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
                    UserLog.WriteLine("Proxy.AsyncActionHardwareVersion.HardwareVersionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Proxy.AsyncActionHardwareVersion.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aHardwareVersion = aHandler.ReadArgumentString("aHardwareVersion");
                }
                
                public string aHardwareVersion;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProxy iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
        }

    }
}
    
