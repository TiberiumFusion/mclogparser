using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class CouldNotPassEventEvent : GameEvent
    {
        public string CraftbukkitEvent = "";
        public string ReceivingPlugin = "";
        public string FullTrace = "";

        public CouldNotPassEventEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;

            int spot = 21;
            int spot2 = check.IndexOf(' ', spot + 1);

            CraftbukkitEvent = check.Substring(spot, spot2 - spot);

            spot = check.IndexOf("to ", spot2) + 3;
            char[] linebreaks = { '\r', '\n' };
            spot2 = check.IndexOfAny(linebreaks, spot + 1);

            ReceivingPlugin = check.Substring(spot, spot2 - spot);

            while (check[spot2] == '\r' || check[spot2] == '\n')
                spot2++;

            FullTrace = check.Substring(spot2);
        }
    }
}
