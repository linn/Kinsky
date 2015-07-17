using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceDebug : ServiceUpnp
    {


        public ServiceDebug(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceDebug(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetDebugLevel");
            action.AddInArgument(new Argument("aDebugLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DebugLevel");
            action.AddOutArgument(new Argument("aDebugLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("MemWrite");
            action.AddInArgument(new Argument("aMemAddress", Argument.EType.eUint));
            action.AddInArgument(new Argument("aMemData", Argument.EType.eBinary));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Debug", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Debug", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetDebugLevel CreateAsyncActionSetDebugLevel()
        {
            return (new AsyncActionSetDebugLevel(this));
        }

        public AsyncActionDebugLevel CreateAsyncActionDebugLevel()
        {
            return (new AsyncActionDebugLevel(this));
        }

        public AsyncActionMemWrite CreateAsyncActionMemWrite()
        {
            return (new AsyncActionMemWrite(this));
        }


        // Synchronous actions
        
        public void SetDebugLevelSync(uint aDebugLevel)
        {
            AsyncActionSetDebugLevel action = CreateAsyncActionSetDebugLevel();
            
            object result = action.SetDebugLevelBeginSync(aDebugLevel);

            action.SetDebugLevelEnd(result);
        }
        
        public uint DebugLevelSync()
        {
            AsyncActionDebugLevel action = CreateAsyncActionDebugLevel();
            
            object result = action.DebugLevelBeginSync();

            AsyncActionDebugLevel.EventArgsResponse response = action.DebugLevelEnd(result);
                
            return(response.aDebugLevel);
        }
        
        public void MemWriteSync(uint aMemAddress, byte[] aMemData)
        {
            AsyncActionMemWrite action = CreateAsyncActionMemWrite();
            
            object result = action.MemWriteBeginSync(aMemAddress, aMemData);

            action.MemWriteEnd(result);
        }
        

        // AsyncActionSetDebugLevel

        public class AsyncActionSetDebugLevel
        {
            internal AsyncActionSetDebugLevel(ServiceDebug aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetDebugLevelBeginSync(uint aDebugLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDebugLevel", aDebugLevel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetDebugLevelBegin(uint aDebugLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDebugLevel", aDebugLevel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Debug.AsyncActionSetDebugLevel.SetDebugLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetDebugLevelEnd(object aResult)
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
                    UserLog.WriteLine("Debug.AsyncActionSetDebugLevel.SetDebugLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Debug.AsyncActionSetDebugLevel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDebug iService;
        }
        
        
        // AsyncActionDebugLevel

        public class AsyncActionDebugLevel
        {
            internal AsyncActionDebugLevel(ServiceDebug aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object DebugLevelBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DebugLevelBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Debug.AsyncActionDebugLevel.DebugLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DebugLevelEnd(object aResult)
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
                    UserLog.WriteLine("Debug.AsyncActionDebugLevel.DebugLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Debug.AsyncActionDebugLevel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDebugLevel = aHandler.ReadArgumentUint("aDebugLevel");
                }
                
                public uint aDebugLevel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDebug iService;
        }
        
        
        // AsyncActionMemWrite

        public class AsyncActionMemWrite
        {
            internal AsyncActionMemWrite(ServiceDebug aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object MemWriteBeginSync(uint aMemAddress, byte[] aMemData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aMemAddress", aMemAddress);           
                iHandler.WriteArgumentBinary("aMemData", aMemData);           
                
                return (iHandler.WriteEnd(null));
            }

            public void MemWriteBegin(uint aMemAddress, byte[] aMemData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aMemAddress", aMemAddress);                
                iHandler.WriteArgumentBinary("aMemData", aMemData);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Debug.AsyncActionMemWrite.MemWriteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MemWriteEnd(object aResult)
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
                    UserLog.WriteLine("Debug.AsyncActionMemWrite.MemWriteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Debug.AsyncActionMemWrite.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDebug iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
        }

    }
}
    
