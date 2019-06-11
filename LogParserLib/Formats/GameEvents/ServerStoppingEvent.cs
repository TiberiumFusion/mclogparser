using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerStoppingEvent : GameEvent
    {
        public ServerStoppingEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            
        }
    }
}
