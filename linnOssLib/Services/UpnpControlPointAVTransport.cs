using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceAVTransport : ServiceUpnp
    {

        public const string kTransportStateStopped = "STOPPED";
        public const string kTransportStatePausedPlayback = "PAUSED_PLAYBACK";
        public const string kTransportStatePausedRecording = "PAUSED_RECORDING";
        public const string kTransportStatePlaying = "PLAYING";
        public const string kTransportStateRecording = "RECORDING";
        public const string kTransportStateTransitioning = "TRANSITIONING";
        public const string kTransportStateNoMediaPresent = "NO_MEDIA_PRESENT";
        public const string kTransportStatusOk = "OK";
        public const string kTransportStatusErrorOccurred = "ERROR_OCCURRED";
        public const string kCurrentMediaCategoryNoMedia = "NO_MEDIA";
        public const string kCurrentMediaCategoryTrackAware = "TRACK_AWARE";
        public const string kCurrentMediaCategoryTrackUnaware = "TRACK_UNAWARE";
        public const string kPlaybackStorageMediumUnknown = "UNKNOWN";
        public const string kPlaybackStorageMediumDv = "DV";
        public const string kPlaybackStorageMediumMiniDv = "MINI-DV";
        public const string kPlaybackStorageMediumVhs = "VHS";
        public const string kPlaybackStorageMediumWVhs = "W-VHS";
        public const string kPlaybackStorageMediumSVhs = "S-VHS";
        public const string kPlaybackStorageMediumDVhs = "D-VHS";
        public const string kPlaybackStorageMediumVhsc = "VHSC";
        public const string kPlaybackStorageMediumVideo8 = "VIDEO8";
        public const string kPlaybackStorageMediumHi8 = "HI8";
        public const string kPlaybackStorageMediumCdRom = "CD-ROM";
        public const string kPlaybackStorageMediumCdDa = "CD-DA";
        public const string kPlaybackStorageMediumCdR = "CD-R";
        public const string kPlaybackStorageMediumCdRw = "CD-RW";
        public const string kPlaybackStorageMediumVideoCd = "VIDEO-CD";
        public const string kPlaybackStorageMediumSacd = "SACD";
        public const string kPlaybackStorageMediumMdAudio = "MD-AUDIO";
        public const string kPlaybackStorageMediumMdPicture = "MD-PICTURE";
        public const string kPlaybackStorageMediumDvdRom = "DVD-ROM";
        public const string kPlaybackStorageMediumDvdVideo = "DVD-VIDEO";
        public const string kPlaybackStorageMediumDvdR = "DVD-R";
        public const string kPlaybackStorageMediumDvdPlusRw = "DVD+RW";
        public const string kPlaybackStorageMediumDvdRw = "DVD-RW";
        public const string kPlaybackStorageMediumDvdRam = "DVD-RAM";
        public const string kPlaybackStorageMediumDvdAudio = "DVD-AUDIO";
        public const string kPlaybackStorageMediumDat = "DAT";
        public const string kPlaybackStorageMediumLd = "LD";
        public const string kPlaybackStorageMediumHdd = "HDD";
        public const string kPlaybackStorageMediumMicroMv = "MICRO-MV";
        public const string kPlaybackStorageMediumNetwork = "NETWORK";
        public const string kPlaybackStorageMediumNone = "NONE";
        public const string kPlaybackStorageMediumNotImplemented = "NOT_IMPLEMENTED";
        public const string kPlaybackStorageMediumSd = "SD";
        public const string kPlaybackStorageMediumPcCard = "PC-CARD";
        public const string kPlaybackStorageMediumMmc = "MMC";
        public const string kPlaybackStorageMediumCf = "CF";
        public const string kPlaybackStorageMediumBd = "BD";
        public const string kPlaybackStorageMediumMs = "MS";
        public const string kPlaybackStorageMediumHdDvd = "HD_DVD";
        public const string kRecordStorageMediumUnknown = "UNKNOWN";
        public const string kRecordStorageMediumDv = "DV";
        public const string kRecordStorageMediumMiniDv = "MINI-DV";
        public const string kRecordStorageMediumVhs = "VHS";
        public const string kRecordStorageMediumWVhs = "W-VHS";
        public const string kRecordStorageMediumSVhs = "S-VHS";
        public const string kRecordStorageMediumDVhs = "D-VHS";
        public const string kRecordStorageMediumVhsc = "VHSC";
        public const string kRecordStorageMediumVideo8 = "VIDEO8";
        public const string kRecordStorageMediumHi8 = "HI8";
        public const string kRecordStorageMediumCdRom = "CD-ROM";
        public const string kRecordStorageMediumCdDa = "CD-DA";
        public const string kRecordStorageMediumCdR = "CD-R";
        public const string kRecordStorageMediumCdRw = "CD-RW";
        public const string kRecordStorageMediumVideoCd = "VIDEO-CD";
        public const string kRecordStorageMediumSacd = "SACD";
        public const string kRecordStorageMediumMdAudio = "MD-AUDIO";
        public const string kRecordStorageMediumMdPicture = "MD-PICTURE";
        public const string kRecordStorageMediumDvdRom = "DVD-ROM";
        public const string kRecordStorageMediumDvdVideo = "DVD-VIDEO";
        public const string kRecordStorageMediumDvdR = "DVD-R";
        public const string kRecordStorageMediumDvdPlusRw = "DVD+RW";
        public const string kRecordStorageMediumDvdRw = "DVD-RW";
        public const string kRecordStorageMediumDvdRam = "DVD-RAM";
        public const string kRecordStorageMediumDvdAudio = "DVD-AUDIO";
        public const string kRecordStorageMediumDat = "DAT";
        public const string kRecordStorageMediumLd = "LD";
        public const string kRecordStorageMediumHdd = "HDD";
        public const string kRecordStorageMediumMicroMv = "MICRO-MV";
        public const string kRecordStorageMediumNetwork = "NETWORK";
        public const string kRecordStorageMediumNone = "NONE";
        public const string kRecordStorageMediumNotImplemented = "NOT_IMPLEMENTED";
        public const string kRecordStorageMediumSd = "SD";
        public const string kRecordStorageMediumPcCard = "PC-CARD";
        public const string kRecordStorageMediumMmc = "MMC";
        public const string kRecordStorageMediumCf = "CF";
        public const string kRecordStorageMediumBd = "BD";
        public const string kRecordStorageMediumMs = "MS";
        public const string kRecordStorageMediumHdDvd = "HD_DVD";
        public const string kCurrentPlayModeNormal = "NORMAL";
        public const string kCurrentPlayModeShuffle = "SHUFFLE";
        public const string kCurrentPlayModeRepeatOne = "REPEAT_ONE";
        public const string kCurrentPlayModeRepeatAll = "REPEAT_ALL";
        public const string kCurrentPlayModeRandom = "RANDOM";
        public const string kCurrentPlayModeDirect1 = "DIRECT_1";
        public const string kCurrentPlayModeIntro = "INTRO";
        public const string kTransportPlaySpeed1 = "1";
        public const string kRecordMediumWriteStatusWritable = "WRITABLE";
        public const string kRecordMediumWriteStatusProtected = "PROTECTED";
        public const string kRecordMediumWriteStatusNotWritable = "NOT_WRITABLE";
        public const string kRecordMediumWriteStatusUnknown = "UNKNOWN";
        public const string kRecordMediumWriteStatusNotImplemented = "NOT_IMPLEMENTED";
        public const string kCurrentRecordQualityMode0Ep = "0:EP";
        public const string kCurrentRecordQualityMode1Lp = "1:LP";
        public const string kCurrentRecordQualityMode2Sp = "2:SP";
        public const string kCurrentRecordQualityMode0Basic = "0:BASIC";
        public const string kCurrentRecordQualityMode1Medium = "1:MEDIUM";
        public const string kCurrentRecordQualityMode2High = "2:HIGH";
        public const string kCurrentRecordQualityModeNotImplemented = "NOT_IMPLEMENTED";
        public const string kDRMStateOk = "OK";
        public const string kDRMStateUnknown = "UNKNOWN";
        public const string kDRMStateProcessingContentKey = "PROCESSING_CONTENT_KEY";
        public const string kDRMStateContentKeyFailure = "CONTENT_KEY_FAILURE";
        public const string kDRMStateAttemptingAuthentication = "ATTEMPTING_AUTHENTICATION";
        public const string kDRMStateFailedAuthentication = "FAILED_AUTHENTICATION";
        public const string kDRMStateNotAuthenticated = "NOT_AUTHENTICATED";
        public const string kDRMStateDeviceRevocation = "DEVICE_REVOCATION";
        public const string kSeekModeAbsTime = "ABS_TIME";
        public const string kSeekModeRelTime = "REL_TIME";
        public const string kSeekModeAbsCount = "ABS_COUNT";
        public const string kSeekModeRelCount = "REL_COUNT";
        public const string kSeekModeTrackNr = "TRACK_NR";
        public const string kSeekModeChannelFreq = "CHANNEL_FREQ";
        public const string kSeekModeTapeIndex = "TAPE-INDEX";
        public const string kSeekModeFrame = "FRAME";

        public ServiceAVTransport(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceAVTransport(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("SetAVTransportURI");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("CurrentURI", Argument.EType.eString));
            action.AddInArgument(new Argument("CurrentURIMetaData", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetNextAVTransportURI");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("NextURI", Argument.EType.eString));
            action.AddInArgument(new Argument("NextURIMetaData", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetMediaInfo");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("NrTracks", Argument.EType.eUint));
            action.AddOutArgument(new Argument("MediaDuration", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentURIMetaData", Argument.EType.eString));
            action.AddOutArgument(new Argument("NextURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("NextURIMetaData", Argument.EType.eString));
            action.AddOutArgument(new Argument("PlayMedium", Argument.EType.eString));
            action.AddOutArgument(new Argument("RecordMedium", Argument.EType.eString));
            action.AddOutArgument(new Argument("WriteStatus", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetMediaInfo_Ext");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentType", Argument.EType.eString));
            action.AddOutArgument(new Argument("NrTracks", Argument.EType.eUint));
            action.AddOutArgument(new Argument("MediaDuration", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentURIMetaData", Argument.EType.eString));
            action.AddOutArgument(new Argument("NextURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("NextURIMetaData", Argument.EType.eString));
            action.AddOutArgument(new Argument("PlayMedium", Argument.EType.eString));
            action.AddOutArgument(new Argument("RecordMedium", Argument.EType.eString));
            action.AddOutArgument(new Argument("WriteStatus", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetTransportInfo");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentTransportState", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentTransportStatus", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentSpeed", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetPositionInfo");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Track", Argument.EType.eUint));
            action.AddOutArgument(new Argument("TrackDuration", Argument.EType.eString));
            action.AddOutArgument(new Argument("TrackMetaData", Argument.EType.eString));
            action.AddOutArgument(new Argument("TrackURI", Argument.EType.eString));
            action.AddOutArgument(new Argument("RelTime", Argument.EType.eString));
            action.AddOutArgument(new Argument("AbsTime", Argument.EType.eString));
            action.AddOutArgument(new Argument("RelCount", Argument.EType.eInt));
            action.AddOutArgument(new Argument("AbsCount", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetDeviceCapabilities");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("PlayMedia", Argument.EType.eString));
            action.AddOutArgument(new Argument("RecMedia", Argument.EType.eString));
            action.AddOutArgument(new Argument("RecQualityModes", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetTransportSettings");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("PlayMode", Argument.EType.eString));
            action.AddOutArgument(new Argument("RecQualityMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Stop");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Play");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Speed", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Pause");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Record");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Seek");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Unit", Argument.EType.eString));
            action.AddInArgument(new Argument("Target", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Next");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Previous");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetPlayMode");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("NewPlayMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetRecordQualityMode");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("NewRecordQualityMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetCurrentTransportActions");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("Actions", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetDRMState");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurentDRMState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetStateVariables");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("StateVariableList", Argument.EType.eString));
            action.AddOutArgument(new Argument("StateVariableValuePairs", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetStateVariables");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("AVTransportUDN", Argument.EType.eString));
            action.AddInArgument(new Argument("ServiceType", Argument.EType.eString));
            action.AddInArgument(new Argument("ServiceId", Argument.EType.eString));
            action.AddInArgument(new Argument("StateVariableValuePairs", Argument.EType.eString));
            action.AddOutArgument(new Argument("StateVariableList", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("upnp.org", "AVTransport", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("upnp.org", "AVTransport", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionSetAVTransportURI CreateAsyncActionSetAVTransportURI()
        {
            return (new AsyncActionSetAVTransportURI(this));
        }

        public AsyncActionSetNextAVTransportURI CreateAsyncActionSetNextAVTransportURI()
        {
            return (new AsyncActionSetNextAVTransportURI(this));
        }

        public AsyncActionGetMediaInfo CreateAsyncActionGetMediaInfo()
        {
            return (new AsyncActionGetMediaInfo(this));
        }

        public AsyncActionGetMediaInfo_Ext CreateAsyncActionGetMediaInfo_Ext()
        {
            return (new AsyncActionGetMediaInfo_Ext(this));
        }

        public AsyncActionGetTransportInfo CreateAsyncActionGetTransportInfo()
        {
            return (new AsyncActionGetTransportInfo(this));
        }

        public AsyncActionGetPositionInfo CreateAsyncActionGetPositionInfo()
        {
            return (new AsyncActionGetPositionInfo(this));
        }

        public AsyncActionGetDeviceCapabilities CreateAsyncActionGetDeviceCapabilities()
        {
            return (new AsyncActionGetDeviceCapabilities(this));
        }

        public AsyncActionGetTransportSettings CreateAsyncActionGetTransportSettings()
        {
            return (new AsyncActionGetTransportSettings(this));
        }

        public AsyncActionStop CreateAsyncActionStop()
        {
            return (new AsyncActionStop(this));
        }

        public AsyncActionPlay CreateAsyncActionPlay()
        {
            return (new AsyncActionPlay(this));
        }

        public AsyncActionPause CreateAsyncActionPause()
        {
            return (new AsyncActionPause(this));
        }

        public AsyncActionRecord CreateAsyncActionRecord()
        {
            return (new AsyncActionRecord(this));
        }

        public AsyncActionSeek CreateAsyncActionSeek()
        {
            return (new AsyncActionSeek(this));
        }

        public AsyncActionNext CreateAsyncActionNext()
        {
            return (new AsyncActionNext(this));
        }

        public AsyncActionPrevious CreateAsyncActionPrevious()
        {
            return (new AsyncActionPrevious(this));
        }

        public AsyncActionSetPlayMode CreateAsyncActionSetPlayMode()
        {
            return (new AsyncActionSetPlayMode(this));
        }

        public AsyncActionSetRecordQualityMode CreateAsyncActionSetRecordQualityMode()
        {
            return (new AsyncActionSetRecordQualityMode(this));
        }

        public AsyncActionGetCurrentTransportActions CreateAsyncActionGetCurrentTransportActions()
        {
            return (new AsyncActionGetCurrentTransportActions(this));
        }

        public AsyncActionGetDRMState CreateAsyncActionGetDRMState()
        {
            return (new AsyncActionGetDRMState(this));
        }

        public AsyncActionGetStateVariables CreateAsyncActionGetStateVariables()
        {
            return (new AsyncActionGetStateVariables(this));
        }

        public AsyncActionSetStateVariables CreateAsyncActionSetStateVariables()
        {
            return (new AsyncActionSetStateVariables(this));
        }


        // Synchronous actions
        
        public void SetAVTransportURISync(uint InstanceID, string CurrentURI, string CurrentURIMetaData)
        {
            AsyncActionSetAVTransportURI action = CreateAsyncActionSetAVTransportURI();
            
            object result = action.SetAVTransportURIBeginSync(InstanceID, CurrentURI, CurrentURIMetaData);

            action.SetAVTransportURIEnd(result);
        }
        
        public void SetNextAVTransportURISync(uint InstanceID, string NextURI, string NextURIMetaData)
        {
            AsyncActionSetNextAVTransportURI action = CreateAsyncActionSetNextAVTransportURI();
            
            object result = action.SetNextAVTransportURIBeginSync(InstanceID, NextURI, NextURIMetaData);

            action.SetNextAVTransportURIEnd(result);
        }
        
        public void GetMediaInfoSync(uint InstanceID, out uint NrTracks, out string MediaDuration, out string CurrentURI, out string CurrentURIMetaData, out string NextURI, out string NextURIMetaData, out string PlayMedium, out string RecordMedium, out string WriteStatus)
        {
            AsyncActionGetMediaInfo action = CreateAsyncActionGetMediaInfo();
            
            object result = action.GetMediaInfoBeginSync(InstanceID);

            AsyncActionGetMediaInfo.EventArgsResponse response = action.GetMediaInfoEnd(result);
                
            NrTracks = response.NrTracks;
            MediaDuration = response.MediaDuration;
            CurrentURI = response.CurrentURI;
            CurrentURIMetaData = response.CurrentURIMetaData;
            NextURI = response.NextURI;
            NextURIMetaData = response.NextURIMetaData;
            PlayMedium = response.PlayMedium;
            RecordMedium = response.RecordMedium;
            WriteStatus = response.WriteStatus;
        }
        
        public void GetMediaInfo_ExtSync(uint InstanceID, out string CurrentType, out uint NrTracks, out string MediaDuration, out string CurrentURI, out string CurrentURIMetaData, out string NextURI, out string NextURIMetaData, out string PlayMedium, out string RecordMedium, out string WriteStatus)
        {
            AsyncActionGetMediaInfo_Ext action = CreateAsyncActionGetMediaInfo_Ext();
            
            object result = action.GetMediaInfo_ExtBeginSync(InstanceID);

            AsyncActionGetMediaInfo_Ext.EventArgsResponse response = action.GetMediaInfo_ExtEnd(result);
                
            CurrentType = response.CurrentType;
            NrTracks = response.NrTracks;
            MediaDuration = response.MediaDuration;
            CurrentURI = response.CurrentURI;
            CurrentURIMetaData = response.CurrentURIMetaData;
            NextURI = response.NextURI;
            NextURIMetaData = response.NextURIMetaData;
            PlayMedium = response.PlayMedium;
            RecordMedium = response.RecordMedium;
            WriteStatus = response.WriteStatus;
        }
        
        public void GetTransportInfoSync(uint InstanceID, out string CurrentTransportState, out string CurrentTransportStatus, out string CurrentSpeed)
        {
            AsyncActionGetTransportInfo action = CreateAsyncActionGetTransportInfo();
            
            object result = action.GetTransportInfoBeginSync(InstanceID);

            AsyncActionGetTransportInfo.EventArgsResponse response = action.GetTransportInfoEnd(result);
                
            CurrentTransportState = response.CurrentTransportState;
            CurrentTransportStatus = response.CurrentTransportStatus;
            CurrentSpeed = response.CurrentSpeed;
        }
        
        public void GetPositionInfoSync(uint InstanceID, out uint Track, out string TrackDuration, out string TrackMetaData, out string TrackURI, out string RelTime, out string AbsTime, out int RelCount, out uint AbsCount)
        {
            AsyncActionGetPositionInfo action = CreateAsyncActionGetPositionInfo();
            
            object result = action.GetPositionInfoBeginSync(InstanceID);

            AsyncActionGetPositionInfo.EventArgsResponse response = action.GetPositionInfoEnd(result);
                
            Track = response.Track;
            TrackDuration = response.TrackDuration;
            TrackMetaData = response.TrackMetaData;
            TrackURI = response.TrackURI;
            RelTime = response.RelTime;
            AbsTime = response.AbsTime;
            RelCount = response.RelCount;
            AbsCount = response.AbsCount;
        }
        
        public void GetDeviceCapabilitiesSync(uint InstanceID, out string PlayMedia, out string RecMedia, out string RecQualityModes)
        {
            AsyncActionGetDeviceCapabilities action = CreateAsyncActionGetDeviceCapabilities();
            
            object result = action.GetDeviceCapabilitiesBeginSync(InstanceID);

            AsyncActionGetDeviceCapabilities.EventArgsResponse response = action.GetDeviceCapabilitiesEnd(result);
                
            PlayMedia = response.PlayMedia;
            RecMedia = response.RecMedia;
            RecQualityModes = response.RecQualityModes;
        }
        
        public void GetTransportSettingsSync(uint InstanceID, out string PlayMode, out string RecQualityMode)
        {
            AsyncActionGetTransportSettings action = CreateAsyncActionGetTransportSettings();
            
            object result = action.GetTransportSettingsBeginSync(InstanceID);

            AsyncActionGetTransportSettings.EventArgsResponse response = action.GetTransportSettingsEnd(result);
                
            PlayMode = response.PlayMode;
            RecQualityMode = response.RecQualityMode;
        }
        
        public void StopSync(uint InstanceID)
        {
            AsyncActionStop action = CreateAsyncActionStop();
            
            object result = action.StopBeginSync(InstanceID);

            action.StopEnd(result);
        }
        
        public void PlaySync(uint InstanceID, string Speed)
        {
            AsyncActionPlay action = CreateAsyncActionPlay();
            
            object result = action.PlayBeginSync(InstanceID, Speed);

            action.PlayEnd(result);
        }
        
        public void PauseSync(uint InstanceID)
        {
            AsyncActionPause action = CreateAsyncActionPause();
            
            object result = action.PauseBeginSync(InstanceID);

            action.PauseEnd(result);
        }
        
        public void RecordSync(uint InstanceID)
        {
            AsyncActionRecord action = CreateAsyncActionRecord();
            
            object result = action.RecordBeginSync(InstanceID);

            action.RecordEnd(result);
        }
        
        public void SeekSync(uint InstanceID, string Unit, string Target)
        {
            AsyncActionSeek action = CreateAsyncActionSeek();
            
            object result = action.SeekBeginSync(InstanceID, Unit, Target);

            action.SeekEnd(result);
        }
        
        public void NextSync(uint InstanceID)
        {
            AsyncActionNext action = CreateAsyncActionNext();
            
            object result = action.NextBeginSync(InstanceID);

            action.NextEnd(result);
        }
        
        public void PreviousSync(uint InstanceID)
        {
            AsyncActionPrevious action = CreateAsyncActionPrevious();
            
            object result = action.PreviousBeginSync(InstanceID);

            action.PreviousEnd(result);
        }
        
        public void SetPlayModeSync(uint InstanceID, string NewPlayMode)
        {
            AsyncActionSetPlayMode action = CreateAsyncActionSetPlayMode();
            
            object result = action.SetPlayModeBeginSync(InstanceID, NewPlayMode);

            action.SetPlayModeEnd(result);
        }
        
        public void SetRecordQualityModeSync(uint InstanceID, string NewRecordQualityMode)
        {
            AsyncActionSetRecordQualityMode action = CreateAsyncActionSetRecordQualityMode();
            
            object result = action.SetRecordQualityModeBeginSync(InstanceID, NewRecordQualityMode);

            action.SetRecordQualityModeEnd(result);
        }
        
        public string GetCurrentTransportActionsSync(uint InstanceID)
        {
            AsyncActionGetCurrentTransportActions action = CreateAsyncActionGetCurrentTransportActions();
            
            object result = action.GetCurrentTransportActionsBeginSync(InstanceID);

            AsyncActionGetCurrentTransportActions.EventArgsResponse response = action.GetCurrentTransportActionsEnd(result);
                
            return(response.Actions);
        }
        
        public string GetDRMStateSync(uint InstanceID)
        {
            AsyncActionGetDRMState action = CreateAsyncActionGetDRMState();
            
            object result = action.GetDRMStateBeginSync(InstanceID);

            AsyncActionGetDRMState.EventArgsResponse response = action.GetDRMStateEnd(result);
                
            return(response.CurentDRMState);
        }
        
        public string GetStateVariablesSync(uint InstanceID, string StateVariableList)
        {
            AsyncActionGetStateVariables action = CreateAsyncActionGetStateVariables();
            
            object result = action.GetStateVariablesBeginSync(InstanceID, StateVariableList);

            AsyncActionGetStateVariables.EventArgsResponse response = action.GetStateVariablesEnd(result);
                
            return(response.StateVariableValuePairs);
        }
        
        public string SetStateVariablesSync(uint InstanceID, string AVTransportUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
        {
            AsyncActionSetStateVariables action = CreateAsyncActionSetStateVariables();
            
            object result = action.SetStateVariablesBeginSync(InstanceID, AVTransportUDN, ServiceType, ServiceId, StateVariableValuePairs);

            AsyncActionSetStateVariables.EventArgsResponse response = action.SetStateVariablesEnd(result);
                
            return(response.StateVariableList);
        }
        

        // AsyncActionSetAVTransportURI

        public class AsyncActionSetAVTransportURI
        {
            internal AsyncActionSetAVTransportURI(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object SetAVTransportURIBeginSync(uint InstanceID, string CurrentURI, string CurrentURIMetaData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("CurrentURI", CurrentURI);           
                iHandler.WriteArgumentString("CurrentURIMetaData", CurrentURIMetaData);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetAVTransportURIBegin(uint InstanceID, string CurrentURI, string CurrentURIMetaData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("CurrentURI", CurrentURI);                
                iHandler.WriteArgumentString("CurrentURIMetaData", CurrentURIMetaData);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSetAVTransportURI.SetAVTransportURIBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetAVTransportURIEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSetAVTransportURI.SetAVTransportURIEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSetAVTransportURI.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionSetNextAVTransportURI

        public class AsyncActionSetNextAVTransportURI
        {
            internal AsyncActionSetNextAVTransportURI(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object SetNextAVTransportURIBeginSync(uint InstanceID, string NextURI, string NextURIMetaData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("NextURI", NextURI);           
                iHandler.WriteArgumentString("NextURIMetaData", NextURIMetaData);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetNextAVTransportURIBegin(uint InstanceID, string NextURI, string NextURIMetaData)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("NextURI", NextURI);                
                iHandler.WriteArgumentString("NextURIMetaData", NextURIMetaData);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSetNextAVTransportURI.SetNextAVTransportURIBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetNextAVTransportURIEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSetNextAVTransportURI.SetNextAVTransportURIEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSetNextAVTransportURI.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetMediaInfo

        public class AsyncActionGetMediaInfo
        {
            internal AsyncActionGetMediaInfo(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object GetMediaInfoBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetMediaInfoBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo.GetMediaInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetMediaInfoEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo.GetMediaInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    NrTracks = aHandler.ReadArgumentUint("NrTracks");
                    MediaDuration = aHandler.ReadArgumentString("MediaDuration");
                    CurrentURI = aHandler.ReadArgumentString("CurrentURI");
                    CurrentURIMetaData = aHandler.ReadArgumentString("CurrentURIMetaData");
                    NextURI = aHandler.ReadArgumentString("NextURI");
                    NextURIMetaData = aHandler.ReadArgumentString("NextURIMetaData");
                    PlayMedium = aHandler.ReadArgumentString("PlayMedium");
                    RecordMedium = aHandler.ReadArgumentString("RecordMedium");
                    WriteStatus = aHandler.ReadArgumentString("WriteStatus");
                }
                
                public uint NrTracks;
                public string MediaDuration;
                public string CurrentURI;
                public string CurrentURIMetaData;
                public string NextURI;
                public string NextURIMetaData;
                public string PlayMedium;
                public string RecordMedium;
                public string WriteStatus;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetMediaInfo_Ext

        public class AsyncActionGetMediaInfo_Ext
        {
            internal AsyncActionGetMediaInfo_Ext(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object GetMediaInfo_ExtBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetMediaInfo_ExtBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo_Ext.GetMediaInfo_ExtBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetMediaInfo_ExtEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo_Ext.GetMediaInfo_ExtEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetMediaInfo_Ext.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentType = aHandler.ReadArgumentString("CurrentType");
                    NrTracks = aHandler.ReadArgumentUint("NrTracks");
                    MediaDuration = aHandler.ReadArgumentString("MediaDuration");
                    CurrentURI = aHandler.ReadArgumentString("CurrentURI");
                    CurrentURIMetaData = aHandler.ReadArgumentString("CurrentURIMetaData");
                    NextURI = aHandler.ReadArgumentString("NextURI");
                    NextURIMetaData = aHandler.ReadArgumentString("NextURIMetaData");
                    PlayMedium = aHandler.ReadArgumentString("PlayMedium");
                    RecordMedium = aHandler.ReadArgumentString("RecordMedium");
                    WriteStatus = aHandler.ReadArgumentString("WriteStatus");
                }
                
                public string CurrentType;
                public uint NrTracks;
                public string MediaDuration;
                public string CurrentURI;
                public string CurrentURIMetaData;
                public string NextURI;
                public string NextURIMetaData;
                public string PlayMedium;
                public string RecordMedium;
                public string WriteStatus;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetTransportInfo

        public class AsyncActionGetTransportInfo
        {
            internal AsyncActionGetTransportInfo(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object GetTransportInfoBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetTransportInfoBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportInfo.GetTransportInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetTransportInfoEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportInfo.GetTransportInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentTransportState = aHandler.ReadArgumentString("CurrentTransportState");
                    CurrentTransportStatus = aHandler.ReadArgumentString("CurrentTransportStatus");
                    CurrentSpeed = aHandler.ReadArgumentString("CurrentSpeed");
                }
                
                public string CurrentTransportState;
                public string CurrentTransportStatus;
                public string CurrentSpeed;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetPositionInfo

        public class AsyncActionGetPositionInfo
        {
            internal AsyncActionGetPositionInfo(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object GetPositionInfoBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetPositionInfoBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetPositionInfo.GetPositionInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetPositionInfoEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetPositionInfo.GetPositionInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetPositionInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Track = aHandler.ReadArgumentUint("Track");
                    TrackDuration = aHandler.ReadArgumentString("TrackDuration");
                    TrackMetaData = aHandler.ReadArgumentString("TrackMetaData");
                    TrackURI = aHandler.ReadArgumentString("TrackURI");
                    RelTime = aHandler.ReadArgumentString("RelTime");
                    AbsTime = aHandler.ReadArgumentString("AbsTime");
                    RelCount = aHandler.ReadArgumentInt("RelCount");
                    AbsCount = aHandler.ReadArgumentUint("AbsCount");
                }
                
                public uint Track;
                public string TrackDuration;
                public string TrackMetaData;
                public string TrackURI;
                public string RelTime;
                public string AbsTime;
                public int RelCount;
                public uint AbsCount;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetDeviceCapabilities

        public class AsyncActionGetDeviceCapabilities
        {
            internal AsyncActionGetDeviceCapabilities(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object GetDeviceCapabilitiesBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetDeviceCapabilitiesBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetDeviceCapabilities.GetDeviceCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetDeviceCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetDeviceCapabilities.GetDeviceCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetDeviceCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    PlayMedia = aHandler.ReadArgumentString("PlayMedia");
                    RecMedia = aHandler.ReadArgumentString("RecMedia");
                    RecQualityModes = aHandler.ReadArgumentString("RecQualityModes");
                }
                
                public string PlayMedia;
                public string RecMedia;
                public string RecQualityModes;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetTransportSettings

        public class AsyncActionGetTransportSettings
        {
            internal AsyncActionGetTransportSettings(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object GetTransportSettingsBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetTransportSettingsBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportSettings.GetTransportSettingsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetTransportSettingsEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportSettings.GetTransportSettingsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetTransportSettings.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    PlayMode = aHandler.ReadArgumentString("PlayMode");
                    RecQualityMode = aHandler.ReadArgumentString("RecQualityMode");
                }
                
                public string PlayMode;
                public string RecQualityMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionStop

        public class AsyncActionStop
        {
            internal AsyncActionStop(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object StopBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void StopBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionStop.StopBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse StopEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionStop.StopEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionStop.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionPlay

        public class AsyncActionPlay
        {
            internal AsyncActionPlay(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object PlayBeginSync(uint InstanceID, string Speed)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Speed", Speed);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PlayBegin(uint InstanceID, string Speed)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Speed", Speed);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionPlay.PlayBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlayEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionPlay.PlayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionPlay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionPause

        public class AsyncActionPause
        {
            internal AsyncActionPause(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object PauseBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PauseBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionPause.PauseBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PauseEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionPause.PauseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionPause.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionRecord

        public class AsyncActionRecord
        {
            internal AsyncActionRecord(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object RecordBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void RecordBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionRecord.RecordBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RecordEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionRecord.RecordEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionRecord.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionSeek

        public class AsyncActionSeek
        {
            internal AsyncActionSeek(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SeekBeginSync(uint InstanceID, string Unit, string Target)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Unit", Unit);           
                iHandler.WriteArgumentString("Target", Target);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SeekBegin(uint InstanceID, string Unit, string Target)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Unit", Unit);                
                iHandler.WriteArgumentString("Target", Target);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSeek.SeekBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SeekEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSeek.SeekEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSeek.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionNext

        public class AsyncActionNext
        {
            internal AsyncActionNext(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object NextBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void NextBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionNext.NextBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse NextEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionNext.NextEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionNext.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionPrevious

        public class AsyncActionPrevious
        {
            internal AsyncActionPrevious(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object PreviousBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void PreviousBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionPrevious.PreviousBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PreviousEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionPrevious.PreviousEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionPrevious.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionSetPlayMode

        public class AsyncActionSetPlayMode
        {
            internal AsyncActionSetPlayMode(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object SetPlayModeBeginSync(uint InstanceID, string NewPlayMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("NewPlayMode", NewPlayMode);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetPlayModeBegin(uint InstanceID, string NewPlayMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("NewPlayMode", NewPlayMode);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSetPlayMode.SetPlayModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetPlayModeEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSetPlayMode.SetPlayModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSetPlayMode.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionSetRecordQualityMode

        public class AsyncActionSetRecordQualityMode
        {
            internal AsyncActionSetRecordQualityMode(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object SetRecordQualityModeBeginSync(uint InstanceID, string NewRecordQualityMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("NewRecordQualityMode", NewRecordQualityMode);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRecordQualityModeBegin(uint InstanceID, string NewRecordQualityMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("NewRecordQualityMode", NewRecordQualityMode);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSetRecordQualityMode.SetRecordQualityModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRecordQualityModeEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSetRecordQualityMode.SetRecordQualityModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSetRecordQualityMode.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetCurrentTransportActions

        public class AsyncActionGetCurrentTransportActions
        {
            internal AsyncActionGetCurrentTransportActions(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object GetCurrentTransportActionsBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetCurrentTransportActionsBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetCurrentTransportActions.GetCurrentTransportActionsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetCurrentTransportActionsEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetCurrentTransportActions.GetCurrentTransportActionsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetCurrentTransportActions.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    Actions = aHandler.ReadArgumentString("Actions");
                }
                
                public string Actions;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetDRMState

        public class AsyncActionGetDRMState
        {
            internal AsyncActionGetDRMState(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object GetDRMStateBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetDRMStateBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetDRMState.GetDRMStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetDRMStateEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetDRMState.GetDRMStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetDRMState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurentDRMState = aHandler.ReadArgumentString("CurentDRMState");
                }
                
                public string CurentDRMState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionGetStateVariables

        public class AsyncActionGetStateVariables
        {
            internal AsyncActionGetStateVariables(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object GetStateVariablesBeginSync(uint InstanceID, string StateVariableList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("StateVariableList", StateVariableList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetStateVariablesBegin(uint InstanceID, string StateVariableList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("StateVariableList", StateVariableList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionGetStateVariables.GetStateVariablesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetStateVariablesEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionGetStateVariables.GetStateVariablesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionGetStateVariables.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    StateVariableValuePairs = aHandler.ReadArgumentString("StateVariableValuePairs");
                }
                
                public string StateVariableValuePairs;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
        }
        
        
        // AsyncActionSetStateVariables

        public class AsyncActionSetStateVariables
        {
            internal AsyncActionSetStateVariables(ServiceAVTransport aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(20));
                iService = aService;
            }

            internal object SetStateVariablesBeginSync(uint InstanceID, string AVTransportUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("AVTransportUDN", AVTransportUDN);           
                iHandler.WriteArgumentString("ServiceType", ServiceType);           
                iHandler.WriteArgumentString("ServiceId", ServiceId);           
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStateVariablesBegin(uint InstanceID, string AVTransportUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("AVTransportUDN", AVTransportUDN);                
                iHandler.WriteArgumentString("ServiceType", ServiceType);                
                iHandler.WriteArgumentString("ServiceId", ServiceId);                
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("AVTransport.AsyncActionSetStateVariables.SetStateVariablesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetStateVariablesEnd(object aResult)
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
                    UserLog.WriteLine("AVTransport.AsyncActionSetStateVariables.SetStateVariablesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("AVTransport.AsyncActionSetStateVariables.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    StateVariableList = aHandler.ReadArgumentString("StateVariableList");
                }
                
                public string StateVariableList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceAVTransport iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceAVTransport): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventLastChange = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "LastChange", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                LastChange = value;

                eventLastChange = true;
            }

            bool eventDRMState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DRMState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                DRMState = value;

                eventDRMState = true;
            }

          
            
            if(eventLastChange)
            {
                if (EventStateLastChange != null)
                {
					try
					{
						EventStateLastChange(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateLastChange: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDRMState)
            {
                if (EventStateDRMState != null)
                {
					try
					{
						EventStateDRMState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDRMState: " + ex);
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
        public event EventHandler<EventArgs> EventStateLastChange;
        public event EventHandler<EventArgs> EventStateDRMState;

        public string LastChange;
        public string DRMState;
    }
}
    
