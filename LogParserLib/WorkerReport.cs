using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Simple means to report data from a BackgroundWorker
    public struct WorkerReport
    {
        public string Message;
        public bool WithNewline;
        public bool WithLead;
        public int CursorLeft;
        public long ReportNum;

        private static long NextReportNum = 0;

        public WorkerReport(string message, bool withNewline = true, bool withLead = true, int cursorLeft = -1)
        {
            Message = message;
            WithNewline = withNewline;
            WithLead = withLead;
            CursorLeft = cursorLeft;

            ReportNum = NextReportNum;
            NextReportNum++;
        }
    }
}
