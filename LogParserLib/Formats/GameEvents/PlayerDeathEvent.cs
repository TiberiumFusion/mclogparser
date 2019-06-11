using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class PlayerDeathEvent : GameEvent
    {
        public NameWithUUID Player;

        public PlayerDeathEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf(' ');
            Player.Name = check.Substring(0, spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(Player.Name, Source.Time);
            Player.UUID = (UUID != null) ? UUID : "";
        }
    }
}
