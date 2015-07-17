using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

using Linn.Control;

namespace Linn.ControlPoint.Upnp
{
    public class ServiceRenderingControl : ServiceUpnp
    {

        public const string kChannelMaster = "Master";
        public const string kChannelLf = "LF";
        public const string kChannelRf = "RF";
        public const string kChannelCf = "CF";
        public const string kChannelLfe = "LFE";
        public const string kChannelLs = "LS";
        public const string kChannelRs = "RS";
        public const string kChannelLfc = "LFC";
        public const string kChannelRfc = "RFC";
        public const string kChannelSd = "SD";
        public const string kChannelSl = "SL";
        public const string kChannelSr = "SR";
        public const string kChannelT = "T";
        public const string kChannelB = "B";
        public const string kPresetNameFactoryDefaults = "FactoryDefaults";
        public const string kPresetNameInstallationDefaults = "InstallationDefaults";

        public ServiceRenderingControl(Device aDevice)
            : this(aDevice, null)
        {
        }

        public ServiceRenderingControl(Device aDevice, IEventUpnpProvider aEventServer)
            : base(aDevice, ServiceType(), new ProtocolUpnp(), aEventServer)
        {
            Action action = null;
            
            action = new Action("ListPresets");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentPresetNameList", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SelectPreset");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("PresetName", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("GetBrightness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentBrightness", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetBrightness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredBrightness", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetContrast");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentContrast", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetContrast");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredContrast", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetSharpness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentSharpness", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetSharpness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredSharpness", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetRedVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentRedVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetRedVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredRedVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetGreenVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentGreenVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetGreenVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredGreenVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetBlueVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentBlueVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetBlueVideoGain");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredBlueVideoGain", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetRedVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentRedVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetRedVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredRedVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetGreenVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentGreenVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetGreenVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredGreenVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetBlueVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentBlueVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetBlueVideoBlackLevel");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredBlueVideoBlackLevel", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetColorTemperature");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentColorTemperature", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetColorTemperature");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredColorTemperature", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetHorizontalKeystone");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentHorizontalKeystone", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetHorizontalKeystone");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredHorizontalKeystone", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("GetVerticalKeystone");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddOutArgument(new Argument("CurrentVerticalKeystone", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetVerticalKeystone");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("DesiredVerticalKeystone", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("GetMute");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentMute", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetMute");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddInArgument(new Argument("DesiredMute", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("GetVolume");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentVolume", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("SetVolume");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddInArgument(new Argument("DesiredVolume", Argument.EType.eUint));
            iActions.Add(action);
            
            action = new Action("GetVolumeDB");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentVolume", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("SetVolumeDB");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddInArgument(new Argument("DesiredVolume", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("GetVolumeDBRange");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddOutArgument(new Argument("MinValue", Argument.EType.eInt));
            action.AddOutArgument(new Argument("MaxValue", Argument.EType.eInt));
            iActions.Add(action);
            
            action = new Action("GetLoudness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddOutArgument(new Argument("CurrentLoudness", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("SetLoudness");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("Channel", Argument.EType.eString));
            action.AddInArgument(new Argument("DesiredLoudness", Argument.EType.eBool));
            iActions.Add(action);
            
            action = new Action("GetStateVariables");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("StateVariableList", Argument.EType.eString));
            action.AddInArgument(new Argument("StateVariableValuePairs", Argument.EType.eString));
            iActions.Add(action);
            
            action = new Action("SetStateVariables");
            action.AddInArgument(new Argument("InstanceID", Argument.EType.eUint));
            action.AddInArgument(new Argument("RenderingControlUDN", Argument.EType.eString));
            action.AddInArgument(new Argument("ServiceType", Argument.EType.eString));
            action.AddInArgument(new Argument("ServiceId", Argument.EType.eString));
            action.AddInArgument(new Argument("StateVariableValuePairs", Argument.EType.eString));
            action.AddOutArgument(new Argument("StateVariableList", Argument.EType.eString));
            iActions.Add(action);
            
        
        }

        public static ServiceType ServiceType()
        {
            return (new ServiceType("upnp.org", "RenderingControl", 1));
        }
        
        public static ServiceType ServiceType(uint aVersion)
        {
            return (new ServiceType("upnp.org", "RenderingControl", aVersion));
        }
        
        // Create async action objects
        
        public AsyncActionListPresets CreateAsyncActionListPresets()
        {
            return (new AsyncActionListPresets(this));
        }

        public AsyncActionSelectPreset CreateAsyncActionSelectPreset()
        {
            return (new AsyncActionSelectPreset(this));
        }

        public AsyncActionGetBrightness CreateAsyncActionGetBrightness()
        {
            return (new AsyncActionGetBrightness(this));
        }

        public AsyncActionSetBrightness CreateAsyncActionSetBrightness()
        {
            return (new AsyncActionSetBrightness(this));
        }

        public AsyncActionGetContrast CreateAsyncActionGetContrast()
        {
            return (new AsyncActionGetContrast(this));
        }

        public AsyncActionSetContrast CreateAsyncActionSetContrast()
        {
            return (new AsyncActionSetContrast(this));
        }

        public AsyncActionGetSharpness CreateAsyncActionGetSharpness()
        {
            return (new AsyncActionGetSharpness(this));
        }

        public AsyncActionSetSharpness CreateAsyncActionSetSharpness()
        {
            return (new AsyncActionSetSharpness(this));
        }

        public AsyncActionGetRedVideoGain CreateAsyncActionGetRedVideoGain()
        {
            return (new AsyncActionGetRedVideoGain(this));
        }

        public AsyncActionSetRedVideoGain CreateAsyncActionSetRedVideoGain()
        {
            return (new AsyncActionSetRedVideoGain(this));
        }

        public AsyncActionGetGreenVideoGain CreateAsyncActionGetGreenVideoGain()
        {
            return (new AsyncActionGetGreenVideoGain(this));
        }

        public AsyncActionSetGreenVideoGain CreateAsyncActionSetGreenVideoGain()
        {
            return (new AsyncActionSetGreenVideoGain(this));
        }

        public AsyncActionGetBlueVideoGain CreateAsyncActionGetBlueVideoGain()
        {
            return (new AsyncActionGetBlueVideoGain(this));
        }

        public AsyncActionSetBlueVideoGain CreateAsyncActionSetBlueVideoGain()
        {
            return (new AsyncActionSetBlueVideoGain(this));
        }

        public AsyncActionGetRedVideoBlackLevel CreateAsyncActionGetRedVideoBlackLevel()
        {
            return (new AsyncActionGetRedVideoBlackLevel(this));
        }

        public AsyncActionSetRedVideoBlackLevel CreateAsyncActionSetRedVideoBlackLevel()
        {
            return (new AsyncActionSetRedVideoBlackLevel(this));
        }

        public AsyncActionGetGreenVideoBlackLevel CreateAsyncActionGetGreenVideoBlackLevel()
        {
            return (new AsyncActionGetGreenVideoBlackLevel(this));
        }

        public AsyncActionSetGreenVideoBlackLevel CreateAsyncActionSetGreenVideoBlackLevel()
        {
            return (new AsyncActionSetGreenVideoBlackLevel(this));
        }

        public AsyncActionGetBlueVideoBlackLevel CreateAsyncActionGetBlueVideoBlackLevel()
        {
            return (new AsyncActionGetBlueVideoBlackLevel(this));
        }

        public AsyncActionSetBlueVideoBlackLevel CreateAsyncActionSetBlueVideoBlackLevel()
        {
            return (new AsyncActionSetBlueVideoBlackLevel(this));
        }

        public AsyncActionGetColorTemperature CreateAsyncActionGetColorTemperature()
        {
            return (new AsyncActionGetColorTemperature(this));
        }

        public AsyncActionSetColorTemperature CreateAsyncActionSetColorTemperature()
        {
            return (new AsyncActionSetColorTemperature(this));
        }

        public AsyncActionGetHorizontalKeystone CreateAsyncActionGetHorizontalKeystone()
        {
            return (new AsyncActionGetHorizontalKeystone(this));
        }

        public AsyncActionSetHorizontalKeystone CreateAsyncActionSetHorizontalKeystone()
        {
            return (new AsyncActionSetHorizontalKeystone(this));
        }

        public AsyncActionGetVerticalKeystone CreateAsyncActionGetVerticalKeystone()
        {
            return (new AsyncActionGetVerticalKeystone(this));
        }

        public AsyncActionSetVerticalKeystone CreateAsyncActionSetVerticalKeystone()
        {
            return (new AsyncActionSetVerticalKeystone(this));
        }

        public AsyncActionGetMute CreateAsyncActionGetMute()
        {
            return (new AsyncActionGetMute(this));
        }

        public AsyncActionSetMute CreateAsyncActionSetMute()
        {
            return (new AsyncActionSetMute(this));
        }

        public AsyncActionGetVolume CreateAsyncActionGetVolume()
        {
            return (new AsyncActionGetVolume(this));
        }

        public AsyncActionSetVolume CreateAsyncActionSetVolume()
        {
            return (new AsyncActionSetVolume(this));
        }

        public AsyncActionGetVolumeDB CreateAsyncActionGetVolumeDB()
        {
            return (new AsyncActionGetVolumeDB(this));
        }

        public AsyncActionSetVolumeDB CreateAsyncActionSetVolumeDB()
        {
            return (new AsyncActionSetVolumeDB(this));
        }

        public AsyncActionGetVolumeDBRange CreateAsyncActionGetVolumeDBRange()
        {
            return (new AsyncActionGetVolumeDBRange(this));
        }

        public AsyncActionGetLoudness CreateAsyncActionGetLoudness()
        {
            return (new AsyncActionGetLoudness(this));
        }

        public AsyncActionSetLoudness CreateAsyncActionSetLoudness()
        {
            return (new AsyncActionSetLoudness(this));
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
        
        public string ListPresetsSync(uint InstanceID)
        {
            AsyncActionListPresets action = CreateAsyncActionListPresets();
            
            object result = action.ListPresetsBeginSync(InstanceID);

            AsyncActionListPresets.EventArgsResponse response = action.ListPresetsEnd(result);
                
            return(response.CurrentPresetNameList);
        }
        
        public void SelectPresetSync(uint InstanceID, string PresetName)
        {
            AsyncActionSelectPreset action = CreateAsyncActionSelectPreset();
            
            object result = action.SelectPresetBeginSync(InstanceID, PresetName);

            action.SelectPresetEnd(result);
        }
        
        public uint GetBrightnessSync(uint InstanceID)
        {
            AsyncActionGetBrightness action = CreateAsyncActionGetBrightness();
            
            object result = action.GetBrightnessBeginSync(InstanceID);

            AsyncActionGetBrightness.EventArgsResponse response = action.GetBrightnessEnd(result);
                
            return(response.CurrentBrightness);
        }
        
        public void SetBrightnessSync(uint InstanceID, uint DesiredBrightness)
        {
            AsyncActionSetBrightness action = CreateAsyncActionSetBrightness();
            
            object result = action.SetBrightnessBeginSync(InstanceID, DesiredBrightness);

            action.SetBrightnessEnd(result);
        }
        
        public uint GetContrastSync(uint InstanceID)
        {
            AsyncActionGetContrast action = CreateAsyncActionGetContrast();
            
            object result = action.GetContrastBeginSync(InstanceID);

            AsyncActionGetContrast.EventArgsResponse response = action.GetContrastEnd(result);
                
            return(response.CurrentContrast);
        }
        
        public void SetContrastSync(uint InstanceID, uint DesiredContrast)
        {
            AsyncActionSetContrast action = CreateAsyncActionSetContrast();
            
            object result = action.SetContrastBeginSync(InstanceID, DesiredContrast);

            action.SetContrastEnd(result);
        }
        
        public uint GetSharpnessSync(uint InstanceID)
        {
            AsyncActionGetSharpness action = CreateAsyncActionGetSharpness();
            
            object result = action.GetSharpnessBeginSync(InstanceID);

            AsyncActionGetSharpness.EventArgsResponse response = action.GetSharpnessEnd(result);
                
            return(response.CurrentSharpness);
        }
        
        public void SetSharpnessSync(uint InstanceID, uint DesiredSharpness)
        {
            AsyncActionSetSharpness action = CreateAsyncActionSetSharpness();
            
            object result = action.SetSharpnessBeginSync(InstanceID, DesiredSharpness);

            action.SetSharpnessEnd(result);
        }
        
        public uint GetRedVideoGainSync(uint InstanceID)
        {
            AsyncActionGetRedVideoGain action = CreateAsyncActionGetRedVideoGain();
            
            object result = action.GetRedVideoGainBeginSync(InstanceID);

            AsyncActionGetRedVideoGain.EventArgsResponse response = action.GetRedVideoGainEnd(result);
                
            return(response.CurrentRedVideoGain);
        }
        
        public void SetRedVideoGainSync(uint InstanceID, uint DesiredRedVideoGain)
        {
            AsyncActionSetRedVideoGain action = CreateAsyncActionSetRedVideoGain();
            
            object result = action.SetRedVideoGainBeginSync(InstanceID, DesiredRedVideoGain);

            action.SetRedVideoGainEnd(result);
        }
        
        public uint GetGreenVideoGainSync(uint InstanceID)
        {
            AsyncActionGetGreenVideoGain action = CreateAsyncActionGetGreenVideoGain();
            
            object result = action.GetGreenVideoGainBeginSync(InstanceID);

            AsyncActionGetGreenVideoGain.EventArgsResponse response = action.GetGreenVideoGainEnd(result);
                
            return(response.CurrentGreenVideoGain);
        }
        
        public void SetGreenVideoGainSync(uint InstanceID, uint DesiredGreenVideoGain)
        {
            AsyncActionSetGreenVideoGain action = CreateAsyncActionSetGreenVideoGain();
            
            object result = action.SetGreenVideoGainBeginSync(InstanceID, DesiredGreenVideoGain);

            action.SetGreenVideoGainEnd(result);
        }
        
        public uint GetBlueVideoGainSync(uint InstanceID)
        {
            AsyncActionGetBlueVideoGain action = CreateAsyncActionGetBlueVideoGain();
            
            object result = action.GetBlueVideoGainBeginSync(InstanceID);

            AsyncActionGetBlueVideoGain.EventArgsResponse response = action.GetBlueVideoGainEnd(result);
                
            return(response.CurrentBlueVideoGain);
        }
        
        public void SetBlueVideoGainSync(uint InstanceID, uint DesiredBlueVideoGain)
        {
            AsyncActionSetBlueVideoGain action = CreateAsyncActionSetBlueVideoGain();
            
            object result = action.SetBlueVideoGainBeginSync(InstanceID, DesiredBlueVideoGain);

            action.SetBlueVideoGainEnd(result);
        }
        
        public uint GetRedVideoBlackLevelSync(uint InstanceID)
        {
            AsyncActionGetRedVideoBlackLevel action = CreateAsyncActionGetRedVideoBlackLevel();
            
            object result = action.GetRedVideoBlackLevelBeginSync(InstanceID);

            AsyncActionGetRedVideoBlackLevel.EventArgsResponse response = action.GetRedVideoBlackLevelEnd(result);
                
            return(response.CurrentRedVideoBlackLevel);
        }
        
        public void SetRedVideoBlackLevelSync(uint InstanceID, uint DesiredRedVideoBlackLevel)
        {
            AsyncActionSetRedVideoBlackLevel action = CreateAsyncActionSetRedVideoBlackLevel();
            
            object result = action.SetRedVideoBlackLevelBeginSync(InstanceID, DesiredRedVideoBlackLevel);

            action.SetRedVideoBlackLevelEnd(result);
        }
        
        public uint GetGreenVideoBlackLevelSync(uint InstanceID)
        {
            AsyncActionGetGreenVideoBlackLevel action = CreateAsyncActionGetGreenVideoBlackLevel();
            
            object result = action.GetGreenVideoBlackLevelBeginSync(InstanceID);

            AsyncActionGetGreenVideoBlackLevel.EventArgsResponse response = action.GetGreenVideoBlackLevelEnd(result);
                
            return(response.CurrentGreenVideoBlackLevel);
        }
        
        public void SetGreenVideoBlackLevelSync(uint InstanceID, uint DesiredGreenVideoBlackLevel)
        {
            AsyncActionSetGreenVideoBlackLevel action = CreateAsyncActionSetGreenVideoBlackLevel();
            
            object result = action.SetGreenVideoBlackLevelBeginSync(InstanceID, DesiredGreenVideoBlackLevel);

            action.SetGreenVideoBlackLevelEnd(result);
        }
        
        public uint GetBlueVideoBlackLevelSync(uint InstanceID)
        {
            AsyncActionGetBlueVideoBlackLevel action = CreateAsyncActionGetBlueVideoBlackLevel();
            
            object result = action.GetBlueVideoBlackLevelBeginSync(InstanceID);

            AsyncActionGetBlueVideoBlackLevel.EventArgsResponse response = action.GetBlueVideoBlackLevelEnd(result);
                
            return(response.CurrentBlueVideoBlackLevel);
        }
        
        public void SetBlueVideoBlackLevelSync(uint InstanceID, uint DesiredBlueVideoBlackLevel)
        {
            AsyncActionSetBlueVideoBlackLevel action = CreateAsyncActionSetBlueVideoBlackLevel();
            
            object result = action.SetBlueVideoBlackLevelBeginSync(InstanceID, DesiredBlueVideoBlackLevel);

            action.SetBlueVideoBlackLevelEnd(result);
        }
        
        public uint GetColorTemperatureSync(uint InstanceID)
        {
            AsyncActionGetColorTemperature action = CreateAsyncActionGetColorTemperature();
            
            object result = action.GetColorTemperatureBeginSync(InstanceID);

            AsyncActionGetColorTemperature.EventArgsResponse response = action.GetColorTemperatureEnd(result);
                
            return(response.CurrentColorTemperature);
        }
        
        public void SetColorTemperatureSync(uint InstanceID, uint DesiredColorTemperature)
        {
            AsyncActionSetColorTemperature action = CreateAsyncActionSetColorTemperature();
            
            object result = action.SetColorTemperatureBeginSync(InstanceID, DesiredColorTemperature);

            action.SetColorTemperatureEnd(result);
        }
        
        public int GetHorizontalKeystoneSync(uint InstanceID)
        {
            AsyncActionGetHorizontalKeystone action = CreateAsyncActionGetHorizontalKeystone();
            
            object result = action.GetHorizontalKeystoneBeginSync(InstanceID);

            AsyncActionGetHorizontalKeystone.EventArgsResponse response = action.GetHorizontalKeystoneEnd(result);
                
            return(response.CurrentHorizontalKeystone);
        }
        
        public void SetHorizontalKeystoneSync(uint InstanceID, int DesiredHorizontalKeystone)
        {
            AsyncActionSetHorizontalKeystone action = CreateAsyncActionSetHorizontalKeystone();
            
            object result = action.SetHorizontalKeystoneBeginSync(InstanceID, DesiredHorizontalKeystone);

            action.SetHorizontalKeystoneEnd(result);
        }
        
        public int GetVerticalKeystoneSync(uint InstanceID)
        {
            AsyncActionGetVerticalKeystone action = CreateAsyncActionGetVerticalKeystone();
            
            object result = action.GetVerticalKeystoneBeginSync(InstanceID);

            AsyncActionGetVerticalKeystone.EventArgsResponse response = action.GetVerticalKeystoneEnd(result);
                
            return(response.CurrentVerticalKeystone);
        }
        
        public void SetVerticalKeystoneSync(uint InstanceID, int DesiredVerticalKeystone)
        {
            AsyncActionSetVerticalKeystone action = CreateAsyncActionSetVerticalKeystone();
            
            object result = action.SetVerticalKeystoneBeginSync(InstanceID, DesiredVerticalKeystone);

            action.SetVerticalKeystoneEnd(result);
        }
        
        public bool GetMuteSync(uint InstanceID, string Channel)
        {
            AsyncActionGetMute action = CreateAsyncActionGetMute();
            
            object result = action.GetMuteBeginSync(InstanceID, Channel);

            AsyncActionGetMute.EventArgsResponse response = action.GetMuteEnd(result);
                
            return(response.CurrentMute);
        }
        
        public void SetMuteSync(uint InstanceID, string Channel, bool DesiredMute)
        {
            AsyncActionSetMute action = CreateAsyncActionSetMute();
            
            object result = action.SetMuteBeginSync(InstanceID, Channel, DesiredMute);

            action.SetMuteEnd(result);
        }
        
        public uint GetVolumeSync(uint InstanceID, string Channel)
        {
            AsyncActionGetVolume action = CreateAsyncActionGetVolume();
            
            object result = action.GetVolumeBeginSync(InstanceID, Channel);

            AsyncActionGetVolume.EventArgsResponse response = action.GetVolumeEnd(result);
                
            return(response.CurrentVolume);
        }
        
        public void SetVolumeSync(uint InstanceID, string Channel, uint DesiredVolume)
        {
            AsyncActionSetVolume action = CreateAsyncActionSetVolume();
            
            object result = action.SetVolumeBeginSync(InstanceID, Channel, DesiredVolume);

            action.SetVolumeEnd(result);
        }
        
        public int GetVolumeDBSync(uint InstanceID, string Channel)
        {
            AsyncActionGetVolumeDB action = CreateAsyncActionGetVolumeDB();
            
            object result = action.GetVolumeDBBeginSync(InstanceID, Channel);

            AsyncActionGetVolumeDB.EventArgsResponse response = action.GetVolumeDBEnd(result);
                
            return(response.CurrentVolume);
        }
        
        public void SetVolumeDBSync(uint InstanceID, string Channel, int DesiredVolume)
        {
            AsyncActionSetVolumeDB action = CreateAsyncActionSetVolumeDB();
            
            object result = action.SetVolumeDBBeginSync(InstanceID, Channel, DesiredVolume);

            action.SetVolumeDBEnd(result);
        }
        
        public void GetVolumeDBRangeSync(uint InstanceID, string Channel, out int MinValue, out int MaxValue)
        {
            AsyncActionGetVolumeDBRange action = CreateAsyncActionGetVolumeDBRange();
            
            object result = action.GetVolumeDBRangeBeginSync(InstanceID, Channel);

            AsyncActionGetVolumeDBRange.EventArgsResponse response = action.GetVolumeDBRangeEnd(result);
                
            MinValue = response.MinValue;
            MaxValue = response.MaxValue;
        }
        
        public bool GetLoudnessSync(uint InstanceID, string Channel)
        {
            AsyncActionGetLoudness action = CreateAsyncActionGetLoudness();
            
            object result = action.GetLoudnessBeginSync(InstanceID, Channel);

            AsyncActionGetLoudness.EventArgsResponse response = action.GetLoudnessEnd(result);
                
            return(response.CurrentLoudness);
        }
        
        public void SetLoudnessSync(uint InstanceID, string Channel, bool DesiredLoudness)
        {
            AsyncActionSetLoudness action = CreateAsyncActionSetLoudness();
            
            object result = action.SetLoudnessBeginSync(InstanceID, Channel, DesiredLoudness);

            action.SetLoudnessEnd(result);
        }
        
        public void GetStateVariablesSync(uint InstanceID, string StateVariableList, string StateVariableValuePairs)
        {
            AsyncActionGetStateVariables action = CreateAsyncActionGetStateVariables();
            
            object result = action.GetStateVariablesBeginSync(InstanceID, StateVariableList, StateVariableValuePairs);

            action.GetStateVariablesEnd(result);
        }
        
        public string SetStateVariablesSync(uint InstanceID, string RenderingControlUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
        {
            AsyncActionSetStateVariables action = CreateAsyncActionSetStateVariables();
            
            object result = action.SetStateVariablesBeginSync(InstanceID, RenderingControlUDN, ServiceType, ServiceId, StateVariableValuePairs);

            AsyncActionSetStateVariables.EventArgsResponse response = action.SetStateVariablesEnd(result);
                
            return(response.StateVariableList);
        }
        

        // AsyncActionListPresets

        public class AsyncActionListPresets
        {
            internal AsyncActionListPresets(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(0));
                iService = aService;
            }

            internal object ListPresetsBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void ListPresetsBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionListPresets.ListPresetsBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse ListPresetsEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionListPresets.ListPresetsEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionListPresets.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentPresetNameList = aHandler.ReadArgumentString("CurrentPresetNameList");
                }
                
                public string CurrentPresetNameList;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSelectPreset

        public class AsyncActionSelectPreset
        {
            internal AsyncActionSelectPreset(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(1));
                iService = aService;
            }

            internal object SelectPresetBeginSync(uint InstanceID, string PresetName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("PresetName", PresetName);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SelectPresetBegin(uint InstanceID, string PresetName)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("PresetName", PresetName);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSelectPreset.SelectPresetBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SelectPresetEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSelectPreset.SelectPresetEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSelectPreset.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetBrightness

        public class AsyncActionGetBrightness
        {
            internal AsyncActionGetBrightness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(2));
                iService = aService;
            }

            internal object GetBrightnessBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetBrightnessBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBrightness.GetBrightnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetBrightnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBrightness.GetBrightnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBrightness.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentBrightness = aHandler.ReadArgumentUint("CurrentBrightness");
                }
                
                public uint CurrentBrightness;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetBrightness

        public class AsyncActionSetBrightness
        {
            internal AsyncActionSetBrightness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(3));
                iService = aService;
            }

            internal object SetBrightnessBeginSync(uint InstanceID, uint DesiredBrightness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredBrightness", DesiredBrightness);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBrightnessBegin(uint InstanceID, uint DesiredBrightness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredBrightness", DesiredBrightness);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBrightness.SetBrightnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetBrightnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBrightness.SetBrightnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBrightness.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetContrast

        public class AsyncActionGetContrast
        {
            internal AsyncActionGetContrast(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(4));
                iService = aService;
            }

            internal object GetContrastBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetContrastBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetContrast.GetContrastBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetContrastEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetContrast.GetContrastEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetContrast.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentContrast = aHandler.ReadArgumentUint("CurrentContrast");
                }
                
                public uint CurrentContrast;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetContrast

        public class AsyncActionSetContrast
        {
            internal AsyncActionSetContrast(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(5));
                iService = aService;
            }

            internal object SetContrastBeginSync(uint InstanceID, uint DesiredContrast)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredContrast", DesiredContrast);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetContrastBegin(uint InstanceID, uint DesiredContrast)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredContrast", DesiredContrast);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetContrast.SetContrastBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetContrastEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetContrast.SetContrastEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetContrast.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetSharpness

        public class AsyncActionGetSharpness
        {
            internal AsyncActionGetSharpness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(6));
                iService = aService;
            }

            internal object GetSharpnessBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetSharpnessBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetSharpness.GetSharpnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetSharpnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetSharpness.GetSharpnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetSharpness.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentSharpness = aHandler.ReadArgumentUint("CurrentSharpness");
                }
                
                public uint CurrentSharpness;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetSharpness

        public class AsyncActionSetSharpness
        {
            internal AsyncActionSetSharpness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(7));
                iService = aService;
            }

            internal object SetSharpnessBeginSync(uint InstanceID, uint DesiredSharpness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredSharpness", DesiredSharpness);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetSharpnessBegin(uint InstanceID, uint DesiredSharpness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredSharpness", DesiredSharpness);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetSharpness.SetSharpnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetSharpnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetSharpness.SetSharpnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetSharpness.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetRedVideoGain

        public class AsyncActionGetRedVideoGain
        {
            internal AsyncActionGetRedVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(8));
                iService = aService;
            }

            internal object GetRedVideoGainBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetRedVideoGainBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoGain.GetRedVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetRedVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoGain.GetRedVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentRedVideoGain = aHandler.ReadArgumentUint("CurrentRedVideoGain");
                }
                
                public uint CurrentRedVideoGain;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetRedVideoGain

        public class AsyncActionSetRedVideoGain
        {
            internal AsyncActionSetRedVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(9));
                iService = aService;
            }

            internal object SetRedVideoGainBeginSync(uint InstanceID, uint DesiredRedVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredRedVideoGain", DesiredRedVideoGain);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRedVideoGainBegin(uint InstanceID, uint DesiredRedVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredRedVideoGain", DesiredRedVideoGain);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoGain.SetRedVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRedVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoGain.SetRedVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetGreenVideoGain

        public class AsyncActionGetGreenVideoGain
        {
            internal AsyncActionGetGreenVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(10));
                iService = aService;
            }

            internal object GetGreenVideoGainBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetGreenVideoGainBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoGain.GetGreenVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetGreenVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoGain.GetGreenVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentGreenVideoGain = aHandler.ReadArgumentUint("CurrentGreenVideoGain");
                }
                
                public uint CurrentGreenVideoGain;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetGreenVideoGain

        public class AsyncActionSetGreenVideoGain
        {
            internal AsyncActionSetGreenVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(11));
                iService = aService;
            }

            internal object SetGreenVideoGainBeginSync(uint InstanceID, uint DesiredGreenVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredGreenVideoGain", DesiredGreenVideoGain);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetGreenVideoGainBegin(uint InstanceID, uint DesiredGreenVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredGreenVideoGain", DesiredGreenVideoGain);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoGain.SetGreenVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetGreenVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoGain.SetGreenVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetBlueVideoGain

        public class AsyncActionGetBlueVideoGain
        {
            internal AsyncActionGetBlueVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(12));
                iService = aService;
            }

            internal object GetBlueVideoGainBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetBlueVideoGainBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoGain.GetBlueVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetBlueVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoGain.GetBlueVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentBlueVideoGain = aHandler.ReadArgumentUint("CurrentBlueVideoGain");
                }
                
                public uint CurrentBlueVideoGain;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetBlueVideoGain

        public class AsyncActionSetBlueVideoGain
        {
            internal AsyncActionSetBlueVideoGain(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(13));
                iService = aService;
            }

            internal object SetBlueVideoGainBeginSync(uint InstanceID, uint DesiredBlueVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredBlueVideoGain", DesiredBlueVideoGain);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBlueVideoGainBegin(uint InstanceID, uint DesiredBlueVideoGain)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredBlueVideoGain", DesiredBlueVideoGain);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoGain.SetBlueVideoGainBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetBlueVideoGainEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoGain.SetBlueVideoGainEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoGain.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetRedVideoBlackLevel

        public class AsyncActionGetRedVideoBlackLevel
        {
            internal AsyncActionGetRedVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(14));
                iService = aService;
            }

            internal object GetRedVideoBlackLevelBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetRedVideoBlackLevelBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoBlackLevel.GetRedVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetRedVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoBlackLevel.GetRedVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetRedVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentRedVideoBlackLevel = aHandler.ReadArgumentUint("CurrentRedVideoBlackLevel");
                }
                
                public uint CurrentRedVideoBlackLevel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetRedVideoBlackLevel

        public class AsyncActionSetRedVideoBlackLevel
        {
            internal AsyncActionSetRedVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(15));
                iService = aService;
            }

            internal object SetRedVideoBlackLevelBeginSync(uint InstanceID, uint DesiredRedVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredRedVideoBlackLevel", DesiredRedVideoBlackLevel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetRedVideoBlackLevelBegin(uint InstanceID, uint DesiredRedVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredRedVideoBlackLevel", DesiredRedVideoBlackLevel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoBlackLevel.SetRedVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetRedVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoBlackLevel.SetRedVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetRedVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetGreenVideoBlackLevel

        public class AsyncActionGetGreenVideoBlackLevel
        {
            internal AsyncActionGetGreenVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(16));
                iService = aService;
            }

            internal object GetGreenVideoBlackLevelBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetGreenVideoBlackLevelBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoBlackLevel.GetGreenVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetGreenVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoBlackLevel.GetGreenVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetGreenVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentGreenVideoBlackLevel = aHandler.ReadArgumentUint("CurrentGreenVideoBlackLevel");
                }
                
                public uint CurrentGreenVideoBlackLevel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetGreenVideoBlackLevel

        public class AsyncActionSetGreenVideoBlackLevel
        {
            internal AsyncActionSetGreenVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(17));
                iService = aService;
            }

            internal object SetGreenVideoBlackLevelBeginSync(uint InstanceID, uint DesiredGreenVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredGreenVideoBlackLevel", DesiredGreenVideoBlackLevel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetGreenVideoBlackLevelBegin(uint InstanceID, uint DesiredGreenVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredGreenVideoBlackLevel", DesiredGreenVideoBlackLevel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoBlackLevel.SetGreenVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetGreenVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoBlackLevel.SetGreenVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetGreenVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetBlueVideoBlackLevel

        public class AsyncActionGetBlueVideoBlackLevel
        {
            internal AsyncActionGetBlueVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(18));
                iService = aService;
            }

            internal object GetBlueVideoBlackLevelBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetBlueVideoBlackLevelBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoBlackLevel.GetBlueVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetBlueVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoBlackLevel.GetBlueVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetBlueVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentBlueVideoBlackLevel = aHandler.ReadArgumentUint("CurrentBlueVideoBlackLevel");
                }
                
                public uint CurrentBlueVideoBlackLevel;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetBlueVideoBlackLevel

        public class AsyncActionSetBlueVideoBlackLevel
        {
            internal AsyncActionSetBlueVideoBlackLevel(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(19));
                iService = aService;
            }

            internal object SetBlueVideoBlackLevelBeginSync(uint InstanceID, uint DesiredBlueVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredBlueVideoBlackLevel", DesiredBlueVideoBlackLevel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetBlueVideoBlackLevelBegin(uint InstanceID, uint DesiredBlueVideoBlackLevel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredBlueVideoBlackLevel", DesiredBlueVideoBlackLevel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoBlackLevel.SetBlueVideoBlackLevelBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetBlueVideoBlackLevelEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoBlackLevel.SetBlueVideoBlackLevelEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetBlueVideoBlackLevel.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetColorTemperature

        public class AsyncActionGetColorTemperature
        {
            internal AsyncActionGetColorTemperature(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(20));
                iService = aService;
            }

            internal object GetColorTemperatureBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetColorTemperatureBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetColorTemperature.GetColorTemperatureBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetColorTemperatureEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetColorTemperature.GetColorTemperatureEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetColorTemperature.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentColorTemperature = aHandler.ReadArgumentUint("CurrentColorTemperature");
                }
                
                public uint CurrentColorTemperature;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetColorTemperature

        public class AsyncActionSetColorTemperature
        {
            internal AsyncActionSetColorTemperature(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(21));
                iService = aService;
            }

            internal object SetColorTemperatureBeginSync(uint InstanceID, uint DesiredColorTemperature)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentUint("DesiredColorTemperature", DesiredColorTemperature);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetColorTemperatureBegin(uint InstanceID, uint DesiredColorTemperature)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentUint("DesiredColorTemperature", DesiredColorTemperature);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetColorTemperature.SetColorTemperatureBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetColorTemperatureEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetColorTemperature.SetColorTemperatureEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetColorTemperature.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetHorizontalKeystone

        public class AsyncActionGetHorizontalKeystone
        {
            internal AsyncActionGetHorizontalKeystone(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(22));
                iService = aService;
            }

            internal object GetHorizontalKeystoneBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetHorizontalKeystoneBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetHorizontalKeystone.GetHorizontalKeystoneBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetHorizontalKeystoneEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetHorizontalKeystone.GetHorizontalKeystoneEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetHorizontalKeystone.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentHorizontalKeystone = aHandler.ReadArgumentInt("CurrentHorizontalKeystone");
                }
                
                public int CurrentHorizontalKeystone;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetHorizontalKeystone

        public class AsyncActionSetHorizontalKeystone
        {
            internal AsyncActionSetHorizontalKeystone(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(23));
                iService = aService;
            }

            internal object SetHorizontalKeystoneBeginSync(uint InstanceID, int DesiredHorizontalKeystone)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentInt("DesiredHorizontalKeystone", DesiredHorizontalKeystone);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetHorizontalKeystoneBegin(uint InstanceID, int DesiredHorizontalKeystone)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentInt("DesiredHorizontalKeystone", DesiredHorizontalKeystone);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetHorizontalKeystone.SetHorizontalKeystoneBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetHorizontalKeystoneEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetHorizontalKeystone.SetHorizontalKeystoneEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetHorizontalKeystone.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetVerticalKeystone

        public class AsyncActionGetVerticalKeystone
        {
            internal AsyncActionGetVerticalKeystone(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(24));
                iService = aService;
            }

            internal object GetVerticalKeystoneBeginSync(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetVerticalKeystoneBegin(uint InstanceID)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVerticalKeystone.GetVerticalKeystoneBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetVerticalKeystoneEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVerticalKeystone.GetVerticalKeystoneEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVerticalKeystone.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentVerticalKeystone = aHandler.ReadArgumentInt("CurrentVerticalKeystone");
                }
                
                public int CurrentVerticalKeystone;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetVerticalKeystone

        public class AsyncActionSetVerticalKeystone
        {
            internal AsyncActionSetVerticalKeystone(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(25));
                iService = aService;
            }

            internal object SetVerticalKeystoneBeginSync(uint InstanceID, int DesiredVerticalKeystone)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentInt("DesiredVerticalKeystone", DesiredVerticalKeystone);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetVerticalKeystoneBegin(uint InstanceID, int DesiredVerticalKeystone)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentInt("DesiredVerticalKeystone", DesiredVerticalKeystone);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVerticalKeystone.SetVerticalKeystoneBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetVerticalKeystoneEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVerticalKeystone.SetVerticalKeystoneEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVerticalKeystone.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetMute

        public class AsyncActionGetMute
        {
            internal AsyncActionGetMute(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(26));
                iService = aService;
            }

            internal object GetMuteBeginSync(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetMuteBegin(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetMute.GetMuteBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetMuteEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetMute.GetMuteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetMute.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentMute = aHandler.ReadArgumentBool("CurrentMute");
                }
                
                public bool CurrentMute;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetMute

        public class AsyncActionSetMute
        {
            internal AsyncActionSetMute(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(27));
                iService = aService;
            }

            internal object SetMuteBeginSync(uint InstanceID, string Channel, bool DesiredMute)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                iHandler.WriteArgumentBool("DesiredMute", DesiredMute);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetMuteBegin(uint InstanceID, string Channel, bool DesiredMute)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                iHandler.WriteArgumentBool("DesiredMute", DesiredMute);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetMute.SetMuteBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetMute.SetMuteEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetMute.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetVolume

        public class AsyncActionGetVolume
        {
            internal AsyncActionGetVolume(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(28));
                iService = aService;
            }

            internal object GetVolumeBeginSync(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetVolumeBegin(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolume.GetVolumeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetVolumeEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolume.GetVolumeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolume.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentVolume = aHandler.ReadArgumentUint("CurrentVolume");
                }
                
                public uint CurrentVolume;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetVolume

        public class AsyncActionSetVolume
        {
            internal AsyncActionSetVolume(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(29));
                iService = aService;
            }

            internal object SetVolumeBeginSync(uint InstanceID, string Channel, uint DesiredVolume)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                iHandler.WriteArgumentUint("DesiredVolume", DesiredVolume);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetVolumeBegin(uint InstanceID, string Channel, uint DesiredVolume)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                iHandler.WriteArgumentUint("DesiredVolume", DesiredVolume);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolume.SetVolumeBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolume.SetVolumeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolume.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetVolumeDB

        public class AsyncActionGetVolumeDB
        {
            internal AsyncActionGetVolumeDB(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(30));
                iService = aService;
            }

            internal object GetVolumeDBBeginSync(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetVolumeDBBegin(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDB.GetVolumeDBBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetVolumeDBEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDB.GetVolumeDBEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDB.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentVolume = aHandler.ReadArgumentInt("CurrentVolume");
                }
                
                public int CurrentVolume;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetVolumeDB

        public class AsyncActionSetVolumeDB
        {
            internal AsyncActionSetVolumeDB(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(31));
                iService = aService;
            }

            internal object SetVolumeDBBeginSync(uint InstanceID, string Channel, int DesiredVolume)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                iHandler.WriteArgumentInt("DesiredVolume", DesiredVolume);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetVolumeDBBegin(uint InstanceID, string Channel, int DesiredVolume)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                iHandler.WriteArgumentInt("DesiredVolume", DesiredVolume);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolumeDB.SetVolumeDBBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetVolumeDBEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolumeDB.SetVolumeDBEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetVolumeDB.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetVolumeDBRange

        public class AsyncActionGetVolumeDBRange
        {
            internal AsyncActionGetVolumeDBRange(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(32));
                iService = aService;
            }

            internal object GetVolumeDBRangeBeginSync(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetVolumeDBRangeBegin(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDBRange.GetVolumeDBRangeBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetVolumeDBRangeEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDBRange.GetVolumeDBRangeEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetVolumeDBRange.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    MinValue = aHandler.ReadArgumentInt("MinValue");
                    MaxValue = aHandler.ReadArgumentInt("MaxValue");
                }
                
                public int MinValue;
                public int MaxValue;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetLoudness

        public class AsyncActionGetLoudness
        {
            internal AsyncActionGetLoudness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(33));
                iService = aService;
            }

            internal object GetLoudnessBeginSync(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetLoudnessBegin(uint InstanceID, string Channel)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetLoudness.GetLoudnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse GetLoudnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetLoudness.GetLoudnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetLoudness.Callback(" + iService.ControlUri + "): " + e.Message);
                }
            }

            public class EventArgsResponse : EventArgs
            {
                internal EventArgsResponse(IInvokeHandler aHandler)
                {
                    CurrentLoudness = aHandler.ReadArgumentBool("CurrentLoudness");
                }
                
                public bool CurrentLoudness;
            }

            public event EventHandler<EventArgsResponse> EventResponse;
            public event EventHandler<EventArgsError> EventError;

            private IInvokeHandler iHandler;
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetLoudness

        public class AsyncActionSetLoudness
        {
            internal AsyncActionSetLoudness(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(34));
                iService = aService;
            }

            internal object SetLoudnessBeginSync(uint InstanceID, string Channel, bool DesiredLoudness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("Channel", Channel);           
                iHandler.WriteArgumentBool("DesiredLoudness", DesiredLoudness);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetLoudnessBegin(uint InstanceID, string Channel, bool DesiredLoudness)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("Channel", Channel);                
                iHandler.WriteArgumentBool("DesiredLoudness", DesiredLoudness);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetLoudness.SetLoudnessBegin(" + iService.ControlUri + "): " + e);
                }
            }

            internal EventArgsResponse SetLoudnessEnd(object aResult)
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetLoudness.SetLoudnessEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetLoudness.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionGetStateVariables

        public class AsyncActionGetStateVariables
        {
            internal AsyncActionGetStateVariables(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(35));
                iService = aService;
            }

            internal object GetStateVariablesBeginSync(uint InstanceID, string StateVariableList, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("StateVariableList", StateVariableList);           
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);           
                
                return (iHandler.WriteEnd(null));
            }

            public void GetStateVariablesBegin(uint InstanceID, string StateVariableList, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("StateVariableList", StateVariableList);                
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionGetStateVariables.GetStateVariablesBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetStateVariables.GetStateVariablesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionGetStateVariables.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
        }
        
        
        // AsyncActionSetStateVariables

        public class AsyncActionSetStateVariables
        {
            internal AsyncActionSetStateVariables(ServiceRenderingControl aService)
            {
                iHandler = aService.Protocol.CreateInvokeHandler(aService, aService.GetActionAt(36));
                iService = aService;
            }

            internal object SetStateVariablesBeginSync(uint InstanceID, string RenderingControlUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);           
                iHandler.WriteArgumentString("RenderingControlUDN", RenderingControlUDN);           
                iHandler.WriteArgumentString("ServiceType", ServiceType);           
                iHandler.WriteArgumentString("ServiceId", ServiceId);           
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);           
                
                return (iHandler.WriteEnd(null));
            }

            public void SetStateVariablesBegin(uint InstanceID, string RenderingControlUDN, string ServiceType, string ServiceId, string StateVariableValuePairs)
            {
                iHandler.WriteBegin();
                
                iHandler.WriteArgumentUint("InstanceID", InstanceID);                
                iHandler.WriteArgumentString("RenderingControlUDN", RenderingControlUDN);                
                iHandler.WriteArgumentString("ServiceType", ServiceType);                
                iHandler.WriteArgumentString("ServiceId", ServiceId);                
                iHandler.WriteArgumentString("StateVariableValuePairs", StateVariableValuePairs);                
                
                try
                {
                    iHandler.WriteEnd(this.Callback);
                }
                catch(Exception e)
                {
                    UserLog.WriteLine("RenderingControl.AsyncActionSetStateVariables.SetStateVariablesBegin(" + iService.ControlUri + "): " + e);
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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetStateVariables.SetStateVariablesEnd(" + iService.ControlUri + "): " + e.Message);

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
                    UserLog.WriteLine("RenderingControl.AsyncActionSetStateVariables.Callback(" + iService.ControlUri + "): " + e.Message);
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
            private ServiceRenderingControl iService;
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
				    UserLog.WriteLine("EventServerEvent(ServiceRenderingControl): " + SubscriptionId + " Out of sequence event received. Expected " + iExpectedSequenceNumber + " got " + e.SequenceNo);

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

        public string LastChange;
    }
}
    
