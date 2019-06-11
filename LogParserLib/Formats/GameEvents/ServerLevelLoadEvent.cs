using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerLevelLoadEvent : GameEvent
    {
        public string LevelName = "";
        public string LevelSeed = "";

        public ServerLevelLoadEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(" (Seed: ");
            LevelName = check.Substring(33, spot - 33);

            spot += 8;
            int spot2 = check.LastIndexOf(')');
            LevelSeed = check.Substring(spot, spot2 - spot);
        }
    }
}
