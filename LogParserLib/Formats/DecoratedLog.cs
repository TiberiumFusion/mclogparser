using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Top-level format of a log
    public class DecoratedLog
    {
        [JsonProperty(Order = 0)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals


        //================| Serializable fields |================//

        [JsonProperty(Order = 203)] public string SourceFilepath { get; private set; }
            public bool ShouldSerializeSourceFilepath() { return E_Options.DecoratedLog_IncludeSourceFilepath; }

        [JsonProperty(Order = 201)] public string SourceFilename { get; private set; }
            public bool ShouldSerializeSourceFilename() { return E_Options.DecoratedLog_IncludeSourceFilename; }

        [JsonProperty(Order = 202)] public string SourceFilenameNoExt { get; private set; }
            public bool ShouldSerializeSourceFilenameNoExt() { return E_Options.DecoratedLog_IncludeSourceFilenameNoExt; }

        [JsonProperty(Order = 204)] public string SourceDirectory { get; private set; }
            public bool ShouldSerializeSourceDirectory() { return E_Options.DecoratedLog_IncludeSourceDirectory; }

        [JsonProperty(Order = 206)] public DateTime FilenameDate { get; private set; }
            public bool ShouldSerializeFilenameDate() { return E_Options.DecoratedLog_IncludeFilenameDate; }

        [JsonProperty(Order = 205)] public int FilenameNumber { get; private set; }
            public bool ShouldSerializeFilenameNumber() { return E_Options.DecoratedLog_IncludeFilenameNumber; }

        [JsonProperty(Order = 1000)] public LogLineList LogLines;
        
        [JsonProperty(Order = 101)] public TimeRange TimeRange; // Filled in by the Analyzer
            public bool ShouldSerializeTimeRange() { return E_Options.DecoratedLog_IncludeTimeRange; }

        [JsonProperty(Order = 301)] public List<GameEvent> GameEvents = new List<GameEvent>(); // All game events in this log's timespan
            public bool ShouldSerializeGameEvents() { return E_Options.DecoratedLog_IncludeGameEvents && !E_Options.DecoratedLog_UseCatalogIDsForGameEvents; }
        [JsonProperty(Order = 302)] public List<long> GameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeGameEventIDs() { return E_Options.DecoratedLog_IncludeGameEvents && E_Options.DecoratedLog_UseCatalogIDsForGameEvents; }

        [JsonProperty(Order = 401)] public Dictionary<string, int> GameEventTotals = new Dictionary<string, int>(); // Count of each type of GameEvent
            public bool ShouldSerializeGameEventTotals() { return E_Options.DecoratedLog_IncludeGameEventTotals; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public DecoratedLog(string path)
        {
            SourceFilepath = path;
            SourceFilename = Path.GetFileName(path);
            SourceFilenameNoExt = Path.GetFileNameWithoutExtension(path);
            SourceDirectory = Path.GetDirectoryName(path);

            // First determine log date and number (if applicable)
            string[] cuts = SourceFilenameNoExt.Split('-');
            if (cuts.Length == 4)
                FilenameNumber = int.Parse(cuts[3]);
            else
                FilenameNumber = -1;
            FilenameDate = new DateTime(int.Parse(cuts[0]), int.Parse(cuts[1]), int.Parse(cuts[2]));

            // Then read in and parse the log lines
            LogLines = new LogLineList(File.ReadAllLines(path), FilenameDate, this);
        }

        public void WriteToFile(string outputPath, bool includeINFOs = true, bool includeWARNs = true, bool includeERRORs = true)
        {
            List<string> writeLines = new List<string>();
            foreach (LogLine line in LogLines.DissectedLines)
            {
                if (   line.TagLevel == LogTagLevel.INFO && includeINFOs
                    || line.TagLevel == LogTagLevel.WARN && includeWARNs
                    || line.TagLevel == LogTagLevel.ERROR && includeERRORs)
                    writeLines.Add(line.ToSourceFormatString());
            }

            File.WriteAllLines(outputPath, writeLines.ToArray());
        }

        //////////////////////////////////////////// ANALYSIS ////////////////////////////////////////////
        public void Analyze_TimeRange()
        {
            TimeRange = new TimeRange();
            TimeRange.Start = LogLines.DissectedLines[0].Time;
            TimeRange.End = LogLines.DissectedLines[LogLines.DissectedLines.Count - 1].Time;
        }
    }
}