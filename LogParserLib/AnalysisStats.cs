using com.tiberiumfusion.minecraft.logparserlib.Formats;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Provides some basic stats about the analysis process
    public class AnalysisStats
    {
        [JsonProperty(Order = 990)] public AnalyzerOptions AnalyzerOptionsUsed; // The options used for analysis

        [JsonProperty(Order = 101)] public TimeRange AnalysisTime = new TimeRange(); // Duration of entire analysis process
        [JsonProperty(Order = 102)] public TimeRange FirstPassTime = new TimeRange(); // Duration of preliminary pass
        [JsonProperty(Order = 103)] public TimeRange PlayerStatsFirstPassTime = new TimeRange(); // etc...
        [JsonProperty(Order = 104)] public TimeRange PlayerStatsUUIDPassTime = new TimeRange(); 
        [JsonProperty(Order = 105)] public TimeRange PlayerStatsSecondPassTime = new TimeRange();
        [JsonProperty(Order = 106)] public TimeRange ServerSessionsPassTime = new TimeRange();
        [JsonProperty(Order = 107)] public TimeRange SessionLinkingPassTime = new TimeRange();

        public AnalysisStats()
        {

        }
    }
}
