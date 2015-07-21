using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceDiagnostics : ServiceUpnp
    {


        public ServiceDiagnostics(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceDiagnostics(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Echo");
            action.AddInArgument(new Argument("aIn", Argument.EType.eString));
            action.AddOutArgument(new Argument("aOut", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ElfFile");
            action.AddOutArgument(new Argument("aElfFile", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ElfFingerprint");
            action.AddOutArgument(new Argument("aElfFileFingerprint", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("CrashDataStatus");
            action.AddOutArgument(new Argument("aCrashDataStatus", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("CrashDataFetch");
            action.AddOutArgument(new Argument("aCrashData", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("CrashDataClear");
            iActions.Add(action);
            
            action = new Action("SysLog");
            action.AddOutArgument(new Argument("aSysLog", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("Diagnostic");
            action.AddInArgument(new Argument("aDiagnosticType", Argument.EType.eString));
            action.AddOutArgument(new Argument("aDiagnosticInfo", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiagnosticTest");
            action.AddInArgument(new Argument("aDiagnosticType", Argument.EType.eString));
            action.AddInArgument(new Argument("aDiagnosticInput", Argument.EType.eString));
            action.AddOutArgument(new Argument("aDiagnosticInfo", Argument.EType.eString));
            action.AddOutArgument(new Argument("aDiagnosticResult", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("StateVariable");
            action.AddOutArgument(new Argument("aStateVariable", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetStateVariable");
            action.AddInArgument(new Argument("aStateVariable", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("StateVariablePeriod");
            action.AddOutArgument(new Argument("aPeriod", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetStateVariablePeriod");
            action.AddInArgument(new Argument("aPeriod", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Reboot");
            action.AddInArgument(new Argument("aDelay", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSongcastPercentageLoss");
            action.AddInArgument(new Argument("aPercentage", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Diagnostics", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Diagnostics", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionEcho CreateAsyncActionEcho()
        {
            return (new AsyncActionEcho(this));
        }

        public AsyncActionElfFile CreateAsyncActionElfFile()
        {
            return (new AsyncActionElfFile(this));
        }

        public AsyncActionElfFingerprint CreateAsyncActionElfFingerprint()
        {
            return (new AsyncActionElfFingerprint(this));
        }

        public AsyncActionCrashDataStatus CreateAsyncActionCrashDataStatus()
        {
            return (new AsyncActionCrashDataStatus(this));
        }

        public AsyncActionCrashDataFetch CreateAsyncActionCrashDataFetch()
        {
            return (new AsyncActionCrashDataFetch(this));
        }

        public AsyncActionCrashDataClear CreateAsyncActionCrashDataClear()
        {
            return (new AsyncActionCrashDataClear(this));
        }

        public AsyncActionSysLog CreateAsyncActionSysLog()
        {
            return (new AsyncActionSysLog(this));
        }

        public AsyncActionDiagnostic CreateAsyncActionDiagnostic()
        {
            return (new AsyncActionDiagnostic(this));
        }

        public AsyncActionDiagnosticTest CreateAsyncActionDiagnosticTest()
        {
            return (new AsyncActionDiagnosticTest(this));
        }

        public AsyncActionStateVariable CreateAsyncActionStateVariable()
        {
            return (new AsyncActionStateVariable(this));
        }

        public AsyncActionSetStateVariable CreateAsyncActionSetStateVariable()
        {
            return (new AsyncActionSetStateVariable(this));
        }

        public AsyncActionStateVariablePeriod CreateAsyncActionStateVariablePeriod()
        {
            return (new AsyncActionStateVariablePeriod(this));
        }

        public AsyncActionSetStateVariablePeriod CreateAsyncActionSetStateVariablePeriod()
        {
            return (new AsyncActionSetStateVariablePeriod(this));
        }

        public AsyncActionReboot CreateAsyncActionReboot()
        {
            return (new AsyncActionReboot(this));
        }

        public AsyncActionSetSongcastPercentageLoss CreateAsyncActionSetSongcastPercentageLoss()
        {
            return (new AsyncActionSetSongcastPercentageLoss(this));
        }


        // Synchronous actions
        
        public string EchoSync(string aIn)
        {
            AsyncActionEcho action = CreateAsyncActionEcho();
            
            object result = action.EchoBeginSync(aIn);

            AsyncActionEcho.EventArgsResponse response = action.EchoEnd(result);
                
            return(response.aOut);
        }
        
        public string ElfFileSync()
        {
            AsyncActionElfFile action = CreateAsyncActionElfFile();
            
            object result = action.ElfFileBeginSync();

            AsyncActionElfFile.EventArgsResponse response = action.ElfFileEnd(result);
                
            return(response.aElfFile);
        }
        
        public string ElfFingerprintSync()
        {
            AsyncActionElfFingerprint action = CreateAsyncActionElfFingerprint();
            
            object result = action.ElfFingerprintBeginSync();

            AsyncActionElfFingerprint.EventArgsResponse response = action.ElfFingerprintEnd(result);
                
            return(response.aElfFileFingerprint);
        }
        
        public string CrashDataStatusSync()
        {
            AsyncActionCrashDataStatus action = CreateAsyncActionCrashDataStatus();
            
            object result = action.CrashDataStatusBeginSync();

            AsyncActionCrashDataStatus.EventArgsResponse response = action.CrashDataStatusEnd(result);
                
            return(response.aCrashDataStatus);
        }
        
        public byte[] CrashDataFetchSync()
        {
            AsyncActionCrashDataFetch action = CreateAsyncActionCrashDataFetch();
            
            object result = action.CrashDataFetchBeginSync();

            AsyncActionCrashDataFetch.EventArgsResponse response = action.CrashDataFetchEnd(result);
                
            return(response.aCrashData);
        }
        
        public void CrashDataClearSync()
        {
            AsyncActionCrashDataClear action = CreateAsyncActionCrashDataClear();
            
            object result = action.CrashDataClearBeginSync();

            action.CrashDataClearEnd(result);
        }
        
        public byte[] SysLogSync()
        {
            AsyncActionSysLog action = CreateAsyncActionSysLog();
            
            object result = action.SysLogBeginSync();

            AsyncActionSysLog.EventArgsResponse response = action.SysLogEnd(result);
                
            return(response.aSysLog);
        }
        
        public string DiagnosticSync(string aDiagnosticType)
        {
            AsyncActionDiagnostic action = CreateAsyncActionDiagnostic();
            
            object result = action.DiagnosticBeginSync(aDiagnosticType);

            AsyncActionDiagnostic.EventArgsResponse response = action.DiagnosticEnd(result);
                
            return(response.aDiagnosticInfo);
        }
        
        public void DiagnosticTestSync(string aDiagnosticType, string aDiagnosticInput, out string aDiagnosticInfo, out bool aDiagnosticResult)
        {
            AsyncActionDiagnosticTest action = CreateAsyncActionDiagnosticTest();
            
            object result = action.DiagnosticTestBeginSync(aDiagnosticType, aDiagnosticInput);

            AsyncActionDiagnosticTest.EventArgsResponse response = action.DiagnosticTestEnd(result);
                
            aDiagnosticInfo = response.aDiagnosticInfo;
            aDiagnosticResult = response.aDiagnosticResult;
        }
        
        public uint StateVariableSync()
        {
            AsyncActionStateVariable action = CreateAsyncActionStateVariable();
            
            object result = action.StateVariableBeginSync();

            AsyncActionStateVariable.EventArgsResponse response = action.StateVariableEnd(result);
                
            return(response.aStateVariable);
        }
        
        public void SetStateVariableSync(uint aStateVariable)
        {
            AsyncActionSetStateVariable action = CreateAsyncActionSetStateVariable();
            
            object result = action.SetStateVariableBeginSync(aStateVariable);

            action.SetStateVariableEnd(result);
        }
        
        public uint StateVariablePeriodSync()
        {
            AsyncActionStateVariablePeriod action = CreateAsyncActionStateVariablePeriod();
            
            object result = action.StateVariablePeriodBeginSync();

            AsyncActionStateVariablePeriod.EventArgsResponse response = action.StateVariablePeriodEnd(result);
                
            return(response.aPeriod);
        }
        
        public void SetStateVariablePeriodSync(uint aPeriod)
        {
            AsyncActionSetStateVariablePeriod action = CreateAsyncActionSetStateVariablePeriod();
            
            object result = action.SetStateVariablePeriodBeginSync(aPeriod);

            action.SetStateVariablePeriodEnd(result);
        }
        
        public void RebootSync(uint aDelay)
        {
            AsyncActionReboot action = CreateAsyncActionReboot();
            
            object result = action.RebootBeginSync(aDelay);

            action.RebootEnd(result);
        }
        
        public void SetSongcastPercentageLossSync(uint aPercentage)
        {
            AsyncActionSetSongcastPercentageLoss action = CreateAsyncActionSetSongcastPercentageLoss();
            
            object result = action.SetSongcastPercentageLossBeginSync(aPercentage);

            action.SetSongcastPercentageLossEnd(result);
        }
        

        // AsyncActionEcho

        public class AsyncActionEcho
        {
            internal AsyncActionEcho(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object EchoBeginSync(string aIn)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aIn", aIn);           
                
                return (iHandler.WriteEnd(null));
            }

            public void EchoBegin(string aIn)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aIn", aIn);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionEcho.EchoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EchoEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionEcho.EchoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionEcho.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aOut = aHandler.ReadArgumentString("aOut");
                }
                
                public string aOut;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionElfFile

        public class AsyncActionElfFile
        {
            internal AsyncActionElfFile(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object ElfFileBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ElfFileBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFile.ElfFileBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ElfFileEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFile.ElfFileEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFile.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aElfFile = aHandler.ReadArgumentString("aElfFile");
                }
                
                public string aElfFile;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionElfFingerprint

        public class AsyncActionElfFingerprint
        {
            internal AsyncActionElfFingerprint(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object ElfFingerprintBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ElfFingerprintBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFingerprint.ElfFingerprintBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ElfFingerprintEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFingerprint.ElfFingerprintEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionElfFingerprint.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aElfFileFingerprint = aHandler.ReadArgumentString("aElfFileFingerprint");
                }
                
                public string aElfFileFingerprint;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionCrashDataStatus

        public class AsyncActionCrashDataStatus
        {
            internal AsyncActionCrashDataStatus(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object CrashDataStatusBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CrashDataStatusBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataStatus.CrashDataStatusBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CrashDataStatusEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataStatus.CrashDataStatusEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataStatus.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aCrashDataStatus = aHandler.ReadArgumentString("aCrashDataStatus");
                }
                
                public string aCrashDataStatus;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionCrashDataFetch

        public class AsyncActionCrashDataFetch
        {
            internal AsyncActionCrashDataFetch(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object CrashDataFetchBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CrashDataFetchBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataFetch.CrashDataFetchBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CrashDataFetchEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataFetch.CrashDataFetchEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataFetch.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aCrashData = aHandler.ReadArgumentBinary("aCrashData");
                }
                
                public byte[] aCrashData;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionCrashDataClear

        public class AsyncActionCrashDataClear
        {
            internal AsyncActionCrashDataClear(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object CrashDataClearBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CrashDataClearBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataClear.CrashDataClearBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CrashDataClearEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataClear.CrashDataClearEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionCrashDataClear.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionSysLog

        public class AsyncActionSysLog
        {
            internal AsyncActionSysLog(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SysLogBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SysLogBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionSysLog.SysLogBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SysLogEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionSysLog.SysLogEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionSysLog.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSysLog = aHandler.ReadArgumentBinary("aSysLog");
                }
                
                public byte[] aSysLog;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionDiagnostic

        public class AsyncActionDiagnostic
        {
            internal AsyncActionDiagnostic(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object DiagnosticBeginSync(string aDiagnosticType)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aDiagnosticType", aDiagnosticType);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DiagnosticBegin(string aDiagnosticType)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aDiagnosticType", aDiagnosticType);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnostic.DiagnosticBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiagnosticEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnostic.DiagnosticEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnostic.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiagnosticInfo = aHandler.ReadArgumentString("aDiagnosticInfo");
                }
                
                public string aDiagnosticInfo;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionDiagnosticTest

        public class AsyncActionDiagnosticTest
        {
            internal AsyncActionDiagnosticTest(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object DiagnosticTestBeginSync(string aDiagnosticType, string aDiagnosticInput)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aDiagnosticType", aDiagnosticType);           
                iHandler.WriteArgumentString("aDiagnosticInput", aDiagnosticInput);           
                
                return (iHandler.WriteEnd(null));
            }

            public void DiagnosticTestBegin(string aDiagnosticType, string aDiagnosticInput)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aDiagnosticType", aDiagnosticType);                
                iHandler.WriteArgumentString("aDiagnosticInput", aDiagnosticInput);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnosticTest.DiagnosticTestBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiagnosticTestEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnosticTest.DiagnosticTestEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionDiagnosticTest.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiagnosticInfo = aHandler.ReadArgumentString("aDiagnosticInfo");
                    aDiagnosticResult = aHandler.ReadArgumentBool("aDiagnosticResult");
                }
                
                public string aDiagnosticInfo;
                public bool aDiagnosticResult;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionStateVariable

        public class AsyncActionStateVariable
        {
            internal AsyncActionStateVariable(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object StateVariableBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StateVariableBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariable.StateVariableBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StateVariableEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariable.StateVariableEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariable.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aStateVariable = aHandler.ReadArgumentUint("aStateVariable");
                }
                
                public uint aStateVariable;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionSetStateVariable

        public class AsyncActionSetStateVariable
        {
            internal AsyncActionSetStateVariable(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object SetStateVariableBeginSync(uint aStateVariable)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStateVariable", aStateVariable);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStateVariableBegin(uint aStateVariable)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aStateVariable", aStateVariable);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariable.SetStateVariableBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStateVariableEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariable.SetStateVariableEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariable.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionStateVariablePeriod

        public class AsyncActionStateVariablePeriod
        {
            internal AsyncActionStateVariablePeriod(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object StateVariablePeriodBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StateVariablePeriodBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariablePeriod.StateVariablePeriodBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StateVariablePeriodEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariablePeriod.StateVariablePeriodEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionStateVariablePeriod.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aPeriod = aHandler.ReadArgumentUint("aPeriod");
                }
                
                public uint aPeriod;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionSetStateVariablePeriod

        public class AsyncActionSetStateVariablePeriod
        {
            internal AsyncActionSetStateVariablePeriod(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SetStateVariablePeriodBeginSync(uint aPeriod)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPeriod", aPeriod);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStateVariablePeriodBegin(uint aPeriod)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPeriod", aPeriod);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariablePeriod.SetStateVariablePeriodBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStateVariablePeriodEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariablePeriod.SetStateVariablePeriodEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetStateVariablePeriod.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionReboot

        public class AsyncActionReboot
        {
            internal AsyncActionReboot(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object RebootBeginSync(uint aDelay)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDelay", aDelay);           
                
                return (iHandler.WriteEnd(null));
            }

            public void RebootBegin(uint aDelay)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aDelay", aDelay);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionReboot.RebootBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RebootEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionReboot.RebootEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionReboot.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDiagnostics iService;
        }
        
        
        // AsyncActionSetSongcastPercentageLoss

        public class AsyncActionSetSongcastPercentageLoss
        {
            internal AsyncActionSetSongcastPercentageLoss(ServiceDiagnostics aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object SetSongcastPercentageLossBeginSync(uint aPercentage)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPercentage", aPercentage);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSongcastPercentageLossBegin(uint aPercentage)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aPercentage", aPercentage);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Diagnostics.AsyncActionSetSongcastPercentageLoss.SetSongcastPercentageLossBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSongcastPercentageLossEnd(object aResult)
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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetSongcastPercentageLoss.SetSongcastPercentageLossEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Diagnostics.AsyncActionSetSongcastPercentageLoss.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceDiagnostics iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceDiagnostics): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventaStateVariable = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "aStateVariable", nsmanager);

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
					aStateVariable = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse aStateVariable with value {1}", DateTime.Now, value));
				}

                eventaStateVariable = true;
            }

            bool eventLastTerminalInputCode = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "LastTerminalInputCode", nsmanager);

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
					LastTerminalInputCode = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse LastTerminalInputCode with value {1}", DateTime.Now, value));
				}

                eventLastTerminalInputCode = true;
            }

            bool eventLastTerminalInputName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "LastTerminalInputName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                LastTerminalInputName = value;

                eventLastTerminalInputName = true;
            }

          
            
            if(eventaStateVariable)
            {
                if (EventStateaStateVariable != null)
                {
					try
					{
						EventStateaStateVariable(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateaStateVariable: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventLastTerminalInputCode)
            {
                if (EventStateLastTerminalInputCode != null)
                {
					try
					{
						EventStateLastTerminalInputCode(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateLastTerminalInputCode: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventLastTerminalInputName)
            {
                if (EventStateLastTerminalInputName != null)
                {
					try
					{
						EventStateLastTerminalInputName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateLastTerminalInputName: " + ex);
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
        public event EventHandler<EventArgs> EventStateaStateVariable;
        public event EventHandler<EventArgs> EventStateLastTerminalInputCode;
        public event EventHandler<EventArgs> EventStateLastTerminalInputName;

        public uint aStateVariable;
        public uint LastTerminalInputCode;
        public string LastTerminalInputName;
    }
}
    
