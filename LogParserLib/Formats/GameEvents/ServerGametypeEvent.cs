using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class ServerGametypeEvent : GameEvent
    {
        public string DefaultGametype = "";

        public ServerGametypeEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            DefaultGametype = check.Substring(19, check.Length - 19);
        }
    }
}
