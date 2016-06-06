using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceJukebox : ServiceUpnp
    {


        public ServiceJukebox(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceJukebox(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetCurrentPreset");
            action.AddInArgument(new Argument("aPreset", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("CurrentPreset");
            action.AddOutArgument(new Argument("aPreset", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetCurrentBookmark");
            action.AddInArgument(new Argument("aBookmark", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("CurrentBookmark");
            action.AddOutArgument(new Argument("aBookmark", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PresetMetadata");
            action.AddInArgument(new Argument("aPreset", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aMetadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("BookmarkMetadata");
            action.AddInArgument(new Argument("aBookmark", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aMetadata", Argument.EType.eString));
            action.AddOutArgument(new Argument("aFirstPreset", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("PresetMetadataList");
            action.AddInArgument(new Argument("aStartPreset", Argument.EType.eUint));
            action.AddInArgument(new Argument("aEndPreset", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aMetadataList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("BookmarkMetadataList");
            action.AddInArgument(new Argument("aStartBookmark", Argument.EType.eUint));
            action.AddInArgument(new Argument("aEndBookmark", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aMetadataList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("LoadManifestFile");
            action.AddOutArgument(new Argument("aTotalPresets", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aTotalBookmarks", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Jukebox", 3));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Jukebox", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetCurrentPreset CreateAsyncActionSetCurrentPreset()
        {
            return (new AsyncActionSetCurrentPreset(this));
        }

        public AsyncActionCurrentPreset CreateAsyncActionCurrentPreset()
        {
            return (new AsyncActionCurrentPreset(this));
        }

        public AsyncActionSetCurrentBookmark CreateAsyncActionSetCurrentBookmark()
        {
            return (new AsyncActionSetCurrentBookmark(this));
        }

        public AsyncActionCurrentBookmark CreateAsyncActionCurrentBookmark()
        {
            return (new AsyncActionCurrentBookmark(this));
        }

        public AsyncActionPresetMetadata CreateAsyncActionPresetMetadata()
        {
            return (new AsyncActionPresetMetadata(this));
        }

        public AsyncActionBookmarkMetadata CreateAsyncActionBookmarkMetadata()
        {
            return (new AsyncActionBookmarkMetadata(this));
        }

        public AsyncActionPresetMetadataList CreateAsyncActionPresetMetadataList()
        {
            return (new AsyncActionPresetMetadataList(this));
        }

        public AsyncActionBookmarkMetadataList CreateAsyncActionBookmarkMetadataList()
        {
            return (new AsyncActionBookmarkMetadataList(this));
        }

        public AsyncActionLoadManifestFile CreateAsyncActionLoadManifestFile()
        {
            return (new AsyncActionLoadManifestFile(this));
        }


        // Synchronous actions
        
        public void SetCurrentPresetSync(uint aPreset)
        {
            AsyncActionSetCurrentPreset action = CreateAsyncActionSetCurrentPreset();
            
            object result = action.SetCurrentPresetBeginSync(aPreset);

            action.SetCurrentPresetEnd(result);
        }
        
        public uint CurrentPresetSync()
        {
            AsyncActionCurrentPreset action = CreateAsyncActionCurrentPreset();
            
            object result = action.CurrentPresetBeginSync();

            AsyncActionCurrentPreset.EventArgsResponse response = action.CurrentPresetEnd(result);
                
            return(response.aPreset);
        }
        
        public void SetCurrentBookmarkSync(uint aBookmark)
        {
            AsyncActionSetCurrentBookmark action = CreateAsyncActionSetCurrentBookmark();
            
            object result = action.SetCurrentBookmarkBeginSync(aBookmark);

            action.SetCurrentBookmarkEnd(result);
        }
        
        public uint CurrentBookmarkSync()
        {
            AsyncActionCurrentBookmark action = CreateAsyncActionCurrentBookmark();
            
            object result = action.CurrentBookmarkBeginSync();

            AsyncActionCurrentBookmark.EventArgsResponse response = action.CurrentBookmarkEnd(result);
                
            return(response.aBookmark);
        }
        
        public string PresetMetadataSync(uint aPreset)
        {
            AsyncActionPresetMetadata action = CreateAsyncActionPresetMetadata();
            
            object result = action.PresetMetadataBeginSync(aPreset);

            AsyncActionPresetMetadata.EventArgsResponse response = action.PresetMetadataEnd(result);
                
            return(response.aMetadata);
        }
        
        public void BookmarkMetadataSync(uint aBookmark, out string aMetadata, out uint aFirstPreset)
        {
            AsyncActionBookmarkMetadata action = CreateAsyncActionBookmarkMetadata();
            
            object result = action.BookmarkMetadataBeginSync(aBookmark);

            AsyncActionBookmarkMetadata.EventArgsResponse response = action.BookmarkMetadataEnd(result);
                
            aMetadata = response.aMetadata;
            aFirstPreset = response.aFirstPreset;
        }
        
        public string PresetMetadataListSync(uint aStartPreset, uint aEndPreset)
        {
            AsyncActionPresetMetadataList action = CreateAsyncActionPresetMetadataList();
            
            object result = action.PresetMetadataListBeginSync(aStartPreset, aEndPreset);

            AsyncActionPresetMetadataList.EventArgsResponse response = action.PresetMetadataListEnd(result);
                
            return(response.aMetadataList);
        }
        
        public string BookmarkMetadataListSync(uint aStartBookmark, uint aEndBookmark)
        {
            AsyncActionBookmarkMetadataList action = CreateAsyncActionBookmarkMetadataList();
            
            object result = action.BookmarkMetadataListBeginSync(aStartBookmark, aEndBookmark);

            AsyncActionBookmarkMetadataList.EventArgsResponse response = action.BookmarkMetadataListEnd(result);
                
            return(response.aMetadataList);
        }
        
        public void LoadManifestFileSync(out uint aTotalPresets, out uint aTotalBookmarks)
        {
            AsyncActionLoadManifestFile action = CreateAsyncActionLoadManifestFile();
            
            object result = action.LoadManifestFileBeginSync();

            AsyncActionLoadManifestFile.EventArgsResponse response = action.LoadManifestFileEnd(result);
                
            aTotalPresets = response.aTotalPresets;
            aTotalBookmarks = response.aTotalBookmarks;
        }
        

        // AsyncActionSetCurrentPreset

        public class AsyncActionSetCurrentPreset
        {
            internal AsyncActionSetCurrentPreset(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetCurrentPresetBeginSync(uint aPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPreset", aPreset);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetCurrentPresetBegin(uint aPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPreset", aPreset);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentPreset.SetCurrentPresetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetCurrentPresetEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentPreset.SetCurrentPresetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentPreset.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionCurrentPreset

        public class AsyncActionCurrentPreset
        {
            internal AsyncActionCurrentPreset(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object CurrentPresetBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CurrentPresetBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentPreset.CurrentPresetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CurrentPresetEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentPreset.CurrentPresetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentPreset.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aPreset = aHandler.ReadArgumentUint("aPreset");
                }
                
                public uint aPreset;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionSetCurrentBookmark

        public class AsyncActionSetCurrentBookmark
        {
            internal AsyncActionSetCurrentBookmark(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetCurrentBookmarkBeginSync(uint aBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aBookmark", aBookmark);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetCurrentBookmarkBegin(uint aBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aBookmark", aBookmark);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentBookmark.SetCurrentBookmarkBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetCurrentBookmarkEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentBookmark.SetCurrentBookmarkEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionSetCurrentBookmark.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionCurrentBookmark

        public class AsyncActionCurrentBookmark
        {
            internal AsyncActionCurrentBookmark(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object CurrentBookmarkBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CurrentBookmarkBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentBookmark.CurrentBookmarkBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CurrentBookmarkEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentBookmark.CurrentBookmarkEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionCurrentBookmark.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBookmark = aHandler.ReadArgumentUint("aBookmark");
                }
                
                public uint aBookmark;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionPresetMetadata

        public class AsyncActionPresetMetadata
        {
            internal AsyncActionPresetMetadata(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object PresetMetadataBeginSync(uint aPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPreset", aPreset);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PresetMetadataBegin(uint aPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPreset", aPreset);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadata.PresetMetadataBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PresetMetadataEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadata.PresetMetadataEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadata.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMetadata = aHandler.ReadArgumentString("aMetadata");
                }
                
                public string aMetadata;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionBookmarkMetadata

        public class AsyncActionBookmarkMetadata
        {
            internal AsyncActionBookmarkMetadata(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object BookmarkMetadataBeginSync(uint aBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aBookmark", aBookmark);           
                
                return (iHandler.WriteEnd(null));
            }

            public void BookmarkMetadataBegin(uint aBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aBookmark", aBookmark);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadata.BookmarkMetadataBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BookmarkMetadataEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadata.BookmarkMetadataEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadata.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMetadata = aHandler.ReadArgumentString("aMetadata");
                    aFirstPreset = aHandler.ReadArgumentUint("aFirstPreset");
                }
                
                public string aMetadata;
                public uint aFirstPreset;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionPresetMetadataList

        public class AsyncActionPresetMetadataList
        {
            internal AsyncActionPresetMetadataList(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object PresetMetadataListBeginSync(uint aStartPreset, uint aEndPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStartPreset", aStartPreset);           
                iHandler.WriteArgumentUint("aEndPreset", aEndPreset);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PresetMetadataListBegin(uint aStartPreset, uint aEndPreset)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStartPreset", aStartPreset);                
                iHandler.WriteArgumentUint("aEndPreset", aEndPreset);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadataList.PresetMetadataListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PresetMetadataListEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadataList.PresetMetadataListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionPresetMetadataList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMetadataList = aHandler.ReadArgumentString("aMetadataList");
                }
                
                public string aMetadataList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionBookmarkMetadataList

        public class AsyncActionBookmarkMetadataList
        {
            internal AsyncActionBookmarkMetadataList(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object BookmarkMetadataListBeginSync(uint aStartBookmark, uint aEndBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStartBookmark", aStartBookmark);           
                iHandler.WriteArgumentUint("aEndBookmark", aEndBookmark);           
                
                return (iHandler.WriteEnd(null));
            }

            public void BookmarkMetadataListBegin(uint aStartBookmark, uint aEndBookmark)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStartBookmark", aStartBookmark);                
                iHandler.WriteArgumentUint("aEndBookmark", aEndBookmark);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadataList.BookmarkMetadataListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BookmarkMetadataListEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadataList.BookmarkMetadataListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionBookmarkMetadataList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aMetadataList = aHandler.ReadArgumentString("aMetadataList");
                }
                
                public string aMetadataList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
        }
        
        
        // AsyncActionLoadManifestFile

        public class AsyncActionLoadManifestFile
        {
            internal AsyncActionLoadManifestFile(ServiceJukebox aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object LoadManifestFileBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void LoadManifestFileBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Jukebox.AsyncActionLoadManifestFile.LoadManifestFileBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse LoadManifestFileEnd(object aResult)
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
                    UserLog.WriteLine("Jukebox.AsyncActionLoadManifestFile.LoadManifestFileEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Jukebox.AsyncActionLoadManifestFile.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTotalPresets = aHandler.ReadArgumentUint("aTotalPresets");
                    aTotalBookmarks = aHandler.ReadArgumentUint("aTotalBookmarks");
                }
                
                public uint aTotalPresets;
                public uint aTotalBookmarks;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceJukebox iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceJukebox): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventCurrentPreset = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "CurrentPreset", nsmanager);

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
					CurrentPreset = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse CurrentPreset with value {1}", DateTime.Now, value));
				}

                eventCurrentPreset = true;
            }

            bool eventCurrentBookmark = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "CurrentBookmark", nsmanager);

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
					CurrentBookmark = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse CurrentBookmark with value {1}", DateTime.Now, value));
				}

                eventCurrentBookmark = true;
            }

          
            
            if(eventCurrentPreset)
            {
                if (EventStateCurrentPreset != null)
                {
					try
					{
						EventStateCurrentPreset(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateCurrentPreset: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventCurrentBookmark)
            {
                if (EventStateCurrentBookmark != null)
                {
					try
					{
						EventStateCurrentBookmark(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateCurrentBookmark: " + ex);
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
        public event EventHandler<EventArgs> EventStateCurrentPreset;
        public event EventHandler<EventArgs> EventStateCurrentBookmark;

        public uint CurrentPreset;
        public uint CurrentBookmark;
    }
}
    
