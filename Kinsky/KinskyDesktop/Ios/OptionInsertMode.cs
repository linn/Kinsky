using System;

using Linn;

namespace KinskyTouch
{
    public class OptionInsertMode : OptionEnum
    {
        public const string kPlayNow = "Play Now";
        public const string kPlayNext = "Play Next";
        public const string kPlayLater = "Play Later";

        public OptionInsertMode()
            : base("insertmode", "Insert Mode", "How to insert tracks into the playlist")
        {
            AddDefault(kPlayNow);
            Add(kPlayNext);
            Add(kPlayLater);
        }
    }
}

