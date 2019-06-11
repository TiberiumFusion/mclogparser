using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    public class TimeRange
    {
        public DateTime Start;
        public DateTime End;
        public TimeSpan Duration { get { return End - Start; } }

        public TimeRange()
        { }
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}
