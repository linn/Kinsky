using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceVolume : ServiceUpnp
    {


        public ServiceVolume(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceVolume(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Characteristics");
            action.AddOutArgument(new Argument("VolumeMax", Argument.EType.eUint));
            action.AddOutArgument(new Argument("VolumeUnity", Argument.EType.eUint));
            action.AddOutArgument(new Argument("VolumeSteps", Argument.EType.eUint));
            action.AddOutArgument(new Argument("VolumeMilliDbPerStep", Argument.EType.eUint));
            action.AddOutArgument(new Argument("BalanceMax", Argument.EType.eUint));
            action.AddOutArgument(new Argument("FadeMax", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetVolume");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("VolumeInc");
            iActions.Add(action);
            
            action = new Action("VolumeDec");
            iActions.Add(action);
            
            action = new Action("Volume");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetBalance");
            action.AddInArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("BalanceInc");
            iActions.Add(action);
            
            action = new Action("BalanceDec");
            iActions.Add(action);
            
            action = new Action("Balance");
            action.AddOutArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetFade");
            action.AddInArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("FadeInc");
            iActions.Add(action);
            
            action = new Action("FadeDec");
            iActions.Add(action);
            
            action = new Action("Fade");
            action.AddOutArgument(new Argument("Value", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetMute");
            action.AddInArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Mute");
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("VolumeLimit");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Volume", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Volume", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionCharacteristics CreateAsyncActionCharacteristics()
        {
            return (new AsyncActionCharacteristics(this));
        }

        public AsyncActionSetVolume CreateAsyncActionSetVolume()
        {
            return (new AsyncActionSetVolume(this));
        }

        public AsyncActionVolumeInc CreateAsyncActionVolumeInc()
        {
            return (new AsyncActionVolumeInc(this));
        }

        public AsyncActionVolumeDec CreateAsyncActionVolumeDec()
        {
            return (new AsyncActionVolumeDec(this));
        }

        public AsyncActionVolume CreateAsyncActionVolume()
        {
            return (new AsyncActionVolume(this));
        }

        public AsyncActionSetBalance CreateAsyncActionSetBalance()
        {
            return (new AsyncActionSetBalance(this));
        }

        public AsyncActionBalanceInc CreateAsyncActionBalanceInc()
        {
            return (new AsyncActionBalanceInc(this));
        }

        public AsyncActionBalanceDec CreateAsyncActionBalanceDec()
        {
            return (new AsyncActionBalanceDec(this));
        }

        public AsyncActionBalance CreateAsyncActionBalance()
        {
            return (new AsyncActionBalance(this));
        }

        public AsyncActionSetFade CreateAsyncActionSetFade()
        {
            return (new AsyncActionSetFade(this));
        }

        public AsyncActionFadeInc CreateAsyncActionFadeInc()
        {
            return (new AsyncActionFadeInc(this));
        }

        public AsyncActionFadeDec CreateAsyncActionFadeDec()
        {
            return (new AsyncActionFadeDec(this));
        }

        public AsyncActionFade CreateAsyncActionFade()
        {
            return (new AsyncActionFade(this));
        }

        public AsyncActionSetMute CreateAsyncActionSetMute()
        {
            return (new AsyncActionSetMute(this));
        }

        public AsyncActionMute CreateAsyncActionMute()
        {
            return (new AsyncActionMute(this));
        }

        public AsyncActionVolumeLimit CreateAsyncActionVolumeLimit()
        {
            return (new AsyncActionVolumeLimit(this));
        }


        // Synchronous actions
        
        public void CharacteristicsSync(out uint VolumeMax, out uint VolumeUnity, out uint VolumeSteps, out uint VolumeMilliDbPerStep, out uint BalanceMax, out uint FadeMax)
        {
            AsyncActionCharacteristics action = CreateAsyncActionCharacteristics();
            
            object result = action.CharacteristicsBeginSync();

            AsyncActionCharacteristics.EventArgsResponse response = action.CharacteristicsEnd(result);
                
            VolumeMax = response.VolumeMax;
            VolumeUnity = response.VolumeUnity;
            VolumeSteps = response.VolumeSteps;
            VolumeMilliDbPerStep = response.VolumeMilliDbPerStep;
            BalanceMax = response.BalanceMax;
            FadeMax = response.FadeMax;
        }
        
        public void SetVolumeSync(uint Value)
        {
            AsyncActionSetVolume action = CreateAsyncActionSetVolume();
            
            object result = action.SetVolumeBeginSync(Value);

            action.SetVolumeEnd(result);
        }
        
        public void VolumeIncSync()
        {
            AsyncActionVolumeInc action = CreateAsyncActionVolumeInc();
            
            object result = action.VolumeIncBeginSync();

            action.VolumeIncEnd(result);
        }
        
        public void VolumeDecSync()
        {
            AsyncActionVolumeDec action = CreateAsyncActionVolumeDec();
            
            object result = action.VolumeDecBeginSync();

            action.VolumeDecEnd(result);
        }
        
        public uint VolumeSync()
        {
            AsyncActionVolume action = CreateAsyncActionVolume();
            
            object result = action.VolumeBeginSync();

            AsyncActionVolume.EventArgsResponse response = action.VolumeEnd(result);
                
            return(response.Value);
        }
        
        public void SetBalanceSync(int Value)
        {
            AsyncActionSetBalance action = CreateAsyncActionSetBalance();
            
            object result = action.SetBalanceBeginSync(Value);

            action.SetBalanceEnd(result);
        }
        
        public void BalanceIncSync()
        {
            AsyncActionBalanceInc action = CreateAsyncActionBalanceInc();
            
            object result = action.BalanceIncBeginSync();

            action.BalanceIncEnd(result);
        }
        
        public void BalanceDecSync()
        {
            AsyncActionBalanceDec action = CreateAsyncActionBalanceDec();
            
            object result = action.BalanceDecBeginSync();

            action.BalanceDecEnd(result);
        }
        
        public int BalanceSync()
        {
            AsyncActionBalance action = CreateAsyncActionBalance();
            
            object result = action.BalanceBeginSync();

            AsyncActionBalance.EventArgsResponse response = action.BalanceEnd(result);
                
            return(response.Value);
        }
        
        public void SetFadeSync(int Value)
        {
            AsyncActionSetFade action = CreateAsyncActionSetFade();
            
            object result = action.SetFadeBeginSync(Value);

            action.SetFadeEnd(result);
        }
        
        public void FadeIncSync()
        {
            AsyncActionFadeInc action = CreateAsyncActionFadeInc();
            
            object result = action.FadeIncBeginSync();

            action.FadeIncEnd(result);
        }
        
        public void FadeDecSync()
        {
            AsyncActionFadeDec action = CreateAsyncActionFadeDec();
            
            object result = action.FadeDecBeginSync();

            action.FadeDecEnd(result);
        }
        
        public int FadeSync()
        {
            AsyncActionFade action = CreateAsyncActionFade();
            
            object result = action.FadeBeginSync();

            AsyncActionFade.EventArgsResponse response = action.FadeEnd(result);
                
            return(response.Value);
        }
        
        public void SetMuteSync(bool Value)
        {
            AsyncActionSetMute action = CreateAsyncActionSetMute();
            
            object result = action.SetMuteBeginSync(Value);

            action.SetMuteEnd(result);
        }
        
        public bool MuteSync()
        {
            AsyncActionMute action = CreateAsyncActionMute();
            
            object result = action.MuteBeginSync();

            AsyncActionMute.EventArgsResponse response = action.MuteEnd(result);
                
            return(response.Value);
        }
        
        public uint VolumeLimitSync()
        {
            AsyncActionVolumeLimit action = CreateAsyncActionVolumeLimit();
            
            object result = action.VolumeLimitBeginSync();

            AsyncActionVolumeLimit.EventArgsResponse response = action.VolumeLimitEnd(result);
                
            return(response.Value);
        }
        

        // AsyncActionCharacteristics

        public class AsyncActionCharacteristics
        {
            internal AsyncActionCharacteristics(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object CharacteristicsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CharacteristicsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionCharacteristics.CharacteristicsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CharacteristicsEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionCharacteristics.CharacteristicsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionCharacteristics.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    VolumeMax = aHandler.ReadArgumentUint("VolumeMax");
                    VolumeUnity = aHandler.ReadArgumentUint("VolumeUnity");
                    VolumeSteps = aHandler.ReadArgumentUint("VolumeSteps");
                    VolumeMilliDbPerStep = aHandler.ReadArgumentUint("VolumeMilliDbPerStep");
                    BalanceMax = aHandler.ReadArgumentUint("BalanceMax");
                    FadeMax = aHandler.ReadArgumentUint("FadeMax");
                }
                
                public uint VolumeMax;
                public uint VolumeUnity;
                public uint VolumeSteps;
                public uint VolumeMilliDbPerStep;
                public uint BalanceMax;
                public uint FadeMax;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolume iService;
        }
        
        
        // AsyncActionSetVolume

        public class AsyncActionSetVolume
        {
            internal AsyncActionSetVolume(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object SetVolumeBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetVolumeBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionSetVolume.SetVolumeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetVolumeEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionSetVolume.SetVolumeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionSetVolume.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionVolumeInc

        public class AsyncActionVolumeInc
        {
            internal AsyncActionVolumeInc(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object VolumeIncBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void VolumeIncBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionVolumeInc.VolumeIncBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse VolumeIncEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionVolumeInc.VolumeIncEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionVolumeInc.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionVolumeDec

        public class AsyncActionVolumeDec
        {
            internal AsyncActionVolumeDec(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object VolumeDecBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void VolumeDecBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionVolumeDec.VolumeDecBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse VolumeDecEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionVolumeDec.VolumeDecEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionVolumeDec.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionVolume

        public class AsyncActionVolume
        {
            internal AsyncActionVolume(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object VolumeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void VolumeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionVolume.VolumeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse VolumeEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionVolume.VolumeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionVolume.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionSetBalance

        public class AsyncActionSetBalance
        {
            internal AsyncActionSetBalance(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object SetBalanceBeginSync(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBalanceBegin(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionSetBalance.SetBalanceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetBalanceEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionSetBalance.SetBalanceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionSetBalance.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionBalanceInc

        public class AsyncActionBalanceInc
        {
            internal AsyncActionBalanceInc(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object BalanceIncBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void BalanceIncBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionBalanceInc.BalanceIncBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BalanceIncEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionBalanceInc.BalanceIncEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionBalanceInc.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionBalanceDec

        public class AsyncActionBalanceDec
        {
            internal AsyncActionBalanceDec(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object BalanceDecBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void BalanceDecBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionBalanceDec.BalanceDecBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BalanceDecEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionBalanceDec.BalanceDecEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionBalanceDec.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionBalance

        public class AsyncActionBalance
        {
            internal AsyncActionBalance(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object BalanceBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void BalanceBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionBalance.BalanceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse BalanceEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionBalance.BalanceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionBalance.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentInt("Value");
                }
                
                public int Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolume iService;
        }
        
        
        // AsyncActionSetFade

        public class AsyncActionSetFade
        {
            internal AsyncActionSetFade(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetFadeBeginSync(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetFadeBegin(int Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionSetFade.SetFadeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetFadeEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionSetFade.SetFadeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionSetFade.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionFadeInc

        public class AsyncActionFadeInc
        {
            internal AsyncActionFadeInc(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object FadeIncBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void FadeIncBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionFadeInc.FadeIncBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse FadeIncEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionFadeInc.FadeIncEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionFadeInc.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionFadeDec

        public class AsyncActionFadeDec
        {
            internal AsyncActionFadeDec(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object FadeDecBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void FadeDecBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionFadeDec.FadeDecBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse FadeDecEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionFadeDec.FadeDecEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionFadeDec.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionFade

        public class AsyncActionFade
        {
            internal AsyncActionFade(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object FadeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void FadeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionFade.FadeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse FadeEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionFade.FadeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionFade.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Value = aHandler.ReadArgumentInt("Value");
                }
                
                public int Value;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceVolume iService;
        }
        
        
        // AsyncActionSetMute

        public class AsyncActionSetMute
        {
            internal AsyncActionSetMute(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object SetMuteBeginSync(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetMuteBegin(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionSetMute.SetMuteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetMuteEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionSetMute.SetMuteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionSetMute.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionMute

        public class AsyncActionMute
        {
            internal AsyncActionMute(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object MuteBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MuteBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionMute.MuteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MuteEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionMute.MuteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionMute.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
        }
        
        
        // AsyncActionVolumeLimit

        public class AsyncActionVolumeLimit
        {
            internal AsyncActionVolumeLimit(ServiceVolume aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object VolumeLimitBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void VolumeLimitBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Volume.AsyncActionVolumeLimit.VolumeLimitBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse VolumeLimitEnd(object aResult)
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
                    UserLog.WriteLine("Volume.AsyncActionVolumeLimit.VolumeLimitEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Volume.AsyncActionVolumeLimit.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceVolume iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceVolume): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventVolume = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Volume", nsmanager);

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
					Volume = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Volume with value {1}", DateTime.Now, value));
				}

                eventVolume = true;
            }

            bool eventMute = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Mute", nsmanager);

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
	                Mute = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Mute = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Mute = false; 
    	            }
                }

                eventMute = true;
            }

            bool eventBalance = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Balance", nsmanager);

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
					Balance = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Balance with value {1}", DateTime.Now, value));
				}

                eventBalance = true;
            }

            bool eventFade = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Fade", nsmanager);

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
					Fade = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Fade with value {1}", DateTime.Now, value));
				}

                eventFade = true;
            }

            bool eventVolumeLimit = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "VolumeLimit", nsmanager);

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
					VolumeLimit = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse VolumeLimit with value {1}", DateTime.Now, value));
				}

                eventVolumeLimit = true;
            }

            bool eventVolumeMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "VolumeMax", nsmanager);

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
					VolumeMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse VolumeMax with value {1}", DateTime.Now, value));
				}

                eventVolumeMax = true;
            }

            bool eventVolumeUnity = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "VolumeUnity", nsmanager);

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
					VolumeUnity = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse VolumeUnity with value {1}", DateTime.Now, value));
				}

                eventVolumeUnity = true;
            }

            bool eventVolumeSteps = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "VolumeSteps", nsmanager);

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
					VolumeSteps = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse VolumeSteps with value {1}", DateTime.Now, value));
				}

                eventVolumeSteps = true;
            }

            bool eventVolumeMilliDbPerStep = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "VolumeMilliDbPerStep", nsmanager);

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
					VolumeMilliDbPerStep = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse VolumeMilliDbPerStep with value {1}", DateTime.Now, value));
				}

                eventVolumeMilliDbPerStep = true;
            }

            bool eventBalanceMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "BalanceMax", nsmanager);

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
					BalanceMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse BalanceMax with value {1}", DateTime.Now, value));
				}

                eventBalanceMax = true;
            }

            bool eventFadeMax = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "FadeMax", nsmanager);

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
					FadeMax = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse FadeMax with value {1}", DateTime.Now, value));
				}

                eventFadeMax = true;
            }

          
            
            if(eventVolume)
            {
                if (EventStateVolume != null)
                {
					try
					{
						EventStateVolume(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolume: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventMute)
            {
                if (EventStateMute != null)
                {
					try
					{
						EventStateMute(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateMute: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventBalance)
            {
                if (EventStateBalance != null)
                {
					try
					{
						EventStateBalance(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateBalance: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventFade)
            {
                if (EventStateFade != null)
                {
					try
					{
						EventStateFade(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateFade: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVolumeLimit)
            {
                if (EventStateVolumeLimit != null)
                {
					try
					{
						EventStateVolumeLimit(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolumeLimit: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVolumeMax)
            {
                if (EventStateVolumeMax != null)
                {
					try
					{
						EventStateVolumeMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolumeMax: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVolumeUnity)
            {
                if (EventStateVolumeUnity != null)
                {
					try
					{
						EventStateVolumeUnity(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolumeUnity: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVolumeSteps)
            {
                if (EventStateVolumeSteps != null)
                {
					try
					{
						EventStateVolumeSteps(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolumeSteps: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventVolumeMilliDbPerStep)
            {
                if (EventStateVolumeMilliDbPerStep != null)
                {
					try
					{
						EventStateVolumeMilliDbPerStep(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateVolumeMilliDbPerStep: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventBalanceMax)
            {
                if (EventStateBalanceMax != null)
                {
					try
					{
						EventStateBalanceMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateBalanceMax: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventFadeMax)
            {
                if (EventStateFadeMax != null)
                {
					try
					{
						EventStateFadeMax(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateFadeMax: " + ex);
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
        public event EventHandler<EventArgs> EventStateVolume;
        public event EventHandler<EventArgs> EventStateMute;
        public event EventHandler<EventArgs> EventStateBalance;
        public event EventHandler<EventArgs> EventStateFade;
        public event EventHandler<EventArgs> EventStateVolumeLimit;
        public event EventHandler<EventArgs> EventStateVolumeMax;
        public event EventHandler<EventArgs> EventStateVolumeUnity;
        public event EventHandler<EventArgs> EventStateVolumeSteps;
        public event EventHandler<EventArgs> EventStateVolumeMilliDbPerStep;
        public event EventHandler<EventArgs> EventStateBalanceMax;
        public event EventHandler<EventArgs> EventStateFadeMax;

        public uint Volume;
        public bool Mute;
        public int Balance;
        public int Fade;
        public uint VolumeLimit;
        public uint VolumeMax;
        public uint VolumeUnity;
        public uint VolumeSteps;
        public uint VolumeMilliDbPerStep;
        public uint BalanceMax;
        public uint FadeMax;
    }
}
    
