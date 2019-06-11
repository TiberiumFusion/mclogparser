using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class ServerDeoppedPlayerEvent : CommandResultEvent
    {
        public NameWithUUID DemotedPlayer;

        public ServerDeoppedPlayerEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = Source.Body;
            DemotedPlayer.Name = check.Substring(9, check.Length - 9);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(DemotedPlayer.Name, Source.Time);
            DemotedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
