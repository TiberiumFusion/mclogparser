using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // These have a superstructure of the mineversechat format (i.e. {{name}}: {{message}}), so testing a LogLine for this event must occur before a chat event test
    public class IngameCommandResultEvent : CommandResultEvent
    {
        public string ExecutorUUID; // Empty if Executor is not a player
        public bool ExecutorIsLikelyAPlayer = false; // Useful b/c commands can be executed by any named entity or a command block (which use @ for name)

        public IngameCommandResultEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(':');
            Executor = check.Substring(1, spot - 1);

            spot += 2; // Skip over space
            ResultMessage = check.Substring(spot, check.Length - spot - 1);

            ExecutorIsServer = false;
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(Executor, Source.Time);
            if (UUID != null)
            {
                ExecutorUUID = UUID;
                ExecutorIsLikelyAPlayer = true;
            }
            else
            {
                ExecutorUUID = "";
                ExecutorIsLikelyAPlayer = false;
            }
        }
    }
}
