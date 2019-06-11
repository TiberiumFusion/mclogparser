using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // NoCheatPlus has a complex log syntax, which I don't really have to the time to dissect
    public class NoCheatPlusEvent : GameEvent
    {
        public string Details = "";

        public NoCheatPlusEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            int spot = Source.Body.IndexOf(' ') + 1;
            Details = Source.Body.Substring(spot, Source.Body.Length - spot);
        }
    }
}
