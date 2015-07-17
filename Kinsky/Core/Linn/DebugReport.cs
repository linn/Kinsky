using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace Linn
{
    public class DebugInformation
    {
        static public string SystemDetails()
        {
            string info = "System Details" + Environment.NewLine;
            try
            {

                List<NetworkInfoModel> aNics = NetworkInfo.GetAllNetworkInterfaces();

                // os details
                info += Environment.NewLine + "OS Info: " + Linn.SystemInfo.VersionString + Environment.NewLine;
                info += "OS Platform: " + Environment.OSVersion.Platform.ToString() + Environment.NewLine;
                info += "OS Version: " + Environment.OSVersion.Version.ToString() + Environment.NewLine;
                info += "OS Service Pack: " + Linn.SystemInfo.ServicePack + Environment.NewLine;

                // system details
                info += Environment.NewLine + "Runtime language version: " + Environment.Version.ToString() + Environment.NewLine;
                info += "Computer Name: " + Linn.SystemInfo.ComputerName + Environment.NewLine;

                // ssdp service
                info += Environment.NewLine + "SSDP Service Name: " + Linn.ServiceInfo.DisplayName + Environment.NewLine;
                info += "SSDP Service State: " + Linn.ServiceInfo.Status.ToString() + Environment.NewLine;

                // network setup
                info += Environment.NewLine + "Network Setup:" + Environment.NewLine + NetworkInterfaceCards(aNics) + Environment.NewLine;
            }
            catch (Exception e)
            {
                info += "Error gathering system details: " + e.Message + Environment.NewLine;
            }
            return info;
        }

        static private string NetworkInterfaceCards(List<NetworkInfoModel> aNics)
        {
            string info = "";
            uint count = 0;
            foreach (NetworkInfoModel adapter in aNics)
            {
                if (adapter.NetworkInterfaceType == ENetworkInterfaceType.eLoopBack)
                {
                    continue; // ignore loopback adapters.
                }
                count++;
                info += Environment.NewLine;
                info += "Adapter " + count + ": " + adapter.Description + Environment.NewLine;
                info += String.Empty.PadLeft(adapter.Description.Length, '=') + Environment.NewLine;
                info += "  IP Address .............................. : " + adapter.IPAddress + Environment.NewLine;
                info += "  Interface type .......................... : " + adapter.NetworkInterfaceType.ToString("F") + Environment.NewLine;
                info += "  Physical Address ........................ : " + adapter.MacAddress + Environment.NewLine;
                info += "  Operational status ...................... : " + adapter.OperationalStatus + Environment.NewLine;

                string versions = adapter.NetworkInterfaceComponent.ToString("F"); // Create a display string for the supported IP versions.

                info += "  IP version .............................. : " + versions + Environment.NewLine;
                foreach (IPAddress gateway in adapter.GatewayIPAddressInformation)
                {
                    info += "  Default Gateway ......................... : " + gateway.ToString() + Environment.NewLine;
                }

                //info += IPAddresses(properties);

                info += "  DNS suffix .............................. : " + GetNullValue(adapter.DnsSuffix) + Environment.NewLine;


                if (adapter.NetworkInterfaceComponent == ENetworkInterfaceComponent.eIPv4)
                {
                    info += "  DHCP Enabled............................. : " + GetNullValue(adapter.IsDhcpEnabled) + Environment.NewLine;
                    info += "  Auto Configuartion Enabled............... : " + GetNullValue(adapter.IsAutomaticPrivateAddressingEnabled) + Environment.NewLine;
                    info += "  MTU...................................... : " + GetNullValue(adapter.Mtu) + Environment.NewLine;
                }

                info += "  DNS enabled ............................. : " + GetNullValue(adapter.IsDnsEnabled) + Environment.NewLine;
                info += "  Dynamically configured DNS .............. : " + GetNullValue(adapter.IsDynamicDnsEnabled) + Environment.NewLine;
                info += "  Receive Only ............................ : " + GetNullValue(adapter.IsReceiveOnly) + Environment.NewLine;
                info += "  Multicast ............................... : " + GetNullValue(adapter.SupportsMulticast) + Environment.NewLine;

            }
            return info;
        }

        static private string GetNullValue(string aValue)
        {
            return (aValue == null)
                ? "Unknown"
                : aValue.ToString();
        }

        static private string GetNullValue(bool? aValue)
        {
            return (aValue == null)
                ? "Unknown"
                : aValue.ToString();
        }

        static private string GetNullValue(int? aValue)
        {
            return (aValue == null)
                ? "Unknown"
                : aValue.ToString();
        }
    }

    public class DebugReport
    {
        public const string kFormHadlerAddress = "http://products.linn.co.uk/cgi-bin/form.cgi";
        public const string kLinnEmailAddress = "support@products.linn.co.uk";

        public DebugReport(string aComment)
            : this("", "", "", aComment)
        {
        }

        public DebugReport(string aName, string aEmail, string aPhone, string aComment)
        {
            iName = aName;
            iEmail = aEmail;
            iPhone = aPhone;
            iComment = aComment;
        }

        public bool Post(string aProductId, string aData)
        {
            string ignoreResponse = "";
            return SendData(aProductId, aData, null, "", out ignoreResponse);
        }

        public bool Post(string aProductId, string aData, out string aSubmitResponse)
        {
            return SendData(aProductId, aData, null, "", out aSubmitResponse);
        }

        public bool Post(string aProductId, string aData, byte[] aCrashData, string aCrashDataCheckSum)
        {
            string ignoreResponse = "";
            return SendData(aProductId, aData, aCrashData, aCrashDataCheckSum, out ignoreResponse);
        }

        public bool Post(string aProductId, string aData, byte[] aCrashData, string aCrashDataCheckSum, out string aSubmitResponse)
        {
            return SendData(aProductId, aData, aCrashData, aCrashDataCheckSum, out aSubmitResponse);
        }

        private bool SendData(string aProductId, string aData, byte[] aCrashData, string aCrashDataCheckSum, out string aSubmitResponse)
        {
            StreamReader reader = null;
            Stream dataStream = null;
            WebResponse response = null;
            bool submitFailed = false;
            aSubmitResponse = "";
            try
            {
                // Create a request using a URL that can receive a post. 
                string boundary = Guid.NewGuid().ToString();

                Uri uri = new Uri(kFormHadlerAddress);
  
                WebRequest request = WebRequest.Create(uri);
                HttpWebRequest webRequest = (HttpWebRequest)request;
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.AllowWriteStreamBuffering = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;

                // create POST data packet
                string header = "--" + boundary;
                string footer = header + "--";
                string postData = null;
                // product id
                postData += header + Environment.NewLine;
                postData += "Content-Disposition: form-data; name=\"prodId\"" + Environment.NewLine + Environment.NewLine;
                postData += aProductId + Environment.NewLine;
                // name
                if (iName != "")
                {
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"name\"" + Environment.NewLine + Environment.NewLine;
                    postData += iName + Environment.NewLine;
                }
                // email
                if (iEmail != "")
                {
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"email\"" + Environment.NewLine + Environment.NewLine;
                    postData += iEmail + Environment.NewLine;
                }
                // phone
                if (iPhone != "")
                {
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"phone\"" + Environment.NewLine + Environment.NewLine;
                    postData += iPhone + Environment.NewLine;
                }
                // comment
                if (iComment != "")
                {
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"comment\"" + Environment.NewLine + Environment.NewLine;
                    postData += iComment + Environment.NewLine;
                }
                // data
                if (aData != "")
                {
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"data\"" + Environment.NewLine + Environment.NewLine;
                    postData += aData + Environment.NewLine;
                }
                // crashData
                byte[] byteArray = null;
                iByteList.Clear();
                if (aCrashData != null)
                {
                    // crashData (checksum)
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"crashDataCheckSum\"" + Environment.NewLine + Environment.NewLine;
                    postData += aCrashDataCheckSum + Environment.NewLine;
                    // crashData (file)
                    postData += header + Environment.NewLine;
                    postData += "Content-Disposition: form-data; name=\"crashData\"; ";
                    postData += "filename=\"Unknown\"" + Environment.NewLine;
                    postData += "Content-Type: application/octet-stream" + Environment.NewLine + Environment.NewLine;
                    // crashData (data)
                    byteArray = Encoding.UTF8.GetBytes(postData);
                    postData = "";
                    iByteList.AddRange(byteArray);
                    iByteList.AddRange(aCrashData);
                }
                // Footer
                postData += footer + Environment.NewLine;
                byteArray = Encoding.UTF8.GetBytes(postData);
                iByteList.AddRange(byteArray);

                byteArray = iByteList.ToArray();
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                response = request.GetResponse();
                if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpPostFailed();
                }
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                aSubmitResponse = responseFromServer;

                Trace.WriteLine(Trace.kCore, "DebugReport.SendData");
            }
            catch (Exception exc)
            {
                submitFailed = true;
                aSubmitResponse = "Could not send the data to Linn.\n\nPlease email the data manually to:\n" + kLinnEmailAddress + "\n\nError: " + exc.Message;
            }
            finally
            {
                // Clean up the streams.
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (dataStream != null)
                {
                    dataStream.Close();
                    dataStream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return submitFailed;
        }

        private class HttpPostFailed : System.Exception
        {
            public HttpPostFailed() : base("HTTP Post Failed") { }
        }

        private string iName = "";
        private string iEmail = "";
        private string iPhone = "";
        private string iComment = "";
        private List<byte> iByteList = new List<byte>();
    }
}
