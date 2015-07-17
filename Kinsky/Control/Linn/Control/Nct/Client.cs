using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Linn;
// for HiResTimer class ...
using System.Runtime.InteropServices;

namespace Linn.Control.Nct
{
// ================================================================================================
// Client (Network Connection Test TCP/UDP Client) ...
// ================================================================================================
// Used to test the speed and integrity of a network connection between a control point and a Linn
// DS product, using either a TCP or UDP connection.
//
// The test consists of transmitting a series of test packets to the DS product and then waiting
// for the DS product to return the test packets, with various results displayed at the conclusion
// of the test.
// ------------------------------------------------------------------------------------------------
public class Client
{
    // ip addresses, ports, protocol and packet information passed in by command line ...
    private string     iClientAddr;
    private int        iClientPort;
    private string     iServerAddr;
    private int        iServerPort;
    private string     iProtocol;
    private int        iPacketSize;
    private int        iPacketTotal;
    private int        iPacketDelay;

    // various ...
    private IConnection iConnection = null;
    private Thread      iRxThread;
    private Semaphore   iSemaphore  = null;
    private HiResTimer  iHiresClock;
    private long[]      iTimeStampTx;
    private long[]      iTimeStampRx;
    private int         iTotalPacketsTx;
    private int         iTotalPacketsRx;
    private int         iTotalInOrder;
    private bool        iTestResult;
    private string      iTestOutput;
    private int         iPacketRequired = 0;
    private Exception   iReceiveThreadException = null;

    public Client(string aProtocol, string aClientAddr, int aClientPort, string aServerAddr, int aServerPort, int aPacketSize, int aPacketTotal, int aPacketDelay)
    {
        iProtocol     = aProtocol;
        iClientAddr   = aClientAddr;
        iClientPort   = aClientPort;
        iServerAddr   = aServerAddr;
        iServerPort   = aServerPort;
        iPacketSize   = aPacketSize;
        iPacketTotal  = aPacketTotal;
        iPacketDelay  = aPacketDelay;
        iTotalInOrder = 0;

        iTestResult   = false;
        iTestOutput   = "";
        iReceiveThreadException = null;

        // ensure minimum values ...
        if (iPacketSize < 4)
        {
            // need at least 4 bytes for packet identifier ...
            iPacketSize = 4;
        }

        if (iPacketTotal < 20)
        {
            // need at least 20 packets to show percentage completed (steps of 5%) ...
            iPacketTotal = 20;
        }

        // create transmit/receive timestamp arrays ...
        iTimeStampTx = new long[iPacketTotal];
        iTimeStampRx = new long[iPacketTotal];

        // initialise transmit/receive timestamps ...
        for (int timeinit = 0; (timeinit < iPacketTotal); timeinit++)
        {
            iTimeStampTx[timeinit] = 0xFFFFFFFF;
            iTimeStampRx[timeinit] = 0xFFFFFFFF;
        }

        // get the host name of local machine ...
        String ipHostName = System.Net.Dns.GetHostName();
        // use the host name to get the IP address list..
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(ipHostName);
        IPAddress[] ipAddr = ipEntry.AddressList;
    }

    public bool ClientServerTest()
    {
        // create instance of appropriate connection ...
        switch (iProtocol)
        {
            case "TCP":
                iPacketRequired = iPacketTotal; // TCP should get 100% Tx and Rx
                iConnection = new TcpConnection();
                break;

            case "UDP":
                iPacketRequired = (iPacketTotal*95)/100; // UDP should get >=95% Tx and Rx
                iConnection = new UdpConnection();
                break;

            default:
                iTestOutput = "Invalid protocol specified! <" + iProtocol + ">\n";
                break;
        }

        if (iConnection != null)
        {
            // attempt to open connection ...
            iTestOutput = iConnection.Open(iClientAddr, iClientPort, iServerAddr, iServerPort);

            // if connection has been opened ...
            if (iTestOutput == "")
            {
                iSemaphore = new Semaphore(0, 1);

                // create/start hi res timer (1ms timer) ...
                iHiresClock = new HiResTimer();
                iHiresClock.Reset();

                // create and start client packet receive thread ...
                iRxThread = new Thread(new ThreadStart(this.ClientPacketReceive));
                iRxThread.Name = "Client - Tx Thread";
                //iThread.IsBackground = true; // Josh said to ignore this for the moment
                iRxThread.Priority = ThreadPriority.AboveNormal;
                iRxThread.Start();

                // wait until client packet receive thread has started ...
                iSemaphore.WaitOne();

                // start client packet transmit ...
                ClientPacketTransmit();

                // wait here until client packet receive thread has finished ...
                iRxThread.Join();

                // all packet Tx's and Rx's have been completed so close connection ...
                iConnection.Shut();

                if (iReceiveThreadException != null) {
                    throw iReceiveThreadException;
                }

                GenerateTestResults();
            }
        }

        Console.WriteLine(iTestOutput);
        return iTestResult;
    }

    private void ClientPacketTransmit()
    {
        byte[] packetData  = new byte[iPacketSize]; // data packet transmit buffer
        long   stepIdPercentage = iPacketTotal / 20;
        long   nextIdPercentage = stepIdPercentage;

        // initialise data packet buffer contents (fill repeatedly with A..Z pattern) ...
        for (int init = 0; init < iPacketSize; init++)
        {
            packetData[init] = (byte)((init % 26) + 65);
        }

        // transmit the required number of packets ...
        for (int packetId = 0; (packetId < iPacketTotal); packetId++)
        {
            // use first four bytes of packet data to store packet id ...
            packetData[0] = (byte)((packetId)         & 0xFF);
            packetData[1] = (byte)((packetId >> 0x08) & 0xFF);
            packetData[2] = (byte)((packetId >> 0x10) & 0xFF);
            packetData[3] = (byte)((packetId >> 0x18) & 0xFF);

            // note time at start of transmission of packet ...
            iTimeStampTx[packetId] = iHiresClock.Peek();

            // transmit packet data ...
            iConnection.Transmit(ref packetData);

            // update number of packets transmitted ...
            iTotalPacketsTx++;

            // display progress for every 5% of test completed ...
            if (packetId == nextIdPercentage)
            {
                Console.WriteLine("Progress Tx: {0}%", 5 * (nextIdPercentage / stepIdPercentage));
                nextIdPercentage += stepIdPercentage;
            }

            // if delay period specified between packets ...
            if (iPacketDelay != 0)
            {
                // wait here until specified time has passed ...
                Thread.Sleep(iPacketDelay);
            }
        }
    }

    private void ClientPacketReceive()
    {
        byte[] receivedData = new byte[iPacketSize];
        long   receivedTime;
        int    receivedId;
        int    expectedId = 0;
        long   stepIdPercentage = iPacketTotal / 20;
        long   nextIdPercentage = stepIdPercentage;

        // signal main thread that receive thread has started ...
        iSemaphore.Release();

        try
        {
            // receive the required number of packets ...
            for (int packetId = 0; (packetId < iPacketTotal); packetId++)
            {
                // receive the required number of bytes per packet ...
                iConnection.Receive(ref receivedData);

                // note time at completion of receiving packet ...
                receivedTime = iHiresClock.Peek();

                // extract packet id ...
                receivedId  = ((int)receivedData[0]);
                receivedId |= ((int)receivedData[1] << 0x08);
                receivedId |= ((int)receivedData[2] << 0x10);
                receivedId |= ((int)receivedData[3] << 0x18);

                // store timestamp for received packet ...
                iTimeStampRx[receivedId] = receivedTime;

                // update total packets received ...
                iTotalPacketsRx++;

                // if packet id matches expected id ...
                if (receivedId == expectedId)
                {
                    // update number of packets received in correct order ...
                    iTotalInOrder++;
                }

                // update next expected packet id ...
                expectedId = receivedId + 1;

                // display progress for every 5% of test completed ...
                if (packetId == nextIdPercentage)
                {
                    Console.WriteLine("Progress         Rx: {0}%", 5 * (nextIdPercentage / stepIdPercentage));
                    nextIdPercentage += stepIdPercentage;
                }
            }
        }
        catch (Exception e)
        {
            iReceiveThreadException = e;
        }
    }

    private void GenerateTestResults()
    {
        long minTripTime   = 0xFFFFFFFF; // minimum time of packet  which successfully completed a round trip
        long maxTripTime   = 0x00000000; // maximum time of packet  which successfully completed a round trip
        long totalTripTxRx = 0;          // total number of packets which successfully completed a round trip
        long totalTripTime = 0;          // total time   of packets which successfully completed a round trip

        // Array used to perform analysis of roundtrip time of packets, with each entry consisting of a maximum round trip
        // time limit and a count of how many packets were within this limit.
        //
        // Times are in units of a millisecond with each entry being greater than the previous, apart from the last entry
        // which is set to zero to signify 'all other times' (will handle any number of entries, just remember the last
        // entry needs to be zero!!!).
        long[,] timeAnalysis = new long[,] {{1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}, {6, 0}, {7, 0}, {8, 0}, {9, 0}, {10, 0}, {0, 0}};


        // calculate some results ...
        for (int checktime = 0; (checktime < iPacketTotal); checktime++)
        {
            // if valid packet transmitted and received timestamps ...
            if ((iTimeStampTx[checktime] != 0xFFFFFFFF) && (iTimeStampRx[checktime] != 0xFFFFFFFF))
            {
                // determine round trip time of packet ...
                long timeTrip = iTimeStampRx[checktime] - iTimeStampTx[checktime];

                // find the smallest round trip time ...
                if (timeTrip < minTripTime)
                {
                    minTripTime = timeTrip;
                }

                // find the largest round trip time ...
                if (timeTrip > maxTripTime)
                {
                    maxTripTime = timeTrip;
                }

                // update total number of round trip packets ...
                totalTripTxRx++;
                totalTripTime += timeTrip;

                // perform basic analysis of round trip time per packet ...
                for (int timeLimit = 0; ; timeLimit++)
                {
                    // determine if round trip time is within time range, or greater than any range specified ...
                    if ((timeAnalysis[timeLimit, 0] == 0) || (timeTrip <= timeAnalysis[timeLimit, 0]))
                    {
                        // if so, increment number of packets found within particular range
                        timeAnalysis[timeLimit, 1]++;
                        break;
                    }
                }
            }
        }

        // generate test results ...
        Console.WriteLine("");
        iTestOutput  = "SETTINGS\n";
        iTestOutput += "Protocol: " + iProtocol + "\n";
        iTestOutput += "Client Address: " + iClientAddr + ":" + iClientPort + "\n";
        iTestOutput += "Server Address: " + iServerAddr + ":" + iServerPort + "\n";
        iTestOutput += "Packet Size: " + iPacketSize + " bytes\n";
        iTestOutput += "Total Packets: " + iPacketTotal + "\n";
        iTestOutput += "Packets required for test to pass: " + iPacketRequired + "\n";
        iTestOutput += "Packet Delay: " + iPacketDelay + "ms\n\n";
        
        iTestOutput += "PACKET TOTALS\n";
        iTestOutput += "Transmitted: " + iTotalPacketsTx + "\n";
        iTestOutput += "Received: " + iTotalPacketsRx + "\n";
        iTestOutput += "Dropped: " + (iTotalPacketsTx - iTotalPacketsRx) + "\n";
        iTestOutput += "In Order: " + iTotalInOrder + "\n\n";

        iTestOutput += "ROUND TRIP TIME ANALYSIS OF " + totalTripTxRx + " PACKETS\n";
        iTestOutput += "Total: " + totalTripTime + "ms\n";
        iTestOutput += "Average: " + (totalTripTime / (double)totalTripTxRx) + "ms\n";
        iTestOutput += "Fastest: " + minTripTime + " ms\n";
        iTestOutput += "Slowest: " + maxTripTime + " ms\n\n";

        // construct and output breakdown of packets within each time range ...
        string header = "";
        string totals = "";
        for (int timeRange = 0; ; timeRange++)
        {
            totals += String.Format("{0}\t", timeAnalysis[timeRange, 1]);

            // if last time range ...
            if (timeAnalysis[timeRange, 0] == 0)
            {
                header += String.Format(">{0}ms\t", timeAnalysis[timeRange - 1, 0]);

                // output final results line ...
                iTestOutput += String.Format("{0}\n", header);
                iTestOutput += String.Format("{0}", totals);
                break;
            }
            else
            {
                header += String.Format("<={0}ms\t", timeAnalysis[timeRange, 0]);

                // output each results line when five results have been added ...
                if ((timeRange % 5) == 4)
                {
                    iTestOutput += String.Format("{0}\n", header);
                    iTestOutput += String.Format("{0}\n", totals);
                    header = "";
                    totals = "";
                }
            }
        }

        // determine if test passed or failed ...
        if ((iTotalPacketsTx >= iPacketRequired) && (iTotalPacketsRx >= iPacketRequired))
        {
            // simply check to see if all packets transmitted and received ...
            iTestResult = true;
        }
    }

    public string ClientServerTestResults()
    {
        return iTestOutput;
    }
} // Client


// ================================================================================================
// IConnection ...
// ================================================================================================
// Interface class to perform open, close, transmit and receive operations over a connection
// ------------------------------------------------------------------------------------------------
interface IConnection
{
    string Open(string aClientAddr, int aClientPort, string aServerAddr, int aServerPort);
    void Shut();
    void Transmit(ref byte[] aData);
    void Receive(ref byte[] aData);
}


// ================================================================================================
// TcpConnection ...
// ================================================================================================
// TCP implementation of IConnection interface class
// ------------------------------------------------------------------------------------------------
class TcpConnection : IConnection
{
    private Socket iSocket = null;

    public string Open(string aClientAddr, int aClientPort, string aServerAddr, int aServerPort)
    {
        string result = "";
        
        // attempt to open connection ...
        try
        {
            iSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            iSocket.Connect(new IPEndPoint(IPAddress.Parse(aServerAddr), aServerPort));
            iSocket.ReceiveTimeout = 2000;
        }
        // if any problem occurs during connection, then ensure it is shut ...
        catch (Exception e)
        {
            if (iSocket != null)
            {
                iSocket.Close();
                iSocket = null;
            }
            throw e;
        }

        return (result);
    }

    public void Shut()
    {
        if (iSocket != null)
        {
            iSocket.Shutdown(SocketShutdown.Both);
            iSocket.Close();
            iSocket = null;
        }
    }

    public void Transmit(ref byte[] aData)
    {
        iSocket.Send(aData);
    }

    public void Receive(ref byte[] aData)
    {
        int dataOffset = 0;            // offset to store data in array ...
        int dataNeeded = aData.Length; // length of data to be received ...
        int dataReadIn;                // bytes of data received per call

        // while packet data has still to be received ...
        while (dataNeeded != 0)
        {
            // read packet data ...
            dataReadIn = iSocket.Receive(aData, dataOffset, dataNeeded, SocketFlags.None);

            // subtract number of packet data bytes received from those still required ...
            dataNeeded -= dataReadIn;

            // add number of packet data bytes received to offset to store data ...
            dataOffset += dataReadIn;
        }
    }
}


// ================================================================================================
// UdpConnection ...
// ================================================================================================
// UDP implementation of IConnection interface class
// ------------------------------------------------------------------------------------------------
class UdpConnection : IConnection
{
    private Socket     iSocket      = null;
    private IPEndPoint iUdpServerEP = null;
    private IPEndPoint iUdpClientEP = null;

    public string Open(string aClientAddr, int aClientPort, string aServerAddr, int aServerPort)
    {
        string result = "";

        // attempt to open connection ...
        try
        {
            iSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            iSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            iUdpClientEP = new IPEndPoint(IPAddress.Parse(aClientAddr), aClientPort);
            iSocket.Bind(iUdpClientEP);
            iUdpServerEP = new IPEndPoint(IPAddress.Parse(aServerAddr), aServerPort);
            iSocket.ReceiveTimeout = 2000;
        }
        // if any problem occurs during connection, then ensure it is shut ...
        catch (Exception e)
        {
            if (iSocket != null)
            {
                iSocket.Close();
                iSocket      = null;
                iUdpServerEP = null;
                iUdpClientEP = null;
            }
            throw e;
        }

        return (result);
    }

    public void Shut()
    {
        if (iSocket != null)
        {
            iSocket.Shutdown(SocketShutdown.Both);
            iSocket.Close();
            iSocket = null;
        }

        iUdpServerEP = null;
        iUdpClientEP = null;
    }

    public void Transmit(ref byte[] aData)
    {
        iSocket.SendTo(aData, iUdpServerEP);
    }

    public void Receive(ref byte[] aData)
    {
        EndPoint clientEP = iUdpClientEP;

        int dataOffset = 0;            // offset to store data in array ...
        int dataNeeded = aData.Length; // length of data to be received ...
        int dataReadIn;                // bytes of data received per call

        // while packet data has still to be received ...
        while (dataNeeded != 0)
        {
            // read packet data ...
            dataReadIn = iSocket.ReceiveFrom(aData, dataOffset, dataNeeded, SocketFlags.None, ref clientEP);

            // subtract number of packet data bytes received from those still required ...
            dataNeeded -= dataReadIn;

            // add number of packet data bytes received to offset to store data ...
            dataOffset += dataReadIn;
        }
    }
}


// ================================================================================================
// HiResTimer (High Resolution Timer) ...
// ================================================================================================
// This class is capable of measuring time to a resolution of 1/10th of a millisecond, however it
// depends on the 'QueryPerformance...' calls to provide this accuracy, which are not available on
// all platforms.
//
// Therefore, the accuracy has been reduced to a millisecond so that either the system 'TickCount'
// or 'QueryPerformance...' methods can be used, with the 'QueryPerformance...' method providing a
// more accurate millisecond timer if available.
// ------------------------------------------------------------------------------------------------
public sealed class HiResTimer
{
    [DllImport("kernel32.dll")]
    extern static int QueryPerformanceCounter(ref long x);
    [DllImport("kernel32.dll")]
    extern static int QueryPerformanceFrequency(ref long x);
    private bool iHiRes;
    private long iStartTime;
    private long iFrequency;

    public HiResTimer()
    {
        Frequency = GetFrequency();
        Reset();
    }
   
    public void Reset()
    {
        StartTime = GetValue();
    }
   
    public long Peek()
    {
        return (long)(((GetValue() - StartTime) / (double)Frequency) * 1000); // convert to milliseconds
    }

    private long GetValue()
    {
        long ret = 0;

        if (iHiRes)
        {
            QueryPerformanceCounter(ref ret);
        }
        else
        {
            ret = System.Environment.TickCount;
        }

        return ret;
    }

    private long GetFrequency()
    {
        long ret = 0;

        try
        {
            QueryPerformanceFrequency(ref ret);
            iHiRes = true;
        }
        catch
        {
            ret = 1000; // 1000 ticks per second (i.e. ms resolution)
            iHiRes = false;
        }

        return ret;
    }

    private long StartTime
    {
        get
        {
            return iStartTime;
        }
        set
        {
            iStartTime = value;
        }
    }

    private long Frequency
    {
        get
        {
            return iFrequency;
        }
        set
        {
            iFrequency = value;
        }
    }
} // class HiResTimer

} // namespace Linn.Control.Nct
