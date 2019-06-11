using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerWhitelistRemoveEvent : CommandResultEvent
    {
        public NameWithUUID UnwhitelistedPlayer;

        public ServerWhitelistRemoveEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = ResultMessage;
            int spot = check.IndexOf("Removed ") + 8;
            int spot2 = check.IndexOf(' ', spot + 1);

            UnwhitelistedPlayer.Name = check.Substring(spot, spot2 - spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(UnwhitelistedPlayer.Name, Source.Time);
            UnwhitelistedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
