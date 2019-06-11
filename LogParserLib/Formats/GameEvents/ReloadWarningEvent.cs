using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ReloadWarningEvent : CommandResultEvent
    {
        public string WarningMessage = "";
        public bool IsSecondMessage = false; // False for first reload warning message, true for second message
        
        public ReloadWarningEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            base.parse();

            WarningMessage = Source._AnalysisExtra.Text;
            IsSecondMessage = Source._AnalysisExtra.SecondMessage;
        }
    }
}
