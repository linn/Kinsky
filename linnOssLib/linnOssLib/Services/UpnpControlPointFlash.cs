using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceFlash : ServiceUpnp
    {


        public ServiceFlash(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceFlash(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Read");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddInArgument(new Argument("aAddress", Argument.EType.eUint));
            action.AddInArgument(new Argument("aLength", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBuffer", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("Write");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddInArgument(new Argument("aAddress", Argument.EType.eUint));
            action.AddInArgument(new Argument("aSource", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("Erase");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddInArgument(new Argument("aAddress", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("EraseSector");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddInArgument(new Argument("aSector", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("EraseSectors");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddInArgument(new Argument("aFirstSector", Argument.EType.eUint));
            action.AddInArgument(new Argument("aLastSector", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("EraseChip");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Sectors");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aSectors", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SectorBytes");
            action.AddInArgument(new Argument("aId", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBytes", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("RomDirInfo");
            action.AddOutArgument(new Argument("aFlashIdMain", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aOffsetMain", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBytesMain", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aFlashIdFallback", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aOffsetFallback", Argument.EType.eUint));
            action.AddOutArgument(new Argument("aBytesFallback", Argument.EType.eUint));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Flash", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Flash", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionRead CreateAsyncActionRead()
        {
            return (new AsyncActionRead(this));
        }

        public AsyncActionWrite CreateAsyncActionWrite()
        {
            return (new AsyncActionWrite(this));
        }

        public AsyncActionErase CreateAsyncActionErase()
        {
            return (new AsyncActionErase(this));
        }

        public AsyncActionEraseSector CreateAsyncActionEraseSector()
        {
            return (new AsyncActionEraseSector(this));
        }

        public AsyncActionEraseSectors CreateAsyncActionEraseSectors()
        {
            return (new AsyncActionEraseSectors(this));
        }

        public AsyncActionEraseChip CreateAsyncActionEraseChip()
        {
            return (new AsyncActionEraseChip(this));
        }

        public AsyncActionSectors CreateAsyncActionSectors()
        {
            return (new AsyncActionSectors(this));
        }

        public AsyncActionSectorBytes CreateAsyncActionSectorBytes()
        {
            return (new AsyncActionSectorBytes(this));
        }

        public AsyncActionRomDirInfo CreateAsyncActionRomDirInfo()
        {
            return (new AsyncActionRomDirInfo(this));
        }


        // Synchronous actions
        
        public byte[] ReadSync(uint aId, uint aAddress, uint aLength)
        {
            AsyncActionRead action = CreateAsyncActionRead();
            
            object result = action.ReadBeginSync(aId, aAddress, aLength);

            AsyncActionRead.EventArgsResponse response = action.ReadEnd(result);
                
            return(response.aBuffer);
        }
        
        public void WriteSync(uint aId, uint aAddress, byte[] aSource)
        {
            AsyncActionWrite action = CreateAsyncActionWrite();
            
            object result = action.WriteBeginSync(aId, aAddress, aSource);

            action.WriteEnd(result);
        }
        
        public void EraseSync(uint aId, uint aAddress)
        {
            AsyncActionErase action = CreateAsyncActionErase();
            
            object result = action.EraseBeginSync(aId, aAddress);

            action.EraseEnd(result);
        }
        
        public void EraseSectorSync(uint aId, uint aSector)
        {
            AsyncActionEraseSector action = CreateAsyncActionEraseSector();
            
            object result = action.EraseSectorBeginSync(aId, aSector);

            action.EraseSectorEnd(result);
        }
        
        public void EraseSectorsSync(uint aId, uint aFirstSector, uint aLastSector)
        {
            AsyncActionEraseSectors action = CreateAsyncActionEraseSectors();
            
            object result = action.EraseSectorsBeginSync(aId, aFirstSector, aLastSector);

            action.EraseSectorsEnd(result);
        }
        
        public void EraseChipSync(uint aId)
        {
            AsyncActionEraseChip action = CreateAsyncActionEraseChip();
            
            object result = action.EraseChipBeginSync(aId);

            action.EraseChipEnd(result);
        }
        
        public uint SectorsSync(uint aId)
        {
            AsyncActionSectors action = CreateAsyncActionSectors();
            
            object result = action.SectorsBeginSync(aId);

            AsyncActionSectors.EventArgsResponse response = action.SectorsEnd(result);
                
            return(response.aSectors);
        }
        
        public uint SectorBytesSync(uint aId)
        {
            AsyncActionSectorBytes action = CreateAsyncActionSectorBytes();
            
            object result = action.SectorBytesBeginSync(aId);

            AsyncActionSectorBytes.EventArgsResponse response = action.SectorBytesEnd(result);
                
            return(response.aBytes);
        }
        
        public void RomDirInfoSync(out uint aFlashIdMain, out uint aOffsetMain, out uint aBytesMain, out uint aFlashIdFallback, out uint aOffsetFallback, out uint aBytesFallback)
        {
            AsyncActionRomDirInfo action = CreateAsyncActionRomDirInfo();
            
            object result = action.RomDirInfoBeginSync();

            AsyncActionRomDirInfo.EventArgsResponse response = action.RomDirInfoEnd(result);
                
            aFlashIdMain = response.aFlashIdMain;
            aOffsetMain = response.aOffsetMain;
            aBytesMain = response.aBytesMain;
            aFlashIdFallback = response.aFlashIdFallback;
            aOffsetFallback = response.aOffsetFallback;
            aBytesFallback = response.aBytesFallback;
        }
        

        // AsyncActionRead

        public class AsyncActionRead
        {
            internal AsyncActionRead(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object ReadBeginSync(uint aId, uint aAddress, uint aLength)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                iHandler.WriteArgumentUint("aAddress", aAddress);           
                iHandler.WriteArgumentUint("aLength", aLength);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ReadBegin(uint aId, uint aAddress, uint aLength)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                iHandler.WriteArgumentUint("aAddress", aAddress);                
                iHandler.WriteArgumentUint("aLength", aLength);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionRead.ReadBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Flash.AsyncActionRead.ReadEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionRead.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBuffer = aHandler.ReadArgumentBinary("aBuffer");
                }
                
                public byte[] aBuffer;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceFlash iService;
        }
        
        
        // AsyncActionWrite

        public class AsyncActionWrite
        {
            internal AsyncActionWrite(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object WriteBeginSync(uint aId, uint aAddress, byte[] aSource)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                iHandler.WriteArgumentUint("aAddress", aAddress);           
                iHandler.WriteArgumentBinary("aSource", aSource);           
                
                return (iHandler.WriteEnd(null));
            }

            public void WriteBegin(uint aId, uint aAddress, byte[] aSource)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                iHandler.WriteArgumentUint("aAddress", aAddress);                
                iHandler.WriteArgumentBinary("aSource", aSource);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionWrite.WriteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse WriteEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionWrite.WriteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionWrite.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceFlash iService;
        }
        
        
        // AsyncActionErase

        public class AsyncActionErase
        {
            internal AsyncActionErase(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object EraseBeginSync(uint aId, uint aAddress)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                iHandler.WriteArgumentUint("aAddress", aAddress);           
                
                return (iHandler.WriteEnd(null));
            }

            public void EraseBegin(uint aId, uint aAddress)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                iHandler.WriteArgumentUint("aAddress", aAddress);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionErase.EraseBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EraseEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionErase.EraseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionErase.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceFlash iService;
        }
        
        
        // AsyncActionEraseSector

        public class AsyncActionEraseSector
        {
            internal AsyncActionEraseSector(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object EraseSectorBeginSync(uint aId, uint aSector)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                iHandler.WriteArgumentUint("aSector", aSector);           
                
                return (iHandler.WriteEnd(null));
            }

            public void EraseSectorBegin(uint aId, uint aSector)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                iHandler.WriteArgumentUint("aSector", aSector);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionEraseSector.EraseSectorBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EraseSectorEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionEraseSector.EraseSectorEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionEraseSector.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceFlash iService;
        }
        
        
        // AsyncActionEraseSectors

        public class AsyncActionEraseSectors
        {
            internal AsyncActionEraseSectors(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object EraseSectorsBeginSync(uint aId, uint aFirstSector, uint aLastSector)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                iHandler.WriteArgumentUint("aFirstSector", aFirstSector);           
                iHandler.WriteArgumentUint("aLastSector", aLastSector);           
                
                return (iHandler.WriteEnd(null));
            }

            public void EraseSectorsBegin(uint aId, uint aFirstSector, uint aLastSector)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                iHandler.WriteArgumentUint("aFirstSector", aFirstSector);                
                iHandler.WriteArgumentUint("aLastSector", aLastSector);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionEraseSectors.EraseSectorsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EraseSectorsEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionEraseSectors.EraseSectorsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionEraseSectors.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceFlash iService;
        }
        
        
        // AsyncActionEraseChip

        public class AsyncActionEraseChip
        {
            internal AsyncActionEraseChip(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object EraseChipBeginSync(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void EraseChipBegin(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionEraseChip.EraseChipBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EraseChipEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionEraseChip.EraseChipEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionEraseChip.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceFlash iService;
        }
        
        
        // AsyncActionSectors

        public class AsyncActionSectors
        {
            internal AsyncActionSectors(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SectorsBeginSync(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SectorsBegin(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionSectors.SectorsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SectorsEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionSectors.SectorsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionSectors.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSectors = aHandler.ReadArgumentUint("aSectors");
                }
                
                public uint aSectors;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceFlash iService;
        }
        
        
        // AsyncActionSectorBytes

        public class AsyncActionSectorBytes
        {
            internal AsyncActionSectorBytes(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SectorBytesBeginSync(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SectorBytesBegin(uint aId)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("aId", aId);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionSectorBytes.SectorBytesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SectorBytesEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionSectorBytes.SectorBytesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionSectorBytes.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aBytes = aHandler.ReadArgumentUint("aBytes");
                }
                
                public uint aBytes;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceFlash iService;
        }
        
        
        // AsyncActionRomDirInfo

        public class AsyncActionRomDirInfo
        {
            internal AsyncActionRomDirInfo(ServiceFlash aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object RomDirInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RomDirInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Flash.AsyncActionRomDirInfo.RomDirInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RomDirInfoEnd(object aResult)
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
                    UserLog.WriteLine("Flash.AsyncActionRomDirInfo.RomDirInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Flash.AsyncActionRomDirInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aFlashIdMain = aHandler.ReadArgumentUint("aFlashIdMain");
                    aOffsetMain = aHandler.ReadArgumentUint("aOffsetMain");
                    aBytesMain = aHandler.ReadArgumentUint("aBytesMain");
                    aFlashIdFallback = aHandler.ReadArgumentUint("aFlashIdFallback");
                    aOffsetFallback = aHandler.ReadArgumentUint("aOffsetFallback");
                    aBytesFallback = aHandler.ReadArgumentUint("aBytesFallback");
                }
                
                public uint aFlashIdMain;
                public uint aOffsetMain;
                public uint aBytesMain;
                public uint aFlashIdFallback;
                public uint aOffsetFallback;
                public uint aBytesFallback;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceFlash iService;
        }
        
        

        protected override void EventServerEvent(EventServerUpnp obj, EventArgsEvent e)
        {
        }

    }
}
    
