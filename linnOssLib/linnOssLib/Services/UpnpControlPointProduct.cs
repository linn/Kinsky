using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceProduct : ServiceUpnp
    {


        public ServiceProduct(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceProduct(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Manufacturer");
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            action.AddOutArgument(new Argument("Info", Argument.EType.eString));
            action.AddOutArgument(new Argument("Url", Argument.EType.eString));
            action.AddOutArgument(new Argument("ImageUri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Model");
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            action.AddOutArgument(new Argument("Info", Argument.EType.eString));
            action.AddOutArgument(new Argument("Url", Argument.EType.eString));
            action.AddOutArgument(new Argument("ImageUri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Product");
            action.AddOutArgument(new Argument("Room", Argument.EType.eString));
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            action.AddOutArgument(new Argument("Info", Argument.EType.eString));
            action.AddOutArgument(new Argument("Url", Argument.EType.eString));
            action.AddOutArgument(new Argument("ImageUri", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Standby");
            action.AddOutArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetStandby");
            action.AddInArgument(new Argument("Value", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SourceCount");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SourceXml");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceIndex");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSourceIndex");
            action.AddInArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSourceIndexByName");
            action.AddInArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Source");
            action.AddInArgument(new Argument("Index", Argument.EType.eUint));
            action.AddOutArgument(new Argument("SystemName", Argument.EType.eString));
            action.AddOutArgument(new Argument("Type", Argument.EType.eString));
            action.AddOutArgument(new Argument("Name", Argument.EType.eString));
            action.AddOutArgument(new Argument("Visible", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("Attributes");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SourceXmlChangeCount");
            action.AddOutArgument(new Argument("Value", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Product", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Product", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionManufacturer CreateAsyncActionManufacturer()
        {
            return (new AsyncActionManufacturer(this));
        }

        public AsyncActionModel CreateAsyncActionModel()
        {
            return (new AsyncActionModel(this));
        }

        public AsyncActionProduct CreateAsyncActionProduct()
        {
            return (new AsyncActionProduct(this));
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

        public AsyncActionSource CreateAsyncActionSource()
        {
            return (new AsyncActionSource(this));
        }

        public AsyncActionAttributes CreateAsyncActionAttributes()
        {
            return (new AsyncActionAttributes(this));
        }

        public AsyncActionSourceXmlChangeCount CreateAsyncActionSourceXmlChangeCount()
        {
            return (new AsyncActionSourceXmlChangeCount(this));
        }


        // Synchronous actions
        
        public void ManufacturerSync(out string Name, out string Info, out string Url, out string ImageUri)
        {
            AsyncActionManufacturer action = CreateAsyncActionManufacturer();
            
            object result = action.ManufacturerBeginSync();

            AsyncActionManufacturer.EventArgsResponse response = action.ManufacturerEnd(result);
                
            Name = response.Name;
            Info = response.Info;
            Url = response.Url;
            ImageUri = response.ImageUri;
        }
        
        public void ModelSync(out string Name, out string Info, out string Url, out string ImageUri)
        {
            AsyncActionModel action = CreateAsyncActionModel();
            
            object result = action.ModelBeginSync();

            AsyncActionModel.EventArgsResponse response = action.ModelEnd(result);
                
            Name = response.Name;
            Info = response.Info;
            Url = response.Url;
            ImageUri = response.ImageUri;
        }
        
        public void ProductSync(out string Room, out string Name, out string Info, out string Url, out string ImageUri)
        {
            AsyncActionProduct action = CreateAsyncActionProduct();
            
            object result = action.ProductBeginSync();

            AsyncActionProduct.EventArgsResponse response = action.ProductEnd(result);
                
            Room = response.Room;
            Name = response.Name;
            Info = response.Info;
            Url = response.Url;
            ImageUri = response.ImageUri;
        }
        
        public bool StandbySync()
        {
            AsyncActionStandby action = CreateAsyncActionStandby();
            
            object result = action.StandbyBeginSync();

            AsyncActionStandby.EventArgsResponse response = action.StandbyEnd(result);
                
            return(response.Value);
        }
        
        public void SetStandbySync(bool Value)
        {
            AsyncActionSetStandby action = CreateAsyncActionSetStandby();
            
            object result = action.SetStandbyBeginSync(Value);

            action.SetStandbyEnd(result);
        }
        
        public uint SourceCountSync()
        {
            AsyncActionSourceCount action = CreateAsyncActionSourceCount();
            
            object result = action.SourceCountBeginSync();

            AsyncActionSourceCount.EventArgsResponse response = action.SourceCountEnd(result);
                
            return(response.Value);
        }
        
        public string SourceXmlSync()
        {
            AsyncActionSourceXml action = CreateAsyncActionSourceXml();
            
            object result = action.SourceXmlBeginSync();

            AsyncActionSourceXml.EventArgsResponse response = action.SourceXmlEnd(result);
                
            return(response.Value);
        }
        
        public uint SourceIndexSync()
        {
            AsyncActionSourceIndex action = CreateAsyncActionSourceIndex();
            
            object result = action.SourceIndexBeginSync();

            AsyncActionSourceIndex.EventArgsResponse response = action.SourceIndexEnd(result);
                
            return(response.Value);
        }
        
        public void SetSourceIndexSync(uint Value)
        {
            AsyncActionSetSourceIndex action = CreateAsyncActionSetSourceIndex();
            
            object result = action.SetSourceIndexBeginSync(Value);

            action.SetSourceIndexEnd(result);
        }
        
        public void SetSourceIndexByNameSync(string Value)
        {
            AsyncActionSetSourceIndexByName action = CreateAsyncActionSetSourceIndexByName();
            
            object result = action.SetSourceIndexByNameBeginSync(Value);

            action.SetSourceIndexByNameEnd(result);
        }
        
        public void SourceSync(uint Index, out string SystemName, out string Type, out string Name, out bool Visible)
        {
            AsyncActionSource action = CreateAsyncActionSource();
            
            object result = action.SourceBeginSync(Index);

            AsyncActionSource.EventArgsResponse response = action.SourceEnd(result);
                
            SystemName = response.SystemName;
            Type = response.Type;
            Name = response.Name;
            Visible = response.Visible;
        }
        
        public string AttributesSync()
        {
            AsyncActionAttributes action = CreateAsyncActionAttributes();
            
            object result = action.AttributesBeginSync();

            AsyncActionAttributes.EventArgsResponse response = action.AttributesEnd(result);
                
            return(response.Value);
        }
        
        public uint SourceXmlChangeCountSync()
        {
            AsyncActionSourceXmlChangeCount action = CreateAsyncActionSourceXmlChangeCount();
            
            object result = action.SourceXmlChangeCountBeginSync();

            AsyncActionSourceXmlChangeCount.EventArgsResponse response = action.SourceXmlChangeCountEnd(result);
                
            return(response.Value);
        }
        

        // AsyncActionManufacturer

        public class AsyncActionManufacturer
        {
            internal AsyncActionManufacturer(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object ManufacturerBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ManufacturerBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionManufacturer.ManufacturerBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ManufacturerEnd(object aResult)
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
                    UserLog.WriteLine("Product.AsyncActionManufacturer.ManufacturerEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionManufacturer.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Name = aHandler.ReadArgumentString("Name");
                    Info = aHandler.ReadArgumentString("Info");
                    Url = aHandler.ReadArgumentString("Url");
                    ImageUri = aHandler.ReadArgumentString("ImageUri");
                }
                
                public string Name;
                public string Info;
                public string Url;
                public string ImageUri;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProduct iService;
        }
        
        
        // AsyncActionModel

        public class AsyncActionModel
        {
            internal AsyncActionModel(ServiceProduct aService)
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
                    UserLog.WriteLine("Product.AsyncActionModel.ModelBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionModel.ModelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionModel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Name = aHandler.ReadArgumentString("Name");
                    Info = aHandler.ReadArgumentString("Info");
                    Url = aHandler.ReadArgumentString("Url");
                    ImageUri = aHandler.ReadArgumentString("ImageUri");
                }
                
                public string Name;
                public string Info;
                public string Url;
                public string ImageUri;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProduct iService;
        }
        
        
        // AsyncActionProduct

        public class AsyncActionProduct
        {
            internal AsyncActionProduct(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object ProductBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ProductBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionProduct.ProductBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ProductEnd(object aResult)
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
                    UserLog.WriteLine("Product.AsyncActionProduct.ProductEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionProduct.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Room = aHandler.ReadArgumentString("Room");
                    Name = aHandler.ReadArgumentString("Name");
                    Info = aHandler.ReadArgumentString("Info");
                    Url = aHandler.ReadArgumentString("Url");
                    ImageUri = aHandler.ReadArgumentString("ImageUri");
                }
                
                public string Room;
                public string Name;
                public string Info;
                public string Url;
                public string ImageUri;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProduct iService;
        }
        
        
        // AsyncActionStandby

        public class AsyncActionStandby
        {
            internal AsyncActionStandby(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
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
                    UserLog.WriteLine("Product.AsyncActionStandby.StandbyBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionStandby.StandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionStandby.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSetStandby

        public class AsyncActionSetStandby
        {
            internal AsyncActionSetStandby(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object SetStandbyBeginSync(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStandbyBegin(bool Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionSetStandby.SetStandbyBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSetStandby.SetStandbyEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSetStandby.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSourceCount

        public class AsyncActionSourceCount
        {
            internal AsyncActionSourceCount(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
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
                    UserLog.WriteLine("Product.AsyncActionSourceCount.SourceCountBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSourceCount.SourceCountEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSourceCount.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSourceXml

        public class AsyncActionSourceXml
        {
            internal AsyncActionSourceXml(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
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
                    UserLog.WriteLine("Product.AsyncActionSourceXml.SourceXmlBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSourceXml.SourceXmlEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSourceXml.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSourceIndex

        public class AsyncActionSourceIndex
        {
            internal AsyncActionSourceIndex(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
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
                    UserLog.WriteLine("Product.AsyncActionSourceIndex.SourceIndexBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSourceIndex.SourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSetSourceIndex

        public class AsyncActionSetSourceIndex
        {
            internal AsyncActionSetSourceIndex(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object SetSourceIndexBeginSync(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceIndexBegin(uint Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndex.SetSourceIndexBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndex.SetSourceIndexEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndex.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSetSourceIndexByName

        public class AsyncActionSetSourceIndexByName
        {
            internal AsyncActionSetSourceIndexByName(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetSourceIndexByNameBeginSync(string Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Value", Value);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSourceIndexByNameBegin(string Value)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("Value", Value);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndexByName.SetSourceIndexByNameBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndexByName.SetSourceIndexByNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSetSourceIndexByName.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSource

        public class AsyncActionSource
        {
            internal AsyncActionSource(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object SourceBeginSync(uint Index)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Index", Index);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceBegin(uint Index)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("Index", Index);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionSource.SourceBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceEnd(object aResult)
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
                    UserLog.WriteLine("Product.AsyncActionSource.SourceEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSource.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    SystemName = aHandler.ReadArgumentString("SystemName");
                    Type = aHandler.ReadArgumentString("Type");
                    Name = aHandler.ReadArgumentString("Name");
                    Visible = aHandler.ReadArgumentBool("Visible");
                }
                
                public string SystemName;
                public string Type;
                public string Name;
                public bool Visible;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceProduct iService;
        }
        
        
        // AsyncActionAttributes

        public class AsyncActionAttributes
        {
            internal AsyncActionAttributes(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object AttributesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AttributesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionAttributes.AttributesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AttributesEnd(object aResult)
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
                    UserLog.WriteLine("Product.AsyncActionAttributes.AttributesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionAttributes.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
        }
        
        
        // AsyncActionSourceXmlChangeCount

        public class AsyncActionSourceXmlChangeCount
        {
            internal AsyncActionSourceXmlChangeCount(ServiceProduct aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SourceXmlChangeCountBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SourceXmlChangeCountBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Product.AsyncActionSourceXmlChangeCount.SourceXmlChangeCountBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SourceXmlChangeCountEnd(object aResult)
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
                    UserLog.WriteLine("Product.AsyncActionSourceXmlChangeCount.SourceXmlChangeCountEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Product.AsyncActionSourceXmlChangeCount.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceProduct iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceProduct): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventManufacturerName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ManufacturerName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ManufacturerName = value;

                eventManufacturerName = true;
            }

            bool eventManufacturerInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ManufacturerInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ManufacturerInfo = value;

                eventManufacturerInfo = true;
            }

            bool eventManufacturerUrl = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ManufacturerUrl", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ManufacturerUrl = value;

                eventManufacturerUrl = true;
            }

            bool eventManufacturerImageUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ManufacturerImageUri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ManufacturerImageUri = value;

                eventManufacturerImageUri = true;
            }

            bool eventModelName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ModelName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ModelName = value;

                eventModelName = true;
            }

            bool eventModelInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ModelInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ModelInfo = value;

                eventModelInfo = true;
            }

            bool eventModelUrl = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ModelUrl", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ModelUrl = value;

                eventModelUrl = true;
            }

            bool eventModelImageUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ModelImageUri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ModelImageUri = value;

                eventModelImageUri = true;
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

            bool eventProductInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductInfo = value;

                eventProductInfo = true;
            }

            bool eventProductUrl = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductUrl", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductUrl = value;

                eventProductUrl = true;
            }

            bool eventProductImageUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProductImageUri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProductImageUri = value;

                eventProductImageUri = true;
            }

            bool eventStandby = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Standby", nsmanager);

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
	                Standby = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Standby = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Standby = false; 
    	            }
                }

                eventStandby = true;
            }

            bool eventSourceIndex = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SourceIndex", nsmanager);

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
					SourceIndex = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SourceIndex with value {1}", DateTime.Now, value));
				}

                eventSourceIndex = true;
            }

            bool eventSourceCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SourceCount", nsmanager);

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
					SourceCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SourceCount with value {1}", DateTime.Now, value));
				}

                eventSourceCount = true;
            }

            bool eventSourceXml = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SourceXml", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SourceXml = value;

                eventSourceXml = true;
            }

            bool eventAttributes = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Attributes", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Attributes = value;

                eventAttributes = true;
            }

          
            
            if(eventManufacturerName)
            {
                if (EventStateManufacturerName != null)
                {
					try
					{
						EventStateManufacturerName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateManufacturerName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventManufacturerInfo)
            {
                if (EventStateManufacturerInfo != null)
                {
					try
					{
						EventStateManufacturerInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateManufacturerInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventManufacturerUrl)
            {
                if (EventStateManufacturerUrl != null)
                {
					try
					{
						EventStateManufacturerUrl(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateManufacturerUrl: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventManufacturerImageUri)
            {
                if (EventStateManufacturerImageUri != null)
                {
					try
					{
						EventStateManufacturerImageUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateManufacturerImageUri: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventModelName)
            {
                if (EventStateModelName != null)
                {
					try
					{
						EventStateModelName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateModelName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventModelInfo)
            {
                if (EventStateModelInfo != null)
                {
					try
					{
						EventStateModelInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateModelInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventModelUrl)
            {
                if (EventStateModelUrl != null)
                {
					try
					{
						EventStateModelUrl(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateModelUrl: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventModelImageUri)
            {
                if (EventStateModelImageUri != null)
                {
					try
					{
						EventStateModelImageUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateModelImageUri: " + ex);
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
            
            if(eventProductInfo)
            {
                if (EventStateProductInfo != null)
                {
					try
					{
						EventStateProductInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductUrl)
            {
                if (EventStateProductUrl != null)
                {
					try
					{
						EventStateProductUrl(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductUrl: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProductImageUri)
            {
                if (EventStateProductImageUri != null)
                {
					try
					{
						EventStateProductImageUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProductImageUri: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventStandby)
            {
                if (EventStateStandby != null)
                {
					try
					{
						EventStateStandby(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateStandby: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSourceIndex)
            {
                if (EventStateSourceIndex != null)
                {
					try
					{
						EventStateSourceIndex(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSourceIndex: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSourceCount)
            {
                if (EventStateSourceCount != null)
                {
					try
					{
						EventStateSourceCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSourceCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSourceXml)
            {
                if (EventStateSourceXml != null)
                {
					try
					{
						EventStateSourceXml(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSourceXml: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAttributes)
            {
                if (EventStateAttributes != null)
                {
					try
					{
						EventStateAttributes(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAttributes: " + ex);
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
        public event EventHandler<EventArgs> EventStateManufacturerName;
        public event EventHandler<EventArgs> EventStateManufacturerInfo;
        public event EventHandler<EventArgs> EventStateManufacturerUrl;
        public event EventHandler<EventArgs> EventStateManufacturerImageUri;
        public event EventHandler<EventArgs> EventStateModelName;
        public event EventHandler<EventArgs> EventStateModelInfo;
        public event EventHandler<EventArgs> EventStateModelUrl;
        public event EventHandler<EventArgs> EventStateModelImageUri;
        public event EventHandler<EventArgs> EventStateProductRoom;
        public event EventHandler<EventArgs> EventStateProductName;
        public event EventHandler<EventArgs> EventStateProductInfo;
        public event EventHandler<EventArgs> EventStateProductUrl;
        public event EventHandler<EventArgs> EventStateProductImageUri;
        public event EventHandler<EventArgs> EventStateStandby;
        public event EventHandler<EventArgs> EventStateSourceIndex;
        public event EventHandler<EventArgs> EventStateSourceCount;
        public event EventHandler<EventArgs> EventStateSourceXml;
        public event EventHandler<EventArgs> EventStateAttributes;

        public string ManufacturerName;
        public string ManufacturerInfo;
        public string ManufacturerUrl;
        public string ManufacturerImageUri;
        public string ModelName;
        public string ModelInfo;
        public string ModelUrl;
        public string ModelImageUri;
        public string ProductRoom;
        public string ProductName;
        public string ProductInfo;
        public string ProductUrl;
        public string ProductImageUri;
        public bool Standby;
        public uint SourceIndex;
        public uint SourceCount;
        public string SourceXml;
        public string Attributes;
    }
}
    
