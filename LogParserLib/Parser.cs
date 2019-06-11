using com.tiberiumfusion.minecraft.logparserlib.Formats;
using System;
using System.ComponentModel;
using System.IO;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Reads in the raw data from log files and performs basic anaylsis and formatting
    public class Parser
    {
        public static void ParseFromFilepaths(string[] filePaths, out AnalyzedData output, BackgroundWorker executor = null)
        {
            bool doLogging = (executor != null);

            DateTime start = DateTime.Now;

            output = null;

            // Begin
            AnalyzedData workingOuput = new AnalyzedData();

            // Read in logs from file, performing basic, initial analysis and formatting
            int grandLogLineTotal = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                string path = filePaths[i];
                if (File.Exists(path))
                {
                    try
                    {
                        if (doLogging)
                            reportProgress(executor, new WorkerReport("Reading path \"" + path + "\" ... ", false));
                        DecoratedLog dlog = new DecoratedLog(path);
                        grandLogLineTotal += dlog.LogLines.DissectedLines.Count;
                        workingOuput.DecoratedLogs.Add(dlog);
                        if (doLogging)
                            reportProgress(executor, new WorkerReport("success", true, false));
                    }
                    catch (Exception e)
                    {
                        reportProgress(executor, new WorkerReport("failed: path could not be read/parsed"));
                    }
                }
                else
                {
                    if (doLogging)
                        reportProgress(executor, new WorkerReport("failed: path does not exist"));
                }
            }

            // Record basic calculations
            workingOuput.GrandLogLineTotal = grandLogLineTotal;

            reportProgress(executor, new WorkerReport("Parsing completed in " + (DateTime.Now - start).ToString()));

            // Yield parsed data back to invoker
            output = workingOuput;
        }

        private static void reportProgress(BackgroundWorker executor, WorkerReport report)
        {
            if (executor != null)
                executor.ReportProgress(0, report);
        }
    }
}
