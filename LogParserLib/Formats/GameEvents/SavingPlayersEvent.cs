﻿using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    class SavingPlayersEvent : GameEvent
    {
        public SavingPlayersEvent(LogLine source) : base(source) { }

        protected override void parse()
        {

        }
    }
}
