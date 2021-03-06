﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // An instant event that occurs on the server
    public class GameEvent
    {
        [JsonProperty(Order = -990)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals


        //================| Serializable fields |================//

        [JsonProperty(Order = 10001)] public DateTime Time; // Directly from LogLine Source
            public bool ShouldSerializeTime() { return E_Options.GameEvent_IncludeTime; }

        [JsonProperty(Order = 10101)] public LogLine Source;
            public bool ShouldSerializeSource() { return E_Options.GameEvent_IncludeSourceLogLine && !E_Options.GameEvent_UseCatalogIDsForSourceLogLine; }
        [JsonProperty(Order = 10102)] public long SourceID;
            public bool ShouldSerializeSourceID() { return E_Options.GameEvent_IncludeSourceLogLine && E_Options.GameEvent_UseCatalogIDsForSourceLogLine; }

        [JsonProperty(Order = -980)] public string EventTypeName;


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public GameEvent(LogLine source)
        {
            Source = source;
            Time = source.Time;
            parse();

            if (this.GetType().Namespace == "com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents")
                EventTypeName = this.GetType().Name;
            else
                EventTypeName = this.GetType().FullName;
        }
        protected virtual void parse()
        {
            // To be overriden by subclasses
        }

        // This is where GameEvents with only contemporary player names (no UUIDs) can fill in those UUIDs
        // Used by GameEvents like PlayerLeaveEvent
        public virtual void UUIDPass(AnalyzedData analyzedData)
        {
            // To be overriden by subclasses
        }

        // Called during export prep process to get the E_Options reference to any fields of GameEvents that may require it for serialization conditionals
        public virtual void PropogateExportOptions()
        {
            // To be overriden by subclasses
        }
    }
}
