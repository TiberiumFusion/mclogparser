using com.tiberiumfusion.minecraft.logparserlib.Formats;
using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Superformat for the parsed & analyzed data. Nearly all members and submembers are public and may be externally written to because this is a data report class.
    public class AnalyzedData
    {
        public List<DecoratedLog> DecoratedLogs = new List<DecoratedLog>(); // All log files as read in by the parser
        public long GrandLogLineTotal; // Count of all log lines across all decorated logs (calculated by parser)
        public List<GameEvent> GameEvents = new List<GameEvent>(); // All GameEvents spanning all analyzed log files. In chronological order.
        public Dictionary<string, PlayerStats> AllPlayerStats = new Dictionary<string, PlayerStats>(); // Map of all gathered UUIDs -> that player's stats (which includes all PlayerSessions)
        public List<ServerSession> ServerSessions = new List<ServerSession>(); // Most-top-level structure of the server's primary history. All analyzed data can be accessed from this field.

        public Dictionary<string, int> CompleteGameEventTotals = new Dictionary<string, int>(); // Count of each type of GameEvent

        public AnalysisStats AnalysisProcessStats = new AnalysisStats();


        public AnalyzedData()
        {

        }

        // Finds the most likely UUID match for a human-readable player name, using the provided time as a "no later than" limit
        // This method is only useful after the UUID pass of assembling player stats is complete
        public string FindBestUUIDMatchFor(string playername, DateTime time)
        {
            string workingUUID = null;
            DateTime workingTime = new DateTime(1, 1, 1);
            foreach (string keyUUID in AllPlayerStats.Keys)
            {
                PlayerStats stats = AllPlayerStats[keyUUID];
                foreach (DateTime dt in stats.AllPlayerContemporaryNames.Keys)
                {
                    string name = stats.AllPlayerContemporaryNames[dt];
                    if (   playername == name
                        && dt <= time
                        && (time - dt) < (time - workingTime))
                    {
                        workingUUID = keyUUID;
                        workingTime = dt;
                    }
                }
            }

            return workingUUID;
        }

        // Finds the PlayerSession that corresponds to a PlayerJoinEvent
        // Used when assembling ServerSessions
        public PlayerSession FindPlayerSessionFor(PlayerJoinEvent joinGE)
        {
            if (AllPlayerStats.ContainsKey(joinGE.Player.UUID))
            {
                foreach (PlayerSession session in AllPlayerStats[joinGE.Player.UUID].Sessions)
                {
                    if (session.AllConcurrentGameEvents[0] == joinGE)
                        return session;
                }
            }

            return null;
        }
    }
}
