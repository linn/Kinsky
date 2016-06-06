using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceRadio : ServiceUpnp
    {

        public const string kTransportStateStopped = "Stopped";
        public const string kTransportStatePlaying = "Playing";
        public const string kTransportStatePaused = "Paused";
        public const string kTransportStateBuffering = "Buffering";

        public ServiceRadio(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceRadio(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Play");
            iActions.Add(action);
            
            action = new Action("Pause");
            iActions.Add(action);
            
            action = new Action("Stop");
            iActions.Add(action);
            
            action = new Action("SeekSecondAbsolute");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SeekSecondRelative");
            action.AddInArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("Channel");
            action.AddOutArgument(new Argument("Uri", Argument.EType.eString));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetChannel");
            action.AddInArgument(new Argument("Uri", Argument.EType.eString));
            action.AddInArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TransportState");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Id");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetId");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            action.AddInArgument(new Argument("Uri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Read");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ReadList");
            action.AddInArgument(new Argument("IdList", Argument.EType.eString));
            action.AddOutArgument(new Argument("ChannelList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("IdArray");
            action.AddOutArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Array", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("IdArrayChanged");
            action.AddInArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("ChannelsMax");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("ProtocolInfo");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Radio", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Radio", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionPlay CreateAsyncActionPlay()
        {
            return (new AsyncActionPlay(this));
        }

        public AsyncActionPause CreateAsyncActionPause()
        {
            return (new AsyncActionPause(this));
        }

        public AsyncActionStop CreateAsyncActionStop()
        {
            return (new AsyncActionStop(this));
        }

        public AsyncActionSeekSecondAbsolute CreateAsyncActionSeekSecondAbsolute()
        {
            return (new AsyncActionSeekSecondAbsolute(this));
        }

        public AsyncActionSeekSecondRelative CreateAsyncActionSeekSecondRelative()
        {
            return (new AsyncActionSeekSecondRelative(this));
        }

        public AsyncActionChannel CreateAsyncActionChannel()
        {
            return (new AsyncActionChannel(this));
        }

        public AsyncActionSetChannel CreateAsyncActionSetChannel()
        {
            return (new AsyncActionSetChannel(this));
        }

        public AsyncActionTransportState CreateAsyncActionTransportState()
        {
            return (new AsyncActionTransportState(this));
        }

        public AsyncActionId CreateAsyncActionId()
        {
            return (new AsyncActionId(this));
        }

        public AsyncActionSetId CreateAsyncActionSetId()
        {
            return (new AsyncActionSetId(this));
        }

        public AsyncActionRead CreateAsyncActionRead()
        {
            return (new AsyncActionRead(this));
        }

        public AsyncActionReadList CreateAsyncActionReadList()
        {
            return (new AsyncActionReadList(this));
        }

        public AsyncActionIdArray CreateAsyncActionIdArray()
        {
            return (new AsyncActionIdArray(this));
        }

        public AsyncActionIdArrayChanged CreateAsyncActionIdArrayChanged()
        {
            return (new AsyncActionIdArrayChanged(this));
        }

        public AsyncActionChannelsMax CreateAsyncActionChannelsMax()
        {
            return (new AsyncActionChannelsMax(this));
        }

        public AsyncActionProtocolInfo CreateAsyncActionProtocolInfo()
        {
            return (new AsyncActionProtocolInfo(this));
        }


        // Synchronous actions
        
        public void PlaySync()
        {
            AsyncActionPlay action = CreateAsyncActionPlay();
            
            object result = action.PlayBeginSync();

            action.PlayEnd(result);
        }
        
        public void PauseSync()
        {
            AsyncActionPause action = CreateAsyncActionPause();
            
            object result = action.PauseBeginSync();

            action.PauseEnd(result);
        }
        
        public void StopSync()
        {
            AsyncActionStop action = CreateAsyncActionStop();
            
            object result = action.StopBeginSync();

            action.StopEnd(result);
        }
        
        public void SeekSecondAbsoluteSync(uint Value)
        {
            AsyncActionSeekSecondAbsolute action = CreateAsyncActionSeekSecondAbsolute();
            
            object result = action.SeekSecondAbsoluteBeginSync(Value);

            action.SeekSecondAbsoluteEnd(result);
        }
        
        public void SeekSecondRelativeSync(int Value)
        {
            AsyncActionSeekSecondRelative action = CreateAsyncActionSeekSecondRelative();
            
            object result = action.SeekSecondRelativeBeginSync(Value);

            action.SeekSecondRelativeEnd(result);
        }
        
        public void ChannelSync(out string Uri, out string Metadata)
        {
            AsyncActionChannel action = CreateAsyncActionChannel();
            
            object result = action.ChannelBeginSync();

            AsyncActionChannel.EventArgsResponse response = action.ChannelEnd(result);
                
            Uri = response.Uri;
            Metadata = response.Metadata;
        }
        
        public void SetChannelSync(string Uri, string Metadata)
        {
            AsyncActionSetChannel action = CreateAsyncActionSetChannel();
            
            object result = action.SetChannelBeginSync(Uri, Metadata);

            action.SetChannelEnd(result);
        }
        
        public string TransportStateSync()
        {
            AsyncActionTransportState action = CreateAsyncActionTransportState();
            
            object result = action.TransportStateBeginSync();

            AsyncActionTransportState.EventArgsResponse response = action.TransportStateEnd(result);
                
            return(response.Value);
        }
        
        public uint IdSync()
        {
            AsyncActionId action = CreateAsyncActionId();
            
            object result = action.IdBeginSync();

            AsyncActionId.EventArgsResponse response = action.IdEnd(result);
                
            return(response.Value);
        }
        
        public void SetIdSync(uint Value, string Uri)
        {
            AsyncActionSetId action = CreateAsyncActionSetId();
            
            object result = action.SetIdBeginSync(Value, Uri);

            action.SetIdEnd(result);
        }
        
        public string ReadSync(uint Id)
        {
            AsyncActionRead action = CreateAsyncActionRead();
            
            object result = action.ReadBeginSync(Id);

            AsyncActionRead.EventArgsResponse response = action.ReadEnd(result);
                
            return(response.Metadata);
        }
        
        public string ReadListSync(string IdList)
        {
            AsyncActionReadList action = CreateAsyncActionReadList();
            
            object result = action.ReadListBeginSync(IdList);

            AsyncActionReadList.EventArgsResponse response = action.ReadListEnd(result);
                
            return(response.ChannelList);
        }
        
        public void IdArraySync(out uint Token, out byte[] Array)
        {
            AsyncActionIdArray action = CreateAsyncActionIdArray();
            
            object result = action.IdArrayBeginSync();

            AsyncActionIdArray.EventArgsResponse response = action.IdArrayEnd(result);
                
            Token = response.Token;
            Array = response.Array;
        }
        
        public bool IdArrayChangedSync(uint Token)
        {
            AsyncActionIdArrayChanged action = CreateAsyncActionIdArrayChanged();
            
            object result = action.IdArrayChangedBeginSync(Token);

            AsyncActionIdArrayChanged.EventArgsResponse response = action.IdArrayChangedEnd(result);
                
            return(response.Value);
        }
        
        public uint ChannelsMaxSync()
        {
            AsyncActionChannelsMax action = CreateAsyncActionChannelsMax();
            
            object result = action.ChannelsMaxBeginSync();

            AsyncActionChannelsMax.EventArgsResponse response = action.ChannelsMaxEnd(result);
                
            return(response.Value);
        }
        
        public string ProtocolInfoSync()
        {
            AsyncActionProtocolInfo action = CreateAsyncActionProtocolInfo();
            
            object result = action.ProtocolInfoBeginSync();

            AsyncActionProtocolInfo.EventArgsResponse response = action.ProtocolInfoEnd(result);
                
            return(response.Value);
        }
        

        // AsyncActionPlay

        public class AsyncActionPlay
        {
            internal AsyncActionPlay(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object PlayBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlayBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionPlay.PlayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlayEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionPlay.PlayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionPlay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionPause

        public class AsyncActionPause
        {
            internal AsyncActionPause(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object PauseBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PauseBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionPause.PauseBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PauseEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionPause.PauseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionPause.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionStop

        public class AsyncActionStop
        {
            internal AsyncActionStop(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object StopBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StopBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionStop.StopBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StopEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionStop.StopEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionStop.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionSeekSecondAbsolute

        public class AsyncActionSeekSecondAbsolute
        {
            internal AsyncActionSeekSecondAbsolute(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SeekSecondAbsoluteBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SeekSecondAbsoluteBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondAbsolute.SeekSecondAbsoluteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SeekSecondAbsoluteEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondAbsolute.SeekSecondAbsoluteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondAbsolute.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionSeekSecondRelative

        public class AsyncActionSeekSecondRelative
        {
            internal AsyncActionSeekSecondRelative(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object SeekSecondRelativeBeginSync(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SeekSecondRelativeBegin(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondRelative.SeekSecondRelativeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SeekSecondRelativeEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondRelative.SeekSecondRelativeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionSeekSecondRelative.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionChannel

        public class AsyncActionChannel
        {
            internal AsyncActionChannel(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object ChannelBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ChannelBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionChannel.ChannelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ChannelEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionChannel.ChannelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionChannel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Uri = aHandler.ReadArgumentString("Uri");
                    Metadata = aHandler.ReadArgumentString("Metadata");
                }
                
                public string Uri;
                public string Metadata;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionSetChannel

        public class AsyncActionSetChannel
        {
            internal AsyncActionSetChannel(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SetChannelBeginSync(string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Uri", Uri);           
                iHandler.WriteArgumentString("Metadata", Metadata);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetChannelBegin(string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Uri", Uri);                
                iHandler.WriteArgumentString("Metadata", Metadata);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionSetChannel.SetChannelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetChannelEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionSetChannel.SetChannelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionSetChannel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionTransportState

        public class AsyncActionTransportState
        {
            internal AsyncActionTransportState(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object TransportStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TransportStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionTransportState.TransportStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TransportStateEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionTransportState.TransportStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionTransportState.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionId

        public class AsyncActionId
        {
            internal AsyncActionId(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object IdBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void IdBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionId.IdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse IdEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionId.IdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionId.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentUint("Value");
                }
                
                public uint Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionSetId

        public class AsyncActionSetId
        {
            internal AsyncActionSetId(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetIdBeginSync(uint Value, string Uri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                iHandler.WriteArgumentString("Uri", Uri);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetIdBegin(uint Value, string Uri)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                iHandler.WriteArgumentString("Uri", Uri);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionSetId.SetIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetIdEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionSetId.SetIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionSetId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionRead

        public class AsyncActionRead
        {
            internal AsyncActionRead(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object ReadBeginSync(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReadBegin(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionRead.ReadBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ReadEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionRead.ReadEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionRead.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Metadata = aHandler.ReadArgumentString("Metadata");
                }
                
                public string Metadata;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionReadList

        public class AsyncActionReadList
        {
            internal AsyncActionReadList(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object ReadListBeginSync(string IdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("IdList", IdList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReadListBegin(string IdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("IdList", IdList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionReadList.ReadListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ReadListEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionReadList.ReadListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionReadList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ChannelList = aHandler.ReadArgumentString("ChannelList");
                }
                
                public string ChannelList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionIdArray

        public class AsyncActionIdArray
        {
            internal AsyncActionIdArray(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object IdArrayBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void IdArrayBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionIdArray.IdArrayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse IdArrayEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionIdArray.IdArrayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionIdArray.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Token = aHandler.ReadArgumentUint("Token");
                    Array = aHandler.ReadArgumentBinary("Array");
                }
                
                public uint Token;
                public byte[] Array;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionIdArrayChanged

        public class AsyncActionIdArrayChanged
        {
            internal AsyncActionIdArrayChanged(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object IdArrayChangedBeginSync(uint Token)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Token", Token);           
                
                return (iHandler.WriteEnd(null));
            }

            public void IdArrayChangedBegin(uint Token)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Token", Token);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionIdArrayChanged.IdArrayChangedBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse IdArrayChangedEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionIdArrayChanged.IdArrayChangedEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionIdArrayChanged.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
        }
        
        
        // AsyncActionChannelsMax

        public class AsyncActionChannelsMax
        {
            internal AsyncActionChannelsMax(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object ChannelsMaxBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ChannelsMaxBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionChannelsMax.ChannelsMaxBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ChannelsMaxEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionChannelsMax.ChannelsMaxEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionChannelsMax.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentUint("Value");
                }
                
                public uint Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRadio iService;
        }
        
        
        // AsyncActionProtocolInfo

        public class AsyncActionProtocolInfo
        {
            internal AsyncActionProtocolInfo(ServiceRadio aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object ProtocolInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ProtocolInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Radio.AsyncActionProtocolInfo.ProtocolInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ProtocolInfoEnd(object aResult)
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
                    UserLog.WriteLine("Radio.AsyncActionProtocolInfo.ProtocolInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Radio.AsyncActionProtocolInfo.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRadio iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceRadio): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Uri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Uri = value;

                eventUri = true;
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

            bool eventTransportState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TransportState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TransportState = value;

                eventTransportState = true;
            }

            bool eventId = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Id", nsmanager);

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
					Id = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Id with value {1}", DateTime.Now, value));
				}

                eventId = true;
            }

            bool eventIdArray = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "IdArray", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    IdArray =  new byte[0];
				}
				else
				{
                    IdArray = Convert.FromBase64String(value);
                }

                eventIdArray = true;
            }

            bool eventChannelsMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ChannelsMax", nsmanager);

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
					ChannelsMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse ChannelsMax with value {1}", DateTime.Now, value));
				}

                eventChannelsMax = true;
            }

            bool eventProtocolInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProtocolInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProtocolInfo = value;

                eventProtocolInfo = true;
            }

          
            
            if(eventUri)
            {
                if (EventStateUri != null)
                {
					try
					{
						EventStateUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateUri: " + ex);
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
            
            if(eventTransportState)
            {
                if (EventStateTransportState != null)
                {
					try
					{
						EventStateTransportState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTransportState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventId)
            {
                if (EventStateId != null)
                {
					try
					{
						EventStateId(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateId: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventIdArray)
            {
                if (EventStateIdArray != null)
                {
					try
					{
						EventStateIdArray(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateIdArray: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventChannelsMax)
            {
                if (EventStateChannelsMax != null)
                {
					try
					{
						EventStateChannelsMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateChannelsMax: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProtocolInfo)
            {
                if (EventStateProtocolInfo != null)
                {
					try
					{
						EventStateProtocolInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProtocolInfo: " + ex);
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
        public event EventHandler<EventArgs> EventStateUri;
        public event EventHandler<EventArgs> EventStateMetadata;
        public event EventHandler<EventArgs> EventStateTransportState;
        public event EventHandler<EventArgs> EventStateId;
        public event EventHandler<EventArgs> EventStateIdArray;
        public event EventHandler<EventArgs> EventStateChannelsMax;
        public event EventHandler<EventArgs> EventStateProtocolInfo;

        public string Uri;
        public string Metadata;
        public string TransportState;
        public uint Id;
        public byte[] IdArray;
        public uint ChannelsMax;
        public string ProtocolInfo;
    }
}
    
