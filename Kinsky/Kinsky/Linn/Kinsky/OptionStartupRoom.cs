using System;
using System.Collections.Generic;

using Linn;

namespace Linn.Kinsky
{
    public class OptionStartupRoom : OptionEnum
    {
        public const string kLastSelected = "Last Selected"; 
        public const string kNone = "None";

        public OptionStartupRoom(Linn.Kinsky.House aHouse)
            : base("room", "Startup room", "Room to automatically select on startup")
        {
            AddDefault(kLastSelected);
            Add(kNone);

            aHouse.EventRoomInserted += RoomAdded;
        }

        public override bool Set(string aValue)
        {
            if (!Allowed.Contains(aValue))
            {
                Add(aValue);
            }

            return base.Set(aValue);
        }

        private void RoomAdded(object sender, EventArgsItemInsert<IRoom> e)
        {
            if (!Allowed.Contains(e.Item.Name))
            {
                Add(e.Item.Name);
            }
        }

        public override IList<string> Allowed
        {
            get
            {
                List<string> sorted = new List<string>(base.Allowed);
                sorted.Sort((x, y) =>
                {
                    if (x == null) return -1;
                    else if (y == null) return 1;
                    else if (x == y) return 0;
                    else
                    {
                        if (x == kNone || y == kNone) return (x == kNone ? -1 : 1);
                        else if (x == kLastSelected || y == kLastSelected) return (x == kLastSelected ? -1 : 1);
                        else return x.CompareTo(y);
                    }
                });
                return (sorted);
            }
        }

    }
}