using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class ServerOppedPlayerEvent : GameEvent
    {
        public NameWithUUID PromotedPlayer;

        public ServerOppedPlayerEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = Source.Body;
            PromotedPlayer.Name = check.Substring(6, check.Length - 6);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(PromotedPlayer.Name, Source.Time);
            PromotedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
