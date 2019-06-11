using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class PlayerCommandEvent : GameEvent
    {
        public NameWithUUID ExecutingPlayer;
        public string Command;

        public PlayerCommandEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(" issued server command: ");
            ExecutingPlayer.Name = check.Substring(0, spot);

            spot += 24;
            Command = check.Substring(spot, check.Length - spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(ExecutingPlayer.Name, Source.Time);
            ExecutingPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
