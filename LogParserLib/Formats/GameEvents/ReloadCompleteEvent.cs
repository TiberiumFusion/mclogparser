using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ReloadCompleteEvent : CommandResultEvent
    {
        public ReloadCompleteEvent(LogLine source) : base(source) { }
    }
}
