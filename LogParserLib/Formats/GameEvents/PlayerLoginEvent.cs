using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class PlayerLoginEvent : GameEvent
    {
        public NameWithUUID Player;
        public string PlayerAddress;
        public string PlayerIP;
        public string PlayerPort;
        public string PlayerEntityID;
        public Location PlayerLocation;

        public PlayerLoginEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.IndexOf("] logged in with entity id ");
            int spot2 = check.LastIndexOf('[', spot - 1);
            Player.Name = check.Substring(0, spot2);

            PlayerAddress = check.Substring(spot2 + 2, spot - spot2 - 2);
            spot2 = PlayerAddress.IndexOf(':');
            PlayerIP = PlayerAddress.Substring(0, spot2);
            spot2++;
            PlayerPort = PlayerAddress.Substring(spot2, PlayerAddress.Length - spot2);

            spot += "] logged in with entity id ".Length;
            spot2 = check.IndexOf(' ', spot + 1);
            PlayerEntityID = check.Substring(spot, spot2 - spot);

            spot = check.IndexOf('[', spot2) + 1;
            spot2 = check.IndexOf(']', spot);
            PlayerLocation.World = check.Substring(spot, spot2 - spot);

            spot2 += (check[spot2 + 1] == ' ') ? 2 : 1;
            spot = check.IndexOf(',', spot2);
            PlayerLocation.X = double.Parse(check.Substring(spot2, spot - spot2));

            spot += 2;
            spot2 = check.IndexOf(',', spot);
            PlayerLocation.Y = double.Parse(check.Substring(spot, spot2 - spot));

            spot2 += 2;
            spot = check.IndexOf(')', spot2);
            PlayerLocation.Z = double.Parse(check.Substring(spot2, spot - spot2));
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(Player.Name, Source.Time);
            Player.UUID = (UUID != null) ? UUID : "";
        }
    }
}
