using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // High-level collection of player statistics, including their sessions
    public class PlayerStats
    {
        //================| Serializable fields |================//

        [JsonProperty(Order = 0)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals

        [JsonProperty(Order = 301)] public List<PlayerSession> Sessions = new List<PlayerSession>();
            public bool ShouldSerializeSessions() { return E_Options.PlayerStats_IncludeSessions && !E_Options.PlayerStats_UseCatalogIDsForSessions; }
        [JsonProperty(Order = 302)] public List<long> SessionIDs = new List<long>();
            public bool ShouldSerializeSessionIDs() { return E_Options.PlayerStats_IncludeSessions && E_Options.PlayerStats_UseCatalogIDsForSessions; }

        [JsonProperty(Order = 201)] public TimeSpan TotalGametime;
            public bool ShouldSerializeTotalGametime() { return E_Options.PlayerStats_IncludeTotalGametime; }
        [JsonProperty(Order = 101)] public SortedDictionary<DateTime, string> AllPlayerContemporaryNames = new SortedDictionary<DateTime, string>();
            public bool ShouldSerializeAllPlayerContemporaryNames() { return E_Options.PlayerStats_IncludeAllPlayerContemporaryNames; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public PlayerStats()
        {

        }

        // Consolidates all contemporary player names into a dictionary at this higher-level
        // This is called after the first pass of player session analysis
        public void ConsolidatePlayerContemporaryNames()
        {
            foreach (PlayerSession s in Sessions)
            {
                foreach (DateTime pnameKey in s.PlayerContemporaryNames.Keys)
                {
                    string pnameVal = s.PlayerContemporaryNames[pnameKey];
                    if (!AllPlayerContemporaryNames.ContainsValue(pnameVal))
                        AllPlayerContemporaryNames[pnameKey] = pnameVal;
                }
            }
        }

        public void CalculateStatsFromSessions()
        {
            foreach (PlayerSession s in Sessions)
            {
                TotalGametime += s.Range.Duration;
            }
        }
    }
}
