using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class JavaExceptionEvent : GameEvent
    {
        public string ExceptionType = "";

        public JavaExceptionEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            int spot = Source.Body.IndexOf(": ");
            ExceptionType = Source.Body.Substring(0, spot);
        }
    }
}
