using com.tiberiumfusion.minecraft.logparserlib.Formats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Analyzes an AnalyzedData object to add lots of context to the raw data
    // Calculations and conclusions:
    // 1. Time range for each DecoratedLog object (aka each physical log file on the disk)
    // 2. Session and event histories for each player over the entire server's existence
    //    a. Player sessions that include...
    //    b. Start and stop times
    //    c. Player deaths
    //    d. Player messages, both outgoing and incoming
    //    e. Player commands (they are registered separately from chat messages)
    // 3. Server history over the entire server's existence (as provided by the log files)
    //    a. Server sessions

    public class Analyzer
    {
        private static readonly Regex ID_ServerBootDone1 = new Regex("^(Done \\().+(\\)! For help, type \"help\")$");
        private static readonly Regex ID_ServerBootDone2 = new Regex("^(Done \\().+(\\)! For help, type \"help\" or \"\\?\")$");
        private static readonly Regex ID_PlayerLogin = new Regex("^.+(] logged in with entity id ).+( at \\().+$");
        private static readonly Regex ID_OldPlayerDeath = new Regex("^.+( died)$"); // 1.8
        private static readonly Regex ID_NewPlayerDeath = new Regex("^.+( has died a gruesome and painful death!)$"); // Circa 1.10, I think
        private static readonly Regex ID_ServerWhitelistAdd = new Regex("^(Added ).+( to the whitelist)$");
        private static readonly Regex ID_ServerWhitelistRemove = new Regex("^(Removed ).+( from the whitelist)$");

        public static void ProcessData(AnalyzedData analyzedData, AnalyzerOptions analyzerOptions, BackgroundWorker executor = null)
        {
            bool doLogging = (executor != null);

            // Take note of what options were used
            analyzedData.Stats.AnalyzerOptionsUsed = analyzerOptions;

            // Mark time start
            analyzedData.Stats.AnalysisTime.Start = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Analysis began at: " + analyzedData.Stats.AnalysisTime.Start.ToString()));

            //////////////////////////////////////////// PRELIMINARY PASS ////////////////////////////////////////////
            analyzedData.Stats.FirstPassTime.Start = DateTime.Now;
            ///// #1
            if (doLogging)
                reportProgress(executor, new WorkerReport("Calculating log time ranges"));
            foreach (DecoratedLog dlog in analyzedData.DecoratedLogs)
                dlog.Analyze_TimeRange();
            // Now re-sort the DecoratedLogs by their start dates so that they are in chronological order
            analyzedData.DecoratedLogs = analyzedData.DecoratedLogs.OrderBy(x => x.TimeRange.Start).ToList();

            ///// #2 and #3 foundation
            // We can now start to iterate through every line of every log file and start collecting, tagging, and marking events
            if (doLogging)
                reportProgress(executor, new WorkerReport("Creating event record"));

            ConcurrentDictionary<string, byte> workingPlayerNames = new ConcurrentDictionary<string, byte>(); // For death message, chat message, and command result analysis, and more
            List<LogLine> unmatchedLines = new List<LogLine>();

            // Fields for logging progress in an efficient manner
            int totalLogs = analyzedData.DecoratedLogs.Count;
            int currentLog = 0;
            float totalLines = 0;
            float currentLine = 0;
            string clearance = "              ";
            DateTime lastReport = DateTime.Now;
            string reportLogNum = "";
            string reportFront = "";
            string reportFrontP = "";
            ///// Each DecoratedLog is processed serially, but the LogLines of each DecoratedLog can be processed in parallel 
            for (int i = 0; i < analyzedData.DecoratedLogs.Count; i++)
            {
                DecoratedLog dlog = analyzedData.DecoratedLogs[i];
                currentLog++;

                currentLine = 0.0f;
                totalLines = (float)dlog.LogLines.DissectedLines.Count;

                if (doLogging)
                {
                    string total = totalLogs.ToString();
                    reportLogNum = currentLog.ToString().PadLeft(total.Length) + "/" + total;
                    reportFront = "    - [" + reportLogNum + "] Processing: " + dlog.SourceFilenameNoExt + " ... ";
                    
                    reportProgress(executor, new WorkerReport(reportFront + clearance, false, false, 0));
                }
                lastReport = DateTime.Now;

                //////////////////////////////////////////// SERIAL vs PARALLEL SWITCH ////////////////////////////////////////////
                
                ///// Parallel processing is typically slower than serial proessing if your log files are small
                // You may only get worthwhile performance from parallel mode if the majority of your log files are very long (i.e. over 1MB uncompressed)
                // This mode will probably perform better for large batches of massive server logs
                if (analyzerOptions.ProcessLogLinesInParallel)
                {
                    ///// First parallel pass (assume workingPlayerNames record is incomplete the entire time)
                    // The event collection process can be parallelized rather easily, but events that require the workingPlayerNames record
                    // in order to ID must wait until all other events have been collected (and player names gathered)
                    unmatchedLines.Clear(); // List of lines that did not ID (and so are possibly lines that depend on the workingPlayerNames record)
                    reportFrontP = reportFront + " Pass 1/2: ";
                    Parallel.For(0, dlog.LogLines.DissectedLines.Count, k =>
                    {
                        analyzeLogLine(dlog.LogLines.DissectedLines, k, ref currentLine, ref totalLines,
                                       workingPlayerNames, unmatchedLines, ref lastReport, ref reportFrontP, ref clearance, ref doLogging,
                                       analyzedData, analyzerOptions, executor);
                    });

                    ///// Second parallel pass (workingPlayerNames record is now complete)
                    // Go through the unmatchedLines and try to ID them again
                    reportFrontP = reportFront + " Pass 2/2: ";
                    currentLine = 0.0f;
                    totalLines = (float)unmatchedLines.Count;
                    Parallel.For(0, unmatchedLines.Count, k =>
                    {
                        analyzeLogLine(unmatchedLines, k, ref currentLine, ref totalLines,
                                       workingPlayerNames, null, ref lastReport, ref reportFrontP, ref clearance, ref doLogging,
                                       analyzedData, analyzerOptions, executor);
                    });

                    // Fix the sorting of the GameEvents b/c of parallel processing
                    analyzedData.GameEvents = analyzedData.GameEvents.OrderBy(x => x.Source.Time).ToList();
                }

                ///// Serial processing is generally a good bit faster than parallel processing if your log files are small
                // You should most typically use this mode, especially if your log files are of normal size (i.e. less than 1MB uncompressed)
                // This mode will probably perform better for smaller servers with smaller log files
                else
                {
                    for (int k = 0; k < dlog.LogLines.DissectedLines.Count; k++)
                    {
                        //try
                        //{
                        analyzeLogLine(dlog.LogLines.DissectedLines, k, ref currentLine, ref totalLines,
                                       workingPlayerNames, null, ref lastReport, ref reportFront, ref clearance, ref doLogging,
                                       analyzedData, analyzerOptions, executor);
                        //}
                        //catch (Exception e) { }
                    }
                }


                if (doLogging)
                    reportProgress(executor, new WorkerReport(reportFront + "done" + clearance, true, false, 0));
            }
            analyzedData.Stats.FirstPassTime.End = DateTime.Now;

            //////////////////////////////////////////// PlayerStats "FIRST PASS" ////////////////////////////////////////////
            ///// Player sessions first pass
            analyzedData.Stats.PlayerStatsFirstPassTime.Start = DateTime.Now;
            // Gather up all players' UUIDs before anything else
            if (doLogging)
                reportProgress(executor, new WorkerReport("Assembling player session data, first pass"));
            Dictionary<string, List<int>> joinEventIndices = new Dictionary<string, List<int>>(); // Key'd indices for each players' join event
            for (int i = 0; i < analyzedData.GameEvents.Count; i++)
            {
                GameEvent ge = analyzedData.GameEvents[i];

                PlayerJoinEvent joinGE = ge as PlayerJoinEvent;
                if (joinGE != null)
                {
                    if (!joinEventIndices.ContainsKey(joinGE.Player.UUID))
                        joinEventIndices[joinGE.Player.UUID] = new List<int>();
                    joinEventIndices[joinGE.Player.UUID].Add(i);
                }
            }
            // Create session records and stats for each player
            foreach (string playerUUID in joinEventIndices.Keys)
            {
                analyzedData.AllPlayerStats[playerUUID] = new PlayerStats();
                List<int> indices = joinEventIndices[playerUUID];
                foreach (int index in indices)
                {
                    PlayerSession session = new PlayerSession(playerUUID);
                    session.PopulateFromGameEventsFirstPass(analyzedData.GameEvents, index);
                    analyzedData.AllPlayerStats[playerUUID].Sessions.Add(session);
                }
                analyzedData.AllPlayerStats[playerUUID].ConsolidatePlayerContemporaryNames();
            }
            analyzedData.Stats.PlayerStatsFirstPassTime.End = DateTime.Now;

            //////////////////////////////////////////// PlayerStats "UUID PASS" ////////////////////////////////////////////
            // Using the basic PlayerSession data just created, determine player UUIDs and add them to all relevant GameEvents
            analyzedData.Stats.PlayerStatsUUIDPassTime.Start = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Determining player UUIDs ... ", false, true, 0));
            float totalGECount = (float)analyzedData.GameEvents.Count;
            for (int i = 0; i < analyzedData.GameEvents.Count; i++)
            {
                if (doLogging)
                {
                    if ((DateTime.Now - lastReport).TotalMilliseconds > 100)
                    {
                        string logProgress = (((float)(i + 1) / totalGECount) * 100.0f).ToString("N2") + "%";
                        reportProgress(executor, new WorkerReport("Determining player UUIDs ... " + logProgress + "      ", false, true, 0));
                        lastReport = DateTime.Now;
                    }
                }

                GameEvent ge = analyzedData.GameEvents[i];
                ge.UUIDPass(analyzedData);
            }
            analyzedData.Stats.PlayerStatsUUIDPassTime.End = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Determining player UUIDs ... done      ", true, true, 0));

            //////////////////////////////////////////// PlayerStats "SECOND PASS" ////////////////////////////////////////////
            // With the player UUIDs to all GameEvents (hopefully) filled in, we can perform the second pass over session data
            analyzedData.Stats.PlayerStatsSecondPassTime.Start = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Assembling player session data, second pass"));
            foreach (string playerUUID in analyzedData.AllPlayerStats.Keys)
            {
                PlayerStats stats = analyzedData.AllPlayerStats[playerUUID];
                foreach (PlayerSession session in stats.Sessions)
                    session.PopulateSecondPass();
            }
            analyzedData.Stats.PlayerStatsSecondPassTime.End = DateTime.Now;

            //////////////////////////////////////////// ServerSession PASS ////////////////////////////////////////////
            // With the PlayerSessions constructed, we can now create the ServerSession records
            analyzedData.Stats.ServerSessionsPassTime.Start = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Assembling server session data"));
            for (int i = 0; i < analyzedData.GameEvents.Count; i++)
            {
                GameEvent ge = analyzedData.GameEvents[i];

                ServerStartEvent startGE = ge as ServerStartEvent;
                if (startGE != null)
                {
                    ServerSession session = new ServerSession();
                    session.PopulateFromGameEvents(analyzedData, i);
                    analyzedData.ServerSessions.Add(session);
                }
            }
            analyzedData.Stats.ServerSessionsPassTime.End = DateTime.Now;

            //////////////////////////////////////////// PlayerSession up-link to concurrent ServerSession PASS ////////////////////////////////////////////
            // With the ServerSessions constructed, we can go back through the PlayerSessions and link them to their concurrent ServerSession
            analyzedData.Stats.SessionLinkingPassTime.Start = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Linking player/server session data"));
            foreach (string playerUUID in analyzedData.AllPlayerStats.Keys)
            {
                PlayerStats stats = analyzedData.AllPlayerStats[playerUUID];
                foreach (PlayerSession ps in stats.Sessions)
                {
                    for (int i = analyzedData.ServerSessions.Count - 1; i >= 0; i--)
                    {
                        ServerSession ss = analyzedData.ServerSessions[i];
                        if (ps.Range.Start >= ss.Range.Start)
                            ps.ConcurrentServerSession = ss;
                    }
                }
            }
            analyzedData.Stats.SessionLinkingPassTime.End = DateTime.Now;

            // Mark time end
            analyzedData.Stats.AnalysisTime.End = DateTime.Now;
            if (doLogging)
                reportProgress(executor, new WorkerReport("Analysis finished at: " + analyzedData.Stats.AnalysisTime.End.ToString() + ", total time: " + analyzedData.Stats.AnalysisTime.Duration.ToString()));
        }

        //////////////////////////////////////////// EVENT ANALYSIS PROCEDURES ////////////////////////////////////////////
        ///// The per-log analyzer
        private static void analyzeLogLine(List<LogLine> lines, int lineIndex, ref float currentLine, ref float totalLines,
                                             ConcurrentDictionary<string, byte> workingPlayerNames, List<LogLine> unmatchedLines,
                                             ref DateTime lastReport, ref string reportFront, ref string clearance, ref bool doLogging,
                                             AnalyzedData analyzedData, AnalyzerOptions analyzerOptions, BackgroundWorker executor = null)
        {
            //try
            //{
            LogLine line = lines[lineIndex];
            GameEvent ge = analyzeForEvent(line, workingPlayerNames, unmatchedLines, analyzerOptions);
            if (ge != null)
            {
                lock (analyzedData.GameEvents)
                {
                    analyzedData.GameEvents.Add(ge);
                }

                
                ///// Gather as seen player contemporary names
                // For more advanced analysis procedures

                PlayerJoinEvent joinGE = ge as PlayerJoinEvent;
                if (joinGE != null)
                    workingPlayerNames[joinGE.Player.Name] = 1;

                PlayerNicknameEvent nicknameGE = ge as PlayerNicknameEvent;
                if (nicknameGE != null)
                    workingPlayerNames[nicknameGE.AssignedPlayerNickname] = 1;
            }

            currentLine += 1.0f;
            if (doLogging)
            {
                if ((DateTime.Now - lastReport).TotalMilliseconds > 100) // Push requests to the main thread no faster than every 100ms to prevent mass slowdown
                {
                    string logProgress = ((currentLine / totalLines) * 100.0f).ToString("N2") + "%";
                    reportProgress(executor, new WorkerReport(reportFront + logProgress + clearance, false, false, 0));
                    lastReport = DateTime.Now;
                }
            }
            //}
            //catch (Exception e) { }
        }
        ///// The per-line analyzer
        private static GameEvent analyzeForEvent(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames, List<LogLine> unmatchedLines, AnalyzerOptions analyzerOptions)
        {
            bool ignoreCommonTags = analyzerOptions.IgnoreCommonTags;

            if (line.IsLikelyException) // The Parser assigned this field
                return new JavaExceptionEvent(line);
            
            if (line.Body.Length >= 33 && line.Body.Substring(0, 33) == "Starting minecraft server version" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerStartEvent(line);

            else if (line.Body.Length >= 19 && line.Body.Substring(0, 19) == "Default game type: " && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerGametypeEvent(line);

            else if (line.Body.Length >= 22 && line.Body.Substring(0, 22) == "This server is running" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerVersionEvent(line);

            else if (line.Body.Length >= 28 && line.Body.Substring(0, 28) == "Starting Minecraft server on" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerAddressEvent(line);

            else if (line.Body.Length >= 17 && line.Body.Substring(0, 17) == "Custom Map Seeds:" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerCustomSeedsEvent(line);

            else if (line.Body.Length >= 33 && line.Body.Substring(0, 33) == "Preparing start region for level " && line.Body.Contains(" (Seed: ") && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerLevelLoadEvent(line);

            else if (line.Body.Length >= 23 && line.Body.Substring(0, 23) == "Saving chunks for level" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new SavingChunksEvent(line);

            else if (line.Body.Length >= 14 && line.Body.Substring(0, 14) == "Can't keep up!" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new CantKeepUpEvent(line);

            else if (line.Body.Length >= 5 && line.Body.Substring(0, 5) == "Opped" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerOppedPlayerEvent(line);

            else if (line.Body.Length >= 8 && line.Body.Substring(0, 8) == "De-opped" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerDeoppedPlayerEvent(line);

            else if (line.Body.Length >= 14 && line.Body.Substring(0, 14) == "[NoCheatPlus] ")
                return new NoCheatPlusEvent(line);

            else if (line.Body == "Saving players" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new SavingPlayersEvent(line);

            else if (line.Body == "Saving worlds" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new SavingWorldsEvent(line);

            else if (line.Body == "Turned on the whitelist" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerWhitelistEnableEvent(line);

            else if (line.Body == "Turned off the whitelist" && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerWhitelistDisableEvent(line);

            else if (line.Body.Length >= 14 && line.Body.Substring(0, 14) == "UUID of player" && (ignoreCommonTags || line.Tag.Contains("User Authenticator")))
                return new PlayerJoinEvent(line);

            else if (((line.Body.Length >= 14 && line.Body.Substring(line.Body.Length - 14, 14) == " left the game")
                      || (line.Body.Length >= 15 && line.Body.Substring(line.Body.Length - 15, 15) == " left the game."))
                     && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new PlayerLeaveEvent(line);

            else if (analysisPlayerLostConn(line, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new PlayerLostConnEvent(line);

            else if (ID_PlayerLogin.IsMatch(line.Body) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new PlayerLoginEvent(line);

            else if (((line.Body.Length == 19 && line.Body == "Stopping the server")
                      || (line.Body.Length == 15 && line.Body == "Stopping server"))
                     && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerStoppingEvent(line);

            else if ((ID_ServerBootDone1.IsMatch(line.Body) || ID_ServerBootDone2.IsMatch(line.Body)) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerBootDoneEvent(line);

            else if (analysisReloadWarning(line, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ReloadWarningEvent(line);

            else if (analysisReloadComplete(line, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ReloadCompleteEvent(line);

            else if (analysisPlayerDeath(line.Body, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new PlayerDeathEvent(line);

            else if (ID_ServerWhitelistAdd.IsMatch(line.Body) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerWhitelistAddEvent(line);

            else if (ID_ServerWhitelistRemove.IsMatch(line.Body) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return new ServerWhitelistRemoveEvent(line);

            else if (analysisPlayerCommand(line, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return extraAnalysisPlayerCommand(line);

            else if (analysisIngameCommandResult(line.Body, workingPlayerNames) && (ignoreCommonTags || line.Tag.Contains("Server thread")))
                return extraAnalysisIngameCommandResult(line, workingPlayerNames);

            else if (analysisChestShop(line, workingPlayerNames))
                return extraAnalysisChestShop(line);

            else if (analysisPlayerChat(line, workingPlayerNames, analyzerOptions))
                return new PlayerChatEvent(line);

            else
            {
                if (unmatchedLines != null)
                {
                    lock (unmatchedLines)
                    {
                        unmatchedLines.Add(line);
                    }
                }
                return null;
            }
        }

        // This more advanced check for a player lost connection event helps prevent false positives
        private static bool analysisPlayerLostConn(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;

            int spot2 = check.IndexOf(" lost connection: ");
            if (spot2 == -1)
                return false;
            
            int spot = check.IndexOf(' ');
            if (spot > -1)
            {
                string name = check.Substring(0, spot);
                if (spot2 >= spot && workingPlayerNames.ContainsKey(name))
                    return true;
            }

            return false;
        }
        
        // Another more advanced check to help reduce false positives
        private static bool analysisPlayerCommand(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;
            int spot2 = check.IndexOf(" issued server command: ");
            if (spot2 == -1)
                return false;
            
            int spot = check.IndexOf(' ');
            if (spot > -1)
            {
                string name = check.Substring(0, spot);
                if (spot2 >= spot && workingPlayerNames.ContainsKey(name))
                    return true;
            }

            return false;
        }
        // This check is necessary because some high-profile, specific commands (like /tell) need to be distinguished from mundane ones
        private static PlayerCommandEvent extraAnalysisPlayerCommand(LogLine line)
        {
            string check = line.Body;
            int spot = check.IndexOf(" issued server command: ");
            int spot2 = spot + 24;
            if (spot2 + 6 < check.Length && check.Substring(spot2, 6) == "/tell ")
                return new PlayerTellEvent(line);
            else if (spot2 + 6 < check.Length && check.Substring(spot2, 6) == "/nick ")
                return new PlayerNicknameEvent(line);
            else if (spot2 + 5 < check.Length && check.Substring(spot2, 5) == "/ban ")
                return new PlayerBanEvent(line);
            else
                return new PlayerCommandEvent(line);
        }

        // Command result messages have a very similar syntax to some chat message formats, but have one key difference that must be ID'd before trying to ID as a chat message
        private static bool analysisIngameCommandResult(string check, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            if (check.Length < 5)
                return false;

            if (check[0] != '[')
                return false;

            if (check[check.Length - 1] != ']')
                return false;

            int spot = check.IndexOf(':', 1);
            if (spot == -1)
                return false;

            string test = check.Substring(1, spot - 1);
            if (test.Contains(' '))
                return false;
            else
                return true;
        }
        // To handle subclassed command result events
        private static IngameCommandResultEvent extraAnalysisIngameCommandResult(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;
            int spot = check.IndexOf(':') + 2;
            int spot2 = check.IndexOf(']', spot + 1);
            string test = check.Substring(spot, spot2 - spot);

            if (test == "Turned on the whitelist")
                return new IngameWhitelistEnableEvent(line);

            else if (test == "Turned off the whitelist")
                return new IngameWhitelistDisableEvent(line);

            else if (test.Length > 6 && test.Substring(0, 6) == "Added ")
            {
                spot = test.IndexOf(' ', 6);
                if (spot > 7)
                {
                    //string name = test.Substring(6, spot - 6);
                    spot++;
                    string rest = test.Substring(spot, test.Length - spot);
                    if (rest == "to the whitelist")
                        return new IngameWhitelistAddEvent(line);
                }
            }

            else if (test.Length > 8 && test.Substring(0, 8) == "Removed ")
            {
                spot = test.IndexOf(' ', 8);
                if (spot > 9)
                {
                    //string name = test.Substring(8, spot - 8);
                    spot++;
                    string rest = test.Substring(spot, test.Length - spot);
                    if (rest == "from the whitelist")
                        return new IngameWhitelistRemoveEvent(line);
                }
            }

            return new IngameCommandResultEvent(line);
        }

        // Advanced check for the craftbukkit plugin reload warning messages
        private static bool analysisReloadWarning(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;
            
            int spot = check.IndexOf(": ");
            if (spot == -1)
                return false;

            string nameTest = check.Substring(0, spot);
            if (workingPlayerNames.ContainsKey(nameTest))
            {
                spot += 2;
                if (spot > check.Length - 1)
                    return false;

                string rest = check.Substring(spot, check.Length - spot); // Has some formatting to help aid ID'ing
                if (rest == PlayerChatHelper.CCT_RED + "Please note that this command is not supported and may cause issues when using some plugins." + PlayerChatHelper.CCT_RESET)
                {
                    line._AnalysisExtra.SecondMessage = false;
                    line._AnalysisExtra.Text = rest;
                    return true;
                }
                else if (rest == PlayerChatHelper.CCT_RED + "If you encounter any issues please use the /stop command to restart your server." + PlayerChatHelper.CCT_RESET)
                {
                    line._AnalysisExtra.SecondMessage = true;
                    line._AnalysisExtra.Text = rest;
                    return true;
                }
            }

            return false;
        }

        // Advanced check for the craftbukkit plugin reload complete message
        private static bool analysisReloadComplete(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;

            int spot = check.IndexOf(": ");
            if (spot == -1)
                return false;

            string nameTest = check.Substring(0, spot);
            if (workingPlayerNames.ContainsKey(nameTest))
            {
                spot += 2;
                if (spot > check.Length - 1)
                    return false;

                string rest = check.Substring(spot, check.Length - spot); // Has some formatting to help aid ID'ing
                if (rest == PlayerChatHelper.CCT_GREEN + "Reload complete." + PlayerChatHelper.CCT_RESET)
                    return true;
            }

            return false;
        }

        // Death messages are very not-unique in structure and thus require advanced analysis
        private static bool analysisPlayerDeath(string check, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            if (ID_OldPlayerDeath.IsMatch(check))
            {
                int spot = check.LastIndexOf(" died");
                string name = check.Substring(0, spot);
                if (workingPlayerNames.ContainsKey(name))
                    return true;
            }
            else if (ID_NewPlayerDeath.IsMatch(check))
            {
                int spot = check.LastIndexOf(" has died a gruesome and painful death!");
                string name = check.Substring(0, spot);
                if (workingPlayerNames.ContainsKey(name))
                    return true;
            }
            return false;
        }

        // ChestShop is a plugin with log line formatting that has : chars and player names in it, which will confuse the general chat analysis procedure
        private static bool analysisChestShop(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames)
        {
            string check = line.Body;
            if (check.Length >= 12 && check.Substring(0, 12) == "[ChestShop] ")
            {
                int spot = check.IndexOf(' ', 12);
                string nameTest = check.Substring(12, spot - 12);
                if (workingPlayerNames.ContainsKey(nameTest))
                {
                    int spot2 = check.IndexOf(' ', spot + 1);
                    string test = check.Substring(spot, spot2 - spot);
                    if (test == " created")
                    {
                        line._AnalysisExtra.Type = 0;
                        return true;
                    }
                    else if (test == " bought")
                    {
                        line._AnalysisExtra.Type = 1;
                        return true;
                    }
                    else if (test == " sold")
                    {
                        line._AnalysisExtra.Type = 2;
                        return true;
                    }
                }
            }

            return false;
        }
        private static GameEvent extraAnalysisChestShop(LogLine line)
        {
            if (line._AnalysisExtra.Type == 0)
                return new ChestShopCreatedEvent(line);
            else if (line._AnalysisExtra.Type == 1)
                return new ChestShopBuyEvent(line);
            else if (line._AnalysisExtra.Type == 2)
                return new ChestShopSellEvent(line);
            else
                return null;
        }

        // Ditto for chat messages, there is no distinct tag that indicates a message is player chat.
        // However, from the manual review of logs I've done, it seems many chat messages are tagged with one of the options below (BUT NOT ALL!)
        private static bool analysisPlayerChat(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames, AnalyzerOptions analyzerOptions)
        {
            // AnalyzerOptions provides a way for the user to specify how chat messages are analyzed to help reduce misses and false positives

            if (analyzerOptions.ChatAnalysisMethod == ChatAnalysisMethodType.GeneralAnalysisOnly)
            {
                return analysisPlayerChat_General(line, workingPlayerNames, analyzerOptions);
            }
            else if (analyzerOptions.ChatAnalysisMethod == ChatAnalysisMethodType.UserRegexesOnly)
            {
                return analysisPlayerChat_UserRegexes(line, workingPlayerNames, analyzerOptions);
            }
            else if (analyzerOptions.ChatAnalysisMethod == ChatAnalysisMethodType.UserRegexesAndGeneralAnalysis)
            {
                bool res = analysisPlayerChat_UserRegexes(line, workingPlayerNames, analyzerOptions);
                if (res)
                    return true;
                else
                    return analysisPlayerChat_General(line, workingPlayerNames, analyzerOptions);
            }

            return false;
        }
        private static bool analysisPlayerChat_UserRegexes(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames, AnalyzerOptions analyzerOptions)
        {
            // Mark for dissection later (if ID'd positive)
            line._AnalysisExtra.MethodUsed = 1;

            string lineTagCheck = line.Tag;
            string lineBodyCheckRaw = line.Body;
            string lineBodyCheckClean = PlayerChatHelper.RemoveChatColorsFrom(lineBodyCheckRaw);

            // Returns positive if any of the user provided ID regexes match
            foreach (ChatAnalysisRegexSet rSet in analyzerOptions.ChatAnalysisUserRegexes)
            {
                string lineBodyCheck = rSet.CleanForLineBodyTest ? lineBodyCheckClean : lineBodyCheckRaw;

                // Depending on the user's choice, both the log line's tag and body can be tested, or just one can be tested
                // Also depending on user choice, either the tag OR the body regex needs to match, or BOTH need to match

                // Test tag first
                if (rSet.InitialIDLineTagSource != null)
                {
                    if (rSet.InitialIDLineTagRegex != null)
                    {
                        if (rSet.InitialIDLineTagRegex.IsMatch(lineTagCheck))
                        {
                            line._AnalysisExtra.RegexSet = rSet;
                            line._AnalysisExtra.MatchedUsername = null;
                            line._AnalysisExtra.MatchedPart = 0; // 0 for ID'd by tag

                            if (!rSet.RequireMatchOnBothInitialID)
                                return true;
                        }
                    }
                    else
                    {
                        // No idea why one might need to wildcard player names in the log line's tag, but some bizarre situation may call for it
                        foreach (string pname in workingPlayerNames.Keys)
                        {
                            Regex dyn = new Regex(rSet.InitialIDLineTagSource.Replace("\x1A", Regex.Escape(pname)));
                            if (dyn.IsMatch(lineTagCheck))
                            {
                                line._AnalysisExtra.RegexSet = rSet;
                                line._AnalysisExtra.MatchedUsername = pname;
                                line._AnalysisExtra.MatchedPart = 0; // 0 for ID'd by tag

                                if (rSet.RequireMatchOnBothInitialID)
                                    break;
                                else
                                    return true;
                            }
                        }
                    }
                }

                // Test body second
                if (rSet.InitialIDLineBodySource != null)
                {
                    // User regexes that dont use ascii 26 to wildcard to workingPlayerNames are straightforward and fast (as they can be)
                    if (rSet.InitialIDLineBodyRegex != null)
                    {
                        if (rSet.InitialIDLineBodyRegex.IsMatch(lineBodyCheck))
                        {
                            line._AnalysisExtra.RegexSet = rSet;
                            line._AnalysisExtra.MatchedUsername = null;
                            line._AnalysisExtra.MatchedPart = 1; // 1 for ID'd by body
                            return true;
                        }
                    }
                    // Using ascii 26 to wildcard will slow things down quite a bit
                    else
                    {
                        foreach (string pname in workingPlayerNames.Keys)
                        {
                            Regex dyn = new Regex(rSet.InitialIDLineBodySource.Replace("\x1A", Regex.Escape(pname)));
                            if (dyn.IsMatch(lineBodyCheck))
                            {
                                line._AnalysisExtra.RegexSet = rSet;
                                line._AnalysisExtra.MatchedUsername = pname;
                                line._AnalysisExtra.MatchedPart = 1; // 1 for ID'd by body
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        private static bool analysisPlayerChat_General(LogLine line, ConcurrentDictionary<string, byte> workingPlayerNames, AnalyzerOptions analyzerOptions)
        {
            // Mark for dissection later (if ID'd positive)
            line._AnalysisExtra.MethodUsed = 0;

            if (   !analyzerOptions.StrictChatMessageAnalysis // Will shortcircuit the subsequent tag checks
                                                              // Strict chat analysis is not recommended. I've seen a good number of chat messages registered under the ubiquitous "Server Info" tag (instead of a "async chat thread" or "netty ...xyz")
                                                              // Only use strict mode if you know for certain that your player's chat messages are logged using one of the magic tags below
                || line.Tag.Length >= 21 && line.Tag.Substring(0, 21) == "Netty Epoll Server IO"    // seen in 1.8
                || line.Tag.Length >= 15 && line.Tag.Substring(0, 15) == "Netty Server IO"          // seen in 1.8
                || line.Tag.Length >= 17 && line.Tag.Substring(0, 17) == "Async Chat Thread")       // seen in 1.9+
            {
                string cleaned = PlayerChatHelper.RemoveChatColorsFrom(line.Body);

                // Vanilla chat format      <{{username}}> {{message}}
                if (analyzerOptions.GeneralChatAnalysisCheckVanilla)
                {
                    int spot = cleaned.IndexOf('<');
                    if (spot == 0) // There is never anything before the starting '<' in a vanilla chat message
                    {
                        spot++;
                        int spot2 = cleaned.IndexOf('>', spot);
                        if (spot2 > spot)
                        {
                            string test = cleaned.Substring(spot, spot2 - spot);
                            foreach (string username in workingPlayerNames.Keys) // WHEN PARALLELIZED: must assume workingPlayerNames is not complete! (and thus this log line may end up in the 2nd pass bin)
                                if (test.Contains(username))
                                {
                                    line._AnalysisExtra.Type = 0; // 0 for vanilla
                                    line._AnalysisExtra.Username = username;
                                    line._AnalysisExtra.FullCleaned = cleaned;

                                    return true;
                                }
                            return false;
                        }
                    }
                }

                // MineverseChat format     {{username}}: {{message}}       (actually just a pretty common chat format in games in general, but I use mineversechat/venturechat on my server so I'm going to call it that)
                if (analyzerOptions.GeneralChatAnalysisCheckMineverse)
                {
                    int colons = cleaned.Count(x => x == ':');
                    if (colons == 1)
                    {
                        int spot = cleaned.IndexOf(':');
                        int spot2 = cleaned.LastIndexOf(' ', spot) + 1;
                        string test = cleaned.Substring(spot2, spot - spot2);
                        foreach (string username in workingPlayerNames.Keys)
                            if (test.Contains(username))
                            {
                                line._AnalysisExtra.Type = 1; // 1 for mineversechat
                                line._AnalysisExtra.Username = username;
                                line._AnalysisExtra.UsernameColon = 0;
                                line._AnalysisExtra.FullCleaned = cleaned;

                                return true;
                            }
                        return false;
                    }
                    else if (colons > 1)
                    {
                        // Some buffoon may be using a : in a group's tag, like so   [DumbRank:VIPKid] jason.smith.2009: hEY tHeRE gUyz
                        // So from front to back of the string, we'll test if a username is in each : delimited segment
                        string[] cuts = cleaned.Split(':');
                        for (int i = 0; i < cuts.Length; i++)
                        {
                            int spot = cuts[i].LastIndexOf(' ') + 1;
                            string test = cleaned.Substring(spot, cuts[i].Length - spot);
                            foreach (string username in workingPlayerNames.Keys)
                                if (test.Contains(username))
                                {
                                    line._AnalysisExtra.Type = 1; // 1 for mineversechat
                                    line._AnalysisExtra.Username = username;
                                    line._AnalysisExtra.UsernameColon = i;
                                    line._AnalysisExtra.FullCleaned = cleaned;

                                    return true;
                                }
                            return false;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        private static void reportProgress(BackgroundWorker executor, WorkerReport report)
        {
            if (executor != null)
                executor.ReportProgress(0, report);
        }
    }
}
