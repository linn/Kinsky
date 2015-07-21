using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServicePlaylistManager : ServiceUpnp
    {


        public ServicePlaylistManager(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServicePlaylistManager(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Metadata");
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ImagesXml");
            action.AddOutArgument(new Argument("ImagesXml", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PlaylistReadArray");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Array", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("PlaylistReadList");
            action.AddInArgument(new Argument("IdList", Argument.EType.eString));
            action.AddOutArgument(new Argument("PlaylistList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PlaylistRead");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            action.AddOutArgument(new Argument("Description", Argument.EType.eString));
            action.AddOutArgument(new Argument("ImageId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistSetName");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("Name", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PlaylistSetDescription");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("Description", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PlaylistSetImageId");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("ImageId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistInsert");
            action.AddInArgument(new Argument("AfterId", Argument.EType.eUint));
            action.AddInArgument(new Argument("Name", Argument.EType.eString));
            action.AddInArgument(new Argument("Description", Argument.EType.eString));
            action.AddInArgument(new Argument("ImageId", Argument.EType.eUint));
            action.AddOutArgument(new Argument("NewId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistDeleteId");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistMove");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("AfterId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistsMax");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("TracksMax");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PlaylistArrays");
            action.AddOutArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("IdArray", Argument.EType.eBinary));
            action.AddOutArgument(new Argument("TokenArray", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("PlaylistArraysChanged");
            action.AddInArgument(new Argument("Token", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Read");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("TrackId", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ReadList");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("TrackIdList", Argument.EType.eString));
            action.AddOutArgument(new Argument("TrackList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Insert");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("AfterTrackId", Argument.EType.eUint));
            action.AddInArgument(new Argument("Metadata", Argument.EType.eString));
            action.AddOutArgument(new Argument("NewTrackId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DeleteId");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            action.AddInArgument(new Argument("TrackId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DeleteAll");
            action.AddInArgument(new Argument("Id", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "PlaylistManager", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "PlaylistManager", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionMetadata CreateAsyncActionMetadata()
        {
            return (new AsyncActionMetadata(this));
        }

        public AsyncActionImagesXml CreateAsyncActionImagesXml()
        {
            return (new AsyncActionImagesXml(this));
        }

        public AsyncActionPlaylistReadArray CreateAsyncActionPlaylistReadArray()
        {
            return (new AsyncActionPlaylistReadArray(this));
        }

        public AsyncActionPlaylistReadList CreateAsyncActionPlaylistReadList()
        {
            return (new AsyncActionPlaylistReadList(this));
        }

        public AsyncActionPlaylistRead CreateAsyncActionPlaylistRead()
        {
            return (new AsyncActionPlaylistRead(this));
        }

        public AsyncActionPlaylistSetName CreateAsyncActionPlaylistSetName()
        {
            return (new AsyncActionPlaylistSetName(this));
        }

        public AsyncActionPlaylistSetDescription CreateAsyncActionPlaylistSetDescription()
        {
            return (new AsyncActionPlaylistSetDescription(this));
        }

        public AsyncActionPlaylistSetImageId CreateAsyncActionPlaylistSetImageId()
        {
            return (new AsyncActionPlaylistSetImageId(this));
        }

        public AsyncActionPlaylistInsert CreateAsyncActionPlaylistInsert()
        {
            return (new AsyncActionPlaylistInsert(this));
        }

        public AsyncActionPlaylistDeleteId CreateAsyncActionPlaylistDeleteId()
        {
            return (new AsyncActionPlaylistDeleteId(this));
        }

        public AsyncActionPlaylistMove CreateAsyncActionPlaylistMove()
        {
            return (new AsyncActionPlaylistMove(this));
        }

        public AsyncActionPlaylistsMax CreateAsyncActionPlaylistsMax()
        {
            return (new AsyncActionPlaylistsMax(this));
        }

        public AsyncActionTracksMax CreateAsyncActionTracksMax()
        {
            return (new AsyncActionTracksMax(this));
        }

        public AsyncActionPlaylistArrays CreateAsyncActionPlaylistArrays()
        {
            return (new AsyncActionPlaylistArrays(this));
        }

        public AsyncActionPlaylistArraysChanged CreateAsyncActionPlaylistArraysChanged()
        {
            return (new AsyncActionPlaylistArraysChanged(this));
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


        // Synchronous actions
        
        public string MetadataSync()
        {
            AsyncActionMetadata action = CreateAsyncActionMetadata();
            
            object result = action.MetadataBeginSync();

            AsyncActionMetadata.EventArgsResponse response = action.MetadataEnd(result);
                
            return(response.Metadata);
        }
        
        public string ImagesXmlSync()
        {
            AsyncActionImagesXml action = CreateAsyncActionImagesXml();
            
            object result = action.ImagesXmlBeginSync();

            AsyncActionImagesXml.EventArgsResponse response = action.ImagesXmlEnd(result);
                
            return(response.ImagesXml);
        }
        
        public byte[] PlaylistReadArraySync(uint Id)
        {
            AsyncActionPlaylistReadArray action = CreateAsyncActionPlaylistReadArray();
            
            object result = action.PlaylistReadArrayBeginSync(Id);

            AsyncActionPlaylistReadArray.EventArgsResponse response = action.PlaylistReadArrayEnd(result);
                
            return(response.Array);
        }
        
        public string PlaylistReadListSync(string IdList)
        {
            AsyncActionPlaylistReadList action = CreateAsyncActionPlaylistReadList();
            
            object result = action.PlaylistReadListBeginSync(IdList);

            AsyncActionPlaylistReadList.EventArgsResponse response = action.PlaylistReadListEnd(result);
                
            return(response.PlaylistList);
        }
        
        public void PlaylistReadSync(uint Id, out string Name, out string Description, out uint ImageId)
        {
            AsyncActionPlaylistRead action = CreateAsyncActionPlaylistRead();
            
            object result = action.PlaylistReadBeginSync(Id);

            AsyncActionPlaylistRead.EventArgsResponse response = action.PlaylistReadEnd(result);
                
            Name = response.Name;
            Description = response.Description;
            ImageId = response.ImageId;
        }
        
        public void PlaylistSetNameSync(uint Id, string Name)
        {
            AsyncActionPlaylistSetName action = CreateAsyncActionPlaylistSetName();
            
            object result = action.PlaylistSetNameBeginSync(Id, Name);

            action.PlaylistSetNameEnd(result);
        }
        
        public void PlaylistSetDescriptionSync(uint Id, string Description)
        {
            AsyncActionPlaylistSetDescription action = CreateAsyncActionPlaylistSetDescription();
            
            object result = action.PlaylistSetDescriptionBeginSync(Id, Description);

            action.PlaylistSetDescriptionEnd(result);
        }
        
        public void PlaylistSetImageIdSync(uint Id, uint ImageId)
        {
            AsyncActionPlaylistSetImageId action = CreateAsyncActionPlaylistSetImageId();
            
            object result = action.PlaylistSetImageIdBeginSync(Id, ImageId);

            action.PlaylistSetImageIdEnd(result);
        }
        
        public uint PlaylistInsertSync(uint AfterId, string Name, string Description, uint ImageId)
        {
            AsyncActionPlaylistInsert action = CreateAsyncActionPlaylistInsert();
            
            object result = action.PlaylistInsertBeginSync(AfterId, Name, Description, ImageId);

            AsyncActionPlaylistInsert.EventArgsResponse response = action.PlaylistInsertEnd(result);
                
            return(response.NewId);
        }
        
        public void PlaylistDeleteIdSync(uint Value)
        {
            AsyncActionPlaylistDeleteId action = CreateAsyncActionPlaylistDeleteId();
            
            object result = action.PlaylistDeleteIdBeginSync(Value);

            action.PlaylistDeleteIdEnd(result);
        }
        
        public void PlaylistMoveSync(uint Id, uint AfterId)
        {
            AsyncActionPlaylistMove action = CreateAsyncActionPlaylistMove();
            
            object result = action.PlaylistMoveBeginSync(Id, AfterId);

            action.PlaylistMoveEnd(result);
        }
        
        public uint PlaylistsMaxSync()
        {
            AsyncActionPlaylistsMax action = CreateAsyncActionPlaylistsMax();
            
            object result = action.PlaylistsMaxBeginSync();

            AsyncActionPlaylistsMax.EventArgsResponse response = action.PlaylistsMaxEnd(result);
                
            return(response.Value);
        }
        
        public uint TracksMaxSync()
        {
            AsyncActionTracksMax action = CreateAsyncActionTracksMax();
            
            object result = action.TracksMaxBeginSync();

            AsyncActionTracksMax.EventArgsResponse response = action.TracksMaxEnd(result);
                
            return(response.Value);
        }
        
        public void PlaylistArraysSync(out uint Token, out byte[] IdArray, out byte[] TokenArray)
        {
            AsyncActionPlaylistArrays action = CreateAsyncActionPlaylistArrays();
            
            object result = action.PlaylistArraysBeginSync();

            AsyncActionPlaylistArrays.EventArgsResponse response = action.PlaylistArraysEnd(result);
                
            Token = response.Token;
            IdArray = response.IdArray;
            TokenArray = response.TokenArray;
        }
        
        public bool PlaylistArraysChangedSync(uint Token)
        {
            AsyncActionPlaylistArraysChanged action = CreateAsyncActionPlaylistArraysChanged();
            
            object result = action.PlaylistArraysChangedBeginSync(Token);

            AsyncActionPlaylistArraysChanged.EventArgsResponse response = action.PlaylistArraysChangedEnd(result);
                
            return(response.Value);
        }
        
        public string ReadSync(uint Id, uint TrackId)
        {
            AsyncActionRead action = CreateAsyncActionRead();
            
            object result = action.ReadBeginSync(Id, TrackId);

            AsyncActionRead.EventArgsResponse response = action.ReadEnd(result);
                
            return(response.Metadata);
        }
        
        public string ReadListSync(uint Id, string TrackIdList)
        {
            AsyncActionReadList action = CreateAsyncActionReadList();
            
            object result = action.ReadListBeginSync(Id, TrackIdList);

            AsyncActionReadList.EventArgsResponse response = action.ReadListEnd(result);
                
            return(response.TrackList);
        }
        
        public uint InsertSync(uint Id, uint AfterTrackId, string Metadata)
        {
            AsyncActionInsert action = CreateAsyncActionInsert();
            
            object result = action.InsertBeginSync(Id, AfterTrackId, Metadata);

            AsyncActionInsert.EventArgsResponse response = action.InsertEnd(result);
                
            return(response.NewTrackId);
        }
        
        public void DeleteIdSync(uint Id, uint TrackId)
        {
            AsyncActionDeleteId action = CreateAsyncActionDeleteId();
            
            object result = action.DeleteIdBeginSync(Id, TrackId);

            action.DeleteIdEnd(result);
        }
        
        public void DeleteAllSync(uint Id)
        {
            AsyncActionDeleteAll action = CreateAsyncActionDeleteAll();
            
            object result = action.DeleteAllBeginSync(Id);

            action.DeleteAllEnd(result);
        }
        

        // AsyncActionMetadata

        public class AsyncActionMetadata
        {
            internal AsyncActionMetadata(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object MetadataBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MetadataBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionMetadata.MetadataBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MetadataEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionMetadata.MetadataEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionMetadata.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionImagesXml

        public class AsyncActionImagesXml
        {
            internal AsyncActionImagesXml(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object ImagesXmlBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ImagesXmlBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionImagesXml.ImagesXmlBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ImagesXmlEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionImagesXml.ImagesXmlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionImagesXml.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ImagesXml = aHandler.ReadArgumentString("ImagesXml");
                }
                
                public string ImagesXml;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistReadArray

        public class AsyncActionPlaylistReadArray
        {
            internal AsyncActionPlaylistReadArray(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object PlaylistReadArrayBeginSync(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistReadArrayBegin(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadArray.PlaylistReadArrayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistReadArrayEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadArray.PlaylistReadArrayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadArray.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Array = aHandler.ReadArgumentBinary("Array");
                }
                
                public byte[] Array;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistReadList

        public class AsyncActionPlaylistReadList
        {
            internal AsyncActionPlaylistReadList(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object PlaylistReadListBeginSync(string IdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("IdList", IdList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistReadListBegin(string IdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("IdList", IdList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadList.PlaylistReadListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistReadListEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadList.PlaylistReadListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistReadList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    PlaylistList = aHandler.ReadArgumentString("PlaylistList");
                }
                
                public string PlaylistList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistRead

        public class AsyncActionPlaylistRead
        {
            internal AsyncActionPlaylistRead(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object PlaylistReadBeginSync(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistReadBegin(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistRead.PlaylistReadBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistReadEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistRead.PlaylistReadEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistRead.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Name = aHandler.ReadArgumentString("Name");
                    Description = aHandler.ReadArgumentString("Description");
                    ImageId = aHandler.ReadArgumentUint("ImageId");
                }
                
                public string Name;
                public string Description;
                public uint ImageId;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistSetName

        public class AsyncActionPlaylistSetName
        {
            internal AsyncActionPlaylistSetName(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object PlaylistSetNameBeginSync(uint Id, string Name)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentString("Name", Name);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistSetNameBegin(uint Id, string Name)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentString("Name", Name);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetName.PlaylistSetNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistSetNameEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetName.PlaylistSetNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistSetDescription

        public class AsyncActionPlaylistSetDescription
        {
            internal AsyncActionPlaylistSetDescription(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object PlaylistSetDescriptionBeginSync(uint Id, string Description)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentString("Description", Description);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistSetDescriptionBegin(uint Id, string Description)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentString("Description", Description);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetDescription.PlaylistSetDescriptionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistSetDescriptionEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetDescription.PlaylistSetDescriptionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetDescription.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistSetImageId

        public class AsyncActionPlaylistSetImageId
        {
            internal AsyncActionPlaylistSetImageId(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object PlaylistSetImageIdBeginSync(uint Id, uint ImageId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentUint("ImageId", ImageId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistSetImageIdBegin(uint Id, uint ImageId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentUint("ImageId", ImageId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetImageId.PlaylistSetImageIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistSetImageIdEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetImageId.PlaylistSetImageIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistSetImageId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistInsert

        public class AsyncActionPlaylistInsert
        {
            internal AsyncActionPlaylistInsert(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object PlaylistInsertBeginSync(uint AfterId, string Name, string Description, uint ImageId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("AfterId", AfterId);           
                iHandler.WriteArgumentString("Name", Name);           
                iHandler.WriteArgumentString("Description", Description);           
                iHandler.WriteArgumentUint("ImageId", ImageId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistInsertBegin(uint AfterId, string Name, string Description, uint ImageId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("AfterId", AfterId);                
                iHandler.WriteArgumentString("Name", Name);                
                iHandler.WriteArgumentString("Description", Description);                
                iHandler.WriteArgumentUint("ImageId", ImageId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistInsert.PlaylistInsertBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistInsertEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistInsert.PlaylistInsertEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistInsert.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistDeleteId

        public class AsyncActionPlaylistDeleteId
        {
            internal AsyncActionPlaylistDeleteId(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object PlaylistDeleteIdBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistDeleteIdBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistDeleteId.PlaylistDeleteIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistDeleteIdEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistDeleteId.PlaylistDeleteIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistDeleteId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistMove

        public class AsyncActionPlaylistMove
        {
            internal AsyncActionPlaylistMove(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object PlaylistMoveBeginSync(uint Id, uint AfterId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentUint("AfterId", AfterId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistMoveBegin(uint Id, uint AfterId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentUint("AfterId", AfterId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistMove.PlaylistMoveBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistMoveEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistMove.PlaylistMoveEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistMove.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistsMax

        public class AsyncActionPlaylistsMax
        {
            internal AsyncActionPlaylistsMax(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object PlaylistsMaxBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistsMaxBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistsMax.PlaylistsMaxBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistsMaxEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistsMax.PlaylistsMaxEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistsMax.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionTracksMax

        public class AsyncActionTracksMax
        {
            internal AsyncActionTracksMax(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionTracksMax.TracksMaxBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionTracksMax.TracksMaxEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionTracksMax.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistArrays

        public class AsyncActionPlaylistArrays
        {
            internal AsyncActionPlaylistArrays(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object PlaylistArraysBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistArraysBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArrays.PlaylistArraysBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistArraysEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArrays.PlaylistArraysEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArrays.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Token = aHandler.ReadArgumentUint("Token");
                    IdArray = aHandler.ReadArgumentBinary("IdArray");
                    TokenArray = aHandler.ReadArgumentBinary("TokenArray");
                }
                
                public uint Token;
                public byte[] IdArray;
                public byte[] TokenArray;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionPlaylistArraysChanged

        public class AsyncActionPlaylistArraysChanged
        {
            internal AsyncActionPlaylistArraysChanged(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object PlaylistArraysChangedBeginSync(uint Token)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Token", Token);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlaylistArraysChangedBegin(uint Token)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Token", Token);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArraysChanged.PlaylistArraysChangedBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlaylistArraysChangedEnd(object aResult)
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArraysChanged.PlaylistArraysChangedEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionPlaylistArraysChanged.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionRead

        public class AsyncActionRead
        {
            internal AsyncActionRead(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object ReadBeginSync(uint Id, uint TrackId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentUint("TrackId", TrackId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReadBegin(uint Id, uint TrackId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentUint("TrackId", TrackId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionRead.ReadBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionRead.ReadEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionRead.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionReadList

        public class AsyncActionReadList
        {
            internal AsyncActionReadList(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object ReadListBeginSync(uint Id, string TrackIdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentString("TrackIdList", TrackIdList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReadListBegin(uint Id, string TrackIdList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentString("TrackIdList", TrackIdList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionReadList.ReadListBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionReadList.ReadListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionReadList.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionInsert

        public class AsyncActionInsert
        {
            internal AsyncActionInsert(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object InsertBeginSync(uint Id, uint AfterTrackId, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentUint("AfterTrackId", AfterTrackId);           
                iHandler.WriteArgumentString("Metadata", Metadata);           
                
                return (iHandler.WriteEnd(null));
            }

            public void InsertBegin(uint Id, uint AfterTrackId, string Metadata)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentUint("AfterTrackId", AfterTrackId);                
                iHandler.WriteArgumentString("Metadata", Metadata);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionInsert.InsertBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionInsert.InsertEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionInsert.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NewTrackId = aHandler.ReadArgumentUint("NewTrackId");
                }
                
                public uint NewTrackId;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionDeleteId

        public class AsyncActionDeleteId
        {
            internal AsyncActionDeleteId(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object DeleteIdBeginSync(uint Id, uint TrackId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                iHandler.WriteArgumentUint("TrackId", TrackId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DeleteIdBegin(uint Id, uint TrackId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                iHandler.WriteArgumentUint("TrackId", TrackId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteId.DeleteIdBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteId.DeleteIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteId.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
        }
        
        
        // AsyncActionDeleteAll

        public class AsyncActionDeleteAll
        {
            internal AsyncActionDeleteAll(ServicePlaylistManager aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object DeleteAllBeginSync(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DeleteAllBegin(uint Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteAll.DeleteAllBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteAll.DeleteAllEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("PlaylistManager.AsyncActionDeleteAll.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServicePlaylistManager iService;
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
				    UserLog.WriteLine("EventServerEvent(ServicePlaylistManager): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventImagesXml = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ImagesXml", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ImagesXml = value;

                eventImagesXml = true;
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

            bool eventTokenArray = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TokenArray", nsmanager);

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
				    TokenArray =  new byte[0];
				}
				else
				{
                    TokenArray = Convert.FromBase64String(value);
                }

                eventTokenArray = true;
            }

            bool eventPlaylistsMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PlaylistsMax", nsmanager);

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
					PlaylistsMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse PlaylistsMax with value {1}", DateTime.Now, value));
				}

                eventPlaylistsMax = true;
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
            
            if(eventImagesXml)
            {
                if (EventStateImagesXml != null)
                {
					try
					{
						EventStateImagesXml(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateImagesXml: " + ex);
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
            
            if(eventTokenArray)
            {
                if (EventStateTokenArray != null)
                {
					try
					{
						EventStateTokenArray(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTokenArray: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventPlaylistsMax)
            {
                if (EventStatePlaylistsMax != null)
                {
					try
					{
						EventStatePlaylistsMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePlaylistsMax: " + ex);
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
        public event EventHandler<EventArgs> EventStateMetadata;
        public event EventHandler<EventArgs> EventStateImagesXml;
        public event EventHandler<EventArgs> EventStateIdArray;
        public event EventHandler<EventArgs> EventStateTokenArray;
        public event EventHandler<EventArgs> EventStatePlaylistsMax;
        public event EventHandler<EventArgs> EventStateTracksMax;

        public string Metadata;
        public string ImagesXml;
        public byte[] IdArray;
        public byte[] TokenArray;
        public uint PlaylistsMax;
        public uint TracksMax;
    }
}
    
