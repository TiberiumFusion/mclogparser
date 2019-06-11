using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerAddressEvent : GameEvent
    {
        public string ServerAddress = "";
        public string ServerIP = "";
        public string ServerPort = "";

        public ServerAddressEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf("on") + 3;
            ServerAddress = check.Substring(spot, check.Length - spot);

            int spot2 = check.IndexOf(':');
            ServerIP = check.Substring(spot, spot2 - spot);
            ServerPort = check.Substring(spot2 + 1, check.Length - spot2 - 1);
        }
    }
}
