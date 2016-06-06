using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceProductV3 : ServiceUpnp
    {


        public ServiceProductV3(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceProductV3(Device aDevice, IEventUpnpProvider aEventServer)
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
            
            action = new Action("SourceXml");
            action.AddOutArgument(new Argument("aSourceXml", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceIndex");
            action.AddOutArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSourceIndex");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSourceIndexByName");
            action.AddInArgument(new Argument("aSourceName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetStartupSourceIndexByName");
            action.AddInArgument(new Argument("aSourceName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("StartupSourceIndex");
            action.AddOutArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetStartupSourceIndex");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("StartupSourceEnabled");
            action.AddOutArgument(new Argument("aStartupSourceEnabled", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetStartupSourceEnabled");
            action.AddInArgument(new Argument("aStartupSourceEnabled", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SourceSystemName");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSourceName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceName");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSourceName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetSourceName");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddInArgument(new Argument("aSourceName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceType");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSourceType", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceVisible");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSourceVisible", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetSourceVisible");
            action.AddInArgument(new Argument("aSourceIndex", Argument.EType.eUint));
            action.AddInArgument(new Argument("aSourceVisible", Argument.EType.eBool));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Product", 3));
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

        public AsyncActionSourceXml CreateAsyncActionSourceXml()
        {
            return (new AsyncActionSourceXml(this));
        }

        public AsyncActionSourceIndex CreateAsyncActionSourceIndex()
        {
            return (new AsyncActionSourceIndex(this));
        }

        public AsyncActionSetSourceIndex CreateAsyncActionSetSourceIndex()
        {
            return (new AsyncActionSetSourceIndex(this));
        }

        public AsyncActionSetSourceIndexByName CreateAsyncActionSetSourceIndexByName()
        {
            return (new AsyncActionSetSourceIndexByName(this));
        }

        public AsyncActionSetStartupSourceIndexByName CreateAsyncActionSetStartupSourceIndexByName()
        {
            return (new AsyncActionSetStartupSourceIndexByName(this));
        }

        public AsyncActionStartupSourceIndex CreateAsyncActionStartupSourceIndex()
        {
            return (new AsyncActionStartupSourceIndex(this));
        }

        public AsyncActionSetStartupSourceIndex CreateAsyncActionSetStartupSourceIndex()
        {
            return (new AsyncActionSetStartupSourceIndex(this));
        }

        public AsyncActionStartupSourceEnabled CreateAsyncActionStartupSourceEnabled()
        {
            return (new AsyncActionStartupSourceEnabled(this));
        }

        public AsyncActionSetStartupSourceEnabled CreateAsyncActionSetStartupSourceEnabled()
        {
            return (new AsyncActionSetStartupSourceEnabled(this));
        }

        public AsyncActionSourceSystemName CreateAsyncActionSourceSystemName()
        {
            return (new AsyncActionSourceSystemName(this));
        }

        public AsyncActionSourceName CreateAsyncActionSourceName()
        {
            return (new AsyncActionSourceName(this));
        }

        public AsyncActionSetSourceName CreateAsyncActionSetSourceName()
        {
            return (new AsyncActionSetSourceName(this));
        }

        public AsyncActionSourceType CreateAsyncActionSourceType()
        {
            return (new AsyncActionSourceType(this));
        }

        public AsyncActionSourceVisible CreateAsyncActionSourceVisible()
        {
            return (new AsyncActionSourceVisible(this));
        }

        public AsyncActionSetSourceVisible CreateAsyncActionSetSourceVisible()
        {
            return (new AsyncActionSetSourceVisible(this));
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
        
        public string SourceXmlSync()
        {
            AsyncActionSourceXml action = CreateAsyncActionSourceXml();
            
            object result = action.SourceXmlBeginSync();

            AsyncActionSourceXml.EventArgsResponse response = action.SourceXmlEnd(result);
                
            return(response.aSourceXml);
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
        
        public void SetSourceIndexByNameSync(string aSourceName)
        {
            AsyncActionSetSourceIndexByName action = CreateAsyncActionSetSourceIndexByName();
            
            object result = action.SetSourceIndexByNameBeginSync(aSourceName);

            action.SetSourceIndexByNameEnd(result);
        }
        
        public void SetStartupSourceIndexByNameSync(string aSourceName)
        {
            AsyncActionSetStartupSourceIndexByName action = CreateAsyncActionSetStartupSourceIndexByName();
            
            object result = action.SetStartupSourceIndexByNameBeginSync(aSourceName);

            action.SetStartupSourceIndexByNameEnd(result);
        }
        
        public uint StartupSourceIndexSync()
        {
            AsyncActionStartupSourceIndex action = CreateAsyncActionStartupSourceIndex();
            
            object result = action.StartupSourceIndexBeginSync();

            AsyncActionStartupSourceIndex.EventArgsResponse response = action.StartupSourceIndexEnd(result);
                
            return(response.aSourceIndex);
        }
        
        public void SetStartupSourceIndexSync(uint aSourceIndex)
        {
            AsyncActionSetStartupSourceIndex action = CreateAsyncActionSetStartupSourceIndex();
            
            object result = action.SetStartupSourceIndexBeginSync(aSourceIndex);

            action.SetStartupSourceIndexEnd(result);
        }
        
        public bool StartupSourceEnabledSync()
        {
            AsyncActionStartupSourceEnabled action = CreateAsyncActionStartupSourceEnabled();
            
            object result = action.StartupSourceEnabledBeginSync();

            AsyncActionStartupSourceEnabled.EventArgsResponse response = action.StartupSourceEnabledEnd(result);
                
            return(response.aStartupSourceEnabled);
        }
        
        public void SetStartupSourceEnabledSync(bool aStartupSourceEnabled)
        {
            AsyncActionSetStartupSourceEnabled action = CreateAsyncActionSetStartupSourceEnabled();
            
            object result = action.SetStartupSourceEnabledBeginSync(aStartupSourceEnabled);

            action.SetStartupSourceEnabledEnd(result);
        }
        
        public string SourceSystemNameSync(uint aSourceIndex)
        {
            AsyncActionSourceSystemName action = CreateAsyncActionSourceSystemName();
            
            object result = action.SourceSystemNameBeginSync(aSourceIndex);

            AsyncActionSourceSystemName.EventArgsResponse response = action.SourceSystemNameEnd(result);
                
            return(response.aSourceName);
        }
        
        public string SourceNameSync(uint aSourceIndex)
        {
            AsyncActionSourceName action = CreateAsyncActionSourceName();
            
            object result = action.SourceNameBeginSync(aSourceIndex);

            AsyncActionSourceName.EventArgsResponse response = action.SourceNameEnd(result);
                
            return(response.aSourceName);
        }
        
        public void SetSourceNameSync(uint aSourceIndex, string aSourceName)
        {
            AsyncActionSetSourceName action = CreateAsyncActionSetSourceName();
            
            object result = action.SetSourceNameBeginSync(aSourceIndex, aSourceName);

            action.SetSourceNameEnd(result);
        }
        
        public string SourceTypeSync(uint aSourceIndex)
        {
            AsyncActionSourceType action = CreateAsyncActionSourceType();
            
            object result = action.SourceTypeBeginSync(aSourceIndex);

            AsyncActionSourceType.EventArgsResponse response = action.SourceTypeEnd(result);
                
            return(response.aSourceType);
        }
        
        public bool SourceVisibleSync(uint aSourceIndex)
        {
            AsyncActionSourceVisible action = CreateAsyncActionSourceVisible();
            
            object result = action.SourceVisibleBeginSync(aSourceIndex);

            AsyncActionSourceVisible.EventArgsResponse response = action.SourceVisibleEnd(result);
                
            return(response.aSourceVisible);
        }
        
        public void SetSourceVisibleSync(uint aSourceIndex, bool aSourceVisible)
        {
            AsyncActionSetSourceVisible action = CreateAsyncActionSetSourceVisible();
            
            object result = action.SetSourceVisibleBeginSync(aSourceIndex, aSourceVisible);

            action.SetSourceVisibleEnd(result);
        }
        

        // AsyncActionType

        public class AsyncActionType
        {
            internal AsyncActionType(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionType.TypeBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionType.TypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionType.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionModel

        public class AsyncActionModel
        {
            internal AsyncActionModel(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionModel.ModelBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionModel.ModelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionModel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionName

        public class AsyncActionName
        {
            internal AsyncActionName(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionName.NameBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionName.NameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetName

        public class AsyncActionSetName
        {
            internal AsyncActionSetName(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetName.SetNameBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetName.SetNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionRoom

        public class AsyncActionRoom
        {
            internal AsyncActionRoom(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionRoom.RoomBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionRoom.RoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionRoom.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetRoom

        public class AsyncActionSetRoom
        {
            internal AsyncActionSetRoom(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetRoom.SetRoomBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetRoom.SetRoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetRoom.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionStandby

        public class AsyncActionStandby
        {
            internal AsyncActionStandby(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionStandby.StandbyBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionStandby.StandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionStandby.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetStandby

        public class AsyncActionSetStandby
        {
            internal AsyncActionSetStandby(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStandby.SetStandbyBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStandby.SetStandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStandby.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceCount

        public class AsyncActionSourceCount
        {
            internal AsyncActionSourceCount(ServiceProductV3 aService)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceCount.SourceCountBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceCount.SourceCountEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceCount.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceXml

        public class AsyncActionSourceXml
        {
            internal AsyncActionSourceXml(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SourceXmlBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceXmlBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSourceXml.SourceXmlBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceXmlEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceXml.SourceXmlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceXml.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceXml = aHandler.ReadArgumentString("aSourceXml");
                }
                
                public string aSourceXml;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceIndex

        public class AsyncActionSourceIndex
        {
            internal AsyncActionSourceIndex(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceIndex.SourceIndexBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceIndex.SourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetSourceIndex

        public class AsyncActionSetSourceIndex
        {
            internal AsyncActionSetSourceIndex(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndex.SetSourceIndexBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndex.SetSourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetSourceIndexByName

        public class AsyncActionSetSourceIndexByName
        {
            internal AsyncActionSetSourceIndexByName(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SetSourceIndexByNameBeginSync(string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSourceName", aSourceName);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceIndexByNameBegin(string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSourceName", aSourceName);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndexByName.SetSourceIndexByNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSourceIndexByNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndexByName.SetSourceIndexByNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceIndexByName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetStartupSourceIndexByName

        public class AsyncActionSetStartupSourceIndexByName
        {
            internal AsyncActionSetStartupSourceIndexByName(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object SetStartupSourceIndexByNameBeginSync(string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSourceName", aSourceName);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStartupSourceIndexByNameBegin(string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSourceName", aSourceName);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndexByName.SetStartupSourceIndexByNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStartupSourceIndexByNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndexByName.SetStartupSourceIndexByNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndexByName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionStartupSourceIndex

        public class AsyncActionStartupSourceIndex
        {
            internal AsyncActionStartupSourceIndex(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object StartupSourceIndexBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StartupSourceIndexBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceIndex.StartupSourceIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StartupSourceIndexEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceIndex.StartupSourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetStartupSourceIndex

        public class AsyncActionSetStartupSourceIndex
        {
            internal AsyncActionSetStartupSourceIndex(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object SetStartupSourceIndexBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStartupSourceIndexBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndex.SetStartupSourceIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStartupSourceIndexEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndex.SetStartupSourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionStartupSourceEnabled

        public class AsyncActionStartupSourceEnabled
        {
            internal AsyncActionStartupSourceEnabled(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object StartupSourceEnabledBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StartupSourceEnabledBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceEnabled.StartupSourceEnabledBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StartupSourceEnabledEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceEnabled.StartupSourceEnabledEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionStartupSourceEnabled.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aStartupSourceEnabled = aHandler.ReadArgumentBool("aStartupSourceEnabled");
                }
                
                public bool aStartupSourceEnabled;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetStartupSourceEnabled

        public class AsyncActionSetStartupSourceEnabled
        {
            internal AsyncActionSetStartupSourceEnabled(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object SetStartupSourceEnabledBeginSync(bool aStartupSourceEnabled)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStartupSourceEnabled", aStartupSourceEnabled);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStartupSourceEnabledBegin(bool aStartupSourceEnabled)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aStartupSourceEnabled", aStartupSourceEnabled);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceEnabled.SetStartupSourceEnabledBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStartupSourceEnabledEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceEnabled.SetStartupSourceEnabledEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetStartupSourceEnabled.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceSystemName

        public class AsyncActionSourceSystemName
        {
            internal AsyncActionSourceSystemName(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object SourceSystemNameBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceSystemNameBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSourceSystemName.SourceSystemNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceSystemNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceSystemName.SourceSystemNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceSystemName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceName = aHandler.ReadArgumentString("aSourceName");
                }
                
                public string aSourceName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceName

        public class AsyncActionSourceName
        {
            internal AsyncActionSourceName(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object SourceNameBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceNameBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSourceName.SourceNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceName.SourceNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceName = aHandler.ReadArgumentString("aSourceName");
                }
                
                public string aSourceName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetSourceName

        public class AsyncActionSetSourceName
        {
            internal AsyncActionSetSourceName(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(20));
                iService = aService;
            }

            internal object SetSourceNameBeginSync(uint aSourceIndex, string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                iHandler.WriteArgumentString("aSourceName", aSourceName);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceNameBegin(uint aSourceIndex, string aSourceName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                iHandler.WriteArgumentString("aSourceName", aSourceName);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceName.SetSourceNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSourceNameEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceName.SetSourceNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceType

        public class AsyncActionSourceType
        {
            internal AsyncActionSourceType(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(21));
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceType.SourceTypeBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceType.SourceTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceType.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSourceVisible

        public class AsyncActionSourceVisible
        {
            internal AsyncActionSourceVisible(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(22));
                iService = aService;
            }

            internal object SourceVisibleBeginSync(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceVisibleBegin(uint aSourceIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSourceVisible.SourceVisibleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceVisibleEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceVisible.SourceVisibleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSourceVisible.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSourceVisible = aHandler.ReadArgumentBool("aSourceVisible");
                }
                
                public bool aSourceVisible;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProductV3 iService;
        }
        
        
        // AsyncActionSetSourceVisible

        public class AsyncActionSetSourceVisible
        {
            internal AsyncActionSetSourceVisible(ServiceProductV3 aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(23));
                iService = aService;
            }

            internal object SetSourceVisibleBeginSync(uint aSourceIndex, bool aSourceVisible)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);           
                iHandler.WriteArgumentBool("aSourceVisible", aSourceVisible);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceVisibleBegin(uint aSourceIndex, bool aSourceVisible)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aSourceIndex", aSourceIndex);                
                iHandler.WriteArgumentBool("aSourceVisible", aSourceVisible);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceVisible.SetSourceVisibleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSourceVisibleEnd(object aResult)
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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceVisible.SetSourceVisibleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ProductV3.AsyncActionSetSourceVisible.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProductV3 iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceProductV3): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventProductType = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductType", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductType = value;

                eventProductType = true;
            }

            bool eventProductModel = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductModel", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductModel = value;

                eventProductModel = true;
            }

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

            bool eventProductSourceCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductSourceCount", nsmanager);

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
					ProductSourceCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ProductSourceCount with value {1}", DateTime.Now, value));
				}

                eventProductSourceCount = true;
            }

            bool eventProductSourceXml = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductSourceXml", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductSourceXml = value;

                eventProductSourceXml = true;
            }

            bool eventStartupSourceIndex = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "StartupSourceIndex", nsmanager);

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
					StartupSourceIndex = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse StartupSourceIndex with value {1}", DateTime.Now, value));
				}

                eventStartupSourceIndex = true;
            }

            bool eventStartupSourceEnabled = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "StartupSourceEnabled", nsmanager);

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
	                StartupSourceEnabled = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		StartupSourceEnabled = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	StartupSourceEnabled = false; 
    	            }
                }

                eventStartupSourceEnabled = true;
            }

            bool eventProductAnySourceName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductAnySourceName", nsmanager);

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
					ProductAnySourceName = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ProductAnySourceName with value {1}", DateTime.Now, value));
				}

                eventProductAnySourceName = true;
            }

            bool eventProductAnySourceVisible = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductAnySourceVisible", nsmanager);

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
					ProductAnySourceVisible = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ProductAnySourceVisible with value {1}", DateTime.Now, value));
				}

                eventProductAnySourceVisible = true;
            }

            bool eventProductAnySourceType = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductAnySourceType", nsmanager);

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
					ProductAnySourceType = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ProductAnySourceType with value {1}", DateTime.Now, value));
				}

                eventProductAnySourceType = true;
            }

          
            
            if(eventProductType)
            {
                if (EventStateProductType != null)
                {
					try
					{
						EventStateProductType(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductType: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductModel)
            {
                if (EventStateProductModel != null)
                {
					try
					{
						EventStateProductModel(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductModel: " + ex);
						Assert.CheckDebug(false);
					}
                }
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
            
            if(eventProductSourceCount)
            {
                if (EventStateProductSourceCount != null)
                {
					try
					{
						EventStateProductSourceCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductSourceCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductSourceXml)
            {
                if (EventStateProductSourceXml != null)
                {
					try
					{
						EventStateProductSourceXml(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductSourceXml: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventStartupSourceIndex)
            {
                if (EventStateStartupSourceIndex != null)
                {
					try
					{
						EventStateStartupSourceIndex(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateStartupSourceIndex: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventStartupSourceEnabled)
            {
                if (EventStateStartupSourceEnabled != null)
                {
					try
					{
						EventStateStartupSourceEnabled(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateStartupSourceEnabled: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductAnySourceName)
            {
                if (EventStateProductAnySourceName != null)
                {
					try
					{
						EventStateProductAnySourceName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductAnySourceName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductAnySourceVisible)
            {
                if (EventStateProductAnySourceVisible != null)
                {
					try
					{
						EventStateProductAnySourceVisible(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductAnySourceVisible: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductAnySourceType)
            {
                if (EventStateProductAnySourceType != null)
                {
					try
					{
						EventStateProductAnySourceType(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductAnySourceType: " + ex);
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
        public event EventHandler<EventArgs> EventStateProductType;
        public event EventHandler<EventArgs> EventStateProductModel;
        public event EventHandler<EventArgs> EventStateProductName;
        public event EventHandler<EventArgs> EventStateProductRoom;
        public event EventHandler<EventArgs> EventStateProductStandby;
        public event EventHandler<EventArgs> EventStateProductSourceIndex;
        public event EventHandler<EventArgs> EventStateProductSourceCount;
        public event EventHandler<EventArgs> EventStateProductSourceXml;
        public event EventHandler<EventArgs> EventStateStartupSourceIndex;
        public event EventHandler<EventArgs> EventStateStartupSourceEnabled;
        public event EventHandler<EventArgs> EventStateProductAnySourceName;
        public event EventHandler<EventArgs> EventStateProductAnySourceVisible;
        public event EventHandler<EventArgs> EventStateProductAnySourceType;

        public string ProductType;
        public string ProductModel;
        public string ProductName;
        public string ProductRoom;
        public bool ProductStandby;
        public uint ProductSourceIndex;
        public uint ProductSourceCount;
        public string ProductSourceXml;
        public uint StartupSourceIndex;
        public bool StartupSourceEnabled;
        public uint ProductAnySourceName;
        public uint ProductAnySourceVisible;
        public uint ProductAnySourceType;
    }
}
    
