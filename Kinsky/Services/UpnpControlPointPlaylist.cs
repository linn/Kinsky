using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServicePlaylist : ServiceUpnp
    {

        public const string kTransportStatePlaying = "Playing";
        public const string kTransportStatePaused = "Paused";
        public const string kTransportStateStopped = "Stopped";
        public const string kTransportStateBuffering = "Buffering";

        public ServicePlaylist(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServicePlaylist(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Play");
            iActions.Add(action);
            
            action = new Action("Pause");
            iActions.Add(action);
            
            action = new Action("Stop");
            iActions.Add(action);
            
            action = new Action("Next");
            iActions.Add(action);
            
            action = new Action("Previous");
            iActions.Add(action);
            
            action = new Action("SetRepeat");
            action.AddInArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Repeat");
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetShuffle");
            action.AddInArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Shuffle");
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SeekSecondAbsolute");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SeekSecondRelative");
            action.AddInArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SeekId");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SeekIndex");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("TransportState");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Id");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Read");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Uri", Argument.EType.eString));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ReadList");
            action.AddInArgument(new Argument("IdList", Argument.EType.eString));
            action.AddOutArgument(new Argument("TrackList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Insert");
            action.AddInArgument(new Argument("AfterId", Argument.EType.eUint));
            action.AddInArgument(new Argument("Uri", Argument.EType.eString));
            action.AddInArgument(new Argument("Metadata", Argument.EType.eString));
            action.AddOutArgument(new Argument("NewId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DeleteId");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DeleteAll");
            iActions.Add(action);
            
            action = new Action("TracksMax");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("IdArray");
            action.AddOutArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Array", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("IdArrayChanged");
            action.AddInArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("ProtocolInfo");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Playlist", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Playlist", aVersion));
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

        public AsyncActionNext CreateAsyncActionNext()
        {
            return (new AsyncActionNext(this));
        }

        public AsyncActionPrevious CreateAsyncActionPrevious()
        {
            return (new AsyncActionPrevious(this));
        }

        public AsyncActionSetRepeat CreateAsyncActionSetRepeat()
        {
            return (new AsyncActionSetRepeat(this));
        }

        public AsyncActionRepeat CreateAsyncActionRepeat()
        {
            return (new AsyncActionRepeat(this));
        }

        public AsyncActionSetShuffle CreateAsyncActionSetShuffle()
        {
            return (new AsyncActionSetShuffle(this));
        }

        public AsyncActionShuffle CreateAsyncActionShuffle()
        {
            return (new AsyncActionShuffle(this));
        }

        public AsyncActionSeekSecondAbsolute CreateAsyncActionSeekSecondAbsolute()
        {
            return (new AsyncActionSeekSecondAbsolute(this));
        }

        public AsyncActionSeekSecondRelative CreateAsyncActionSeekSecondRelative()
        {
            return (new AsyncActionSeekSecondRelative(this));
        }

        public AsyncActionSeekId CreateAsyncActionSeekId()
        {
            return (new AsyncActionSeekId(this));
        }

        public AsyncActionSeekIndex CreateAsyncActionSeekIndex()
        {
            return (new AsyncActionSeekIndex(this));
        }

        public AsyncActionTransportState CreateAsyncActionTransportState()
        {
            return (new AsyncActionTransportState(this));
        }

        public AsyncActionId CreateAsyncActionId()
        {
            return (new AsyncActionId(this));
        }

        public AsyncActionRead CreateAsyncActionRead()
        {
            return (new AsyncActionRead(this));
        }

        public AsyncActionReadList CreateAsyncActionReadList()
        {
            return (new AsyncActionReadList(this));
        }

        public AsyncActionInsert CreateAsyncActionInsert()
        {
            return (new AsyncActionInsert(this));
        }

        public AsyncActionDeleteId CreateAsyncActionDeleteId()
        {
            return (new AsyncActionDeleteId(this));
        }

        public AsyncActionDeleteAll CreateAsyncActionDeleteAll()
        {
            return (new AsyncActionDeleteAll(this));
        }

        public AsyncActionTracksMax CreateAsyncActionTracksMax()
        {
            return (new AsyncActionTracksMax(this));
        }

        public AsyncActionIdArray CreateAsyncActionIdArray()
        {
            return (new AsyncActionIdArray(this));
        }

        public AsyncActionIdArrayChanged CreateAsyncActionIdArrayChanged()
        {
            return (new AsyncActionIdArrayChanged(this));
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
        
        public void NextSync()
        {
            AsyncActionNext action = CreateAsyncActionNext();
            
            object result = action.NextBeginSync();

            action.NextEnd(result);
        }
        
        public void PreviousSync()
        {
            AsyncActionPrevious action = CreateAsyncActionPrevious();
            
            object result = action.PreviousBeginSync();

            action.PreviousEnd(result);
        }
        
        public void SetRepeatSync(bool Value)
        {
            AsyncActionSetRepeat action = CreateAsyncActionSetRepeat();
            
            object result = action.SetRepeatBeginSync(Value);

            action.SetRepeatEnd(result);
        }
        
        public bool RepeatSync()
        {
            AsyncActionRepeat action = CreateAsyncActionRepeat();
            
            object result = action.RepeatBeginSync();

            AsyncActionRepeat.EventArgsResponse response = action.RepeatEnd(result);
                
            return(response.Value);
        }
        
        public void SetShuffleSync(bool Value)
        {
            AsyncActionSetShuffle action = CreateAsyncActionSetShuffle();
            
            object result = action.SetShuffleBeginSync(Value);

            action.SetShuffleEnd(result);
        }
        
        public bool ShuffleSync()
        {
            AsyncActionShuffle action = CreateAsyncActionShuffle();
            
            object result = action.ShuffleBeginSync();

            AsyncActionShuffle.EventArgsResponse response = action.ShuffleEnd(result);
                
            return(response.Value);
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
        
        public void SeekIdSync(uint Value)
        {
            AsyncActionSeekId action = CreateAsyncActionSeekId();
            
            object result = action.SeekIdBeginSync(Value);

            action.SeekIdEnd(result);
        }
        
        public void SeekIndexSync(uint Value)
        {
            AsyncActionSeekIndex action = CreateAsyncActionSeekIndex();
            
            object result = action.SeekIndexBeginSync(Value);

            action.SeekIndexEnd(result);
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
        
        public void ReadSync(uint Id, out string Uri, out string Metadata)
        {
            AsyncActionRead action = CreateAsyncActionRead();
            
            object result = action.ReadBeginSync(Id);

            AsyncActionRead.EventArgsResponse response = action.ReadEnd(result);
                
            Uri = response.Uri;
            Metadata = response.Metadata;
        }
        
        public string ReadListSync(string IdList)
        {
            AsyncActionReadList action = CreateAsyncActionReadList();
            
            object result = action.ReadListBeginSync(IdList);

            AsyncActionReadList.EventArgsResponse response = action.ReadListEnd(result);
                
            return(response.TrackList);
        }
        
        public uint InsertSync(uint AfterId, string Uri, string Metadata)
        {
            AsyncActionInsert action = CreateAsyncActionInsert();
            
            object result = action.InsertBeginSync(AfterId, Uri, Metadata);

            AsyncActionInsert.EventArgsResponse response = action.InsertEnd(result);
                
            return(response.NewId);
        }
        
        public void DeleteIdSync(uint Value)
        {
            AsyncActionDeleteId action = CreateAsyncActionDeleteId();
            
            object result = action.DeleteIdBeginSync(Value);

            action.DeleteIdEnd(result);
        }
        
        public void DeleteAllSync()
        {
            AsyncActionDeleteAll action = CreateAsyncActionDeleteAll();
            
            object result = action.DeleteAllBeginSync();

            action.DeleteAllEnd(result);
        }
        
        public uint TracksMaxSync()
        {
            AsyncActionTracksMax action = CreateAsyncActionTracksMax();
            
            object result = action.TracksMaxBeginSync();

            AsyncActionTracksMax.EventArgsResponse response = action.TracksMaxEnd(result);
                
            return(response.Value);
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
            internal AsyncActionPlay(ServicePlaylist aService)
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
                    UserLog.WriteLine("Playlist.AsyncActionPlay.PlayBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionPlay.PlayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionPlay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionPause

        public class AsyncActionPause
        {
            internal AsyncActionPause(ServicePlaylist aService)
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
                    UserLog.WriteLine("Playlist.AsyncActionPause.PauseBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionPause.PauseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionPause.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionStop

        public class AsyncActionStop
        {
            internal AsyncActionStop(ServicePlaylist aService)
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
                    UserLog.WriteLine("Playlist.AsyncActionStop.StopBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionStop.StopEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionStop.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionNext

        public class AsyncActionNext
        {
            internal AsyncActionNext(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object NextBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void NextBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionNext.NextBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse NextEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionNext.NextEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionNext.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionPrevious

        public class AsyncActionPrevious
        {
            internal AsyncActionPrevious(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object PreviousBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PreviousBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionPrevious.PreviousBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PreviousEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionPrevious.PreviousEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionPrevious.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSetRepeat

        public class AsyncActionSetRepeat
        {
            internal AsyncActionSetRepeat(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object SetRepeatBeginSync(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRepeatBegin(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionSetRepeat.SetRepeatBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRepeatEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionSetRepeat.SetRepeatEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSetRepeat.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionRepeat

        public class AsyncActionRepeat
        {
            internal AsyncActionRepeat(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object RepeatBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RepeatBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionRepeat.RepeatBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RepeatEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionRepeat.RepeatEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionRepeat.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSetShuffle

        public class AsyncActionSetShuffle
        {
            internal AsyncActionSetShuffle(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SetShuffleBeginSync(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetShuffleBegin(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionSetShuffle.SetShuffleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetShuffleEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionSetShuffle.SetShuffleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSetShuffle.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionShuffle

        public class AsyncActionShuffle
        {
            internal AsyncActionShuffle(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object ShuffleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ShuffleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionShuffle.ShuffleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ShuffleEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionShuffle.ShuffleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionShuffle.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSeekSecondAbsolute

        public class AsyncActionSeekSecondAbsolute
        {
            internal AsyncActionSeekSecondAbsolute(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondAbsolute.SeekSecondAbsoluteBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondAbsolute.SeekSecondAbsoluteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondAbsolute.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSeekSecondRelative

        public class AsyncActionSeekSecondRelative
        {
            internal AsyncActionSeekSecondRelative(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondRelative.SeekSecondRelativeBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondRelative.SeekSecondRelativeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSeekSecondRelative.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSeekId

        public class AsyncActionSeekId
        {
            internal AsyncActionSeekId(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object SeekIdBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SeekIdBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionSeekId.SeekIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SeekIdEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekId.SeekIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSeekId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionSeekIndex

        public class AsyncActionSeekIndex
        {
            internal AsyncActionSeekIndex(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SeekIndexBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SeekIndexBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionSeekIndex.SeekIndexBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SeekIndexEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionSeekIndex.SeekIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionSeekIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionTransportState

        public class AsyncActionTransportState
        {
            internal AsyncActionTransportState(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
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
                    UserLog.WriteLine("Playlist.AsyncActionTransportState.TransportStateBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionTransportState.TransportStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionTransportState.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionId

        public class AsyncActionId
        {
            internal AsyncActionId(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
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
                    UserLog.WriteLine("Playlist.AsyncActionId.IdBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionId.IdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionRead

        public class AsyncActionRead
        {
            internal AsyncActionRead(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
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
                    UserLog.WriteLine("Playlist.AsyncActionRead.ReadBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionRead.ReadEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionRead.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionReadList

        public class AsyncActionReadList
        {
            internal AsyncActionReadList(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
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
                    UserLog.WriteLine("Playlist.AsyncActionReadList.ReadListBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionReadList.ReadListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionReadList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    TrackList = aHandler.ReadArgumentString("TrackList");
                }
                
                public string TrackList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionInsert

        public class AsyncActionInsert
        {
            internal AsyncActionInsert(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object InsertBeginSync(uint AfterId, string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("AfterId", AfterId);           
                iHandler.WriteArgumentString("Uri", Uri);           
                iHandler.WriteArgumentString("Metadata", Metadata);           
                
                return (iHandler.WriteEnd(null));
            }

            public void InsertBegin(uint AfterId, string Uri, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("AfterId", AfterId);                
                iHandler.WriteArgumentString("Uri", Uri);                
                iHandler.WriteArgumentString("Metadata", Metadata);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionInsert.InsertBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse InsertEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionInsert.InsertEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionInsert.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NewId = aHandler.ReadArgumentUint("NewId");
                }
                
                public uint NewId;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionDeleteId

        public class AsyncActionDeleteId
        {
            internal AsyncActionDeleteId(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object DeleteIdBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DeleteIdBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionDeleteId.DeleteIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeleteIdEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionDeleteId.DeleteIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionDeleteId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionDeleteAll

        public class AsyncActionDeleteAll
        {
            internal AsyncActionDeleteAll(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object DeleteAllBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DeleteAllBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionDeleteAll.DeleteAllBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeleteAllEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionDeleteAll.DeleteAllEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionDeleteAll.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionTracksMax

        public class AsyncActionTracksMax
        {
            internal AsyncActionTracksMax(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(20));
                iService = aService;
            }

            internal object TracksMaxBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TracksMaxBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Playlist.AsyncActionTracksMax.TracksMaxBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TracksMaxEnd(object aResult)
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
                    UserLog.WriteLine("Playlist.AsyncActionTracksMax.TracksMaxEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionTracksMax.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionIdArray

        public class AsyncActionIdArray
        {
            internal AsyncActionIdArray(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(21));
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
                    UserLog.WriteLine("Playlist.AsyncActionIdArray.IdArrayBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionIdArray.IdArrayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionIdArray.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionIdArrayChanged

        public class AsyncActionIdArrayChanged
        {
            internal AsyncActionIdArrayChanged(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(22));
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
                    UserLog.WriteLine("Playlist.AsyncActionIdArrayChanged.IdArrayChangedBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionIdArrayChanged.IdArrayChangedEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionIdArrayChanged.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
        }
        
        
        // AsyncActionProtocolInfo

        public class AsyncActionProtocolInfo
        {
            internal AsyncActionProtocolInfo(ServicePlaylist aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(23));
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
                    UserLog.WriteLine("Playlist.AsyncActionProtocolInfo.ProtocolInfoBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Playlist.AsyncActionProtocolInfo.ProtocolInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Playlist.AsyncActionProtocolInfo.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylist iService;
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
				    UserLog.WriteLine("EventServerEvent(ServicePlaylist): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventRepeat = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Repeat", nsmanager);

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
	                Repeat = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Repeat = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Repeat = false; 
    	            }
                }

                eventRepeat = true;
            }

            bool eventShuffle = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Shuffle", nsmanager);

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
	                Shuffle = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Shuffle = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Shuffle = false; 
    	            }
                }

                eventShuffle = true;
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

            bool eventTracksMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TracksMax", nsmanager);

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
					TracksMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse TracksMax with value {1}", DateTime.Now, value));
				}

                eventTracksMax = true;
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
            
            if(eventRepeat)
            {
                if (EventStateRepeat != null)
                {
					try
					{
						EventStateRepeat(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateRepeat: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventShuffle)
            {
                if (EventStateShuffle != null)
                {
					try
					{
						EventStateShuffle(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateShuffle: " + ex);
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
            
            if(eventTracksMax)
            {
                if (EventStateTracksMax != null)
                {
					try
					{
						EventStateTracksMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTracksMax: " + ex);
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
        public event EventHandler<EventArgs> EventStateTransportState;
        public event EventHandler<EventArgs> EventStateRepeat;
        public event EventHandler<EventArgs> EventStateShuffle;
        public event EventHandler<EventArgs> EventStateId;
        public event EventHandler<EventArgs> EventStateIdArray;
        public event EventHandler<EventArgs> EventStateTracksMax;
        public event EventHandler<EventArgs> EventStateProtocolInfo;

        public string TransportState;
        public bool Repeat;
        public bool Shuffle;
        public uint Id;
        public byte[] IdArray;
        public uint TracksMax;
        public string ProtocolInfo;
    }
}
    
