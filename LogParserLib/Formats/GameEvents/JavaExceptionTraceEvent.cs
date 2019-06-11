using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class JavaExceptionTraceEvent : GameEvent
    {
        public string ExceptionType = "";
        public string FullTrace = "";

        public JavaExceptionTraceEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            int spot = Source.Body.IndexOf(": ");
            ExceptionType = Source.Body.Substring(0, spot);

            FullTrace = Source.Body;
        }
    }
}
