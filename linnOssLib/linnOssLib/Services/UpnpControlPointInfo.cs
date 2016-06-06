using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceInfo : ServiceUpnp
    {


        public ServiceInfo(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceInfo(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Counters");
            action.AddOutArgument(new Argument("TrackCount", Argument.EType.eUint));
            action.AddOutArgument(new Argument("DetailsCount", Argument.EType.eUint));
            action.AddOutArgument(new Argument("MetatextCount", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Track");
            action.AddOutArgument(new Argument("Uri", Argument.EType.eString));
            action.AddOutArgument(new Argument("Metadata", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Details");
            action.AddOutArgument(new Argument("Duration", Argument.EType.eUint));
            action.AddOutArgument(new Argument("BitRate", Argument.EType.eUint));
            action.AddOutArgument(new Argument("BitDepth", Argument.EType.eUint));
            action.AddOutArgument(new Argument("SampleRate", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Lossless", Argument.EType.eBool));
            action.AddOutArgument(new Argument("CodecName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Metatext");
            action.AddOutArgument(new Argument("Value", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("av.openhome.org", "Info", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("av.openhome.org", "Info", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionCounters CreateAsyncActionCounters()
        {
            return (new AsyncActionCounters(this));
        }

        public AsyncActionTrack CreateAsyncActionTrack()
        {
            return (new AsyncActionTrack(this));
        }

        public AsyncActionDetails CreateAsyncActionDetails()
        {
            return (new AsyncActionDetails(this));
        }

        public AsyncActionMetatext CreateAsyncActionMetatext()
        {
            return (new AsyncActionMetatext(this));
        }


        // Synchronous actions
        
        public void CountersSync(out uint TrackCount, out uint DetailsCount, out uint MetatextCount)
        {
            AsyncActionCounters action = CreateAsyncActionCounters();
            
            object result = action.CountersBeginSync();

            AsyncActionCounters.EventArgsResponse response = action.CountersEnd(result);
                
            TrackCount = response.TrackCount;
            DetailsCount = response.DetailsCount;
            MetatextCount = response.MetatextCount;
        }
        
        public void TrackSync(out string Uri, out string Metadata)
        {
            AsyncActionTrack action = CreateAsyncActionTrack();
            
            object result = action.TrackBeginSync();

            AsyncActionTrack.EventArgsResponse response = action.TrackEnd(result);
                
            Uri = response.Uri;
            Metadata = response.Metadata;
        }
        
        public void DetailsSync(out uint Duration, out uint BitRate, out uint BitDepth, out uint SampleRate, out bool Lossless, out string CodecName)
        {
            AsyncActionDetails action = CreateAsyncActionDetails();
            
            object result = action.DetailsBeginSync();

            AsyncActionDetails.EventArgsResponse response = action.DetailsEnd(result);
                
            Duration = response.Duration;
            BitRate = response.BitRate;
            BitDepth = response.BitDepth;
            SampleRate = response.SampleRate;
            Lossless = response.Lossless;
            CodecName = response.CodecName;
        }
        
        public string MetatextSync()
        {
            AsyncActionMetatext action = CreateAsyncActionMetatext();
            
            object result = action.MetatextBeginSync();

            AsyncActionMetatext.EventArgsResponse response = action.MetatextEnd(result);
                
            return(response.Value);
        }
        

        // AsyncActionCounters

        public class AsyncActionCounters
        {
            internal AsyncActionCounters(ServiceInfo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object CountersBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CountersBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Info.AsyncActionCounters.CountersBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CountersEnd(object aResult)
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
                    UserLog.WriteLine("Info.AsyncActionCounters.CountersEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Info.AsyncActionCounters.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    TrackCount = aHandler.ReadArgumentUint("TrackCount");
                    DetailsCount = aHandler.ReadArgumentUint("DetailsCount");
                    MetatextCount = aHandler.ReadArgumentUint("MetatextCount");
                }
                
                public uint TrackCount;
                public uint DetailsCount;
                public uint MetatextCount;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceInfo iService;
        }
        
        
        // AsyncActionTrack

        public class AsyncActionTrack
        {
            internal AsyncActionTrack(ServiceInfo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object TrackBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrackBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Info.AsyncActionTrack.TrackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrackEnd(object aResult)
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
                    UserLog.WriteLine("Info.AsyncActionTrack.TrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Info.AsyncActionTrack.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceInfo iService;
        }
        
        
        // AsyncActionDetails

        public class AsyncActionDetails
        {
            internal AsyncActionDetails(ServiceInfo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object DetailsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DetailsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Info.AsyncActionDetails.DetailsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DetailsEnd(object aResult)
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
                    UserLog.WriteLine("Info.AsyncActionDetails.DetailsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Info.AsyncActionDetails.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Duration = aHandler.ReadArgumentUint("Duration");
                    BitRate = aHandler.ReadArgumentUint("BitRate");
                    BitDepth = aHandler.ReadArgumentUint("BitDepth");
                    SampleRate = aHandler.ReadArgumentUint("SampleRate");
                    Lossless = aHandler.ReadArgumentBool("Lossless");
                    CodecName = aHandler.ReadArgumentString("CodecName");
                }
                
                public uint Duration;
                public uint BitRate;
                public uint BitDepth;
                public uint SampleRate;
                public bool Lossless;
                public string CodecName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceInfo iService;
        }
        
        
        // AsyncActionMetatext

        public class AsyncActionMetatext
        {
            internal AsyncActionMetatext(ServiceInfo aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object MetatextBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void MetatextBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Info.AsyncActionMetatext.MetatextBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse MetatextEnd(object aResult)
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
                    UserLog.WriteLine("Info.AsyncActionMetatext.MetatextEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Info.AsyncActionMetatext.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceInfo iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceInfo): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventTrackCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TrackCount", nsmanager);

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
					TrackCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse TrackCount with value {1}", DateTime.Now, value));
				}

                eventTrackCount = true;
            }

            bool eventDetailsCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DetailsCount", nsmanager);

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
					DetailsCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse DetailsCount with value {1}", DateTime.Now, value));
				}

                eventDetailsCount = true;
            }

            bool eventMetatextCount = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "MetatextCount", nsmanager);

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
					MetatextCount = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse MetatextCount with value {1}", DateTime.Now, value));
				}

                eventMetatextCount = true;
            }

            bool eventUri = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Uri", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Uri = value;

                eventUri = true;
            }

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

            bool eventDuration = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Duration", nsmanager);

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
					Duration = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Duration with value {1}", DateTime.Now, value));
				}

                eventDuration = true;
            }

            bool eventBitRate = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "BitRate", nsmanager);

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
					BitRate = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse BitRate with value {1}", DateTime.Now, value));
				}

                eventBitRate = true;
            }

            bool eventBitDepth = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "BitDepth", nsmanager);

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
					BitDepth = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse BitDepth with value {1}", DateTime.Now, value));
				}

                eventBitDepth = true;
            }

            bool eventSampleRate = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SampleRate", nsmanager);

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
					SampleRate = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SampleRate with value {1}", DateTime.Now, value));
				}

                eventSampleRate = true;
            }

            bool eventLossless = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Lossless", nsmanager);

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
	                Lossless = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		Lossless = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	Lossless = false; 
    	            }
                }

                eventLossless = true;
            }

            bool eventCodecName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "CodecName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                CodecName = value;

                eventCodecName = true;
            }

            bool eventMetatext = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Metatext", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Metatext = value;

                eventMetatext = true;
            }

          
            
            if(eventTrackCount)
            {
                if (EventStateTrackCount != null)
                {
					try
					{
						EventStateTrackCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTrackCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDetailsCount)
            {
                if (EventStateDetailsCount != null)
                {
					try
					{
						EventStateDetailsCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDetailsCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventMetatextCount)
            {
                if (EventStateMetatextCount != null)
                {
					try
					{
						EventStateMetatextCount(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateMetatextCount: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventUri)
            {
                if (EventStateUri != null)
                {
					try
					{
						EventStateUri(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateUri: " + ex);
						Assert.CheckDebug(false);
					}
                }
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
            
            if(eventDuration)
            {
                if (EventStateDuration != null)
                {
					try
					{
						EventStateDuration(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDuration: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventBitRate)
            {
                if (EventStateBitRate != null)
                {
					try
					{
						EventStateBitRate(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateBitRate: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventBitDepth)
            {
                if (EventStateBitDepth != null)
                {
					try
					{
						EventStateBitDepth(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateBitDepth: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSampleRate)
            {
                if (EventStateSampleRate != null)
                {
					try
					{
						EventStateSampleRate(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSampleRate: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventLossless)
            {
                if (EventStateLossless != null)
                {
					try
					{
						EventStateLossless(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateLossless: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventCodecName)
            {
                if (EventStateCodecName != null)
                {
					try
					{
						EventStateCodecName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateCodecName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventMetatext)
            {
                if (EventStateMetatext != null)
                {
					try
					{
						EventStateMetatext(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateMetatext: " + ex);
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
        public event EventHandler<EventArgs> EventStateTrackCount;
        public event EventHandler<EventArgs> EventStateDetailsCount;
        public event EventHandler<EventArgs> EventStateMetatextCount;
        public event EventHandler<EventArgs> EventStateUri;
        public event EventHandler<EventArgs> EventStateMetadata;
        public event EventHandler<EventArgs> EventStateDuration;
        public event EventHandler<EventArgs> EventStateBitRate;
        public event EventHandler<EventArgs> EventStateBitDepth;
        public event EventHandler<EventArgs> EventStateSampleRate;
        public event EventHandler<EventArgs> EventStateLossless;
        public event EventHandler<EventArgs> EventStateCodecName;
        public event EventHandler<EventArgs> EventStateMetatext;

        public uint TrackCount;
        public uint DetailsCount;
        public uint MetatextCount;
        public string Uri;
        public string Metadata;
        public uint Duration;
        public uint BitRate;
        public uint BitDepth;
        public uint SampleRate;
        public bool Lossless;
        public string CodecName;
        public string Metatext;
    }
}
    
