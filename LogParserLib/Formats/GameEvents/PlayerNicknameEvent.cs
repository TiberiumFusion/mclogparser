using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // I can't remember if /nick is a craftbukkit or spigot thing, but it's not an external plugin so there shouldn't be any syntax variation
    public class PlayerNicknameEvent : PlayerCommandEvent
    {
        public NameWithUUID TargetPlayer;
        public string AssignedPlayerNickname;

        public PlayerNicknameEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            string[] cuts = Command.Split(' '); // [0] is "/nick", [1] is target/nickname, [2] is nickname

            if (cuts.Length == 2)
            {
                TargetPlayer = ExecutingPlayer;
                AssignedPlayerNickname = cuts[1];
            }
            else if (cuts.Length > 2)
            {
                TargetPlayer.Name = cuts[1];
                AssignedPlayerNickname = cuts[2];
            }
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            base.UUIDPass(analyzedData);

            if (TargetPlayer.UUID == null)
            {
                string UUID = analyzedData.FindBestUUIDMatchFor(TargetPlayer.Name, Source.Time);
                TargetPlayer.UUID = (UUID != null) ? UUID : "";
            }
        }
    }
}
