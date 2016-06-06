using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Java.Net;
using Android.Runtime;
using Android.Content;
using Android.Net.Wifi;

namespace Linn
{
    public class NetworkInfo
    {
        public static bool GetIsNetworkAvailable()
        {
	        return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private static NetworkInfoModel sWifiInterface = null;
        public static void RefreshWifiInfo(Context aContext)
        {
            WifiManager wifi = (WifiManager)aContext.GetSystemService(Context.WifiService);
            NetworkInfoModel newModel = null;
            if (wifi != null && wifi.IsWifiEnabled)
            {
                Android.Net.Wifi.WifiInfo networkInfo = wifi.ConnectionInfo;// as Android.Net.Wifi.WifiInfo;
                newModel = new NetworkInfoModel(
                networkInfo.SSID,
                networkInfo.MacAddress,
                ENetworkInterfaceType.eEthernet,
                new System.Net.IPAddress(networkInfo.IpAddress),
                null,
                ENetworkInterfaceComponent.eIPv4,
                string.Empty,
                wifi.WifiState == Android.Net.WifiState.Enabled ? EOperationalStatus.eUp : EOperationalStatus.eDown,
                    //networkInfo.SupplicantState == Android.Net.Wifi.SupplicantState.Completed ? EOperationalStatus.eUp : EOperationalStatus.eDown,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                new List<System.Net.IPAddress>()
                );
                networkInfo.Dispose();
                wifi.Dispose();
                UserLog.WriteLine("RefreshWifiInfo(): " + newModel.Name + ", " + newModel.IPAddress + ", " + newModel.OperationalStatus);
            }
            else
            {
                UserLog.WriteLine("RefreshWifiInfo(): No Network");
            }
            sWifiInterface = newModel;
        }

        public static List<NetworkInfoModel> GetAllNetworkInterfaces()
        {
            List<NetworkInfoModel> adaptors = new List<NetworkInfoModel>();
            if (sWifiInterface != null)
            {
                adaptors.Add(sWifiInterface);
            }

            return adaptors;
        }
    }
}
