
using System;
using System.Timers;


namespace Linn
{

// Linn Timer class - implemented to be largely the same as the
// System.Timers.Timer class. We have our own because the System
// one is not present in the .NET compact framework.
// The ElapsedEventArgs class is also not present - the event
// handler in this implementation is a simple EventHandler (rather
// than an ElapsedEventHandler)
public class Timer : IDisposable
{
    public Timer() {
        iTimer = new System.Timers.Timer();
        iTimer.Elapsed += new ElapsedEventHandler(ElapsedEventInternal);
    }
    public Timer(double aInterval) {
        iTimer = new System.Timers.Timer(aInterval);
        iTimer.Elapsed += new ElapsedEventHandler(ElapsedEventInternal);
    }
    
    public void Dispose() {
        iTimer.Dispose();
    }

    public bool AutoReset {
        get { return iTimer.AutoReset; }
        set { iTimer.AutoReset = value; }
    }
    public bool Enabled {
        get { return iTimer.Enabled; }
        set { iTimer.Enabled = value; }
    }
    public double Interval {
        get { return iTimer.Interval; }
        set { iTimer.Interval = value; }
    }
    public void Start() {
        iTimer.Start();
    }
    public void Stop() {
        iTimer.Stop();
    }
    public event EventHandler Elapsed;

    private void ElapsedEventInternal(object aSender, ElapsedEventArgs aArgs) {
        if (Elapsed != null) {
            Elapsed(this, aArgs);
        }
    }
    private System.Timers.Timer iTimer;
}

}   // namespace Linn

