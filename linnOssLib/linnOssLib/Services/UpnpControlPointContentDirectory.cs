using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceContentDirectory : ServiceUpnp
    {

        public const string kBrowseFlagBrowseMetadata = "BrowseMetadata";
        public const string kBrowseFlagBrowseDirectChildren = "BrowseDirectChildren";
        public const string kTransferStatusCompleted = "COMPLETED";
        public const string kTransferStatusError = "ERROR";
        public const string kTransferStatusInProgress = "IN_PROGRESS";
        public const string kTransferStatusStopped = "STOPPED";

        public ServiceContentDirectory(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceContentDirectory(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("GetSearchCapabilities");
            action.AddOutArgument(new Argument("SearchCaps", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetSortCapabilities");
            action.AddOutArgument(new Argument("SortCaps", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetSortExtensionCapabilities");
            action.AddOutArgument(new Argument("SortExtensionCaps", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetFeatureList");
            action.AddOutArgument(new Argument("FeatureList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetSystemUpdateID");
            action.AddOutArgument(new Argument("Id", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Browse");
            action.AddInArgument(new Argument("ObjectID", Argument.EType.eString));
            action.AddInArgument(new Argument("BrowseFlag", Argument.EType.eString));
            action.AddInArgument(new Argument("Filter", Argument.EType.eString));
            action.AddInArgument(new Argument("StartingIndex", Argument.EType.eUint));
            action.AddInArgument(new Argument("RequestedCount", Argument.EType.eUint));
            action.AddInArgument(new Argument("SortCriteria", Argument.EType.eString));
            action.AddOutArgument(new Argument("Result", Argument.EType.eString));
            action.AddOutArgument(new Argument("NumberReturned", Argument.EType.eUint));
            action.AddOutArgument(new Argument("TotalMatches", Argument.EType.eUint));
            action.AddOutArgument(new Argument("UpdateID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Search");
            action.AddInArgument(new Argument("ContainerID", Argument.EType.eString));
            action.AddInArgument(new Argument("SearchCriteria", Argument.EType.eString));
            action.AddInArgument(new Argument("Filter", Argument.EType.eString));
            action.AddInArgument(new Argument("StartingIndex", Argument.EType.eUint));
            action.AddInArgument(new Argument("RequestedCount", Argument.EType.eUint));
            action.AddInArgument(new Argument("SortCriteria", Argument.EType.eString));
            action.AddOutArgument(new Argument("Result", Argument.EType.eString));
            action.AddOutArgument(new Argument("NumberReturned", Argument.EType.eUint));
            action.AddOutArgument(new Argument("TotalMatches", Argument.EType.eUint));
            action.AddOutArgument(new Argument("UpdateID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("CreateObject");
            action.AddInArgument(new Argument("ContainerID", Argument.EType.eString));
            action.AddInArgument(new Argument("Elements", Argument.EType.eString));
            action.AddOutArgument(new Argument("ObjectID", Argument.EType.eString));
            action.AddOutArgument(new Argument("Result", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DestroyObject");
            action.AddInArgument(new Argument("ObjectID", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("UpdateObject");
            action.AddInArgument(new Argument("ObjectID", Argument.EType.eString));
            action.AddInArgument(new Argument("CurrentTagValue", Argument.EType.eString));
            action.AddInArgument(new Argument("NewTagValue", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("MoveObject");
            action.AddInArgument(new Argument("ObjectID", Argument.EType.eString));
            action.AddInArgument(new Argument("NewParentID", Argument.EType.eString));
            action.AddOutArgument(new Argument("NewObjectID", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ImportResource");
            action.AddInArgument(new Argument("SourceURI", Argument.EType.eString));
            action.AddInArgument(new Argument("DestinationURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("TransferID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("ExportResource");
            action.AddInArgument(new Argument("SourceURI", Argument.EType.eString));
            action.AddInArgument(new Argument("DestinationURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("TransferID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("StopTransferResource");
            action.AddInArgument(new Argument("TransferID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DeleteResource");
            action.AddInArgument(new Argument("ResourceURI", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetTransferProgress");
            action.AddInArgument(new Argument("TransferID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("TransferStatus", Argument.EType.eString));
            action.AddOutArgument(new Argument("TransferLength", Argument.EType.eString));
            action.AddOutArgument(new Argument("TransferTotal", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("CreateReference");
            action.AddInArgument(new Argument("ContainerID", Argument.EType.eString));
            action.AddInArgument(new Argument("ObjectID", Argument.EType.eString));
            action.AddOutArgument(new Argument("NewID", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("upnp.org", "ContentDirectory", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("upnp.org", "ContentDirectory", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionGetSearchCapabilities CreateAsyncActionGetSearchCapabilities()
        {
            return (new AsyncActionGetSearchCapabilities(this));
        }

        public AsyncActionGetSortCapabilities CreateAsyncActionGetSortCapabilities()
        {
            return (new AsyncActionGetSortCapabilities(this));
        }

        public AsyncActionGetSortExtensionCapabilities CreateAsyncActionGetSortExtensionCapabilities()
        {
            return (new AsyncActionGetSortExtensionCapabilities(this));
        }

        public AsyncActionGetFeatureList CreateAsyncActionGetFeatureList()
        {
            return (new AsyncActionGetFeatureList(this));
        }

        public AsyncActionGetSystemUpdateID CreateAsyncActionGetSystemUpdateID()
        {
            return (new AsyncActionGetSystemUpdateID(this));
        }

        public AsyncActionBrowse CreateAsyncActionBrowse()
        {
            return (new AsyncActionBrowse(this));
        }

        public AsyncActionSearch CreateAsyncActionSearch()
        {
            return (new AsyncActionSearch(this));
        }

        public AsyncActionCreateObject CreateAsyncActionCreateObject()
        {
            return (new AsyncActionCreateObject(this));
        }

        public AsyncActionDestroyObject CreateAsyncActionDestroyObject()
        {
            return (new AsyncActionDestroyObject(this));
        }

        public AsyncActionUpdateObject CreateAsyncActionUpdateObject()
        {
            return (new AsyncActionUpdateObject(this));
        }

        public AsyncActionMoveObject CreateAsyncActionMoveObject()
        {
            return (new AsyncActionMoveObject(this));
        }

        public AsyncActionImportResource CreateAsyncActionImportResource()
        {
            return (new AsyncActionImportResource(this));
        }

        public AsyncActionExportResource CreateAsyncActionExportResource()
        {
            return (new AsyncActionExportResource(this));
        }

        public AsyncActionStopTransferResource CreateAsyncActionStopTransferResource()
        {
            return (new AsyncActionStopTransferResource(this));
        }

        public AsyncActionDeleteResource CreateAsyncActionDeleteResource()
        {
            return (new AsyncActionDeleteResource(this));
        }

        public AsyncActionGetTransferProgress CreateAsyncActionGetTransferProgress()
        {
            return (new AsyncActionGetTransferProgress(this));
        }

        public AsyncActionCreateReference CreateAsyncActionCreateReference()
        {
            return (new AsyncActionCreateReference(this));
        }


        // Synchronous actions
        
        public string GetSearchCapabilitiesSync()
        {
            AsyncActionGetSearchCapabilities action = CreateAsyncActionGetSearchCapabilities();
            
            object result = action.GetSearchCapabilitiesBeginSync();

            AsyncActionGetSearchCapabilities.EventArgsResponse response = action.GetSearchCapabilitiesEnd(result);
                
            return(response.SearchCaps);
        }
        
        public string GetSortCapabilitiesSync()
        {
            AsyncActionGetSortCapabilities action = CreateAsyncActionGetSortCapabilities();
            
            object result = action.GetSortCapabilitiesBeginSync();

            AsyncActionGetSortCapabilities.EventArgsResponse response = action.GetSortCapabilitiesEnd(result);
                
            return(response.SortCaps);
        }
        
        public string GetSortExtensionCapabilitiesSync()
        {
            AsyncActionGetSortExtensionCapabilities action = CreateAsyncActionGetSortExtensionCapabilities();
            
            object result = action.GetSortExtensionCapabilitiesBeginSync();

            AsyncActionGetSortExtensionCapabilities.EventArgsResponse response = action.GetSortExtensionCapabilitiesEnd(result);
                
            return(response.SortExtensionCaps);
        }
        
        public string GetFeatureListSync()
        {
            AsyncActionGetFeatureList action = CreateAsyncActionGetFeatureList();
            
            object result = action.GetFeatureListBeginSync();

            AsyncActionGetFeatureList.EventArgsResponse response = action.GetFeatureListEnd(result);
                
            return(response.FeatureList);
        }
        
        public uint GetSystemUpdateIDSync()
        {
            AsyncActionGetSystemUpdateID action = CreateAsyncActionGetSystemUpdateID();
            
            object result = action.GetSystemUpdateIDBeginSync();

            AsyncActionGetSystemUpdateID.EventArgsResponse response = action.GetSystemUpdateIDEnd(result);
                
            return(response.Id);
        }
        
        public void BrowseSync(string ObjectID, string BrowseFlag, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria, out string Result, out uint NumberReturned, out uint TotalMatches, out uint UpdateID)
        {
            AsyncActionBrowse action = CreateAsyncActionBrowse();
            
            object result = action.BrowseBeginSync(ObjectID, BrowseFlag, Filter, StartingIndex, RequestedCount, SortCriteria);

            AsyncActionBrowse.EventArgsResponse response = action.BrowseEnd(result);
                
            Result = response.Result;
            NumberReturned = response.NumberReturned;
            TotalMatches = response.TotalMatches;
            UpdateID = response.UpdateID;
        }
        
        public void SearchSync(string ContainerID, string SearchCriteria, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria, out string Result, out uint NumberReturned, out uint TotalMatches, out uint UpdateID)
        {
            AsyncActionSearch action = CreateAsyncActionSearch();
            
            object result = action.SearchBeginSync(ContainerID, SearchCriteria, Filter, StartingIndex, RequestedCount, SortCriteria);

            AsyncActionSearch.EventArgsResponse response = action.SearchEnd(result);
                
            Result = response.Result;
            NumberReturned = response.NumberReturned;
            TotalMatches = response.TotalMatches;
            UpdateID = response.UpdateID;
        }
        
        public void CreateObjectSync(string ContainerID, string Elements, out string ObjectID, out string Result)
        {
            AsyncActionCreateObject action = CreateAsyncActionCreateObject();
            
            object result = action.CreateObjectBeginSync(ContainerID, Elements);

            AsyncActionCreateObject.EventArgsResponse response = action.CreateObjectEnd(result);
                
            ObjectID = response.ObjectID;
            Result = response.Result;
        }
        
        public void DestroyObjectSync(string ObjectID)
        {
            AsyncActionDestroyObject action = CreateAsyncActionDestroyObject();
            
            object result = action.DestroyObjectBeginSync(ObjectID);

            action.DestroyObjectEnd(result);
        }
        
        public void UpdateObjectSync(string ObjectID, string CurrentTagValue, string NewTagValue)
        {
            AsyncActionUpdateObject action = CreateAsyncActionUpdateObject();
            
            object result = action.UpdateObjectBeginSync(ObjectID, CurrentTagValue, NewTagValue);

            action.UpdateObjectEnd(result);
        }
        
        public string MoveObjectSync(string ObjectID, string NewParentID)
        {
            AsyncActionMoveObject action = CreateAsyncActionMoveObject();
            
            object result = action.MoveObjectBeginSync(ObjectID, NewParentID);

            AsyncActionMoveObject.EventArgsResponse response = action.MoveObjectEnd(result);
                
            return(response.NewObjectID);
        }
        
        public uint ImportResourceSync(string SourceURI, string DestinationURI)
        {
            AsyncActionImportResource action = CreateAsyncActionImportResource();
            
            object result = action.ImportResourceBeginSync(SourceURI, DestinationURI);

            AsyncActionImportResource.EventArgsResponse response = action.ImportResourceEnd(result);
                
            return(response.TransferID);
        }
        
        public uint ExportResourceSync(string SourceURI, string DestinationURI)
        {
            AsyncActionExportResource action = CreateAsyncActionExportResource();
            
            object result = action.ExportResourceBeginSync(SourceURI, DestinationURI);

            AsyncActionExportResource.EventArgsResponse response = action.ExportResourceEnd(result);
                
            return(response.TransferID);
        }
        
        public void StopTransferResourceSync(uint TransferID)
        {
            AsyncActionStopTransferResource action = CreateAsyncActionStopTransferResource();
            
            object result = action.StopTransferResourceBeginSync(TransferID);

            action.StopTransferResourceEnd(result);
        }
        
        public void DeleteResourceSync(string ResourceURI)
        {
            AsyncActionDeleteResource action = CreateAsyncActionDeleteResource();
            
            object result = action.DeleteResourceBeginSync(ResourceURI);

            action.DeleteResourceEnd(result);
        }
        
        public void GetTransferProgressSync(uint TransferID, out string TransferStatus, out string TransferLength, out string TransferTotal)
        {
            AsyncActionGetTransferProgress action = CreateAsyncActionGetTransferProgress();
            
            object result = action.GetTransferProgressBeginSync(TransferID);

            AsyncActionGetTransferProgress.EventArgsResponse response = action.GetTransferProgressEnd(result);
                
            TransferStatus = response.TransferStatus;
            TransferLength = response.TransferLength;
            TransferTotal = response.TransferTotal;
        }
        
        public string CreateReferenceSync(string ContainerID, string ObjectID)
        {
            AsyncActionCreateReference action = CreateAsyncActionCreateReference();
            
            object result = action.CreateReferenceBeginSync(ContainerID, ObjectID);

            AsyncActionCreateReference.EventArgsResponse response = action.CreateReferenceEnd(result);
                
            return(response.NewID);
        }
        

        // AsyncActionGetSearchCapabilities

        public class AsyncActionGetSearchCapabilities
        {
            internal AsyncActionGetSearchCapabilities(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object GetSearchCapabilitiesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSearchCapabilitiesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSearchCapabilities.GetSearchCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSearchCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSearchCapabilities.GetSearchCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSearchCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SearchCaps = aHandler.ReadArgumentString("SearchCaps");
                }
                
                public string SearchCaps;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionGetSortCapabilities

        public class AsyncActionGetSortCapabilities
        {
            internal AsyncActionGetSortCapabilities(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object GetSortCapabilitiesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSortCapabilitiesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortCapabilities.GetSortCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSortCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortCapabilities.GetSortCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SortCaps = aHandler.ReadArgumentString("SortCaps");
                }
                
                public string SortCaps;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionGetSortExtensionCapabilities

        public class AsyncActionGetSortExtensionCapabilities
        {
            internal AsyncActionGetSortExtensionCapabilities(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object GetSortExtensionCapabilitiesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSortExtensionCapabilitiesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortExtensionCapabilities.GetSortExtensionCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSortExtensionCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortExtensionCapabilities.GetSortExtensionCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSortExtensionCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SortExtensionCaps = aHandler.ReadArgumentString("SortExtensionCaps");
                }
                
                public string SortExtensionCaps;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionGetFeatureList

        public class AsyncActionGetFeatureList
        {
            internal AsyncActionGetFeatureList(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object GetFeatureListBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetFeatureListBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetFeatureList.GetFeatureListBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetFeatureListEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetFeatureList.GetFeatureListEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetFeatureList.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    FeatureList = aHandler.ReadArgumentString("FeatureList");
                }
                
                public string FeatureList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionGetSystemUpdateID

        public class AsyncActionGetSystemUpdateID
        {
            internal AsyncActionGetSystemUpdateID(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object GetSystemUpdateIDBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSystemUpdateIDBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSystemUpdateID.GetSystemUpdateIDBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSystemUpdateIDEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSystemUpdateID.GetSystemUpdateIDEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetSystemUpdateID.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Id = aHandler.ReadArgumentUint("Id");
                }
                
                public uint Id;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionBrowse

        public class AsyncActionBrowse
        {
            internal AsyncActionBrowse(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object BrowseBeginSync(string ObjectID, string BrowseFlag, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);           
                iHandler.WriteArgumentString("BrowseFlag", BrowseFlag);           
                iHandler.WriteArgumentString("Filter", Filter);           
                iHandler.WriteArgumentUint("StartingIndex", StartingIndex);           
                iHandler.WriteArgumentUint("RequestedCount", RequestedCount);           
                iHandler.WriteArgumentString("SortCriteria", SortCriteria);           
                
                return (iHandler.WriteEnd(null));
            }

            public void BrowseBegin(string ObjectID, string BrowseFlag, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);                
                iHandler.WriteArgumentString("BrowseFlag", BrowseFlag);                
                iHandler.WriteArgumentString("Filter", Filter);                
                iHandler.WriteArgumentUint("StartingIndex", StartingIndex);                
                iHandler.WriteArgumentUint("RequestedCount", RequestedCount);                
                iHandler.WriteArgumentString("SortCriteria", SortCriteria);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionBrowse.BrowseBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BrowseEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionBrowse.BrowseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionBrowse.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Result = aHandler.ReadArgumentString("Result");
                    NumberReturned = aHandler.ReadArgumentUint("NumberReturned");
                    TotalMatches = aHandler.ReadArgumentUint("TotalMatches");
                    UpdateID = aHandler.ReadArgumentUint("UpdateID");
                }
                
                public string Result;
                public uint NumberReturned;
                public uint TotalMatches;
                public uint UpdateID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionSearch

        public class AsyncActionSearch
        {
            internal AsyncActionSearch(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SearchBeginSync(string ContainerID, string SearchCriteria, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);           
                iHandler.WriteArgumentString("SearchCriteria", SearchCriteria);           
                iHandler.WriteArgumentString("Filter", Filter);           
                iHandler.WriteArgumentUint("StartingIndex", StartingIndex);           
                iHandler.WriteArgumentUint("RequestedCount", RequestedCount);           
                iHandler.WriteArgumentString("SortCriteria", SortCriteria);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SearchBegin(string ContainerID, string SearchCriteria, string Filter, uint StartingIndex, uint RequestedCount, string SortCriteria)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);                
                iHandler.WriteArgumentString("SearchCriteria", SearchCriteria);                
                iHandler.WriteArgumentString("Filter", Filter);                
                iHandler.WriteArgumentUint("StartingIndex", StartingIndex);                
                iHandler.WriteArgumentUint("RequestedCount", RequestedCount);                
                iHandler.WriteArgumentString("SortCriteria", SortCriteria);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionSearch.SearchBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SearchEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionSearch.SearchEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionSearch.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Result = aHandler.ReadArgumentString("Result");
                    NumberReturned = aHandler.ReadArgumentUint("NumberReturned");
                    TotalMatches = aHandler.ReadArgumentUint("TotalMatches");
                    UpdateID = aHandler.ReadArgumentUint("UpdateID");
                }
                
                public string Result;
                public uint NumberReturned;
                public uint TotalMatches;
                public uint UpdateID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionCreateObject

        public class AsyncActionCreateObject
        {
            internal AsyncActionCreateObject(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object CreateObjectBeginSync(string ContainerID, string Elements)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);           
                iHandler.WriteArgumentString("Elements", Elements);           
                
                return (iHandler.WriteEnd(null));
            }

            public void CreateObjectBegin(string ContainerID, string Elements)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);                
                iHandler.WriteArgumentString("Elements", Elements);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateObject.CreateObjectBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CreateObjectEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateObject.CreateObjectEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateObject.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    ObjectID = aHandler.ReadArgumentString("ObjectID");
                    Result = aHandler.ReadArgumentString("Result");
                }
                
                public string ObjectID;
                public string Result;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionDestroyObject

        public class AsyncActionDestroyObject
        {
            internal AsyncActionDestroyObject(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object DestroyObjectBeginSync(string ObjectID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DestroyObjectBegin(string ObjectID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionDestroyObject.DestroyObjectBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DestroyObjectEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionDestroyObject.DestroyObjectEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionDestroyObject.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionUpdateObject

        public class AsyncActionUpdateObject
        {
            internal AsyncActionUpdateObject(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object UpdateObjectBeginSync(string ObjectID, string CurrentTagValue, string NewTagValue)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);           
                iHandler.WriteArgumentString("CurrentTagValue", CurrentTagValue);           
                iHandler.WriteArgumentString("NewTagValue", NewTagValue);           
                
                return (iHandler.WriteEnd(null));
            }

            public void UpdateObjectBegin(string ObjectID, string CurrentTagValue, string NewTagValue)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);                
                iHandler.WriteArgumentString("CurrentTagValue", CurrentTagValue);                
                iHandler.WriteArgumentString("NewTagValue", NewTagValue);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionUpdateObject.UpdateObjectBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse UpdateObjectEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionUpdateObject.UpdateObjectEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionUpdateObject.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionMoveObject

        public class AsyncActionMoveObject
        {
            internal AsyncActionMoveObject(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object MoveObjectBeginSync(string ObjectID, string NewParentID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);           
                iHandler.WriteArgumentString("NewParentID", NewParentID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void MoveObjectBegin(string ObjectID, string NewParentID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ObjectID", ObjectID);                
                iHandler.WriteArgumentString("NewParentID", NewParentID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionMoveObject.MoveObjectBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MoveObjectEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionMoveObject.MoveObjectEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionMoveObject.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NewObjectID = aHandler.ReadArgumentString("NewObjectID");
                }
                
                public string NewObjectID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionImportResource

        public class AsyncActionImportResource
        {
            internal AsyncActionImportResource(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object ImportResourceBeginSync(string SourceURI, string DestinationURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("SourceURI", SourceURI);           
                iHandler.WriteArgumentString("DestinationURI", DestinationURI);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ImportResourceBegin(string SourceURI, string DestinationURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("SourceURI", SourceURI);                
                iHandler.WriteArgumentString("DestinationURI", DestinationURI);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionImportResource.ImportResourceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ImportResourceEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionImportResource.ImportResourceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionImportResource.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    TransferID = aHandler.ReadArgumentUint("TransferID");
                }
                
                public uint TransferID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionExportResource

        public class AsyncActionExportResource
        {
            internal AsyncActionExportResource(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object ExportResourceBeginSync(string SourceURI, string DestinationURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("SourceURI", SourceURI);           
                iHandler.WriteArgumentString("DestinationURI", DestinationURI);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ExportResourceBegin(string SourceURI, string DestinationURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("SourceURI", SourceURI);                
                iHandler.WriteArgumentString("DestinationURI", DestinationURI);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionExportResource.ExportResourceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ExportResourceEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionExportResource.ExportResourceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionExportResource.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    TransferID = aHandler.ReadArgumentUint("TransferID");
                }
                
                public uint TransferID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionStopTransferResource

        public class AsyncActionStopTransferResource
        {
            internal AsyncActionStopTransferResource(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object StopTransferResourceBeginSync(uint TransferID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("TransferID", TransferID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void StopTransferResourceBegin(uint TransferID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("TransferID", TransferID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionStopTransferResource.StopTransferResourceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StopTransferResourceEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionStopTransferResource.StopTransferResourceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionStopTransferResource.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionDeleteResource

        public class AsyncActionDeleteResource
        {
            internal AsyncActionDeleteResource(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object DeleteResourceBeginSync(string ResourceURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ResourceURI", ResourceURI);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DeleteResourceBegin(string ResourceURI)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ResourceURI", ResourceURI);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionDeleteResource.DeleteResourceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DeleteResourceEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionDeleteResource.DeleteResourceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionDeleteResource.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionGetTransferProgress

        public class AsyncActionGetTransferProgress
        {
            internal AsyncActionGetTransferProgress(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object GetTransferProgressBeginSync(uint TransferID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("TransferID", TransferID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetTransferProgressBegin(uint TransferID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("TransferID", TransferID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetTransferProgress.GetTransferProgressBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetTransferProgressEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetTransferProgress.GetTransferProgressEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionGetTransferProgress.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    TransferStatus = aHandler.ReadArgumentString("TransferStatus");
                    TransferLength = aHandler.ReadArgumentString("TransferLength");
                    TransferTotal = aHandler.ReadArgumentString("TransferTotal");
                }
                
                public string TransferStatus;
                public string TransferLength;
                public string TransferTotal;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
        }
        
        
        // AsyncActionCreateReference

        public class AsyncActionCreateReference
        {
            internal AsyncActionCreateReference(ServiceContentDirectory aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object CreateReferenceBeginSync(string ContainerID, string ObjectID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);           
                iHandler.WriteArgumentString("ObjectID", ObjectID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void CreateReferenceBegin(string ContainerID, string ObjectID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("ContainerID", ContainerID);                
                iHandler.WriteArgumentString("ObjectID", ObjectID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateReference.CreateReferenceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CreateReferenceEnd(object aResult)
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
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateReference.CreateReferenceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("ContentDirectory.AsyncActionCreateReference.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NewID = aHandler.ReadArgumentString("NewID");
                }
                
                public string NewID;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceContentDirectory iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceContentDirectory): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventSystemUpdateID = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SystemUpdateID", nsmanager);

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
					SystemUpdateID = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SystemUpdateID with value {1}", DateTime.Now, value));
				}

                eventSystemUpdateID = true;
            }

            bool eventContainerUpdateIDs = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ContainerUpdateIDs", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ContainerUpdateIDs = value;

                eventContainerUpdateIDs = true;
            }

            bool eventTransferIDs = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TransferIDs", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TransferIDs = value;

                eventTransferIDs = true;
            }

          
            
            if(eventSystemUpdateID)
            {
                if (EventStateSystemUpdateID != null)
                {
					try
					{
						EventStateSystemUpdateID(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSystemUpdateID: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventContainerUpdateIDs)
            {
                if (EventStateContainerUpdateIDs != null)
                {
					try
					{
						EventStateContainerUpdateIDs(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateContainerUpdateIDs: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTransferIDs)
            {
                if (EventStateTransferIDs != null)
                {
					try
					{
						EventStateTransferIDs(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTransferIDs: " + ex);
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
        public event EventHandler<EventArgs> EventStateSystemUpdateID;
        public event EventHandler<EventArgs> EventStateContainerUpdateIDs;
        public event EventHandler<EventArgs> EventStateTransferIDs;

        public uint SystemUpdateID;
        public string ContainerUpdateIDs;
        public string TransferIDs;
    }
}
    
