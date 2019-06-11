using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // The only logs of /tell is the standard "xyz issued server command: /tell ..." trace for players running a command (no unique trace)
    public class PlayerTellEvent : PlayerCommandEvent
    {
        public NameWithUUID ReceivingPlayer;
        public string Message;

        public PlayerTellEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = Command;
            int spot = check.IndexOf(' ') + 1;
            int spot2 = check.IndexOf(' ', spot + 1);

            ReceivingPlayer.Name = check.Substring(spot, spot2 - spot);

            spot2++;
            Message = check.Substring(spot2, check.Length - spot2);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(ReceivingPlayer.Name, Source.Time);
            ReceivingPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
