using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class IngameWhitelistDisableEvent : IngameCommandResultEvent
    {
        public IngameWhitelistDisableEvent(LogLine source) : base(source) { }
    }
}
