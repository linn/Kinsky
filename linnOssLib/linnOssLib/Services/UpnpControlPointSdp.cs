using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceSdp : ServiceUpnp
    {

        public const string kDiscTypeUnsupportedDiscType = "Unsupported Disc Type";
        public const string kDiscTypeNoDisc = "No Disc";
        public const string kDiscTypeCd = "CD";
        public const string kDiscTypeVcd = "VCD";
        public const string kDiscTypeSvcd = "SVCD";
        public const string kDiscTypeSacd = "SACD";
        public const string kDiscTypeDvd = "DVD";
        public const string kDiscTypeDvdAudio = "DVD-Audio";
        public const string kDiscTypeDataDisc = "Data Disc";
        public const string kDiscTypeUnknown = "Unknown";
        public const string kTrayStateTrayOpen = "Tray Open";
        public const string kTrayStateTrayClosed = "Tray Closed";
        public const string kTrayStateTrayOpening = "Tray Opening";
        public const string kTrayStateTrayClosing = "Tray Closing";
        public const string kTrayStateUnknown = "Unknown";
        public const string kDiscStateDiscLoading = "Disc Loading";
        public const string kDiscStateDiscLoaded = "Disc Loaded";
        public const string kDiscStateUnknown = "Unknown";
        public const string kPlayStatePlaying = "Playing";
        public const string kPlayStateStopped = "Stopped";
        public const string kPlayStatePaused = "Paused";
        public const string kPlayStateSuspended = "Suspended";
        public const string kPlayStateUnknown = "Unknown";
        public const string kSearchTypeNone = "None";
        public const string kSearchTypeFastForward = "Fast Forward";
        public const string kSearchTypeFastReverse = "Fast Reverse";
        public const string kSearchTypeSlowForward = "Slow Forward";
        public const string kSearchTypeSlowReverse = "Slow Reverse";
        public const string kRepeatModeOff = "Off";
        public const string kRepeatModeAll = "All";
        public const string kRepeatModeTrack = "Track";
        public const string kRepeatModeAB = "A-B";
        public const string kProgramModeOff = "Off";
        public const string kProgramModeRandom = "Random";
        public const string kProgramModeShuffle = "Shuffle";
        public const string kProgramModeStored = "Stored";
        public const string kDomainNone = "None";
        public const string kDomainRootMenu = "Root Menu";
        public const string kDomainTitleMenu = "Title Menu";
        public const string kDomainCopyright = "Copyright";
        public const string kDomainPassword = "Password";
        public const string kZoomLevel25 = "25";
        public const string kZoomLevel50 = "50";
        public const string kZoomLevel100 = "100";
        public const string kZoomLevel150 = "150";
        public const string kZoomLevel200 = "200";
        public const string kZoomLevel300 = "300";
        public const string kSacdStateUnknown = "Unknown";
        public const string kSacdStateDsdMultiChannel = "DSD Multi Channel";
        public const string kSacdStateDsdStereo = "DSD Stereo";
        public const string kSacdStateCddaStereo = "CDDA Stereo";
        public const string kSacdStateSelecting = "Selecting";
        public const string kSlideshowOff = "Off";
        public const string kSlideshowSlideShow = "Slide Show";
        public const string kSlideshowSlideShowPhotoPerTrack = "Slide Show (Photo Per Track)";
        public const string kSlideshowNewWipeMode = "New Wipe Mode";
        public const string kSlideshowThumbnails = "Thumbnails";
        public const string kErrorNone = "None";
        public const string kErrorTrackOutOfRange = "Track out of Range";
        public const string kErrorTitleOutOfRange = "Title out of Range";
        public const string kErrorTimeOutOfRange = "Time out of Range";
        public const string kErrorResumeNotAvailable = "Resume Not Available";
        public const string kErrorProgramSizeInvalid = "Program Size Invalid";
        public const string kErrorDomainNotAvailable = "Domain Not Available";
        public const string kErrorSearchTypeNotAvailable = "Search Type Not Available";
        public const string kErrorProgramTypeNotAvailable = "Program Type Not Available";
        public const string kErrorRepeatTypeNotAvailable = "Repeat Type Not Available";
        public const string kErrorIntroModeNotAvailable = "Intro Mode Not Available";
        public const string kErrorSkipTypeNotAvailable = "Skip Type Not Available";
        public const string kErrorSetupModeNotAvailable = "Setup Mode Not Available";
        public const string kErrorAngleNotAvailable = "Angle Not Available";
        public const string kErrorSubtitleNotAvailable = "Subtitle Not Available";
        public const string kErrorZoomLevelNotAvailable = "Zoom Level Not Available";
        public const string kErrorAudioTrackNotAvailable = "Audio Track Not Available";
        public const string kErrorSacdLayerNotAvailable = "Sacd Layer Not Available";
        public const string kErrorInvalidNavigationRequest = "Invalid Navigation Request";
        public const string kErrorSlideShowTypeNotAvailable = "Slide Show Type Not Available";
        public const string kErrorRequestNotSupported = "Request Not Supported";
        public const string kErrorTableOfContentsMissing = "Table Of Contents Missing";
        public const string kErrorInvalidPassword = "Invalid Password";
        public const string kOrientationUnknown = "Unknown";
        public const string kOrientation0Degrees = "0 Degrees";
        public const string kOrientation0DegreesYMirror = "0 Degrees (Y Mirror)";
        public const string kOrientation90Degrees = "90 Degrees";
        public const string kOrientation90DegreesYMirror = "90 Degrees (Y Mirror)";
        public const string kOrientation180Degrees = "180 Degrees";
        public const string kOrientation180DegreesYMirror = "180 Degrees (Y Mirror)";
        public const string kOrientation270Degrees = "270 Degrees";
        public const string kOrientation270DegreesYMirror = "270 Degrees (Y Mirror)";
        public const string kSkipTypeSkipTrack = "SkipTrack";
        public const string kSkipTypeSkipFrame = "SkipFrame";
        public const string kSkipTypeSkipSearchSpeed = "SkipSearchSpeed";
        public const string kSkipTypeSkipFile = "SkipFile";
        public const string kSkipTypeSkipDisc = "SkipDisc";
        public const string kSkipTypeSkipSacdLayer = "SkipSacdLayer";
        public const string kSelectionTypeSelectDefault = "SelectDefault";
        public const string kSelectionTypeSelectNext = "SelectNext";
        public const string kSelectionTypeSelectPrev = "SelectPrev";
        public const string kSelectionTypeSelectIndex = "SelectIndex";
        public const string kNavigationTypeUp = "Up";
        public const string kNavigationTypeDown = "Down";
        public const string kNavigationTypeLeft = "Left";
        public const string kNavigationTypeRight = "Right";
        public const string kNavigationTypeSelect = "Select";
        public const string kNavigationTypeReturn = "Return";

        public ServiceSdp(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceSdp(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("Open");
            iActions.Add(action);
            
            action = new Action("Close");
            iActions.Add(action);
            
            action = new Action("Play");
            iActions.Add(action);
            
            action = new Action("Stop");
            iActions.Add(action);
            
            action = new Action("Pause");
            iActions.Add(action);
            
            action = new Action("Resume");
            iActions.Add(action);
            
            action = new Action("Search");
            action.AddInArgument(new Argument("aSearchType", Argument.EType.eString));
            action.AddInArgument(new Argument("aSearchSpeed", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetTrack");
            action.AddInArgument(new Argument("aTrack", Argument.EType.eInt));
            action.AddInArgument(new Argument("aTitle", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetTime");
            action.AddInArgument(new Argument("aTime", Argument.EType.eString));
            action.AddInArgument(new Argument("aTitle", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetProgramOff");
            iActions.Add(action);
            
            action = new Action("SetProgramInclude");
            action.AddInArgument(new Argument("aIncludeList", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("SetProgramExclude");
            action.AddInArgument(new Argument("aExcludeList", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("SetProgramRandom");
            iActions.Add(action);
            
            action = new Action("SetProgramShuffle");
            iActions.Add(action);
            
            action = new Action("SetRepeatOff");
            iActions.Add(action);
            
            action = new Action("SetRepeatAll");
            iActions.Add(action);
            
            action = new Action("SetRepeatTrack");
            iActions.Add(action);
            
            action = new Action("SetRepeatAb");
            action.AddInArgument(new Argument("aStartTime", Argument.EType.eString));
            action.AddInArgument(new Argument("aEndTime", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetIntroMode");
            action.AddInArgument(new Argument("aIntroMode", Argument.EType.eBool));
            action.AddInArgument(new Argument("aOffset", Argument.EType.eInt));
            action.AddInArgument(new Argument("aSeconds", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetNext");
            action.AddInArgument(new Argument("aSkip", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetPrev");
            action.AddInArgument(new Argument("aSkip", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("RootMenu");
            iActions.Add(action);
            
            action = new Action("TitleMenu");
            iActions.Add(action);
            
            action = new Action("SetSetupMode");
            action.AddInArgument(new Argument("aSetupMode", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetAngle");
            action.AddInArgument(new Argument("aSelect", Argument.EType.eString));
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetAudioTrack");
            action.AddInArgument(new Argument("aSelect", Argument.EType.eString));
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetSubtitle");
            action.AddInArgument(new Argument("aSelect", Argument.EType.eString));
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetZoom");
            action.AddInArgument(new Argument("aSelect", Argument.EType.eString));
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetSacdLayer");
            action.AddInArgument(new Argument("aSacdLayer", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Navigate");
            action.AddInArgument(new Argument("aNavigation", Argument.EType.eString));
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetSlideshow");
            action.AddInArgument(new Argument("aSlideshow", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetPassword");
            action.AddInArgument(new Argument("aPassword", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiscType");
            action.AddOutArgument(new Argument("aDiscType", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Title");
            action.AddOutArgument(new Argument("aTitle", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("TrayState");
            action.AddOutArgument(new Argument("aTrayState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiscState");
            action.AddOutArgument(new Argument("aDiscState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("PlayState");
            action.AddOutArgument(new Argument("aPlayState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SearchType");
            action.AddOutArgument(new Argument("aSearchType", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SearchSpeed");
            action.AddOutArgument(new Argument("aSearchSpeed", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("Track");
            action.AddOutArgument(new Argument("aTrack", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("TrackElapsedTime");
            action.AddOutArgument(new Argument("aTrackElapsedTime", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TrackRemainingTime");
            action.AddOutArgument(new Argument("aTrackRemainingTime", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiscElapsedTime");
            action.AddOutArgument(new Argument("aDiscElapsedTime", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiscRemainingTime");
            action.AddOutArgument(new Argument("aDiscRemainingTime", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("RepeatMode");
            action.AddOutArgument(new Argument("aRepeatMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("IntroMode");
            action.AddOutArgument(new Argument("aIntroMode", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("ProgramMode");
            action.AddOutArgument(new Argument("aProgramMode", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Domain");
            action.AddOutArgument(new Argument("aDomain", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Angle");
            action.AddOutArgument(new Argument("aAngle", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("TotalAngles");
            action.AddOutArgument(new Argument("aTotalAngles", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("Subtitle");
            action.AddOutArgument(new Argument("aSubtitle", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("AudioTrack");
            action.AddOutArgument(new Argument("aAudioTrack", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("ZoomLevel");
            action.AddOutArgument(new Argument("aZoomLevel", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetupMode");
            action.AddOutArgument(new Argument("aSetupMode", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SacdState");
            action.AddOutArgument(new Argument("aSacdState", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Slideshow");
            action.AddOutArgument(new Argument("aSlideshow", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Error");
            action.AddOutArgument(new Argument("aError", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Orientation");
            action.AddOutArgument(new Argument("aOrientation", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("DiscLength");
            action.AddOutArgument(new Argument("aDiscLength", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TrackLength");
            action.AddOutArgument(new Argument("aTrackLength", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TotalTracks");
            action.AddOutArgument(new Argument("aTotalTracks", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("TotalTitles");
            action.AddOutArgument(new Argument("aTotalTitles", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("Genre");
            action.AddOutArgument(new Argument("aGenre", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Encoding");
            action.AddOutArgument(new Argument("aEncoding", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("FileSize");
            action.AddOutArgument(new Argument("aFileSize", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("DiscId");
            action.AddOutArgument(new Argument("aDiscId", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("Year");
            action.AddOutArgument(new Argument("aYear", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("TrackName");
            action.AddOutArgument(new Argument("aTrackName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("ArtistName");
            action.AddOutArgument(new Argument("aArtistName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("AlbumName");
            action.AddOutArgument(new Argument("aAlbumName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("Comment");
            action.AddOutArgument(new Argument("aComment", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("FileName");
            action.AddInArgument(new Argument("aIndex", Argument.EType.eInt));
            action.AddOutArgument(new Argument("aFileName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SystemCapabilities");
            action.AddOutArgument(new Argument("aSystemCapabilities", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("DiscCapabilities");
            action.AddOutArgument(new Argument("aDiscCapabilities", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("ZoomLevelInfo");
            action.AddOutArgument(new Argument("aZoomLevelInfo", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("SubtitleInfo");
            action.AddOutArgument(new Argument("aSubtitleInfo", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("AudioTrackInfo");
            action.AddOutArgument(new Argument("aAudioTrackInfo", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("TableOfContents");
            action.AddOutArgument(new Argument("aTableOfContents", Argument.EType.eBinary));
            iActions.Add(action);
            
            action = new Action("DirectoryStructure");
            action.AddOutArgument(new Argument("aDirectoryStructure", Argument.EType.eBinary));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("linn.co.uk", "Sdp", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("linn.co.uk", "Sdp", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionOpen CreateAsyncActionOpen()
        {
            return (new AsyncActionOpen(this));
        }

        public AsyncActionClose CreateAsyncActionClose()
        {
            return (new AsyncActionClose(this));
        }

        public AsyncActionPlay CreateAsyncActionPlay()
        {
            return (new AsyncActionPlay(this));
        }

        public AsyncActionStop CreateAsyncActionStop()
        {
            return (new AsyncActionStop(this));
        }

        public AsyncActionPause CreateAsyncActionPause()
        {
            return (new AsyncActionPause(this));
        }

        public AsyncActionResume CreateAsyncActionResume()
        {
            return (new AsyncActionResume(this));
        }

        public AsyncActionSearch CreateAsyncActionSearch()
        {
            return (new AsyncActionSearch(this));
        }

        public AsyncActionSetTrack CreateAsyncActionSetTrack()
        {
            return (new AsyncActionSetTrack(this));
        }

        public AsyncActionSetTime CreateAsyncActionSetTime()
        {
            return (new AsyncActionSetTime(this));
        }

        public AsyncActionSetProgramOff CreateAsyncActionSetProgramOff()
        {
            return (new AsyncActionSetProgramOff(this));
        }

        public AsyncActionSetProgramInclude CreateAsyncActionSetProgramInclude()
        {
            return (new AsyncActionSetProgramInclude(this));
        }

        public AsyncActionSetProgramExclude CreateAsyncActionSetProgramExclude()
        {
            return (new AsyncActionSetProgramExclude(this));
        }

        public AsyncActionSetProgramRandom CreateAsyncActionSetProgramRandom()
        {
            return (new AsyncActionSetProgramRandom(this));
        }

        public AsyncActionSetProgramShuffle CreateAsyncActionSetProgramShuffle()
        {
            return (new AsyncActionSetProgramShuffle(this));
        }

        public AsyncActionSetRepeatOff CreateAsyncActionSetRepeatOff()
        {
            return (new AsyncActionSetRepeatOff(this));
        }

        public AsyncActionSetRepeatAll CreateAsyncActionSetRepeatAll()
        {
            return (new AsyncActionSetRepeatAll(this));
        }

        public AsyncActionSetRepeatTrack CreateAsyncActionSetRepeatTrack()
        {
            return (new AsyncActionSetRepeatTrack(this));
        }

        public AsyncActionSetRepeatAb CreateAsyncActionSetRepeatAb()
        {
            return (new AsyncActionSetRepeatAb(this));
        }

        public AsyncActionSetIntroMode CreateAsyncActionSetIntroMode()
        {
            return (new AsyncActionSetIntroMode(this));
        }

        public AsyncActionSetNext CreateAsyncActionSetNext()
        {
            return (new AsyncActionSetNext(this));
        }

        public AsyncActionSetPrev CreateAsyncActionSetPrev()
        {
            return (new AsyncActionSetPrev(this));
        }

        public AsyncActionRootMenu CreateAsyncActionRootMenu()
        {
            return (new AsyncActionRootMenu(this));
        }

        public AsyncActionTitleMenu CreateAsyncActionTitleMenu()
        {
            return (new AsyncActionTitleMenu(this));
        }

        public AsyncActionSetSetupMode CreateAsyncActionSetSetupMode()
        {
            return (new AsyncActionSetSetupMode(this));
        }

        public AsyncActionSetAngle CreateAsyncActionSetAngle()
        {
            return (new AsyncActionSetAngle(this));
        }

        public AsyncActionSetAudioTrack CreateAsyncActionSetAudioTrack()
        {
            return (new AsyncActionSetAudioTrack(this));
        }

        public AsyncActionSetSubtitle CreateAsyncActionSetSubtitle()
        {
            return (new AsyncActionSetSubtitle(this));
        }

        public AsyncActionSetZoom CreateAsyncActionSetZoom()
        {
            return (new AsyncActionSetZoom(this));
        }

        public AsyncActionSetSacdLayer CreateAsyncActionSetSacdLayer()
        {
            return (new AsyncActionSetSacdLayer(this));
        }

        public AsyncActionNavigate CreateAsyncActionNavigate()
        {
            return (new AsyncActionNavigate(this));
        }

        public AsyncActionSetSlideshow CreateAsyncActionSetSlideshow()
        {
            return (new AsyncActionSetSlideshow(this));
        }

        public AsyncActionSetPassword CreateAsyncActionSetPassword()
        {
            return (new AsyncActionSetPassword(this));
        }

        public AsyncActionDiscType CreateAsyncActionDiscType()
        {
            return (new AsyncActionDiscType(this));
        }

        public AsyncActionTitle CreateAsyncActionTitle()
        {
            return (new AsyncActionTitle(this));
        }

        public AsyncActionTrayState CreateAsyncActionTrayState()
        {
            return (new AsyncActionTrayState(this));
        }

        public AsyncActionDiscState CreateAsyncActionDiscState()
        {
            return (new AsyncActionDiscState(this));
        }

        public AsyncActionPlayState CreateAsyncActionPlayState()
        {
            return (new AsyncActionPlayState(this));
        }

        public AsyncActionSearchType CreateAsyncActionSearchType()
        {
            return (new AsyncActionSearchType(this));
        }

        public AsyncActionSearchSpeed CreateAsyncActionSearchSpeed()
        {
            return (new AsyncActionSearchSpeed(this));
        }

        public AsyncActionTrack CreateAsyncActionTrack()
        {
            return (new AsyncActionTrack(this));
        }

        public AsyncActionTrackElapsedTime CreateAsyncActionTrackElapsedTime()
        {
            return (new AsyncActionTrackElapsedTime(this));
        }

        public AsyncActionTrackRemainingTime CreateAsyncActionTrackRemainingTime()
        {
            return (new AsyncActionTrackRemainingTime(this));
        }

        public AsyncActionDiscElapsedTime CreateAsyncActionDiscElapsedTime()
        {
            return (new AsyncActionDiscElapsedTime(this));
        }

        public AsyncActionDiscRemainingTime CreateAsyncActionDiscRemainingTime()
        {
            return (new AsyncActionDiscRemainingTime(this));
        }

        public AsyncActionRepeatMode CreateAsyncActionRepeatMode()
        {
            return (new AsyncActionRepeatMode(this));
        }

        public AsyncActionIntroMode CreateAsyncActionIntroMode()
        {
            return (new AsyncActionIntroMode(this));
        }

        public AsyncActionProgramMode CreateAsyncActionProgramMode()
        {
            return (new AsyncActionProgramMode(this));
        }

        public AsyncActionDomain CreateAsyncActionDomain()
        {
            return (new AsyncActionDomain(this));
        }

        public AsyncActionAngle CreateAsyncActionAngle()
        {
            return (new AsyncActionAngle(this));
        }

        public AsyncActionTotalAngles CreateAsyncActionTotalAngles()
        {
            return (new AsyncActionTotalAngles(this));
        }

        public AsyncActionSubtitle CreateAsyncActionSubtitle()
        {
            return (new AsyncActionSubtitle(this));
        }

        public AsyncActionAudioTrack CreateAsyncActionAudioTrack()
        {
            return (new AsyncActionAudioTrack(this));
        }

        public AsyncActionZoomLevel CreateAsyncActionZoomLevel()
        {
            return (new AsyncActionZoomLevel(this));
        }

        public AsyncActionSetupMode CreateAsyncActionSetupMode()
        {
            return (new AsyncActionSetupMode(this));
        }

        public AsyncActionSacdState CreateAsyncActionSacdState()
        {
            return (new AsyncActionSacdState(this));
        }

        public AsyncActionSlideshow CreateAsyncActionSlideshow()
        {
            return (new AsyncActionSlideshow(this));
        }

        public AsyncActionError CreateAsyncActionError()
        {
            return (new AsyncActionError(this));
        }

        public AsyncActionOrientation CreateAsyncActionOrientation()
        {
            return (new AsyncActionOrientation(this));
        }

        public AsyncActionDiscLength CreateAsyncActionDiscLength()
        {
            return (new AsyncActionDiscLength(this));
        }

        public AsyncActionTrackLength CreateAsyncActionTrackLength()
        {
            return (new AsyncActionTrackLength(this));
        }

        public AsyncActionTotalTracks CreateAsyncActionTotalTracks()
        {
            return (new AsyncActionTotalTracks(this));
        }

        public AsyncActionTotalTitles CreateAsyncActionTotalTitles()
        {
            return (new AsyncActionTotalTitles(this));
        }

        public AsyncActionGenre CreateAsyncActionGenre()
        {
            return (new AsyncActionGenre(this));
        }

        public AsyncActionEncoding CreateAsyncActionEncoding()
        {
            return (new AsyncActionEncoding(this));
        }

        public AsyncActionFileSize CreateAsyncActionFileSize()
        {
            return (new AsyncActionFileSize(this));
        }

        public AsyncActionDiscId CreateAsyncActionDiscId()
        {
            return (new AsyncActionDiscId(this));
        }

        public AsyncActionYear CreateAsyncActionYear()
        {
            return (new AsyncActionYear(this));
        }

        public AsyncActionTrackName CreateAsyncActionTrackName()
        {
            return (new AsyncActionTrackName(this));
        }

        public AsyncActionArtistName CreateAsyncActionArtistName()
        {
            return (new AsyncActionArtistName(this));
        }

        public AsyncActionAlbumName CreateAsyncActionAlbumName()
        {
            return (new AsyncActionAlbumName(this));
        }

        public AsyncActionComment CreateAsyncActionComment()
        {
            return (new AsyncActionComment(this));
        }

        public AsyncActionFileName CreateAsyncActionFileName()
        {
            return (new AsyncActionFileName(this));
        }

        public AsyncActionSystemCapabilities CreateAsyncActionSystemCapabilities()
        {
            return (new AsyncActionSystemCapabilities(this));
        }

        public AsyncActionDiscCapabilities CreateAsyncActionDiscCapabilities()
        {
            return (new AsyncActionDiscCapabilities(this));
        }

        public AsyncActionZoomLevelInfo CreateAsyncActionZoomLevelInfo()
        {
            return (new AsyncActionZoomLevelInfo(this));
        }

        public AsyncActionSubtitleInfo CreateAsyncActionSubtitleInfo()
        {
            return (new AsyncActionSubtitleInfo(this));
        }

        public AsyncActionAudioTrackInfo CreateAsyncActionAudioTrackInfo()
        {
            return (new AsyncActionAudioTrackInfo(this));
        }

        public AsyncActionTableOfContents CreateAsyncActionTableOfContents()
        {
            return (new AsyncActionTableOfContents(this));
        }

        public AsyncActionDirectoryStructure CreateAsyncActionDirectoryStructure()
        {
            return (new AsyncActionDirectoryStructure(this));
        }


        // Synchronous actions
        
        public void OpenSync()
        {
            AsyncActionOpen action = CreateAsyncActionOpen();
            
            object result = action.OpenBeginSync();

            action.OpenEnd(result);
        }
        
        public void CloseSync()
        {
            AsyncActionClose action = CreateAsyncActionClose();
            
            object result = action.CloseBeginSync();

            action.CloseEnd(result);
        }
        
        public void PlaySync()
        {
            AsyncActionPlay action = CreateAsyncActionPlay();
            
            object result = action.PlayBeginSync();

            action.PlayEnd(result);
        }
        
        public void StopSync()
        {
            AsyncActionStop action = CreateAsyncActionStop();
            
            object result = action.StopBeginSync();

            action.StopEnd(result);
        }
        
        public void PauseSync()
        {
            AsyncActionPause action = CreateAsyncActionPause();
            
            object result = action.PauseBeginSync();

            action.PauseEnd(result);
        }
        
        public void ResumeSync()
        {
            AsyncActionResume action = CreateAsyncActionResume();
            
            object result = action.ResumeBeginSync();

            action.ResumeEnd(result);
        }
        
        public void SearchSync(string aSearchType, int aSearchSpeed)
        {
            AsyncActionSearch action = CreateAsyncActionSearch();
            
            object result = action.SearchBeginSync(aSearchType, aSearchSpeed);

            action.SearchEnd(result);
        }
        
        public void SetTrackSync(int aTrack, int aTitle)
        {
            AsyncActionSetTrack action = CreateAsyncActionSetTrack();
            
            object result = action.SetTrackBeginSync(aTrack, aTitle);

            action.SetTrackEnd(result);
        }
        
        public void SetTimeSync(string aTime, int aTitle)
        {
            AsyncActionSetTime action = CreateAsyncActionSetTime();
            
            object result = action.SetTimeBeginSync(aTime, aTitle);

            action.SetTimeEnd(result);
        }
        
        public void SetProgramOffSync()
        {
            AsyncActionSetProgramOff action = CreateAsyncActionSetProgramOff();
            
            object result = action.SetProgramOffBeginSync();

            action.SetProgramOffEnd(result);
        }
        
        public void SetProgramIncludeSync(byte[] aIncludeList)
        {
            AsyncActionSetProgramInclude action = CreateAsyncActionSetProgramInclude();
            
            object result = action.SetProgramIncludeBeginSync(aIncludeList);

            action.SetProgramIncludeEnd(result);
        }
        
        public void SetProgramExcludeSync(byte[] aExcludeList)
        {
            AsyncActionSetProgramExclude action = CreateAsyncActionSetProgramExclude();
            
            object result = action.SetProgramExcludeBeginSync(aExcludeList);

            action.SetProgramExcludeEnd(result);
        }
        
        public void SetProgramRandomSync()
        {
            AsyncActionSetProgramRandom action = CreateAsyncActionSetProgramRandom();
            
            object result = action.SetProgramRandomBeginSync();

            action.SetProgramRandomEnd(result);
        }
        
        public void SetProgramShuffleSync()
        {
            AsyncActionSetProgramShuffle action = CreateAsyncActionSetProgramShuffle();
            
            object result = action.SetProgramShuffleBeginSync();

            action.SetProgramShuffleEnd(result);
        }
        
        public void SetRepeatOffSync()
        {
            AsyncActionSetRepeatOff action = CreateAsyncActionSetRepeatOff();
            
            object result = action.SetRepeatOffBeginSync();

            action.SetRepeatOffEnd(result);
        }
        
        public void SetRepeatAllSync()
        {
            AsyncActionSetRepeatAll action = CreateAsyncActionSetRepeatAll();
            
            object result = action.SetRepeatAllBeginSync();

            action.SetRepeatAllEnd(result);
        }
        
        public void SetRepeatTrackSync()
        {
            AsyncActionSetRepeatTrack action = CreateAsyncActionSetRepeatTrack();
            
            object result = action.SetRepeatTrackBeginSync();

            action.SetRepeatTrackEnd(result);
        }
        
        public void SetRepeatAbSync(string aStartTime, string aEndTime)
        {
            AsyncActionSetRepeatAb action = CreateAsyncActionSetRepeatAb();
            
            object result = action.SetRepeatAbBeginSync(aStartTime, aEndTime);

            action.SetRepeatAbEnd(result);
        }
        
        public void SetIntroModeSync(bool aIntroMode, int aOffset, int aSeconds)
        {
            AsyncActionSetIntroMode action = CreateAsyncActionSetIntroMode();
            
            object result = action.SetIntroModeBeginSync(aIntroMode, aOffset, aSeconds);

            action.SetIntroModeEnd(result);
        }
        
        public void SetNextSync(string aSkip)
        {
            AsyncActionSetNext action = CreateAsyncActionSetNext();
            
            object result = action.SetNextBeginSync(aSkip);

            action.SetNextEnd(result);
        }
        
        public void SetPrevSync(string aSkip)
        {
            AsyncActionSetPrev action = CreateAsyncActionSetPrev();
            
            object result = action.SetPrevBeginSync(aSkip);

            action.SetPrevEnd(result);
        }
        
        public void RootMenuSync()
        {
            AsyncActionRootMenu action = CreateAsyncActionRootMenu();
            
            object result = action.RootMenuBeginSync();

            action.RootMenuEnd(result);
        }
        
        public void TitleMenuSync()
        {
            AsyncActionTitleMenu action = CreateAsyncActionTitleMenu();
            
            object result = action.TitleMenuBeginSync();

            action.TitleMenuEnd(result);
        }
        
        public void SetSetupModeSync(bool aSetupMode)
        {
            AsyncActionSetSetupMode action = CreateAsyncActionSetSetupMode();
            
            object result = action.SetSetupModeBeginSync(aSetupMode);

            action.SetSetupModeEnd(result);
        }
        
        public void SetAngleSync(string aSelect, int aIndex)
        {
            AsyncActionSetAngle action = CreateAsyncActionSetAngle();
            
            object result = action.SetAngleBeginSync(aSelect, aIndex);

            action.SetAngleEnd(result);
        }
        
        public void SetAudioTrackSync(string aSelect, int aIndex)
        {
            AsyncActionSetAudioTrack action = CreateAsyncActionSetAudioTrack();
            
            object result = action.SetAudioTrackBeginSync(aSelect, aIndex);

            action.SetAudioTrackEnd(result);
        }
        
        public void SetSubtitleSync(string aSelect, int aIndex)
        {
            AsyncActionSetSubtitle action = CreateAsyncActionSetSubtitle();
            
            object result = action.SetSubtitleBeginSync(aSelect, aIndex);

            action.SetSubtitleEnd(result);
        }
        
        public void SetZoomSync(string aSelect, int aIndex)
        {
            AsyncActionSetZoom action = CreateAsyncActionSetZoom();
            
            object result = action.SetZoomBeginSync(aSelect, aIndex);

            action.SetZoomEnd(result);
        }
        
        public void SetSacdLayerSync(string aSacdLayer)
        {
            AsyncActionSetSacdLayer action = CreateAsyncActionSetSacdLayer();
            
            object result = action.SetSacdLayerBeginSync(aSacdLayer);

            action.SetSacdLayerEnd(result);
        }
        
        public void NavigateSync(string aNavigation, int aIndex)
        {
            AsyncActionNavigate action = CreateAsyncActionNavigate();
            
            object result = action.NavigateBeginSync(aNavigation, aIndex);

            action.NavigateEnd(result);
        }
        
        public void SetSlideshowSync(string aSlideshow)
        {
            AsyncActionSetSlideshow action = CreateAsyncActionSetSlideshow();
            
            object result = action.SetSlideshowBeginSync(aSlideshow);

            action.SetSlideshowEnd(result);
        }
        
        public void SetPasswordSync(string aPassword)
        {
            AsyncActionSetPassword action = CreateAsyncActionSetPassword();
            
            object result = action.SetPasswordBeginSync(aPassword);

            action.SetPasswordEnd(result);
        }
        
        public string DiscTypeSync()
        {
            AsyncActionDiscType action = CreateAsyncActionDiscType();
            
            object result = action.DiscTypeBeginSync();

            AsyncActionDiscType.EventArgsResponse response = action.DiscTypeEnd(result);
                
            return(response.aDiscType);
        }
        
        public int TitleSync()
        {
            AsyncActionTitle action = CreateAsyncActionTitle();
            
            object result = action.TitleBeginSync();

            AsyncActionTitle.EventArgsResponse response = action.TitleEnd(result);
                
            return(response.aTitle);
        }
        
        public string TrayStateSync()
        {
            AsyncActionTrayState action = CreateAsyncActionTrayState();
            
            object result = action.TrayStateBeginSync();

            AsyncActionTrayState.EventArgsResponse response = action.TrayStateEnd(result);
                
            return(response.aTrayState);
        }
        
        public string DiscStateSync()
        {
            AsyncActionDiscState action = CreateAsyncActionDiscState();
            
            object result = action.DiscStateBeginSync();

            AsyncActionDiscState.EventArgsResponse response = action.DiscStateEnd(result);
                
            return(response.aDiscState);
        }
        
        public string PlayStateSync()
        {
            AsyncActionPlayState action = CreateAsyncActionPlayState();
            
            object result = action.PlayStateBeginSync();

            AsyncActionPlayState.EventArgsResponse response = action.PlayStateEnd(result);
                
            return(response.aPlayState);
        }
        
        public string SearchTypeSync()
        {
            AsyncActionSearchType action = CreateAsyncActionSearchType();
            
            object result = action.SearchTypeBeginSync();

            AsyncActionSearchType.EventArgsResponse response = action.SearchTypeEnd(result);
                
            return(response.aSearchType);
        }
        
        public int SearchSpeedSync()
        {
            AsyncActionSearchSpeed action = CreateAsyncActionSearchSpeed();
            
            object result = action.SearchSpeedBeginSync();

            AsyncActionSearchSpeed.EventArgsResponse response = action.SearchSpeedEnd(result);
                
            return(response.aSearchSpeed);
        }
        
        public int TrackSync()
        {
            AsyncActionTrack action = CreateAsyncActionTrack();
            
            object result = action.TrackBeginSync();

            AsyncActionTrack.EventArgsResponse response = action.TrackEnd(result);
                
            return(response.aTrack);
        }
        
        public string TrackElapsedTimeSync()
        {
            AsyncActionTrackElapsedTime action = CreateAsyncActionTrackElapsedTime();
            
            object result = action.TrackElapsedTimeBeginSync();

            AsyncActionTrackElapsedTime.EventArgsResponse response = action.TrackElapsedTimeEnd(result);
                
            return(response.aTrackElapsedTime);
        }
        
        public string TrackRemainingTimeSync()
        {
            AsyncActionTrackRemainingTime action = CreateAsyncActionTrackRemainingTime();
            
            object result = action.TrackRemainingTimeBeginSync();

            AsyncActionTrackRemainingTime.EventArgsResponse response = action.TrackRemainingTimeEnd(result);
                
            return(response.aTrackRemainingTime);
        }
        
        public string DiscElapsedTimeSync()
        {
            AsyncActionDiscElapsedTime action = CreateAsyncActionDiscElapsedTime();
            
            object result = action.DiscElapsedTimeBeginSync();

            AsyncActionDiscElapsedTime.EventArgsResponse response = action.DiscElapsedTimeEnd(result);
                
            return(response.aDiscElapsedTime);
        }
        
        public string DiscRemainingTimeSync()
        {
            AsyncActionDiscRemainingTime action = CreateAsyncActionDiscRemainingTime();
            
            object result = action.DiscRemainingTimeBeginSync();

            AsyncActionDiscRemainingTime.EventArgsResponse response = action.DiscRemainingTimeEnd(result);
                
            return(response.aDiscRemainingTime);
        }
        
        public string RepeatModeSync()
        {
            AsyncActionRepeatMode action = CreateAsyncActionRepeatMode();
            
            object result = action.RepeatModeBeginSync();

            AsyncActionRepeatMode.EventArgsResponse response = action.RepeatModeEnd(result);
                
            return(response.aRepeatMode);
        }
        
        public bool IntroModeSync()
        {
            AsyncActionIntroMode action = CreateAsyncActionIntroMode();
            
            object result = action.IntroModeBeginSync();

            AsyncActionIntroMode.EventArgsResponse response = action.IntroModeEnd(result);
                
            return(response.aIntroMode);
        }
        
        public string ProgramModeSync()
        {
            AsyncActionProgramMode action = CreateAsyncActionProgramMode();
            
            object result = action.ProgramModeBeginSync();

            AsyncActionProgramMode.EventArgsResponse response = action.ProgramModeEnd(result);
                
            return(response.aProgramMode);
        }
        
        public string DomainSync()
        {
            AsyncActionDomain action = CreateAsyncActionDomain();
            
            object result = action.DomainBeginSync();

            AsyncActionDomain.EventArgsResponse response = action.DomainEnd(result);
                
            return(response.aDomain);
        }
        
        public int AngleSync()
        {
            AsyncActionAngle action = CreateAsyncActionAngle();
            
            object result = action.AngleBeginSync();

            AsyncActionAngle.EventArgsResponse response = action.AngleEnd(result);
                
            return(response.aAngle);
        }
        
        public int TotalAnglesSync()
        {
            AsyncActionTotalAngles action = CreateAsyncActionTotalAngles();
            
            object result = action.TotalAnglesBeginSync();

            AsyncActionTotalAngles.EventArgsResponse response = action.TotalAnglesEnd(result);
                
            return(response.aTotalAngles);
        }
        
        public int SubtitleSync()
        {
            AsyncActionSubtitle action = CreateAsyncActionSubtitle();
            
            object result = action.SubtitleBeginSync();

            AsyncActionSubtitle.EventArgsResponse response = action.SubtitleEnd(result);
                
            return(response.aSubtitle);
        }
        
        public int AudioTrackSync()
        {
            AsyncActionAudioTrack action = CreateAsyncActionAudioTrack();
            
            object result = action.AudioTrackBeginSync();

            AsyncActionAudioTrack.EventArgsResponse response = action.AudioTrackEnd(result);
                
            return(response.aAudioTrack);
        }
        
        public string ZoomLevelSync()
        {
            AsyncActionZoomLevel action = CreateAsyncActionZoomLevel();
            
            object result = action.ZoomLevelBeginSync();

            AsyncActionZoomLevel.EventArgsResponse response = action.ZoomLevelEnd(result);
                
            return(response.aZoomLevel);
        }
        
        public bool SetupModeSync()
        {
            AsyncActionSetupMode action = CreateAsyncActionSetupMode();
            
            object result = action.SetupModeBeginSync();

            AsyncActionSetupMode.EventArgsResponse response = action.SetupModeEnd(result);
                
            return(response.aSetupMode);
        }
        
        public string SacdStateSync()
        {
            AsyncActionSacdState action = CreateAsyncActionSacdState();
            
            object result = action.SacdStateBeginSync();

            AsyncActionSacdState.EventArgsResponse response = action.SacdStateEnd(result);
                
            return(response.aSacdState);
        }
        
        public string SlideshowSync()
        {
            AsyncActionSlideshow action = CreateAsyncActionSlideshow();
            
            object result = action.SlideshowBeginSync();

            AsyncActionSlideshow.EventArgsResponse response = action.SlideshowEnd(result);
                
            return(response.aSlideshow);
        }
        
        public string ErrorSync()
        {
            AsyncActionError action = CreateAsyncActionError();
            
            object result = action.ErrorBeginSync();

            AsyncActionError.EventArgsResponse response = action.ErrorEnd(result);
                
            return(response.aError);
        }
        
        public string OrientationSync()
        {
            AsyncActionOrientation action = CreateAsyncActionOrientation();
            
            object result = action.OrientationBeginSync();

            AsyncActionOrientation.EventArgsResponse response = action.OrientationEnd(result);
                
            return(response.aOrientation);
        }
        
        public string DiscLengthSync()
        {
            AsyncActionDiscLength action = CreateAsyncActionDiscLength();
            
            object result = action.DiscLengthBeginSync();

            AsyncActionDiscLength.EventArgsResponse response = action.DiscLengthEnd(result);
                
            return(response.aDiscLength);
        }
        
        public string TrackLengthSync()
        {
            AsyncActionTrackLength action = CreateAsyncActionTrackLength();
            
            object result = action.TrackLengthBeginSync();

            AsyncActionTrackLength.EventArgsResponse response = action.TrackLengthEnd(result);
                
            return(response.aTrackLength);
        }
        
        public int TotalTracksSync()
        {
            AsyncActionTotalTracks action = CreateAsyncActionTotalTracks();
            
            object result = action.TotalTracksBeginSync();

            AsyncActionTotalTracks.EventArgsResponse response = action.TotalTracksEnd(result);
                
            return(response.aTotalTracks);
        }
        
        public int TotalTitlesSync()
        {
            AsyncActionTotalTitles action = CreateAsyncActionTotalTitles();
            
            object result = action.TotalTitlesBeginSync();

            AsyncActionTotalTitles.EventArgsResponse response = action.TotalTitlesEnd(result);
                
            return(response.aTotalTitles);
        }
        
        public string GenreSync()
        {
            AsyncActionGenre action = CreateAsyncActionGenre();
            
            object result = action.GenreBeginSync();

            AsyncActionGenre.EventArgsResponse response = action.GenreEnd(result);
                
            return(response.aGenre);
        }
        
        public uint EncodingSync()
        {
            AsyncActionEncoding action = CreateAsyncActionEncoding();
            
            object result = action.EncodingBeginSync();

            AsyncActionEncoding.EventArgsResponse response = action.EncodingEnd(result);
                
            return(response.aEncoding);
        }
        
        public uint FileSizeSync()
        {
            AsyncActionFileSize action = CreateAsyncActionFileSize();
            
            object result = action.FileSizeBeginSync();

            AsyncActionFileSize.EventArgsResponse response = action.FileSizeEnd(result);
                
            return(response.aFileSize);
        }
        
        public uint DiscIdSync()
        {
            AsyncActionDiscId action = CreateAsyncActionDiscId();
            
            object result = action.DiscIdBeginSync();

            AsyncActionDiscId.EventArgsResponse response = action.DiscIdEnd(result);
                
            return(response.aDiscId);
        }
        
        public string YearSync()
        {
            AsyncActionYear action = CreateAsyncActionYear();
            
            object result = action.YearBeginSync();

            AsyncActionYear.EventArgsResponse response = action.YearEnd(result);
                
            return(response.aYear);
        }
        
        public string TrackNameSync()
        {
            AsyncActionTrackName action = CreateAsyncActionTrackName();
            
            object result = action.TrackNameBeginSync();

            AsyncActionTrackName.EventArgsResponse response = action.TrackNameEnd(result);
                
            return(response.aTrackName);
        }
        
        public string ArtistNameSync()
        {
            AsyncActionArtistName action = CreateAsyncActionArtistName();
            
            object result = action.ArtistNameBeginSync();

            AsyncActionArtistName.EventArgsResponse response = action.ArtistNameEnd(result);
                
            return(response.aArtistName);
        }
        
        public string AlbumNameSync()
        {
            AsyncActionAlbumName action = CreateAsyncActionAlbumName();
            
            object result = action.AlbumNameBeginSync();

            AsyncActionAlbumName.EventArgsResponse response = action.AlbumNameEnd(result);
                
            return(response.aAlbumName);
        }
        
        public string CommentSync()
        {
            AsyncActionComment action = CreateAsyncActionComment();
            
            object result = action.CommentBeginSync();

            AsyncActionComment.EventArgsResponse response = action.CommentEnd(result);
                
            return(response.aComment);
        }
        
        public string FileNameSync(int aIndex)
        {
            AsyncActionFileName action = CreateAsyncActionFileName();
            
            object result = action.FileNameBeginSync(aIndex);

            AsyncActionFileName.EventArgsResponse response = action.FileNameEnd(result);
                
            return(response.aFileName);
        }
        
        public byte[] SystemCapabilitiesSync()
        {
            AsyncActionSystemCapabilities action = CreateAsyncActionSystemCapabilities();
            
            object result = action.SystemCapabilitiesBeginSync();

            AsyncActionSystemCapabilities.EventArgsResponse response = action.SystemCapabilitiesEnd(result);
                
            return(response.aSystemCapabilities);
        }
        
        public byte[] DiscCapabilitiesSync()
        {
            AsyncActionDiscCapabilities action = CreateAsyncActionDiscCapabilities();
            
            object result = action.DiscCapabilitiesBeginSync();

            AsyncActionDiscCapabilities.EventArgsResponse response = action.DiscCapabilitiesEnd(result);
                
            return(response.aDiscCapabilities);
        }
        
        public byte[] ZoomLevelInfoSync()
        {
            AsyncActionZoomLevelInfo action = CreateAsyncActionZoomLevelInfo();
            
            object result = action.ZoomLevelInfoBeginSync();

            AsyncActionZoomLevelInfo.EventArgsResponse response = action.ZoomLevelInfoEnd(result);
                
            return(response.aZoomLevelInfo);
        }
        
        public byte[] SubtitleInfoSync()
        {
            AsyncActionSubtitleInfo action = CreateAsyncActionSubtitleInfo();
            
            object result = action.SubtitleInfoBeginSync();

            AsyncActionSubtitleInfo.EventArgsResponse response = action.SubtitleInfoEnd(result);
                
            return(response.aSubtitleInfo);
        }
        
        public byte[] AudioTrackInfoSync()
        {
            AsyncActionAudioTrackInfo action = CreateAsyncActionAudioTrackInfo();
            
            object result = action.AudioTrackInfoBeginSync();

            AsyncActionAudioTrackInfo.EventArgsResponse response = action.AudioTrackInfoEnd(result);
                
            return(response.aAudioTrackInfo);
        }
        
        public byte[] TableOfContentsSync()
        {
            AsyncActionTableOfContents action = CreateAsyncActionTableOfContents();
            
            object result = action.TableOfContentsBeginSync();

            AsyncActionTableOfContents.EventArgsResponse response = action.TableOfContentsEnd(result);
                
            return(response.aTableOfContents);
        }
        
        public byte[] DirectoryStructureSync()
        {
            AsyncActionDirectoryStructure action = CreateAsyncActionDirectoryStructure();
            
            object result = action.DirectoryStructureBeginSync();

            AsyncActionDirectoryStructure.EventArgsResponse response = action.DirectoryStructureEnd(result);
                
            return(response.aDirectoryStructure);
        }
        

        // AsyncActionOpen

        public class AsyncActionOpen
        {
            internal AsyncActionOpen(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object OpenBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void OpenBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionOpen.OpenBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse OpenEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionOpen.OpenEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionOpen.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionClose

        public class AsyncActionClose
        {
            internal AsyncActionClose(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object CloseBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CloseBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionClose.CloseBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CloseEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionClose.CloseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionClose.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionPlay

        public class AsyncActionPlay
        {
            internal AsyncActionPlay(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object PlayBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlayBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionPlay.PlayBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Sdp.AsyncActionPlay.PlayEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionPlay.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionStop

        public class AsyncActionStop
        {
            internal AsyncActionStop(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object StopBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void StopBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionStop.StopBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Sdp.AsyncActionStop.StopEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionStop.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionPause

        public class AsyncActionPause
        {
            internal AsyncActionPause(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object PauseBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PauseBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionPause.PauseBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Sdp.AsyncActionPause.PauseEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionPause.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionResume

        public class AsyncActionResume
        {
            internal AsyncActionResume(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object ResumeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ResumeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionResume.ResumeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ResumeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionResume.ResumeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionResume.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSearch

        public class AsyncActionSearch
        {
            internal AsyncActionSearch(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object SearchBeginSync(string aSearchType, int aSearchSpeed)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSearchType", aSearchType);           
                iHandler.WriteArgumentInt("aSearchSpeed", aSearchSpeed);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SearchBegin(string aSearchType, int aSearchSpeed)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSearchType", aSearchType);                
                iHandler.WriteArgumentInt("aSearchSpeed", aSearchSpeed);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSearch.SearchBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Sdp.AsyncActionSearch.SearchEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSearch.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetTrack

        public class AsyncActionSetTrack
        {
            internal AsyncActionSetTrack(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SetTrackBeginSync(int aTrack, int aTitle)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("aTrack", aTrack);           
                iHandler.WriteArgumentInt("aTitle", aTitle);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetTrackBegin(int aTrack, int aTitle)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("aTrack", aTrack);                
                iHandler.WriteArgumentInt("aTitle", aTitle);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetTrack.SetTrackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetTrackEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetTrack.SetTrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetTrack.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetTime

        public class AsyncActionSetTime
        {
            internal AsyncActionSetTime(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object SetTimeBeginSync(string aTime, int aTitle)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aTime", aTime);           
                iHandler.WriteArgumentInt("aTitle", aTitle);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetTimeBegin(string aTime, int aTitle)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aTime", aTime);                
                iHandler.WriteArgumentInt("aTitle", aTitle);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetTime.SetTimeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetTimeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetTime.SetTimeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetTime.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetProgramOff

        public class AsyncActionSetProgramOff
        {
            internal AsyncActionSetProgramOff(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetProgramOffBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetProgramOffBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramOff.SetProgramOffBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetProgramOffEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramOff.SetProgramOffEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramOff.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetProgramInclude

        public class AsyncActionSetProgramInclude
        {
            internal AsyncActionSetProgramInclude(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object SetProgramIncludeBeginSync(byte[] aIncludeList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBinary("aIncludeList", aIncludeList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetProgramIncludeBegin(byte[] aIncludeList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBinary("aIncludeList", aIncludeList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramInclude.SetProgramIncludeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetProgramIncludeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramInclude.SetProgramIncludeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramInclude.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetProgramExclude

        public class AsyncActionSetProgramExclude
        {
            internal AsyncActionSetProgramExclude(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object SetProgramExcludeBeginSync(byte[] aExcludeList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBinary("aExcludeList", aExcludeList);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetProgramExcludeBegin(byte[] aExcludeList)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBinary("aExcludeList", aExcludeList);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramExclude.SetProgramExcludeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetProgramExcludeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramExclude.SetProgramExcludeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramExclude.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetProgramRandom

        public class AsyncActionSetProgramRandom
        {
            internal AsyncActionSetProgramRandom(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object SetProgramRandomBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetProgramRandomBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramRandom.SetProgramRandomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetProgramRandomEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramRandom.SetProgramRandomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramRandom.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetProgramShuffle

        public class AsyncActionSetProgramShuffle
        {
            internal AsyncActionSetProgramShuffle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object SetProgramShuffleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetProgramShuffleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramShuffle.SetProgramShuffleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetProgramShuffleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramShuffle.SetProgramShuffleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetProgramShuffle.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetRepeatOff

        public class AsyncActionSetRepeatOff
        {
            internal AsyncActionSetRepeatOff(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object SetRepeatOffBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRepeatOffBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatOff.SetRepeatOffBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRepeatOffEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatOff.SetRepeatOffEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatOff.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetRepeatAll

        public class AsyncActionSetRepeatAll
        {
            internal AsyncActionSetRepeatAll(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object SetRepeatAllBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRepeatAllBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAll.SetRepeatAllBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRepeatAllEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAll.SetRepeatAllEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAll.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetRepeatTrack

        public class AsyncActionSetRepeatTrack
        {
            internal AsyncActionSetRepeatTrack(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object SetRepeatTrackBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRepeatTrackBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatTrack.SetRepeatTrackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRepeatTrackEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatTrack.SetRepeatTrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatTrack.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetRepeatAb

        public class AsyncActionSetRepeatAb
        {
            internal AsyncActionSetRepeatAb(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object SetRepeatAbBeginSync(string aStartTime, string aEndTime)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aStartTime", aStartTime);           
                iHandler.WriteArgumentString("aEndTime", aEndTime);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRepeatAbBegin(string aStartTime, string aEndTime)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aStartTime", aStartTime);                
                iHandler.WriteArgumentString("aEndTime", aEndTime);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAb.SetRepeatAbBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRepeatAbEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAb.SetRepeatAbEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetRepeatAb.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetIntroMode

        public class AsyncActionSetIntroMode
        {
            internal AsyncActionSetIntroMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object SetIntroModeBeginSync(bool aIntroMode, int aOffset, int aSeconds)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aIntroMode", aIntroMode);           
                iHandler.WriteArgumentInt("aOffset", aOffset);           
                iHandler.WriteArgumentInt("aSeconds", aSeconds);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetIntroModeBegin(bool aIntroMode, int aOffset, int aSeconds)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aIntroMode", aIntroMode);                
                iHandler.WriteArgumentInt("aOffset", aOffset);                
                iHandler.WriteArgumentInt("aSeconds", aSeconds);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetIntroMode.SetIntroModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetIntroModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetIntroMode.SetIntroModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetIntroMode.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetNext

        public class AsyncActionSetNext
        {
            internal AsyncActionSetNext(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object SetNextBeginSync(string aSkip)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSkip", aSkip);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetNextBegin(string aSkip)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSkip", aSkip);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetNext.SetNextBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetNextEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetNext.SetNextEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetNext.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetPrev

        public class AsyncActionSetPrev
        {
            internal AsyncActionSetPrev(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(20));
                iService = aService;
            }

            internal object SetPrevBeginSync(string aSkip)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSkip", aSkip);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetPrevBegin(string aSkip)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSkip", aSkip);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetPrev.SetPrevBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetPrevEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetPrev.SetPrevEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetPrev.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionRootMenu

        public class AsyncActionRootMenu
        {
            internal AsyncActionRootMenu(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(21));
                iService = aService;
            }

            internal object RootMenuBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RootMenuBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionRootMenu.RootMenuBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RootMenuEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionRootMenu.RootMenuEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionRootMenu.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTitleMenu

        public class AsyncActionTitleMenu
        {
            internal AsyncActionTitleMenu(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(22));
                iService = aService;
            }

            internal object TitleMenuBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TitleMenuBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTitleMenu.TitleMenuBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TitleMenuEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTitleMenu.TitleMenuEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTitleMenu.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetSetupMode

        public class AsyncActionSetSetupMode
        {
            internal AsyncActionSetSetupMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(23));
                iService = aService;
            }

            internal object SetSetupModeBeginSync(bool aSetupMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aSetupMode", aSetupMode);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSetupModeBegin(bool aSetupMode)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentBool("aSetupMode", aSetupMode);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetSetupMode.SetSetupModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSetupModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetSetupMode.SetSetupModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetSetupMode.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetAngle

        public class AsyncActionSetAngle
        {
            internal AsyncActionSetAngle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(24));
                iService = aService;
            }

            internal object SetAngleBeginSync(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);           
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetAngleBegin(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetAngle.SetAngleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetAngleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetAngle.SetAngleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetAngle.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetAudioTrack

        public class AsyncActionSetAudioTrack
        {
            internal AsyncActionSetAudioTrack(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(25));
                iService = aService;
            }

            internal object SetAudioTrackBeginSync(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);           
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetAudioTrackBegin(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetAudioTrack.SetAudioTrackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetAudioTrackEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetAudioTrack.SetAudioTrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetAudioTrack.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetSubtitle

        public class AsyncActionSetSubtitle
        {
            internal AsyncActionSetSubtitle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(26));
                iService = aService;
            }

            internal object SetSubtitleBeginSync(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);           
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSubtitleBegin(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetSubtitle.SetSubtitleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSubtitleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetSubtitle.SetSubtitleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetSubtitle.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetZoom

        public class AsyncActionSetZoom
        {
            internal AsyncActionSetZoom(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(27));
                iService = aService;
            }

            internal object SetZoomBeginSync(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);           
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetZoomBegin(string aSelect, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSelect", aSelect);                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetZoom.SetZoomBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetZoomEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetZoom.SetZoomEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetZoom.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetSacdLayer

        public class AsyncActionSetSacdLayer
        {
            internal AsyncActionSetSacdLayer(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(28));
                iService = aService;
            }

            internal object SetSacdLayerBeginSync(string aSacdLayer)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSacdLayer", aSacdLayer);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSacdLayerBegin(string aSacdLayer)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSacdLayer", aSacdLayer);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetSacdLayer.SetSacdLayerBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSacdLayerEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetSacdLayer.SetSacdLayerEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetSacdLayer.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionNavigate

        public class AsyncActionNavigate
        {
            internal AsyncActionNavigate(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(29));
                iService = aService;
            }

            internal object NavigateBeginSync(string aNavigation, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aNavigation", aNavigation);           
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void NavigateBegin(string aNavigation, int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aNavigation", aNavigation);                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionNavigate.NavigateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse NavigateEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionNavigate.NavigateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionNavigate.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetSlideshow

        public class AsyncActionSetSlideshow
        {
            internal AsyncActionSetSlideshow(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(30));
                iService = aService;
            }

            internal object SetSlideshowBeginSync(string aSlideshow)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSlideshow", aSlideshow);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSlideshowBegin(string aSlideshow)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aSlideshow", aSlideshow);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetSlideshow.SetSlideshowBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSlideshowEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetSlideshow.SetSlideshowEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetSlideshow.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetPassword

        public class AsyncActionSetPassword
        {
            internal AsyncActionSetPassword(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(31));
                iService = aService;
            }

            internal object SetPasswordBeginSync(string aPassword)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aPassword", aPassword);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetPasswordBegin(string aPassword)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentString("aPassword", aPassword);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetPassword.SetPasswordBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetPasswordEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetPassword.SetPasswordEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetPassword.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscType

        public class AsyncActionDiscType
        {
            internal AsyncActionDiscType(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(32));
                iService = aService;
            }

            internal object DiscTypeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscTypeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscType.DiscTypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscTypeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscType.DiscTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscType = aHandler.ReadArgumentString("aDiscType");
                }
                
                public string aDiscType;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTitle

        public class AsyncActionTitle
        {
            internal AsyncActionTitle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(33));
                iService = aService;
            }

            internal object TitleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TitleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTitle.TitleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TitleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTitle.TitleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTitle.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTitle = aHandler.ReadArgumentInt("aTitle");
                }
                
                public int aTitle;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrayState

        public class AsyncActionTrayState
        {
            internal AsyncActionTrayState(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(34));
                iService = aService;
            }

            internal object TrayStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrayStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTrayState.TrayStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrayStateEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTrayState.TrayStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrayState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrayState = aHandler.ReadArgumentString("aTrayState");
                }
                
                public string aTrayState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscState

        public class AsyncActionDiscState
        {
            internal AsyncActionDiscState(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(35));
                iService = aService;
            }

            internal object DiscStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscState.DiscStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscStateEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscState.DiscStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscState = aHandler.ReadArgumentString("aDiscState");
                }
                
                public string aDiscState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionPlayState

        public class AsyncActionPlayState
        {
            internal AsyncActionPlayState(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(36));
                iService = aService;
            }

            internal object PlayStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void PlayStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionPlayState.PlayStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse PlayStateEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionPlayState.PlayStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionPlayState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aPlayState = aHandler.ReadArgumentString("aPlayState");
                }
                
                public string aPlayState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSearchType

        public class AsyncActionSearchType
        {
            internal AsyncActionSearchType(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(37));
                iService = aService;
            }

            internal object SearchTypeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SearchTypeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSearchType.SearchTypeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SearchTypeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSearchType.SearchTypeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSearchType.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSearchType = aHandler.ReadArgumentString("aSearchType");
                }
                
                public string aSearchType;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSearchSpeed

        public class AsyncActionSearchSpeed
        {
            internal AsyncActionSearchSpeed(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(38));
                iService = aService;
            }

            internal object SearchSpeedBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SearchSpeedBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSearchSpeed.SearchSpeedBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SearchSpeedEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSearchSpeed.SearchSpeedEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSearchSpeed.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSearchSpeed = aHandler.ReadArgumentInt("aSearchSpeed");
                }
                
                public int aSearchSpeed;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrack

        public class AsyncActionTrack
        {
            internal AsyncActionTrack(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(39));
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
                    UserLog.WriteLine("Sdp.AsyncActionTrack.TrackBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("Sdp.AsyncActionTrack.TrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrack.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrack = aHandler.ReadArgumentInt("aTrack");
                }
                
                public int aTrack;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrackElapsedTime

        public class AsyncActionTrackElapsedTime
        {
            internal AsyncActionTrackElapsedTime(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(40));
                iService = aService;
            }

            internal object TrackElapsedTimeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrackElapsedTimeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTrackElapsedTime.TrackElapsedTimeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrackElapsedTimeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTrackElapsedTime.TrackElapsedTimeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrackElapsedTime.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrackElapsedTime = aHandler.ReadArgumentString("aTrackElapsedTime");
                }
                
                public string aTrackElapsedTime;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrackRemainingTime

        public class AsyncActionTrackRemainingTime
        {
            internal AsyncActionTrackRemainingTime(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(41));
                iService = aService;
            }

            internal object TrackRemainingTimeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrackRemainingTimeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTrackRemainingTime.TrackRemainingTimeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrackRemainingTimeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTrackRemainingTime.TrackRemainingTimeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrackRemainingTime.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrackRemainingTime = aHandler.ReadArgumentString("aTrackRemainingTime");
                }
                
                public string aTrackRemainingTime;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscElapsedTime

        public class AsyncActionDiscElapsedTime
        {
            internal AsyncActionDiscElapsedTime(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(42));
                iService = aService;
            }

            internal object DiscElapsedTimeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscElapsedTimeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscElapsedTime.DiscElapsedTimeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscElapsedTimeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscElapsedTime.DiscElapsedTimeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscElapsedTime.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscElapsedTime = aHandler.ReadArgumentString("aDiscElapsedTime");
                }
                
                public string aDiscElapsedTime;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscRemainingTime

        public class AsyncActionDiscRemainingTime
        {
            internal AsyncActionDiscRemainingTime(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(43));
                iService = aService;
            }

            internal object DiscRemainingTimeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscRemainingTimeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscRemainingTime.DiscRemainingTimeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscRemainingTimeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscRemainingTime.DiscRemainingTimeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscRemainingTime.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscRemainingTime = aHandler.ReadArgumentString("aDiscRemainingTime");
                }
                
                public string aDiscRemainingTime;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionRepeatMode

        public class AsyncActionRepeatMode
        {
            internal AsyncActionRepeatMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(44));
                iService = aService;
            }

            internal object RepeatModeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void RepeatModeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionRepeatMode.RepeatModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse RepeatModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionRepeatMode.RepeatModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionRepeatMode.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aRepeatMode = aHandler.ReadArgumentString("aRepeatMode");
                }
                
                public string aRepeatMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionIntroMode

        public class AsyncActionIntroMode
        {
            internal AsyncActionIntroMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(45));
                iService = aService;
            }

            internal object IntroModeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void IntroModeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionIntroMode.IntroModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse IntroModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionIntroMode.IntroModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionIntroMode.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aIntroMode = aHandler.ReadArgumentBool("aIntroMode");
                }
                
                public bool aIntroMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionProgramMode

        public class AsyncActionProgramMode
        {
            internal AsyncActionProgramMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(46));
                iService = aService;
            }

            internal object ProgramModeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ProgramModeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionProgramMode.ProgramModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ProgramModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionProgramMode.ProgramModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionProgramMode.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aProgramMode = aHandler.ReadArgumentString("aProgramMode");
                }
                
                public string aProgramMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDomain

        public class AsyncActionDomain
        {
            internal AsyncActionDomain(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(47));
                iService = aService;
            }

            internal object DomainBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DomainBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDomain.DomainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DomainEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDomain.DomainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDomain.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDomain = aHandler.ReadArgumentString("aDomain");
                }
                
                public string aDomain;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionAngle

        public class AsyncActionAngle
        {
            internal AsyncActionAngle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(48));
                iService = aService;
            }

            internal object AngleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AngleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionAngle.AngleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AngleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionAngle.AngleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionAngle.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aAngle = aHandler.ReadArgumentInt("aAngle");
                }
                
                public int aAngle;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTotalAngles

        public class AsyncActionTotalAngles
        {
            internal AsyncActionTotalAngles(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(49));
                iService = aService;
            }

            internal object TotalAnglesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TotalAnglesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTotalAngles.TotalAnglesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TotalAnglesEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTotalAngles.TotalAnglesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTotalAngles.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTotalAngles = aHandler.ReadArgumentInt("aTotalAngles");
                }
                
                public int aTotalAngles;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSubtitle

        public class AsyncActionSubtitle
        {
            internal AsyncActionSubtitle(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(50));
                iService = aService;
            }

            internal object SubtitleBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SubtitleBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSubtitle.SubtitleBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SubtitleEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSubtitle.SubtitleEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSubtitle.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSubtitle = aHandler.ReadArgumentInt("aSubtitle");
                }
                
                public int aSubtitle;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionAudioTrack

        public class AsyncActionAudioTrack
        {
            internal AsyncActionAudioTrack(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(51));
                iService = aService;
            }

            internal object AudioTrackBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AudioTrackBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrack.AudioTrackBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AudioTrackEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrack.AudioTrackEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrack.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aAudioTrack = aHandler.ReadArgumentInt("aAudioTrack");
                }
                
                public int aAudioTrack;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionZoomLevel

        public class AsyncActionZoomLevel
        {
            internal AsyncActionZoomLevel(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(52));
                iService = aService;
            }

            internal object ZoomLevelBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ZoomLevelBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevel.ZoomLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ZoomLevelEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevel.ZoomLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aZoomLevel = aHandler.ReadArgumentString("aZoomLevel");
                }
                
                public string aZoomLevel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSetupMode

        public class AsyncActionSetupMode
        {
            internal AsyncActionSetupMode(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(53));
                iService = aService;
            }

            internal object SetupModeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SetupModeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSetupMode.SetupModeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetupModeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSetupMode.SetupModeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSetupMode.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSetupMode = aHandler.ReadArgumentBool("aSetupMode");
                }
                
                public bool aSetupMode;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSacdState

        public class AsyncActionSacdState
        {
            internal AsyncActionSacdState(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(54));
                iService = aService;
            }

            internal object SacdStateBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SacdStateBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSacdState.SacdStateBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SacdStateEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSacdState.SacdStateEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSacdState.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSacdState = aHandler.ReadArgumentString("aSacdState");
                }
                
                public string aSacdState;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSlideshow

        public class AsyncActionSlideshow
        {
            internal AsyncActionSlideshow(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(55));
                iService = aService;
            }

            internal object SlideshowBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SlideshowBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSlideshow.SlideshowBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SlideshowEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSlideshow.SlideshowEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSlideshow.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSlideshow = aHandler.ReadArgumentString("aSlideshow");
                }
                
                public string aSlideshow;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionError

        public class AsyncActionError
        {
            internal AsyncActionError(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(56));
                iService = aService;
            }

            internal object ErrorBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ErrorBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionError.ErrorBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ErrorEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionError.ErrorEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionError.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aError = aHandler.ReadArgumentString("aError");
                }
                
                public string aError;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionOrientation

        public class AsyncActionOrientation
        {
            internal AsyncActionOrientation(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(57));
                iService = aService;
            }

            internal object OrientationBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void OrientationBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionOrientation.OrientationBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse OrientationEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionOrientation.OrientationEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionOrientation.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aOrientation = aHandler.ReadArgumentString("aOrientation");
                }
                
                public string aOrientation;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscLength

        public class AsyncActionDiscLength
        {
            internal AsyncActionDiscLength(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(58));
                iService = aService;
            }

            internal object DiscLengthBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscLengthBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscLength.DiscLengthBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscLengthEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscLength.DiscLengthEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscLength.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscLength = aHandler.ReadArgumentString("aDiscLength");
                }
                
                public string aDiscLength;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrackLength

        public class AsyncActionTrackLength
        {
            internal AsyncActionTrackLength(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(59));
                iService = aService;
            }

            internal object TrackLengthBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrackLengthBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTrackLength.TrackLengthBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrackLengthEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTrackLength.TrackLengthEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrackLength.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrackLength = aHandler.ReadArgumentString("aTrackLength");
                }
                
                public string aTrackLength;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTotalTracks

        public class AsyncActionTotalTracks
        {
            internal AsyncActionTotalTracks(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(60));
                iService = aService;
            }

            internal object TotalTracksBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TotalTracksBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTotalTracks.TotalTracksBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TotalTracksEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTotalTracks.TotalTracksEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTotalTracks.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTotalTracks = aHandler.ReadArgumentInt("aTotalTracks");
                }
                
                public int aTotalTracks;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTotalTitles

        public class AsyncActionTotalTitles
        {
            internal AsyncActionTotalTitles(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(61));
                iService = aService;
            }

            internal object TotalTitlesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TotalTitlesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTotalTitles.TotalTitlesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TotalTitlesEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTotalTitles.TotalTitlesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTotalTitles.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTotalTitles = aHandler.ReadArgumentInt("aTotalTitles");
                }
                
                public int aTotalTitles;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionGenre

        public class AsyncActionGenre
        {
            internal AsyncActionGenre(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(62));
                iService = aService;
            }

            internal object GenreBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void GenreBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionGenre.GenreBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GenreEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionGenre.GenreEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionGenre.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aGenre = aHandler.ReadArgumentString("aGenre");
                }
                
                public string aGenre;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionEncoding

        public class AsyncActionEncoding
        {
            internal AsyncActionEncoding(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(63));
                iService = aService;
            }

            internal object EncodingBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void EncodingBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionEncoding.EncodingBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse EncodingEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionEncoding.EncodingEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionEncoding.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aEncoding = aHandler.ReadArgumentUint("aEncoding");
                }
                
                public uint aEncoding;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionFileSize

        public class AsyncActionFileSize
        {
            internal AsyncActionFileSize(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(64));
                iService = aService;
            }

            internal object FileSizeBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void FileSizeBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionFileSize.FileSizeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse FileSizeEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionFileSize.FileSizeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionFileSize.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aFileSize = aHandler.ReadArgumentUint("aFileSize");
                }
                
                public uint aFileSize;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscId

        public class AsyncActionDiscId
        {
            internal AsyncActionDiscId(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(65));
                iService = aService;
            }

            internal object DiscIdBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscIdBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscId.DiscIdBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscIdEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscId.DiscIdEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscId.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscId = aHandler.ReadArgumentUint("aDiscId");
                }
                
                public uint aDiscId;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionYear

        public class AsyncActionYear
        {
            internal AsyncActionYear(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(66));
                iService = aService;
            }

            internal object YearBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void YearBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionYear.YearBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse YearEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionYear.YearEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionYear.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aYear = aHandler.ReadArgumentString("aYear");
                }
                
                public string aYear;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTrackName

        public class AsyncActionTrackName
        {
            internal AsyncActionTrackName(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(67));
                iService = aService;
            }

            internal object TrackNameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TrackNameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTrackName.TrackNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TrackNameEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTrackName.TrackNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTrackName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTrackName = aHandler.ReadArgumentString("aTrackName");
                }
                
                public string aTrackName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionArtistName

        public class AsyncActionArtistName
        {
            internal AsyncActionArtistName(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(68));
                iService = aService;
            }

            internal object ArtistNameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ArtistNameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionArtistName.ArtistNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ArtistNameEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionArtistName.ArtistNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionArtistName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aArtistName = aHandler.ReadArgumentString("aArtistName");
                }
                
                public string aArtistName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionAlbumName

        public class AsyncActionAlbumName
        {
            internal AsyncActionAlbumName(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(69));
                iService = aService;
            }

            internal object AlbumNameBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AlbumNameBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionAlbumName.AlbumNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AlbumNameEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionAlbumName.AlbumNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionAlbumName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aAlbumName = aHandler.ReadArgumentString("aAlbumName");
                }
                
                public string aAlbumName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionComment

        public class AsyncActionComment
        {
            internal AsyncActionComment(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(70));
                iService = aService;
            }

            internal object CommentBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void CommentBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionComment.CommentBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse CommentEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionComment.CommentEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionComment.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aComment = aHandler.ReadArgumentString("aComment");
                }
                
                public string aComment;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionFileName

        public class AsyncActionFileName
        {
            internal AsyncActionFileName(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(71));
                iService = aService;
            }

            internal object FileNameBeginSync(int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("aIndex", aIndex);           
                
                return (iHandler.WriteEnd(null));
            }

            public void FileNameBegin(int aIndex)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentInt("aIndex", aIndex);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionFileName.FileNameBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse FileNameEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionFileName.FileNameEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionFileName.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aFileName = aHandler.ReadArgumentString("aFileName");
                }
                
                public string aFileName;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSystemCapabilities

        public class AsyncActionSystemCapabilities
        {
            internal AsyncActionSystemCapabilities(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(72));
                iService = aService;
            }

            internal object SystemCapabilitiesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SystemCapabilitiesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSystemCapabilities.SystemCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SystemCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSystemCapabilities.SystemCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSystemCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSystemCapabilities = aHandler.ReadArgumentBinary("aSystemCapabilities");
                }
                
                public byte[] aSystemCapabilities;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDiscCapabilities

        public class AsyncActionDiscCapabilities
        {
            internal AsyncActionDiscCapabilities(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(73));
                iService = aService;
            }

            internal object DiscCapabilitiesBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DiscCapabilitiesBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDiscCapabilities.DiscCapabilitiesBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DiscCapabilitiesEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDiscCapabilities.DiscCapabilitiesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDiscCapabilities.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDiscCapabilities = aHandler.ReadArgumentBinary("aDiscCapabilities");
                }
                
                public byte[] aDiscCapabilities;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionZoomLevelInfo

        public class AsyncActionZoomLevelInfo
        {
            internal AsyncActionZoomLevelInfo(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(74));
                iService = aService;
            }

            internal object ZoomLevelInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void ZoomLevelInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevelInfo.ZoomLevelInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ZoomLevelInfoEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevelInfo.ZoomLevelInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionZoomLevelInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aZoomLevelInfo = aHandler.ReadArgumentBinary("aZoomLevelInfo");
                }
                
                public byte[] aZoomLevelInfo;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionSubtitleInfo

        public class AsyncActionSubtitleInfo
        {
            internal AsyncActionSubtitleInfo(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(75));
                iService = aService;
            }

            internal object SubtitleInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void SubtitleInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionSubtitleInfo.SubtitleInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SubtitleInfoEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionSubtitleInfo.SubtitleInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionSubtitleInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aSubtitleInfo = aHandler.ReadArgumentBinary("aSubtitleInfo");
                }
                
                public byte[] aSubtitleInfo;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionAudioTrackInfo

        public class AsyncActionAudioTrackInfo
        {
            internal AsyncActionAudioTrackInfo(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(76));
                iService = aService;
            }

            internal object AudioTrackInfoBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void AudioTrackInfoBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrackInfo.AudioTrackInfoBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse AudioTrackInfoEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrackInfo.AudioTrackInfoEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionAudioTrackInfo.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aAudioTrackInfo = aHandler.ReadArgumentBinary("aAudioTrackInfo");
                }
                
                public byte[] aAudioTrackInfo;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionTableOfContents

        public class AsyncActionTableOfContents
        {
            internal AsyncActionTableOfContents(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(77));
                iService = aService;
            }

            internal object TableOfContentsBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void TableOfContentsBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionTableOfContents.TableOfContentsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse TableOfContentsEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionTableOfContents.TableOfContentsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionTableOfContents.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aTableOfContents = aHandler.ReadArgumentBinary("aTableOfContents");
                }
                
                public byte[] aTableOfContents;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
        }
        
        
        // AsyncActionDirectoryStructure

        public class AsyncActionDirectoryStructure
        {
            internal AsyncActionDirectoryStructure(ServiceSdp aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(78));
                iService = aService;
            }

            internal object DirectoryStructureBeginSync()
            {
                iHandler.WriteBegin();
                
                
                return (iHandler.WriteEnd(null));
            }

            public void DirectoryStructureBegin()
            {
                iHandler.WriteBegin();
                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("Sdp.AsyncActionDirectoryStructure.DirectoryStructureBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse DirectoryStructureEnd(object aResult)
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
                    UserLog.WriteLine("Sdp.AsyncActionDirectoryStructure.DirectoryStructureEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("Sdp.AsyncActionDirectoryStructure.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    aDirectoryStructure = aHandler.ReadArgumentBinary("aDirectoryStructure");
                }
                
                public byte[] aDirectoryStructure;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceSdp iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceSdp): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

            bool eventDiscType = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DiscType", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                DiscType = value;

                eventDiscType = true;
            }

            bool eventTitle = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Title", nsmanager);

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
					Title = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Title with value {1}", DateTime.Now, value));
				}

                eventTitle = true;
            }

            bool eventTrayState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TrayState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TrayState = value;

                eventTrayState = true;
            }

            bool eventDiscState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DiscState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                DiscState = value;

                eventDiscState = true;
            }

            bool eventPlayState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "PlayState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                PlayState = value;

                eventPlayState = true;
            }

            bool eventSearchType = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SearchType", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SearchType = value;

                eventSearchType = true;
            }

            bool eventSearchSpeed = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SearchSpeed", nsmanager);

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
					SearchSpeed = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse SearchSpeed with value {1}", DateTime.Now, value));
				}

                eventSearchSpeed = true;
            }

            bool eventTrack = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Track", nsmanager);

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
					Track = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Track with value {1}", DateTime.Now, value));
				}

                eventTrack = true;
            }

            bool eventRepeatMode = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "RepeatMode", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                RepeatMode = value;

                eventRepeatMode = true;
            }

            bool eventIntroMode = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "IntroMode", nsmanager);

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
	                IntroMode = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		IntroMode = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	IntroMode = false; 
    	            }
                }

                eventIntroMode = true;
            }

            bool eventProgramMode = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ProgramMode", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ProgramMode = value;

                eventProgramMode = true;
            }

            bool eventDomain = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Domain", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Domain = value;

                eventDomain = true;
            }

            bool eventAngle = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Angle", nsmanager);

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
					Angle = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Angle with value {1}", DateTime.Now, value));
				}

                eventAngle = true;
            }

            bool eventTotalAngles = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TotalAngles", nsmanager);

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
					TotalAngles = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse TotalAngles with value {1}", DateTime.Now, value));
				}

                eventTotalAngles = true;
            }

            bool eventSubtitle = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Subtitle", nsmanager);

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
					Subtitle = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Subtitle with value {1}", DateTime.Now, value));
				}

                eventSubtitle = true;
            }

            bool eventAudioTrack = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "AudioTrack", nsmanager);

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
					AudioTrack = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse AudioTrack with value {1}", DateTime.Now, value));
				}

                eventAudioTrack = true;
            }

            bool eventZoomLevel = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ZoomLevel", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ZoomLevel = value;

                eventZoomLevel = true;
            }

            bool eventSetupMode = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SetupMode", nsmanager);

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
	                SetupMode = bool.Parse(value);
                }
                catch (FormatException)
                {
                	try
                	{
                		SetupMode = (uint.Parse(value) > 0);
                	}
	                catch (FormatException)
    	            {
    	            	SetupMode = false; 
    	            }
                }

                eventSetupMode = true;
            }

            bool eventSacdState = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SacdState", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                SacdState = value;

                eventSacdState = true;
            }

            bool eventSlideshow = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Slideshow", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Slideshow = value;

                eventSlideshow = true;
            }

            bool eventError = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Error", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Error = value;

                eventError = true;
            }

            bool eventOrientation = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Orientation", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Orientation = value;

                eventOrientation = true;
            }

            bool eventTotalTracks = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TotalTracks", nsmanager);

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
					TotalTracks = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse TotalTracks with value {1}", DateTime.Now, value));
				}

                eventTotalTracks = true;
            }

            bool eventTotalTitles = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TotalTitles", nsmanager);

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
					TotalTitles = int.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse TotalTitles with value {1}", DateTime.Now, value));
				}

                eventTotalTitles = true;
            }

            bool eventEncoding = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Encoding", nsmanager);

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
					Encoding = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse Encoding with value {1}", DateTime.Now, value));
				}

                eventEncoding = true;
            }

            bool eventFileSize = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "FileSize", nsmanager);

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
					FileSize = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse FileSize with value {1}", DateTime.Now, value));
				}

                eventFileSize = true;
            }

            bool eventDiscId = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DiscId", nsmanager);

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
					DiscId = uint.Parse(value);
				}
				catch (Exception)
				{
					UserLog.WriteLine(String.Format("{0}: Warning: Exception caught in parse DiscId with value {1}", DateTime.Now, value));
				}

                eventDiscId = true;
            }

            bool eventDiscLength = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DiscLength", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                DiscLength = value;

                eventDiscLength = true;
            }

            bool eventTrackLength = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TrackLength", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TrackLength = value;

                eventTrackLength = true;
            }

            bool eventGenre = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Genre", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Genre = value;

                eventGenre = true;
            }

            bool eventYear = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Year", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Year = value;

                eventYear = true;
            }

            bool eventTrackName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TrackName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                TrackName = value;

                eventTrackName = true;
            }

            bool eventArtistName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ArtistName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                ArtistName = value;

                eventArtistName = true;
            }

            bool eventAlbumName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "AlbumName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                AlbumName = value;

                eventAlbumName = true;
            }

            bool eventComment = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "Comment", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
                Comment = value;

                eventComment = true;
            }

            bool eventFileName = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "FileName", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    FileName =  new byte[0];
				}
				else
				{
                    FileName = Convert.FromBase64String(value);
                }

                eventFileName = true;
            }

            bool eventSystemCapabilities = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SystemCapabilities", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    SystemCapabilities =  new byte[0];
				}
				else
				{
                    SystemCapabilities = Convert.FromBase64String(value);
                }

                eventSystemCapabilities = true;
            }

            bool eventDiscCapabilities = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DiscCapabilities", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    DiscCapabilities =  new byte[0];
				}
				else
				{
                    DiscCapabilities = Convert.FromBase64String(value);
                }

                eventDiscCapabilities = true;
            }

            bool eventZoomLevelInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "ZoomLevelInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    ZoomLevelInfo =  new byte[0];
				}
				else
				{
                    ZoomLevelInfo = Convert.FromBase64String(value);
                }

                eventZoomLevelInfo = true;
            }

            bool eventSubtitleInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "SubtitleInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    SubtitleInfo =  new byte[0];
				}
				else
				{
                    SubtitleInfo = Convert.FromBase64String(value);
                }

                eventSubtitleInfo = true;
            }

            bool eventAudioTrackInfo = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "AudioTrackInfo", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    AudioTrackInfo =  new byte[0];
				}
				else
				{
                    AudioTrackInfo = Convert.FromBase64String(value);
                }

                eventAudioTrackInfo = true;
            }

            bool eventTableOfContents = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "TableOfContents", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    TableOfContents =  new byte[0];
				}
				else
				{
                    TableOfContents = Convert.FromBase64String(value);
                }

                eventTableOfContents = true;
            }

            bool eventDirectoryStructure = false;
            variable = e.Xml.SelectSingleNode(kBasePropertyPath + "DirectoryStructure", nsmanager);

            if (variable != null)
            {
                string value = "";

                XmlNode child = variable.FirstChild;

                if (child != null)
                {
                    value = child.Value;
                }
                    
				if (value == String.Empty)
				{
				    DirectoryStructure =  new byte[0];
				}
				else
				{
                    DirectoryStructure = Convert.FromBase64String(value);
                }

                eventDirectoryStructure = true;
            }

          
            
            if(eventDiscType)
            {
                if (EventStateDiscType != null)
                {
					try
					{
						EventStateDiscType(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDiscType: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTitle)
            {
                if (EventStateTitle != null)
                {
					try
					{
						EventStateTitle(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTitle: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTrayState)
            {
                if (EventStateTrayState != null)
                {
					try
					{
						EventStateTrayState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTrayState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDiscState)
            {
                if (EventStateDiscState != null)
                {
					try
					{
						EventStateDiscState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDiscState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventPlayState)
            {
                if (EventStatePlayState != null)
                {
					try
					{
						EventStatePlayState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStatePlayState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSearchType)
            {
                if (EventStateSearchType != null)
                {
					try
					{
						EventStateSearchType(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSearchType: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSearchSpeed)
            {
                if (EventStateSearchSpeed != null)
                {
					try
					{
						EventStateSearchSpeed(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSearchSpeed: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTrack)
            {
                if (EventStateTrack != null)
                {
					try
					{
						EventStateTrack(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTrack: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventRepeatMode)
            {
                if (EventStateRepeatMode != null)
                {
					try
					{
						EventStateRepeatMode(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateRepeatMode: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventIntroMode)
            {
                if (EventStateIntroMode != null)
                {
					try
					{
						EventStateIntroMode(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateIntroMode: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventProgramMode)
            {
                if (EventStateProgramMode != null)
                {
					try
					{
						EventStateProgramMode(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateProgramMode: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDomain)
            {
                if (EventStateDomain != null)
                {
					try
					{
						EventStateDomain(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDomain: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAngle)
            {
                if (EventStateAngle != null)
                {
					try
					{
						EventStateAngle(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAngle: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTotalAngles)
            {
                if (EventStateTotalAngles != null)
                {
					try
					{
						EventStateTotalAngles(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTotalAngles: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSubtitle)
            {
                if (EventStateSubtitle != null)
                {
					try
					{
						EventStateSubtitle(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSubtitle: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAudioTrack)
            {
                if (EventStateAudioTrack != null)
                {
					try
					{
						EventStateAudioTrack(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAudioTrack: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventZoomLevel)
            {
                if (EventStateZoomLevel != null)
                {
					try
					{
						EventStateZoomLevel(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateZoomLevel: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSetupMode)
            {
                if (EventStateSetupMode != null)
                {
					try
					{
						EventStateSetupMode(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSetupMode: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSacdState)
            {
                if (EventStateSacdState != null)
                {
					try
					{
						EventStateSacdState(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSacdState: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSlideshow)
            {
                if (EventStateSlideshow != null)
                {
					try
					{
						EventStateSlideshow(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSlideshow: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventError)
            {
                if (EventStateError != null)
                {
					try
					{
						EventStateError(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateError: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventOrientation)
            {
                if (EventStateOrientation != null)
                {
					try
					{
						EventStateOrientation(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateOrientation: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTotalTracks)
            {
                if (EventStateTotalTracks != null)
                {
					try
					{
						EventStateTotalTracks(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTotalTracks: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTotalTitles)
            {
                if (EventStateTotalTitles != null)
                {
					try
					{
						EventStateTotalTitles(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTotalTitles: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventEncoding)
            {
                if (EventStateEncoding != null)
                {
					try
					{
						EventStateEncoding(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateEncoding: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventFileSize)
            {
                if (EventStateFileSize != null)
                {
					try
					{
						EventStateFileSize(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateFileSize: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDiscId)
            {
                if (EventStateDiscId != null)
                {
					try
					{
						EventStateDiscId(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDiscId: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDiscLength)
            {
                if (EventStateDiscLength != null)
                {
					try
					{
						EventStateDiscLength(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDiscLength: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTrackLength)
            {
                if (EventStateTrackLength != null)
                {
					try
					{
						EventStateTrackLength(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTrackLength: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventGenre)
            {
                if (EventStateGenre != null)
                {
					try
					{
						EventStateGenre(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateGenre: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventYear)
            {
                if (EventStateYear != null)
                {
					try
					{
						EventStateYear(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateYear: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTrackName)
            {
                if (EventStateTrackName != null)
                {
					try
					{
						EventStateTrackName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTrackName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventArtistName)
            {
                if (EventStateArtistName != null)
                {
					try
					{
						EventStateArtistName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateArtistName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAlbumName)
            {
                if (EventStateAlbumName != null)
                {
					try
					{
						EventStateAlbumName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAlbumName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventComment)
            {
                if (EventStateComment != null)
                {
					try
					{
						EventStateComment(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateComment: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventFileName)
            {
                if (EventStateFileName != null)
                {
					try
					{
						EventStateFileName(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateFileName: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSystemCapabilities)
            {
                if (EventStateSystemCapabilities != null)
                {
					try
					{
						EventStateSystemCapabilities(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSystemCapabilities: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDiscCapabilities)
            {
                if (EventStateDiscCapabilities != null)
                {
					try
					{
						EventStateDiscCapabilities(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDiscCapabilities: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventZoomLevelInfo)
            {
                if (EventStateZoomLevelInfo != null)
                {
					try
					{
						EventStateZoomLevelInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateZoomLevelInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventSubtitleInfo)
            {
                if (EventStateSubtitleInfo != null)
                {
					try
					{
						EventStateSubtitleInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateSubtitleInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventAudioTrackInfo)
            {
                if (EventStateAudioTrackInfo != null)
                {
					try
					{
						EventStateAudioTrackInfo(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateAudioTrackInfo: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventTableOfContents)
            {
                if (EventStateTableOfContents != null)
                {
					try
					{
						EventStateTableOfContents(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateTableOfContents: " + ex);
						Assert.CheckDebug(false);
					}
                }
            }
            
            if(eventDirectoryStructure)
            {
                if (EventStateDirectoryStructure != null)
                {
					try
					{
						EventStateDirectoryStructure(this, EventArgs.Empty);
					}
					catch(Exception ex)
					{
						UserLog.WriteLine("Exception caught in EventStateDirectoryStructure: " + ex);
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
        public event EventHandler<EventArgs> EventStateDiscType;
        public event EventHandler<EventArgs> EventStateTitle;
        public event EventHandler<EventArgs> EventStateTrayState;
        public event EventHandler<EventArgs> EventStateDiscState;
        public event EventHandler<EventArgs> EventStatePlayState;
        public event EventHandler<EventArgs> EventStateSearchType;
        public event EventHandler<EventArgs> EventStateSearchSpeed;
        public event EventHandler<EventArgs> EventStateTrack;
        public event EventHandler<EventArgs> EventStateRepeatMode;
        public event EventHandler<EventArgs> EventStateIntroMode;
        public event EventHandler<EventArgs> EventStateProgramMode;
        public event EventHandler<EventArgs> EventStateDomain;
        public event EventHandler<EventArgs> EventStateAngle;
        public event EventHandler<EventArgs> EventStateTotalAngles;
        public event EventHandler<EventArgs> EventStateSubtitle;
        public event EventHandler<EventArgs> EventStateAudioTrack;
        public event EventHandler<EventArgs> EventStateZoomLevel;
        public event EventHandler<EventArgs> EventStateSetupMode;
        public event EventHandler<EventArgs> EventStateSacdState;
        public event EventHandler<EventArgs> EventStateSlideshow;
        public event EventHandler<EventArgs> EventStateError;
        public event EventHandler<EventArgs> EventStateOrientation;
        public event EventHandler<EventArgs> EventStateTotalTracks;
        public event EventHandler<EventArgs> EventStateTotalTitles;
        public event EventHandler<EventArgs> EventStateEncoding;
        public event EventHandler<EventArgs> EventStateFileSize;
        public event EventHandler<EventArgs> EventStateDiscId;
        public event EventHandler<EventArgs> EventStateDiscLength;
        public event EventHandler<EventArgs> EventStateTrackLength;
        public event EventHandler<EventArgs> EventStateGenre;
        public event EventHandler<EventArgs> EventStateYear;
        public event EventHandler<EventArgs> EventStateTrackName;
        public event EventHandler<EventArgs> EventStateArtistName;
        public event EventHandler<EventArgs> EventStateAlbumName;
        public event EventHandler<EventArgs> EventStateComment;
        public event EventHandler<EventArgs> EventStateFileName;
        public event EventHandler<EventArgs> EventStateSystemCapabilities;
        public event EventHandler<EventArgs> EventStateDiscCapabilities;
        public event EventHandler<EventArgs> EventStateZoomLevelInfo;
        public event EventHandler<EventArgs> EventStateSubtitleInfo;
        public event EventHandler<EventArgs> EventStateAudioTrackInfo;
        public event EventHandler<EventArgs> EventStateTableOfContents;
        public event EventHandler<EventArgs> EventStateDirectoryStructure;

        public string DiscType;
        public int Title;
        public string TrayState;
        public string DiscState;
        public string PlayState;
        public string SearchType;
        public int SearchSpeed;
        public int Track;
        public string RepeatMode;
        public bool IntroMode;
        public string ProgramMode;
        public string Domain;
        public int Angle;
        public int TotalAngles;
        public int Subtitle;
        public int AudioTrack;
        public string ZoomLevel;
        public bool SetupMode;
        public string SacdState;
        public string Slideshow;
        public string Error;
        public string Orientation;
        public int TotalTracks;
        public int TotalTitles;
        public uint Encoding;
        public uint FileSize;
        public uint DiscId;
        public string DiscLength;
        public string TrackLength;
        public string Genre;
        public string Year;
        public string TrackName;
        public string ArtistName;
        public string AlbumName;
        public string Comment;
        public byte[] FileName;
        public byte[] SystemCapabilities;
        public byte[] DiscCapabilities;
        public byte[] ZoomLevelInfo;
        public byte[] SubtitleInfo;
        public byte[] AudioTrackInfo;
        public byte[] TableOfContents;
        public byte[] DirectoryStructure;
    }
}
    
