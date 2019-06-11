using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // Logs do not have a distinct kick trace, so a "PlayerName issued server command: /kick victim reason" is the only way to catch this
    public class PlayerKickEvent : PlayerCommandEvent
    {
        public NameWithUUID KickedPlayer;
        public string Reason; // Null if none provided

        public PlayerKickEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = Command;
            int spot = check.IndexOf(' ') + 1;
            int spot2 = check.IndexOf(' ', spot + 1);
            if (spot2 > spot)
            {
                KickedPlayer.Name = check.Substring(spot, spot2 - spot);
                Reason = check.Substring(spot2 + 1);
            }
            else
            {
                KickedPlayer.Name = check.Substring(spot);
            }
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(KickedPlayer.Name, Source.Time);
            KickedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
