using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class CantKeepUpEvent : GameEvent
    {
        public string TimeBehind = "";
        public string SkippedTicks = "";

        public CantKeepUpEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf("Running") + 8;
            int spot2 = check.IndexOf(' ', spot + 1);

            TimeBehind = check.Substring(spot, spot2 - spot);

            spot = check.IndexOf("skipping") + 9;
            spot2 = check.IndexOf(' ', spot + 1);

            SkippedTicks = check.Substring(spot, spot2 - spot);
        }
    }
}
