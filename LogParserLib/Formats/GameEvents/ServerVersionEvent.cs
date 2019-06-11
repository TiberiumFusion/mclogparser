using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerVersionEvent : GameEvent
    {
        public string ServerFlavor = "";
        public string FlavorVersion = "";
        public string FlavorAPIVersion = "";

        public ServerVersionEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf("running") + 8;
            int space = check.IndexOf(' ', spot + 1);
            ServerFlavor = check.Substring(spot, space - spot);

            spot = check.IndexOf("version") + 8;
            int spot2 = check.IndexOf(" (MC:");
            FlavorVersion = check.Substring(spot, spot2 - spot);

            spot = check.IndexOf("API version ") + 12;
            spot2 = check.LastIndexOf(')');
            FlavorAPIVersion = check.Substring(spot, spot2 - spot);
        }
    }
}
