using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceVideo : ServiceUpnp
    {

        public const string kDeinterlacerInputSyncsSyncEmbedded = "SyncEmbedded";
        public const string kDeinterlacerInputSyncsSyncSeparate = "SyncSeparate";
        public const string kResolutionResolutionVga = "ResolutionVga";
        public const string kResolutionResolution480I = "Resolution480i";
        public const string kResolutionResolution576I = "Resolution576i";
        public const string kResolutionResolution480P = "Resolution480p";
        public const string kResolutionResolution576P = "Resolution576p";
        public const string kResolutionResolution720P = "Resolution720p";
        public const string kResolutionResolution1080I = "Resolution1080i";
        public const string kResolutionResolution1080P = "Resolution1080p";
        public const string kColourSpaceColourSpaceYcbCr422 = "ColourSpaceYCbCr422";
        public const string kColourSpaceColourSpaceYcbCr444 = "ColourSpaceYCbCr444";
        public const string kColourSpaceColourSpaceRgb = "ColourSpaceRgb";
        public const string kAspectRatioAspectRatio4By3 = "AspectRatio4By3";
        public const string kAspectRatioAspectRatio16By9 = "AspectRatio16By9";
        public const string kAspectRatioLetterBox = "LetterBox";

        public ServiceVideo(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceVideo(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("ResetDeinterlacer");
            iActions.Add(action);
            
            action = new Action("SetDeinterlacerInputSyncs");
            action.AddInArgument(new Argument("aInputSync", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetDeinterlacerInputResolution");
            action.AddInArgument(new Argument("aResolution", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetDeinterlacerPassThrough");
            action.AddInArgument(new Argument("aPassThrough", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("ResetScaler");
            iActions.Add(action);
            
            action = new Action("SetScalerPassThrough");
            action.AddInArgument(new Argument("aPassThrough", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetScalerInputResolution");
            action.AddInArgument(new Argument("aResolution", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetScalerOutputResolution");
            action.AddInArgument(new Argument("aResolution", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetScalerOutputColourSpace");
            action.AddInArgument(new Argument("aColourSpace", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetScalerAspectRatio");
            action.AddInArgument(new Argument("aAspectRatio", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Video", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Video", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionResetDeinterlacer CreateAsyncActionResetDeinterlacer()
        {
            return (new AsyncActionResetDeinterlacer(this));
        }

        public AsyncActionSetDeinterlacerInputSyncs CreateAsyncActionSetDeinterlacerInputSyncs()
        {
            return (new AsyncActionSetDeinterlacerInputSyncs(this));
        }

        public AsyncActionSetDeinterlacerInputResolution CreateAsyncActionSetDeinterlacerInputResolution()
        {
            return (new AsyncActionSetDeinterlacerInputResolution(this));
        }

        public AsyncActionSetDeinterlacerPassThrough CreateAsyncActionSetDeinterlacerPassThrough()
        {
            return (new AsyncActionSetDeinterlacerPassThrough(this));
        }

        public AsyncActionResetScaler CreateAsyncActionResetScaler()
        {
            return (new AsyncActionResetScaler(this));
        }

        public AsyncActionSetScalerPassThrough CreateAsyncActionSetScalerPassThrough()
        {
            return (new AsyncActionSetScalerPassThrough(this));
        }

        public AsyncActionSetScalerInputResolution CreateAsyncActionSetScalerInputResolution()
        {
            return (new AsyncActionSetScalerInputResolution(this));
        }

        public AsyncActionSetScalerOutputResolution CreateAsyncActionSetScalerOutputResolution()
        {
            return (new AsyncActionSetScalerOutputResolution(this));
        }

        public AsyncActionSetScalerOutputColourSpace CreateAsyncActionSetScalerOutputColourSpace()
        {
            return (new AsyncActionSetScalerOutputColourSpace(this));
        }

        public AsyncActionSetScalerAspectRatio CreateAsyncActionSetScalerAspectRatio()
        {
            return (new AsyncActionSetScalerAspectRatio(this));
        }


        // Synchronous actions
        
        public void ResetDeinterlacerSync()
        {
            AsyncActionResetDeinterlacer action = CreateAsyncActionResetDeinterlacer();
            
            object result = action.ResetDeinterlacerBeginSync();

            action.ResetDeinterlacerEnd(result);
        }
        
        public void SetDeinterlacerInputSyncsSync(string aInputSync)
        {
            AsyncActionSetDeinterlacerInputSyncs action = CreateAsyncActionSetDeinterlacerInputSyncs();
            
            object result = action.SetDeinterlacerInputSyncsBeginSync(aInputSync);

            action.SetDeinterlacerInputSyncsEnd(result);
        }
        
        public void SetDeinterlacerInputResolutionSync(string aResolution)
        {
            AsyncActionSetDeinterlacerInputResolution action = CreateAsyncActionSetDeinterlacerInputResolution();
            
            object result = action.SetDeinterlacerInputResolutionBeginSync(aResolution);

            action.SetDeinterlacerInputResolutionEnd(result);
        }
        
        public void SetDeinterlacerPassThroughSync(bool aPassThrough)
        {
            AsyncActionSetDeinterlacerPassThrough action = CreateAsyncActionSetDeinterlacerPassThrough();
            
            object result = action.SetDeinterlacerPassThroughBeginSync(aPassThrough);

            action.SetDeinterlacerPassThroughEnd(result);
        }
        
        public void ResetScalerSync()
        {
            AsyncActionResetScaler action = CreateAsyncActionResetScaler();
            
            object result = action.ResetScalerBeginSync();

            action.ResetScalerEnd(result);
        }
        
        public void SetScalerPassThroughSync(bool aPassThrough)
        {
            AsyncActionSetScalerPassThrough action = CreateAsyncActionSetScalerPassThrough();
            
            object result = action.SetScalerPassThroughBeginSync(aPassThrough);

            action.SetScalerPassThroughEnd(result);
        }
        
        public void SetScalerInputResolutionSync(string aResolution)
        {
            AsyncActionSetScalerInputResolution action = CreateAsyncActionSetScalerInputResolution();
            
            object result = action.SetScalerInputResolutionBeginSync(aResolution);

            action.SetScalerInputResolutionEnd(result);
        }
        
        public void SetScalerOutputResolutionSync(string aResolution)
        {
            AsyncActionSetScalerOutputResolution action = CreateAsyncActionSetScalerOutputResolution();
            
            object result = action.SetScalerOutputResolutionBeginSync(aResolution);

            action.SetScalerOutputResolutionEnd(result);
        }
        
        public void SetScalerOutputColourSpaceSync(string aColourSpace)
        {
            AsyncActionSetScalerOutputColourSpace action = CreateAsyncActionSetScalerOutputColourSpace();
            
            object result = action.SetScalerOutputColourSpaceBeginSync(aColourSpace);

            action.SetScalerOutputColourSpaceEnd(result);
        }
        
        public void SetScalerAspectRatioSync(string aAspectRatio)
        {
            AsyncActionSetScalerAspectRatio action = CreateAsyncActionSetScalerAspectRatio();
            
            object result = action.SetScalerAspectRatioBeginSync(aAspectRatio);

            action.SetScalerAspectRatioEnd(result);
        }
        

        // AsyncActionResetDeinterlacer

        public class AsyncActionResetDeinterlacer
        {
            internal AsyncActionResetDeinterlacer(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object ResetDeinterlacerBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ResetDeinterlacerBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionResetDeinterlacer.ResetDeinterlacerBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ResetDeinterlacerEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionResetDeinterlacer.ResetDeinterlacerEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionResetDeinterlacer.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetDeinterlacerInputSyncs

        public class AsyncActionSetDeinterlacerInputSyncs
        {
            internal AsyncActionSetDeinterlacerInputSyncs(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object SetDeinterlacerInputSyncsBeginSync(string aInputSync)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aInputSync", aInputSync);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetDeinterlacerInputSyncsBegin(string aInputSync)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aInputSync", aInputSync);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputSyncs.SetDeinterlacerInputSyncsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetDeinterlacerInputSyncsEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputSyncs.SetDeinterlacerInputSyncsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputSyncs.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetDeinterlacerInputResolution

        public class AsyncActionSetDeinterlacerInputResolution
        {
            internal AsyncActionSetDeinterlacerInputResolution(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetDeinterlacerInputResolutionBeginSync(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetDeinterlacerInputResolutionBegin(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputResolution.SetDeinterlacerInputResolutionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetDeinterlacerInputResolutionEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputResolution.SetDeinterlacerInputResolutionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerInputResolution.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetDeinterlacerPassThrough

        public class AsyncActionSetDeinterlacerPassThrough
        {
            internal AsyncActionSetDeinterlacerPassThrough(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SetDeinterlacerPassThroughBeginSync(bool aPassThrough)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aPassThrough", aPassThrough);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetDeinterlacerPassThroughBegin(bool aPassThrough)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aPassThrough", aPassThrough);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerPassThrough.SetDeinterlacerPassThroughBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetDeinterlacerPassThroughEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerPassThrough.SetDeinterlacerPassThroughEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetDeinterlacerPassThrough.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionResetScaler

        public class AsyncActionResetScaler
        {
            internal AsyncActionResetScaler(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object ResetScalerBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ResetScalerBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionResetScaler.ResetScalerBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ResetScalerEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionResetScaler.ResetScalerEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionResetScaler.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetScalerPassThrough

        public class AsyncActionSetScalerPassThrough
        {
            internal AsyncActionSetScalerPassThrough(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object SetScalerPassThroughBeginSync(bool aPassThrough)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aPassThrough", aPassThrough);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetScalerPassThroughBegin(bool aPassThrough)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aPassThrough", aPassThrough);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetScalerPassThrough.SetScalerPassThroughBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetScalerPassThroughEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetScalerPassThrough.SetScalerPassThroughEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetScalerPassThrough.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetScalerInputResolution

        public class AsyncActionSetScalerInputResolution
        {
            internal AsyncActionSetScalerInputResolution(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SetScalerInputResolutionBeginSync(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetScalerInputResolutionBegin(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetScalerInputResolution.SetScalerInputResolutionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetScalerInputResolutionEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetScalerInputResolution.SetScalerInputResolutionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetScalerInputResolution.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetScalerOutputResolution

        public class AsyncActionSetScalerOutputResolution
        {
            internal AsyncActionSetScalerOutputResolution(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SetScalerOutputResolutionBeginSync(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetScalerOutputResolutionBegin(string aResolution)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aResolution", aResolution);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputResolution.SetScalerOutputResolutionBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetScalerOutputResolutionEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputResolution.SetScalerOutputResolutionEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputResolution.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetScalerOutputColourSpace

        public class AsyncActionSetScalerOutputColourSpace
        {
            internal AsyncActionSetScalerOutputColourSpace(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object SetScalerOutputColourSpaceBeginSync(string aColourSpace)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aColourSpace", aColourSpace);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetScalerOutputColourSpaceBegin(string aColourSpace)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aColourSpace", aColourSpace);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputColourSpace.SetScalerOutputColourSpaceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetScalerOutputColourSpaceEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputColourSpace.SetScalerOutputColourSpaceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetScalerOutputColourSpace.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        
        // AsyncActionSetScalerAspectRatio

        public class AsyncActionSetScalerAspectRatio
        {
            internal AsyncActionSetScalerAspectRatio(ServiceVideo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetScalerAspectRatioBeginSync(string aAspectRatio)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aAspectRatio", aAspectRatio);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetScalerAspectRatioBegin(string aAspectRatio)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aAspectRatio", aAspectRatio);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Video.AsyncActionSetScalerAspectRatio.SetScalerAspectRatioBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetScalerAspectRatioEnd(object aResult)
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
                    UserLog.WriteLine("Video.AsyncActionSetScalerAspectRatio.SetScalerAspectRatioEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Video.AsyncActionSetScalerAspectRatio.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVideo iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
        }

    }
}
    
