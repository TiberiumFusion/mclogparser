using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // For events that the Analyzer does not recognize
    public class UnrecognizedEvent : GameEvent
    {
        public string MessageTag;
        public string MessageBody;

        public UnrecognizedEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            MessageTag = Source.Tag;
            MessageBody = Source.Body;
        }
    }
}
