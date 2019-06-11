using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class PlayerJoinEvent : GameEvent
    {
        public NameWithUUID Player;

        public PlayerJoinEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            int spot = check.LastIndexOf(' ');
            Player.UUID = check.Substring(spot + 1, check.Length - spot - 1);

            int spot2 = check.LastIndexOf(' ', spot - 1);
            spot = check.IndexOf("UUID of player ") + 15;
            Player.Name = check.Substring(spot, spot2 - spot);
        }
    }
}
