using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class IngameWhitelistAddEvent : IngameCommandResultEvent
    {
        public NameWithUUID WhitelistedPlayer;

        public IngameWhitelistAddEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string check = ResultMessage;
            int spot = check.IndexOf("Added ") + 6;
            int spot2 = check.IndexOf(' ', spot + 1);

            WhitelistedPlayer.Name = check.Substring(spot, spot2 - spot);
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            string UUID = analyzedData.FindBestUUIDMatchFor(WhitelistedPlayer.Name, Source.Time);
            WhitelistedPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
