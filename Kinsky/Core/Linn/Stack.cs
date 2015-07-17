
using System;
using System.Net;

namespace Linn
{
    public class EventArgsStackStatus : EventArgs
    {
        public EventArgsStackStatus(StackStatus aStatus)
        {
            Status = aStatus;
        }

        public readonly StackStatus Status;
    }

    public interface IStack
    {
        void Start(IPAddress aIpAddress);
        void Stop();
    }

    public enum EStackState
    {
        eStopped,
        eNoInterface,
        eNonexistentInterface,
        eBadInterface,
        eOk
    }

    public class StackStatus
    {
        public StackStatus(EStackState aState, NetworkInterface aInterface)
        {
            State = aState;
            Interface = aInterface;
        }

        public readonly EStackState State;
        public readonly NetworkInterface Interface;
    }

    public abstract class StackStatusHandler
    {
        public StackStatusHandler() {
            iShowOptions = false;
        }

        public abstract void StackStatusChanged(object sender, EventArgsStackStatus e);
        public abstract void StackStatusStartupChanged(object sender, EventArgsStackStatus e);
        public abstract void StackStatusOptionsChanged(object sender, EventArgsStackStatus e);

        public bool ShowOptions {
            get { return iShowOptions; }
            set { iShowOptions = value; }
        }

        private bool iShowOptions;
    }

    public class Stack
    {
        public Stack(OptionNetworkInterface aOption)
        {
            iStatus = new StackStatus(EStackState.eStopped, null);
            iStackStarted = false;
            iInterface = null;
            iOption = aOption;
            iOption.EventValueChanged += InterfaceChanged;
        }

        public event EventHandler<EventArgsStackStatus> EventStatusChanged;

        public void SetStack(IStack aStack)
        {
            Assert.Check(iStack == null);
            Assert.Check(iStatus.State == EStackState.eStopped);
            iStack = aStack;
        }

        public void SetStatusHandler(StackStatusHandler aHandler)
        {
            Assert.Check(iStackStatusHandler == null);
            iStackStatusHandler = aHandler;
        }

        public StackStatus Status
        {
            get { return iStatus; }
        }

        public StackStatusHandler StatusHandler
        {
            get { return iStackStatusHandler; }
        }

        public void Start()
        {
            if (iStackStatusHandler != null)
            {
                // start the stack - use the startup handler to handle errors and
                // switch to the normal handler when done
                EventStatusChanged -= iStackStatusHandler.StackStatusChanged;
                EventStatusChanged += iStackStatusHandler.StackStatusStartupChanged;
            }

            StackStatus newStatus;
            lock (this)
            {
                StartStack(iOption.Interface);
                newStatus = iStatus;
            }

            EventHandler<EventArgsStackStatus> ev = EventStatusChanged;
            if (ev != null)
            {
                ev(this, new EventArgsStackStatus(newStatus));
            }

            if (iStackStatusHandler != null)
            {
                EventStatusChanged -= iStackStatusHandler.StackStatusStartupChanged;
                EventStatusChanged += iStackStatusHandler.StackStatusChanged;
            }
        }

        public void Stop()
        {
            StackStatus newStatus;
            lock (this)
            {
                StopStack();
                newStatus = iStatus;
            }

            EventHandler<EventArgsStackStatus> ev = EventStatusChanged;
            if (ev != null)
            {
                ev(this, new EventArgsStackStatus(newStatus));
            }

            if (iStackStatusHandler != null)
            {
                EventStatusChanged -= iStackStatusHandler.StackStatusChanged;
            }
        }

        private void InterfaceChanged(object sender, EventArgs e)
        {
            StackStatus newStatus;
            lock (this)
            {
                if (iStatus.State == EStackState.eStopped)
                {
                    return;
                }

                // get the current interface from the option                
                NetworkInterface newInterface = iOption.Interface;

                if (newInterface.Name == iInterface.Name &&
                    iInterface.Status == NetworkInterface.EStatus.eAvailable &&
                    newInterface.Status == NetworkInterface.EStatus.eUnavailable)
                {
                    // the currently available configured interface has become unavailable e.g. gone into standby
                    // - do not stop the stack just yet as this will leave lingering subscriptions
                    iInterface = newInterface;
                    iStatus = new StackStatus(EStackState.eNonexistentInterface, iInterface);

                    Trace.WriteLine(Trace.kCore, "Stack.InterfaceChanged() interface unavailable");
                    UserLog.WriteLine(DateTime.Now + ": Linn.Stack interface now unavailable " + iInterface.ToString());
                }
                else if (newInterface.Name == iInterface.Name &&
                         iInterface.Status == NetworkInterface.EStatus.eUnavailable &&
                         newInterface.Status == NetworkInterface.EStatus.eAvailable)
                {
                    // the currently unavailable configured interface has become available e.g. come out of standby
                    // - restart the stack
                    Trace.WriteLine(Trace.kCore, "Stack.InterfaceChanged() interface now available");
                    UserLog.WriteLine(DateTime.Now + ": Linn.Stack interface now available " + newInterface.ToString());

                    // now that the interface is available again, the stack can be stopped to cleanup existing
                    // subscriptions
                    StopStack();

                    // a pause for coming out of standby - on the PDA, even though it has a valid IP
                    // address, the interface is still not fully operational
                    System.Threading.Thread.Sleep(5000);

                    StartStack(newInterface);
                }
                else
                {
                    // all other interface changes, including switching to a different physical interface
                    // just restart the stack
                    StopStack();
                    StartStack(newInterface);
                }

                newStatus = iStatus;
            }

            EventHandler<EventArgsStackStatus> ev = EventStatusChanged;
            if (ev != null)
            {
                ev(this, new EventArgsStackStatus(newStatus));
            }
        }

        private void StartStack(NetworkInterface aInterface)
        {
            iInterface = aInterface;
            iStackStarted = false;

            switch (aInterface.Status)
            {
                case NetworkInterface.EStatus.eUnconfigured:
                    iStatus = new StackStatus(EStackState.eNoInterface, iInterface);

                    Trace.WriteLine(Trace.kCore, "Stack.StartStack() no configured interface");
                    UserLog.WriteLine(DateTime.Now + ": Linn.Stack start failed - no interface is configured");
                    break;

                case NetworkInterface.EStatus.eUnavailable:
                    iStatus = new StackStatus(EStackState.eNonexistentInterface, iInterface);

                    Trace.WriteLine(Trace.kCore, "Stack.StartStack() configured interface error");
                    UserLog.WriteLine(DateTime.Now + ": Linn.Stack start failed - configured interface is unavailable " + iInterface.ToString());
                    break;

                case NetworkInterface.EStatus.eAvailable:
                    try
                    {
                        Trace.WriteLine(Trace.kCore, "Stack.StartStack() starting...");
                        UserLog.WriteLine(DateTime.Now + ": Linn.Stack starting... " + iInterface.ToString());

                        if (iStack != null)
                        {
                            iStack.Start(iInterface.Info.IPAddress);
                        }
                        iStatus = new StackStatus(EStackState.eOk, iInterface);
                        iStackStarted = true;

                        Trace.WriteLine(Trace.kCore, "Stack.StartStack() OK");
                        UserLog.WriteLine(DateTime.Now + ": Linn.Stack start ok " + iInterface.ToString());
                    }
                    catch (Exception e)
                    {
                        iStatus = new StackStatus(EStackState.eBadInterface, iInterface);
                        iStackStarted = false;

                        Trace.WriteLine(Trace.kCore, "Stack.StartStack() failure: " + e.ToString());
                        Console.Write("Stack.StartStack() failure: " + e.ToString());
                        UserLog.WriteLine(DateTime.Now + ": Linn.Stack start failed " + iInterface.ToString());
                        UserLog.WriteLine("Error Message: " + e.Message);
                        UserLog.WriteLine("Error Message: " + e.ToString());

                        // stop the stack to cleanup any stack components that were started
                        if (iStack != null)
                        {
                            iStack.Stop();
                        }
                    }
                    break;

                default:
                    Assert.Check(false);
                    break;
            }
        }

        private void StopStack()
        {
            if (iStatus.State != EStackState.eStopped)
            {
                Trace.WriteLine(Trace.kCore, "Stack.StopStack() stopping stack...");
                UserLog.WriteLine(DateTime.Now + ": Linn.Stack stopping...");

                if (iStackStarted && iStack != null)
                {
                    iStack.Stop();
                }

                iStatus = new StackStatus(EStackState.eStopped, null);
                iStackStarted = false;
                iInterface = null;

                Trace.WriteLine(Trace.kCore, "Stack.StopStack() stack stopped");
                UserLog.WriteLine(DateTime.Now + ": Linn.Stack stop ok");
            }
        }

        private IStack iStack;
        private StackStatus iStatus;
        private bool iStackStarted;
        private NetworkInterface iInterface;
        private OptionNetworkInterface iOption;
        private StackStatusHandler iStackStatusHandler;
    }
}



