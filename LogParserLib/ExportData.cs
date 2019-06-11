using com.tiberiumfusion.minecraft.logparserlib.Formats;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Superformat to organize and align analyzed data in preparation for an optimized export. B/c serializing the AnalyzedData class directly produces ***enourmous*** output.
    // Provides options to export catalogs as maps (in json, ID is key for data) or lists (in json, ID is in data itself).
    public class ExportData
    {
        //================| Serializable fields |================//

        [JsonProperty(Order = 990)] public ExporterOptions Options; // Required by Exporter.cs
            public bool ShouldSerializeOptions() { return Options.IncludeExportOptions; }

        [JsonProperty(Order = 101)] public DateTime ExportTime; // Time when serialization began

        [JsonProperty(Order = 201)] public string CatalogFormat = ""; // Helpful string for human-reading of JSON

        [JsonIgnore] public Dictionary<LogLine, long> LogLineLookup = new Dictionary<LogLine, long>(); // Used only during export structure construction
        [JsonIgnore] public Dictionary<DecoratedLog, long> DecoratedLogLookup = new Dictionary<DecoratedLog, long>(); // Used only during export structure construction
        [JsonProperty(Order = 301)] public Dictionary<long, DecoratedLog> DecoratedLogCatalogMap = new Dictionary<long, DecoratedLog>(); // Binds each dlog to an ID
            public bool ShouldSerializeDecoratedLogCatalogMap() { return (Options.CatalogFormat == ExportCatalogFormat.Maps); }
        [JsonProperty(Order = 302)] public List<DecoratedLog> DecoratedLogCatalogList = new List<DecoratedLog>(); // Simple, ordered list of dlogs
            public bool ShouldSerializeDecoratedLogCatalogList() { return (Options.CatalogFormat == ExportCatalogFormat.Lists); }

        [JsonProperty(Order = 211)] public long GrandLogLineTotal; // Directly copied from AnalyzedData
            public bool ShouldSerializeIncludeGrandLogLineTotal() { return Options.IncludeGrandLogLineTotal; }

        [JsonIgnore] public Dictionary<GameEvent, long> GameEventLookup = new Dictionary<GameEvent, long>(); // Used only during export structure construction
        [JsonProperty(Order = 401)] public Dictionary<long, GameEvent> GameEventCatalogMap = new Dictionary<long, GameEvent>(); // Binds each GE to an ID
            public bool ShouldSerializeGameEventCatalogMap() { return (Options.CatalogFormat == ExportCatalogFormat.Maps); }
        [JsonProperty(Order = 402)] public List<GameEvent> GameEventCatalogList = new List<GameEvent>(); // Simple, ordered list of GEs
            public bool ShouldSerializeGameEventCatalogList() { return (Options.CatalogFormat == ExportCatalogFormat.Lists); }

        [JsonIgnore] public Dictionary<PlayerSession, long> PlayerSessionLookup = new Dictionary<PlayerSession, long>(); // Used only during export structure construction
        [JsonProperty(Order = 501)] public Dictionary<long, PlayerSession> PlayerSessionCatalogMap = new Dictionary<long, PlayerSession>(); // Binds each player session to an ID
            public bool ShouldSerializePlayerSessionCatalogMap() { return (Options.CatalogFormat == ExportCatalogFormat.Maps); }
        [JsonProperty(Order = 502)] public List<PlayerSession> PlayerSessionCatalogList = new List<PlayerSession>(); // Simple, ordered list of player sessions
            public bool ShouldSerializePlayerSessionCatalogList() { return (Options.CatalogFormat == ExportCatalogFormat.Lists); }

        [JsonIgnore] public Dictionary<ServerSession, long> ServerSessionLookup = new Dictionary<ServerSession, long>(); // Used only during export structure construction
        [JsonProperty(Order = 601)] public Dictionary<long, ServerSession> ServerSessionCatalogMap = new Dictionary<long, ServerSession>(); // Binds each server session to an ID
            public bool ShouldSerializeServerSessionCatalogMap() { return (Options.CatalogFormat == ExportCatalogFormat.Maps); }
        [JsonProperty(Order = 602)] public List<ServerSession> ServerSessionCatalogList = new List<ServerSession>(); // Simple, ordered list of server sessions
            public bool ShouldSerializeServerSessionCatalogList() { return (Options.CatalogFormat == ExportCatalogFormat.Lists); }

        [JsonProperty(Order = 701)] public Dictionary<string, PlayerStats> AllPlayerStats = new Dictionary<string, PlayerStats>(); // Very similar to counterpart in AnalyzedData        
            public bool ShouldSerializeAllPlayerStats() { return Options.IncludeAllPlayerStats; }

        [JsonProperty(Order = 801)] public Dictionary<string, int> CompleteGameEventTotals = new Dictionary<string, int>(); // Count of each type of GameEvent
            public bool ShouldSerializeCompleteGameEventTotals() { return Options.GameEvent_IncludeCompleteGameEventTotals; }

        [JsonProperty(Order = 980)] public AnalysisStats StatsFromAnalysis; // Stats from analysis process (directly from the AnalyzedData object)

        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public ExportData(ExporterOptions opts)
        {
            Options = opts;
        }
    }
}
