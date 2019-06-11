using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class PlayerLostConnEvent : PlayerLeaveEvent
    {
        public string LostConnReason;

        public PlayerLostConnEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(" lost connection:");
            Player.Name = check.Substring(0, spot);

            spot += 18;
            LostConnReason = check.Substring(spot, check.Length - spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(Player.Name, Source.Time);
            Player.UUID = (UUID != null) ? UUID : "";
        }
    }
}
