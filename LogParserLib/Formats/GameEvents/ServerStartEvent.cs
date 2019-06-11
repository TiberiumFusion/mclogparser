using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerStartEvent : GameEvent
    {
        public string ServerMinecraftVersion = "";

        public ServerStartEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf("version") + 8;
            ServerMinecraftVersion = check.Substring(spot, check.Length - spot);
        }
    }
}
