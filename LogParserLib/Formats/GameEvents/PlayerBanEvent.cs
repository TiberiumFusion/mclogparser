using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // Logs do not have a distinct ban trace, so a "PlayerName issued server command: /ban victim reason" is the only way to catch this
    public class PlayerBanEvent : PlayerCommandEvent
    {
        public NameWithUUID BannedPlayer;
        public string Reason; // Null if none provided

        public PlayerBanEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = Command;
            int spot = check.IndexOf(' ') + 1;
            int spot2 = check.IndexOf(' ', spot + 1);
            if (spot2 > spot)
            {
                BannedPlayer.Name = check.Substring(spot, spot2 - spot);
                Reason = check.Substring(spot2 + 1);
            }
            else
            {
                BannedPlayer.Name = check.Substring(spot);
            }
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(BannedPlayer.Name, Source.Time);
            BannedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
