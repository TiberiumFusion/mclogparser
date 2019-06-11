using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerBootDoneEvent : GameEvent
    {
        public string BootTime;

        public ServerBootDoneEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf('(') + 1;
            int spot2 = check.IndexOf(')');
            BootTime = check.Substring(spot, spot2 - spot);
        }
    }
}
