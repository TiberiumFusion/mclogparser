using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerWhitelistDisableEvent : CommandResultEvent
    {
        public ServerWhitelistDisableEvent(LogLine source) : base(source) { }
    }
}
