using System;

namespace Linn {

public class Ticker
{
    public Ticker() {
        Reset();
    }
    
    public void Reset() {
        iTime = Environment.TickCount;
    }

    public int StartTime
    {
        get
        {
            return iTime;
        }
    }

    public int EndTime
    {
        get
        {
            return iEndTime;
        }
    }
    
    public float MilliSeconds {
        get {
            iEndTime = Environment.TickCount;
            int result = iEndTime - iTime;
            return (float)result;// Math.Max(result, 0);
        }
    }
    
    private int iTime = 0;
    private int iEndTime = 0;
}

} // Linn
