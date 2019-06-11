using com.tiberiumfusion.minecraft.logparserlib.Formats;
using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Processes an AnalyzedData object into an ExportData object with respect to an ExporterOptions object (in the ExportData)
    public class Exporter
    {
        public static void ProcessData(AnalyzedData inputData, ExportData outputData, BackgroundWorker executor = null)
        {
            bool doLogging = (executor != null);

            // Link analysis stats
            outputData.StatsFromAnalysis = inputData.Stats;

            // Stringify catalog format enum for human-reading
            outputData.CatalogFormat = Enum.GetName(typeof(ExportCatalogFormat), outputData.Options.CatalogFormat);

            //////////////////////////////////////////// Build logline catalog ////////////////////////////////////////////
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building LogLine catalog ... ", false, true, 0));
            outputData.GrandLogLineTotal = inputData.GrandLogLineTotal;
            float total = (float)outputData.GrandLogLineTotal;
            int current = 0;
            outputData.LogLineLookup.Clear();
            outputData.DecoratedLogLookup.Clear();
            long workingID = 0;
            long workingIDdlog = 0;
            DateTime lastReport = DateTime.Now;
            for (int i = 0; i < inputData.DecoratedLogs.Count; i++)
            {
                DecoratedLog dlog = inputData.DecoratedLogs[i];
                dlog.E_ID = workingIDdlog;

                outputData.DecoratedLogLookup[dlog] = workingIDdlog;
                if (outputData.Options.CatalogFormat == ExportCatalogFormat.Maps)
                    outputData.DecoratedLogCatalogMap[workingIDdlog] = dlog;
                else if (outputData.Options.CatalogFormat == ExportCatalogFormat.Lists)
                    outputData.DecoratedLogCatalogList.Add(dlog);

                workingIDdlog++;

                for (int k = 0; k < dlog.LogLines.DissectedLines.Count; k++)
                {
                    if (doLogging)
                    {
                        if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                        {
                            string logProgress = (((float)(i + 1) / total) * 100.0f).ToString("N2") + "%";
                            reportProgress(executor, new WorkerReport("Building LogLine catalog ... " + logProgress + "      ", false, true, 0));
                            lastReport = DateTime.Now;
                        }
                    }

                    LogLine ll = dlog.LogLines.DissectedLines[k];
                    ll.E_ID = workingID;
                    outputData.LogLineLookup[ll] = workingID;
                    workingID++;

                    current++;
                }

                // And tag each dlog with the exporteroptions
                dlog.E_Options = outputData.Options;
            }
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building LogLine catalog ... done      ", true, true, 0));

            //////////////////////////////////////////// Build GameEvent catalog ////////////////////////////////////////////
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building GameEvent catalog ... ", false, true, 0));
            total = (float)inputData.GameEvents.Count;
            outputData.GameEventLookup.Clear();
            workingID = 0;
            lastReport = DateTime.Now;
            for (int i = 0; i < inputData.GameEvents.Count; i++)
            {
                if (doLogging)
                {
                    if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                    {
                        string logProgress = (((float)(i + 1) / total) * 100.0f).ToString("N2") + "%";
                        reportProgress(executor, new WorkerReport("Building GameEvent catalog ... " + logProgress + "      ", false, true, 0));
                        lastReport = DateTime.Now;
                    }
                }

                GameEvent ge = inputData.GameEvents[i];
                ge.E_ID = workingID;

                // Can fill in logline IDs at this point
                ge.SourceID = outputData.LogLineLookup[ge.Source];

                // And also do work on FormattedChatStrings
                if (outputData.Options.FormattedChatString_UseHTMLFormatting)
                {
                    PlayerChatEvent chatGE = (ge as PlayerChatEvent);
                    if (chatGE != null)
                        chatGE.BuildHTMLText(outputData.Options.FormattedChatString_HTMLTag, outputData.Options.FormattedChatString_HTMLClassForMagic);
                }

                outputData.GameEventLookup[ge] = workingID;
                if (outputData.Options.CatalogFormat == ExportCatalogFormat.Maps)
                    outputData.GameEventCatalogMap[workingID] = ge;
                else if (outputData.Options.CatalogFormat == ExportCatalogFormat.Lists)
                    outputData.GameEventCatalogList.Add(ge);

                workingID++;

                // And tag each GE with the exporteroptions
                ge.E_Options = outputData.Options;
                ge.PropogateExportOptions();
            }
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building GameEvent catalog ... done      ", true, true, 0));

            //////////////////////////////////////////// Build PlayerSession catalog ////////////////////////////////////////////
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building PlayerSession catalog ... ", false, true, 0));
            total = 0f;
            foreach (string uuidKey in inputData.AllPlayerStats.Keys)
                total += (float)inputData.AllPlayerStats[uuidKey].Sessions.Count;
            current = 0;
            outputData.PlayerSessionLookup.Clear();
            workingID = 0;
            lastReport = DateTime.Now;
            foreach (string uuidKey in inputData.AllPlayerStats.Keys)
            {
                PlayerStats stats = inputData.AllPlayerStats[uuidKey];
                for (int i = 0; i < stats.Sessions.Count; i++)
                {
                    if (doLogging)
                    {
                        if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                        {
                            string logProgress = (((float)(current + 1) / total) * 100.0f).ToString("N2") + "%";
                            reportProgress(executor, new WorkerReport("Building PlayerSession catalog ... " + logProgress + "      ", false, true, 0));
                            lastReport = DateTime.Now;
                        }
                    }

                    PlayerSession session = stats.Sessions[i];
                    session.E_ID = workingID;

                    // Can fill in IDs for these 6 lists at this point. ConcurrentServerSessionID will require a second pass after the ServerSession catalog is built.
                    session.AllConcurrentGameEventIDs.Clear();
                    foreach (GameEvent ge in session.AllConcurrentGameEvents)
                        session.AllConcurrentGameEventIDs.Add(outputData.GameEventLookup[ge]);

                    session.AllRelevantGameEventIDs.Clear();
                    foreach (GameEvent ge in session.AllRelevantGameEvents)
                        session.AllRelevantGameEventIDs.Add(outputData.GameEventLookup[ge]);

                    session.SourceLogFileIDs.Clear();
                    foreach (DecoratedLog dl in session.SourceLogFiles)
                        session.SourceLogFileIDs.Add(outputData.DecoratedLogLookup[dl]);

                    session.ChatEventIDs.Clear();
                    foreach (GameEvent ge in session.ChatEvents)
                        session.ChatEventIDs.Add(outputData.GameEventLookup[ge]);

                    session.DeathEventIDs.Clear();
                    foreach (GameEvent ge in session.DeathEvents)
                        session.DeathEventIDs.Add(outputData.GameEventLookup[ge]);

                    session.CommandEventIDs.Clear();
                    foreach (GameEvent ge in session.CommandEvents)
                        session.CommandEventIDs.Add(outputData.GameEventLookup[ge]);
                        
                    outputData.PlayerSessionLookup[session] = workingID;
                    if (outputData.Options.CatalogFormat == ExportCatalogFormat.Maps)
                        outputData.PlayerSessionCatalogMap[workingID] = session;
                    else if (outputData.Options.CatalogFormat == ExportCatalogFormat.Lists)
                        outputData.PlayerSessionCatalogList.Add(session);
                    workingID++;

                    current++;

                    // And tag each PS with the exporteroptions
                    session.E_Options = outputData.Options;
                }
            }
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building PlayerSession catalog ... done      ", true, true, 0));


            //////////////////////////////////////////// Build ServerSession catalog ////////////////////////////////////////////
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building ServerSession catalog ... ", false, true, 0));
            total = (float)inputData.ServerSessions.Count;
            outputData.ServerSessionLookup.Clear();
            workingID = 0;
            lastReport = DateTime.Now;
            for (int i = 0; i < inputData.ServerSessions.Count; i++)
            {
                if (doLogging)
                {
                    if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                    {
                        string logProgress = (((float)(i + 1) / total) * 100.0f).ToString("N2") + "%";
                        reportProgress(executor, new WorkerReport("Building ServerSession catalog ... " + logProgress + "      ", false, true, 0));
                        lastReport = DateTime.Now;
                    }
                }

                ServerSession session = inputData.ServerSessions[i];
                session.E_ID = workingID;

                // Can fill in all ID lists for Server Session at this point
                session.AllGameEventIDs.Clear();
                foreach (GameEvent ge in session.AllGameEvents)
                    session.AllGameEventIDs.Add(outputData.GameEventLookup[ge]);

                session.BootGameEventIDs.Clear();
                foreach (GameEvent ge in session.BootGameEvents)
                    session.BootGameEventIDs.Add(outputData.GameEventLookup[ge]);

                session.RunningGameEventIDs.Clear();
                foreach (GameEvent ge in session.RunningGameEvents)
                    session.RunningGameEventIDs.Add(outputData.GameEventLookup[ge]);

                session.ShutdownGameEventIDs.Clear();
                foreach (GameEvent ge in session.ShutdownGameEvents)
                    session.ShutdownGameEventIDs.Add(outputData.GameEventLookup[ge]);

                session.SourceLogFileIDs.Clear();
                foreach (DecoratedLog dl in session.SourceLogFiles)
                    session.SourceLogFileIDs.Add(outputData.DecoratedLogLookup[dl]);

                session.PlayerChatEventIDs.Clear();
                foreach (GameEvent ge in session.PlayerChatEvents)
                    session.PlayerChatEventIDs.Add(outputData.GameEventLookup[ge]);

                session.PlayerDeathEventIDs.Clear();
                foreach (GameEvent ge in session.PlayerDeathEvents)
                    session.PlayerDeathEventIDs.Add(outputData.GameEventLookup[ge]);

                session.PlayerCommandEventIDs.Clear();
                foreach (GameEvent ge in session.PlayerCommandEvents)
                    session.PlayerCommandEventIDs.Add(outputData.GameEventLookup[ge]);

                session.ConcurrentPlayerSessionIDs.Clear();
                foreach (string uuidKey in session.ConcurrentPlayerSessions.Keys)
                {
                    session.ConcurrentPlayerSessionIDs[uuidKey] = new List<long>();
                    foreach (PlayerSession ps in session.ConcurrentPlayerSessions[uuidKey])
                        session.ConcurrentPlayerSessionIDs[uuidKey].Add(outputData.PlayerSessionLookup[ps]);
                }

                outputData.ServerSessionLookup[session] = workingID;
                if (outputData.Options.CatalogFormat == ExportCatalogFormat.Maps)
                    outputData.ServerSessionCatalogMap[workingID] = session;
                else if (outputData.Options.CatalogFormat == ExportCatalogFormat.Lists)
                    outputData.ServerSessionCatalogList.Add(session);

                workingID++;

                // And tag each SS with the exporteroptions
                session.E_Options = outputData.Options;
            }
            if (doLogging)
                reportProgress(executor, new WorkerReport("Building ServerSession catalog ... done      ", true, true, 0));


            //////////////////////////////////////////// Finalize PlayerSession catalog ////////////////////////////////////////////
            // Now that the serversession IDs have been created, we can finally assign the ConcurrentServerSessionID field
            if (doLogging)
                reportProgress(executor, new WorkerReport("Finalizing PlayerSession catalog ... ", false, true, 0));
            total = 0f;
            foreach (string uuidKey in inputData.AllPlayerStats.Keys)
                total += (float)inputData.AllPlayerStats[uuidKey].Sessions.Count;
            current = 0;
            lastReport = DateTime.Now;
            foreach (string uuidKey in inputData.AllPlayerStats.Keys)
            {
                PlayerStats stats = inputData.AllPlayerStats[uuidKey];
                for (int i = 0; i < stats.Sessions.Count; i++)
                {
                    if (doLogging)
                    {
                        if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                        {
                            string logProgress = (((float)(current + 1) / total) * 100.0f).ToString("N2") + "%";
                            reportProgress(executor, new WorkerReport("Finalizing PlayerSession catalog ... " + logProgress + "      ", false, true, 0));
                            lastReport = DateTime.Now;
                        }
                    }

                    PlayerSession session = stats.Sessions[i];

                    // Fill in ID for ConcurrentServerSession
                    if (session.ConcurrentServerSession != null)
                        session.ConcurrentServerSessionID = outputData.ServerSessionLookup[session.ConcurrentServerSession];
                    else
                        session.ConcurrentServerSessionID = -1;
                    
                    current++;
                }
            }
            // Link directly
            outputData.AllPlayerStats = inputData.AllPlayerStats;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Finalizing PlayerSession catalog ... done      ", true, true, 0));


            //////////////////////////////////////////// Prepare AllPlayerStats collection ////////////////////////////////////////////
            // Now that the serversession IDs have been created, we can finally assign the ConcurrentServerSessionID field
            if (doLogging)
                reportProgress(executor, new WorkerReport("Preparing AllPlayerStats collection ... ", false, true, 0));
            total = (float)inputData.AllPlayerStats.Count;
            workingID = 0;
            current = 0;
            lastReport = DateTime.Now;
            foreach (string uuidKey in inputData.AllPlayerStats.Keys)
            {
                if (doLogging)
                {
                    if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                    {
                        string logProgress = (((float)(current + 1) / total) * 100.0f).ToString("N2") + "%";
                        reportProgress(executor, new WorkerReport("Preparing AllPlayerStats collection ... " + logProgress + "      ", false, true, 0));
                        lastReport = DateTime.Now;
                    }
                }

                PlayerStats stats = inputData.AllPlayerStats[uuidKey];
                stats.E_ID = workingID;

                workingID++;

                stats.SessionIDs.Clear();
                foreach (PlayerSession ps in stats.Sessions)
                    stats.SessionIDs.Add(outputData.PlayerSessionLookup[ps]);

                // And tag each playerstats with the exporteroptions
                stats.E_Options = outputData.Options;

                current++;
            }
            // Directly link to outputdata
            outputData.AllPlayerStats = inputData.AllPlayerStats;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Preparing AllPlayerStats collection ... done      ", true, true, 0));

            ///// Prep work is done
            if (doLogging)
                reportProgress(executor, new WorkerReport("Export prep complete"));
        }

        private static void reportProgress(BackgroundWorker executor, WorkerReport report)
        {
            if (executor != null)
                executor.ReportProgress(0, report);
        }
    }
}
