using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // Command results from the server follow this format. Command results from ingame entities have a wrapped syntax and are subclassed from this as IngameCommandResultEvent
    public class CommandResultEvent : GameEvent
    {
        public string Executor; // Will typically be null if ExecutorIsServer == true
        public bool ExecutorIsServer;
        public string ResultMessage;

        public CommandResultEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            ResultMessage = Source.Body;

            ExecutorIsServer = true;
        }
    }
}
