using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class IngameWhitelistEnableEvent : IngameCommandResultEvent
    {
        public IngameWhitelistEnableEvent(LogLine source) : base(source) { }
    }
}
