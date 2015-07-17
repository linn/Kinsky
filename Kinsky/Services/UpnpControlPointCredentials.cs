using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceCredentials : ServiceUpnp
    {


        public ServiceCredentials(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceCredentials(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Set");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            action.AddInArgument(new Argument("UserName", Argument.EType.eString));
            action.AddInArgument(new Argument("Password", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("Clear");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetEnabled");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            action.AddInArgument(new Argument("Enabled", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Get");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            action.AddOutArgument(new Argument("UserName", Argument.EType.eString));
            action.AddOutArgument(new Argument("Password", Argument.EType.eBinary));
            action.AddOutArgument(new Argument("Enabled", Argument.EType.eBool));
            action.AddOutArgument(new Argument("Status", Argument.EType.eString));
            action.AddOutArgument(new Argument("Data", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Login");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            action.AddOutArgument(new Argument("Token", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ReLogin");
            action.AddInArgument(new Argument("Id", Argument.EType.eString));
            action.AddInArgument(new Argument("CurrentToken", Argument.EType.eString));
            action.AddOutArgument(new Argument("NewToken", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetIds");
            action.AddOutArgument(new Argument("Ids", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetPublicKey");
            action.AddOutArgument(new Argument("PublicKey", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetSequenceNumber");
            action.AddOutArgument(new Argument("SequenceNumber", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Credentials", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Credentials", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSet CreateAsyncActionSet()
        {
            return (new AsyncActionSet(this));
        }

        public AsyncActionClear CreateAsyncActionClear()
        {
            return (new AsyncActionClear(this));
        }

        public AsyncActionSetEnabled CreateAsyncActionSetEnabled()
        {
            return (new AsyncActionSetEnabled(this));
        }

        public AsyncActionGet CreateAsyncActionGet()
        {
            return (new AsyncActionGet(this));
        }

        public AsyncActionLogin CreateAsyncActionLogin()
        {
            return (new AsyncActionLogin(this));
        }

        public AsyncActionReLogin CreateAsyncActionReLogin()
        {
            return (new AsyncActionReLogin(this));
        }

        public AsyncActionGetIds CreateAsyncActionGetIds()
        {
            return (new AsyncActionGetIds(this));
        }

        public AsyncActionGetPublicKey CreateAsyncActionGetPublicKey()
        {
            return (new AsyncActionGetPublicKey(this));
        }

        public AsyncActionGetSequenceNumber CreateAsyncActionGetSequenceNumber()
        {
            return (new AsyncActionGetSequenceNumber(this));
        }


        // Synchronous actions
        
        public void SetSync(string Id, string UserName, byte[] Password)
        {
            AsyncActionSet action = CreateAsyncActionSet();
            
            object result = action.SetBeginSync(Id, UserName, Password);

            action.SetEnd(result);
        }
        
        public void ClearSync(string Id)
        {
            AsyncActionClear action = CreateAsyncActionClear();
            
            object result = action.ClearBeginSync(Id);

            action.ClearEnd(result);
        }
        
        public void SetEnabledSync(string Id, bool Enabled)
        {
            AsyncActionSetEnabled action = CreateAsyncActionSetEnabled();
            
            object result = action.SetEnabledBeginSync(Id, Enabled);

            action.SetEnabledEnd(result);
        }
        
        public void GetSync(string Id, out string UserName, out byte[] Password, out bool Enabled, out string Status, out string Data)
        {
            AsyncActionGet action = CreateAsyncActionGet();
            
            object result = action.GetBeginSync(Id);

            AsyncActionGet.EventArgsResponse response = action.GetEnd(result);
                
            UserName = response.UserName;
            Password = response.Password;
            Enabled = response.Enabled;
            Status = response.Status;
            Data = response.Data;
        }
        
        public string LoginSync(string Id)
        {
            AsyncActionLogin action = CreateAsyncActionLogin();
            
            object result = action.LoginBeginSync(Id);

            AsyncActionLogin.EventArgsResponse response = action.LoginEnd(result);
                
            return(response.Token);
        }
        
        public string ReLoginSync(string Id, string CurrentToken)
        {
            AsyncActionReLogin action = CreateAsyncActionReLogin();
            
            object result = action.ReLoginBeginSync(Id, CurrentToken);

            AsyncActionReLogin.EventArgsResponse response = action.ReLoginEnd(result);
                
            return(response.NewToken);
        }
        
        public string GetIdsSync()
        {
            AsyncActionGetIds action = CreateAsyncActionGetIds();
            
            object result = action.GetIdsBeginSync();

            AsyncActionGetIds.EventArgsResponse response = action.GetIdsEnd(result);
                
            return(response.Ids);
        }
        
        public string GetPublicKeySync()
        {
            AsyncActionGetPublicKey action = CreateAsyncActionGetPublicKey();
            
            object result = action.GetPublicKeyBeginSync();

            AsyncActionGetPublicKey.EventArgsResponse response = action.GetPublicKeyEnd(result);
                
            return(response.PublicKey);
        }
        
        public uint GetSequenceNumberSync()
        {
            AsyncActionGetSequenceNumber action = CreateAsyncActionGetSequenceNumber();
            
            object result = action.GetSequenceNumberBeginSync();

            AsyncActionGetSequenceNumber.EventArgsResponse response = action.GetSequenceNumberEnd(result);
                
            return(response.SequenceNumber);
        }
        

        // AsyncActionSet

        public class AsyncActionSet
        {
            internal AsyncActionSet(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetBeginSync(string Id, string UserName, byte[] Password)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                iHandler.WriteArgumentString("UserName", UserName);           
                iHandler.WriteArgumentBinary("Password", Password);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBegin(string Id, string UserName, byte[] Password)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                iHandler.WriteArgumentString("UserName", UserName);                
                iHandler.WriteArgumentBinary("Password", Password);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionSet.SetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionSet.SetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionSet.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionClear

        public class AsyncActionClear
        {
            internal AsyncActionClear(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object ClearBeginSync(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ClearBegin(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionClear.ClearBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ClearEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionClear.ClearEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionClear.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionSetEnabled

        public class AsyncActionSetEnabled
        {
            internal AsyncActionSetEnabled(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object SetEnabledBeginSync(string Id, bool Enabled)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                iHandler.WriteArgumentBool("Enabled", Enabled);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetEnabledBegin(string Id, bool Enabled)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                iHandler.WriteArgumentBool("Enabled", Enabled);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionSetEnabled.SetEnabledBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetEnabledEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionSetEnabled.SetEnabledEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionSetEnabled.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionGet

        public class AsyncActionGet
        {
            internal AsyncActionGet(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object GetBeginSync(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetBegin(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionGet.GetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionGet.GetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionGet.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    UserName = aHandler.ReadArgumentString("UserName");
                    Password = aHandler.ReadArgumentBinary("Password");
                    Enabled = aHandler.ReadArgumentBool("Enabled");
                    Status = aHandler.ReadArgumentString("Status");
                    Data = aHandler.ReadArgumentString("Data");
                }
                
                public string UserName;
                public byte[] Password;
                public bool Enabled;
                public string Status;
                public string Data;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionLogin

        public class AsyncActionLogin
        {
            internal AsyncActionLogin(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object LoginBeginSync(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                
                return (iHandler.WriteEnd(null));
            }

            public void LoginBegin(string Id)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionLogin.LoginBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse LoginEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionLogin.LoginEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionLogin.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Token = aHandler.ReadArgumentString("Token");
                }
                
                public string Token;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionReLogin

        public class AsyncActionReLogin
        {
            internal AsyncActionReLogin(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object ReLoginBeginSync(string Id, string CurrentToken)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);           
                iHandler.WriteArgumentString("CurrentToken", CurrentToken);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReLoginBegin(string Id, string CurrentToken)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Id", Id);                
                iHandler.WriteArgumentString("CurrentToken", CurrentToken);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionReLogin.ReLoginBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ReLoginEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionReLogin.ReLoginEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionReLogin.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NewToken = aHandler.ReadArgumentString("NewToken");
                }
                
                public string NewToken;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionGetIds

        public class AsyncActionGetIds
        {
            internal AsyncActionGetIds(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object GetIdsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetIdsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionGetIds.GetIdsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetIdsEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionGetIds.GetIdsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionGetIds.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Ids = aHandler.ReadArgumentString("Ids");
                }
                
                public string Ids;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionGetPublicKey

        public class AsyncActionGetPublicKey
        {
            internal AsyncActionGetPublicKey(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object GetPublicKeyBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetPublicKeyBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionGetPublicKey.GetPublicKeyBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetPublicKeyEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionGetPublicKey.GetPublicKeyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionGetPublicKey.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    PublicKey = aHandler.ReadArgumentString("PublicKey");
                }
                
                public string PublicKey;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
        }
        
        
        // AsyncActionGetSequenceNumber

        public class AsyncActionGetSequenceNumber
        {
            internal AsyncActionGetSequenceNumber(ServiceCredentials aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object GetSequenceNumberBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSequenceNumberBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Credentials.AsyncActionGetSequenceNumber.GetSequenceNumberBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSequenceNumberEnd(object aResult)
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
                    UserLog.WriteLine("Credentials.AsyncActionGetSequenceNumber.GetSequenceNumberEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Credentials.AsyncActionGetSequenceNumber.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SequenceNumber = aHandler.ReadArgumentUint("SequenceNumber");
                }
                
                public uint SequenceNumber;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceCredentials iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceCredentials): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventIds = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Ids", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Ids = value;

                eventIds = true;
            }

            bool eventPublicKey = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PublicKey", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                PublicKey = value;

                eventPublicKey = true;
            }

            bool eventSequenceNumber = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SequenceNumber", nsmanager);

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
					SequenceNumber = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SequenceNumber with value {1}", DateTime.Now, value));
				}

                eventSequenceNumber = true;
            }

          
            
            if(eventIds)
            {
                if (EventStateIds != null)
                {
					try
					{
						EventStateIds(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateIds: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventPublicKey)
            {
                if (EventStatePublicKey != null)
                {
					try
					{
						EventStatePublicKey(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePublicKey: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSequenceNumber)
            {
                if (EventStateSequenceNumber != null)
                {
					try
					{
						EventStateSequenceNumber(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSequenceNumber: " + ex);
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
        public event EventHandler<EventArgs> EventStateIds;
        public event EventHandler<EventArgs> EventStatePublicKey;
        public event EventHandler<EventArgs> EventStateSequenceNumber;

        public string Ids;
        public string PublicKey;
        public uint SequenceNumber;
    }
}
    
