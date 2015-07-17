using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceProductV2 : ServiceUpnp
    {


        public ServiceProductV2(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceProductV2(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Type");
            action.AddOutArgument(new Argument("aType", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Model");
            action.AddOutArgument(new Argument("aModel", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Name");
            action.AddOutArgument(new Argument("aName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetName");
            action.AddInArgument(new Argument("aName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Room");
            action.AddOutArgument(new Argument("aRoom", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetRoom");
            action.AddInArgument(new Argument("aRoom", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Standby");
            action.AddOutArgument(new Argument("aStandby", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetStandby");
            action.AddInArgument(new Argument("aStandby", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SourceCount");
            action.AddOutArgument(new Argument("aSourceCount", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SourceIndex");
            action.AddOutArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSourceIndex");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SourceType");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSourceType", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Product", 2));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Product", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionType CreateAsyncActionType()
        {
            return (new AsyncActionType(this));
        }

        public AsyncActionModel CreateAsyncActionModel()
        {
            return (new AsyncActionModel(this));
        }

        public AsyncActionName CreateAsyncActionName()
        {
            return (new AsyncActionName(this));
        }

        public AsyncActionSetName CreateAsyncActionSetName()
        {
            return (new AsyncActionSetName(this));
        }

        public AsyncActionRoom CreateAsyncActionRoom()
        {
            return (new AsyncActionRoom(this));
        }

        public AsyncActionSetRoom CreateAsyncActionSetRoom()
        {
            return (new AsyncActionSetRoom(this));
        }

        public AsyncActionStandby CreateAsyncActionStandby()
        {
            return (new AsyncActionStandby(this));
        }

        public AsyncActionSetStandby CreateAsyncActionSetStandby()
        {
            return (new AsyncActionSetStandby(this));
        }

        public AsyncActionSourceCount CreateAsyncActionSourceCount()
        {
            return (new AsyncActionSourceCount(this));
        }

        public AsyncActionSourceIndex CreateAsyncActionSourceIndex()
        {
            return (new AsyncActionSourceIndex(this));
        }

        public AsyncActionSetSourceIndex CreateAsyncActionSetSourceIndex()
        {
            return (new AsyncActionSetSourceIndex(this));
        }

        public AsyncActionSourceType CreateAsyncActionSourceType()
        {
            return (new AsyncActionSourceType(this));
        }


        // Synchronous actions
        
        public string TypeSync()
        {
            AsyncActionType action = CreateAsyncActionType();
            
            object result = action.TypeBeginSync();

            AsyncActionType.EventArgsResponse response = action.TypeEnd(result);
                
            return(response.aType);
        }
        
        public string ModelSync()
        {
            AsyncActionModel action = CreateAsyncActionModel();
            
            object result = action.ModelBeginSync();

            AsyncActionModel.EventArgsResponse response = action.ModelEnd(result);
                
            return(response.aModel);
        }
        
        public string NameSync()
        {
            AsyncActionName action = CreateAsyncActionName();
            
            object result = action.NameBeginSync();

            AsyncActionName.EventArgsResponse response = action.NameEnd(result);
                
            return(response.aName);
        }
        
        public void SetNameSync(string aName)
        {
            AsyncActionSetName action = CreateAsyncActionSetName();
            
            object result = action.SetNameBeginSync(aName);

            action.SetNameEnd(result);
        }
        
        public string RoomSync()
        {
            AsyncActionRoom action = CreateAsyncActionRoom();
            
            object result = action.RoomBeginSync();

            AsyncActionRoom.EventArgsResponse response = action.RoomEnd(result);
                
            return(response.aRoom);
        }
        
        public void SetRoomSync(string aRoom)
        {
            AsyncActionSetRoom action = CreateAsyncActionSetRoom();
            
            object result = action.SetRoomBeginSync(aRoom);

            action.SetRoomEnd(result);
        }
        
        public bool StandbySync()
        {
            AsyncActionStandby action = CreateAsyncActionStandby();
            
            object result = action.StandbyBeginSync();

            AsyncActionStandby.EventArgsResponse response = action.StandbyEnd(result);
                
            return(response.aStandby);
        }
        
        public void SetStandbySync(bool aStandby)
        {
            AsyncActionSetStandby action = CreateAsyncActionSetStandby();
            
            object result = action.SetStandbyBeginSync(aStandby);

            action.SetStandbyEnd(result);
        }
        
        public uint SourceCountSync()
        {
            AsyncActionSourceCount action = CreateAsyncActionSourceCount();
            
            object result = action.SourceCountBeginSync();

            AsyncActionSourceCount.EventArgsResponse response = action.SourceCountEnd(result);
                
            return(response.aSourceCount);
        }
        
        public uint SourceIndexSync()
        {
            AsyncActionSourceIndex action = CreateAsyncActionSourceIndex();
            
            object result = action.SourceIndexBeginSync();

            AsyncActionSourceIndex.EventArgsResponse response = action.SourceIndexEnd(result);
                
            return(response.aSourceIndex);
        }
        
        public void SetSourceIndexSync(uint aSourceIndex)
        {
            AsyncActionSetSourceIndex action = CreateAsyncActionSetSourceIndex();
            
            object result = action.SetSourceIndexBeginSync(aSourceIndex);

            action.SetSourceIndexEnd(result);
        }
        
        public string SourceTypeSync(uint aSourceIndex)
        {
            AsyncActionSourceType action = CreateAsyncActionSourceType();
            
            object result = action.SourceTypeBeginSync(aSourceIndex);

            AsyncActionSourceType.EventArgsResponse response = action.SourceTypeEnd(result);
                
            return(response.aSourceType);
        }
        

        // AsyncActionType

        public class AsyncActionType
        {
            internal AsyncActionType(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object TypeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TypeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionType.TypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TypeEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionType.TypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aType = aHandler.ReadArgumentString("aType");
                }
                
                public string aType;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionModel

        public class AsyncActionModel
        {
            internal AsyncActionModel(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object ModelBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ModelBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionModel.ModelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ModelEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionModel.ModelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionModel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aModel = aHandler.ReadArgumentString("aModel");
                }
                
                public string aModel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionName

        public class AsyncActionName
        {
            internal AsyncActionName(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object NameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void NameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionName.NameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse NameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionName.NameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aName = aHandler.ReadArgumentString("aName");
                }
                
                public string aName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSetName

        public class AsyncActionSetName
        {
            internal AsyncActionSetName(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SetNameBeginSync(string aName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aName", aName);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetNameBegin(string aName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aName", aName);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSetName.SetNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSetName.SetNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSetName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionRoom

        public class AsyncActionRoom
        {
            internal AsyncActionRoom(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object RoomBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RoomBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionRoom.RoomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RoomEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionRoom.RoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionRoom.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aRoom = aHandler.ReadArgumentString("aRoom");
                }
                
                public string aRoom;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSetRoom

        public class AsyncActionSetRoom
        {
            internal AsyncActionSetRoom(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object SetRoomBeginSync(string aRoom)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aRoom", aRoom);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRoomBegin(string aRoom)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aRoom", aRoom);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSetRoom.SetRoomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRoomEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSetRoom.SetRoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSetRoom.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionStandby

        public class AsyncActionStandby
        {
            internal AsyncActionStandby(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object StandbyBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StandbyBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionStandby.StandbyBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StandbyEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionStandby.StandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionStandby.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aStandby = aHandler.ReadArgumentBool("aStandby");
                }
                
                public bool aStandby;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSetStandby

        public class AsyncActionSetStandby
        {
            internal AsyncActionSetStandby(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SetStandbyBeginSync(bool aStandby)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStandby", aStandby);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStandbyBegin(bool aStandby)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStandby", aStandby);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSetStandby.SetStandbyBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStandbyEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSetStandby.SetStandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSetStandby.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSourceCount

        public class AsyncActionSourceCount
        {
            internal AsyncActionSourceCount(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object SourceCountBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceCountBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSourceCount.SourceCountBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceCountEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceCount.SourceCountEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceCount.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceCount = aHandler.ReadArgumentUint("aSourceCount");
                }
                
                public uint aSourceCount;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSourceIndex

        public class AsyncActionSourceIndex
        {
            internal AsyncActionSourceIndex(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SourceIndexBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceIndexBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSourceIndex.SourceIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceIndexEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceIndex.SourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceIndex = aHandler.ReadArgumentUint("aSourceIndex");
                }
                
                public uint aSourceIndex;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSetSourceIndex

        public class AsyncActionSetSourceIndex
        {
            internal AsyncActionSetSourceIndex(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object SetSourceIndexBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceIndexBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSetSourceIndex.SetSourceIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSourceIndexEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSetSourceIndex.SetSourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSetSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV2 iService;
        }
        
        
        // AsyncActionSourceType

        public class AsyncActionSourceType
        {
            internal AsyncActionSourceType(ServiceProductV2 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object SourceTypeBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceTypeBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV2.AsyncActionSourceType.SourceTypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceTypeEnd(object aResult)
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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceType.SourceTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV2.AsyncActionSourceType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceType = aHandler.ReadArgumentString("aSourceType");
                }
                
                public string aSourceType;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV2 iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceProductV2): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventProductName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductName = value;

                eventProductName = true;
            }

            bool eventProductRoom = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductRoom", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductRoom = value;

                eventProductRoom = true;
            }

            bool eventProductStandby = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductStandby", nsmanager);

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
	                ProductStandby = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		ProductStandby = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	ProductStandby = false; 
    	            }
                }

                eventProductStandby = true;
            }

            bool eventProductSourceIndex = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductSourceIndex", nsmanager);

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
					ProductSourceIndex = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ProductSourceIndex with value {1}", DateTime.Now, value));
				}

                eventProductSourceIndex = true;
            }

          
            
            if(eventProductName)
            {
                if (EventStateProductName != null)
                {
					try
					{
						EventStateProductName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductRoom)
            {
                if (EventStateProductRoom != null)
                {
					try
					{
						EventStateProductRoom(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductRoom: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductStandby)
            {
                if (EventStateProductStandby != null)
                {
					try
					{
						EventStateProductStandby(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductStandby: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductSourceIndex)
            {
                if (EventStateProductSourceIndex != null)
                {
					try
					{
						EventStateProductSourceIndex(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductSourceIndex: " + ex);
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
        public event EventHandler<EventArgs> EventStateProductName;
        public event EventHandler<EventArgs> EventStateProductRoom;
        public event EventHandler<EventArgs> EventStateProductStandby;
        public event EventHandler<EventArgs> EventStateProductSourceIndex;

        public string ProductName;
        public string ProductRoom;
        public bool ProductStandby;
        public uint ProductSourceIndex;
    }
}
    
