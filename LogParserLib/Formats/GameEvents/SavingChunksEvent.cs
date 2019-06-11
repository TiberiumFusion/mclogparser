using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class SavingChunksEvent : GameEvent
    {
        public string LevelName = "";

        public SavingChunksEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;
            LevelName = check.Substring(24, check.Length - 24);
        }
    }
}
