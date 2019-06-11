using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class PlayerLeaveEvent : GameEvent
    {
        public NameWithUUID Player;

        public PlayerLeaveEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(" left the game");
            Player.Name = check.Substring(0, spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(Player.Name, Source.Time);
            Player.UUID = (UUID != null) ? UUID : "";
        }
    }
}
