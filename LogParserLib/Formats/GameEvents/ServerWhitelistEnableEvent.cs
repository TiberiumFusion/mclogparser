using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerWhitelistEnableEvent : CommandResultEvent
    {
        public ServerWhitelistEnableEvent(LogLine source) : base(source) { }
    }
}
