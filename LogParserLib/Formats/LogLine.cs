using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    public enum LogTagLevel
    {
        INFO, WARN, ERROR
    }

    // A single line of a log
    public class LogLine
    {
        [JsonProperty(Order = -990)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.

        [JsonIgnore] public LogLineList ParentLogLineList;

        [JsonProperty(Order = 102)] public string Tag;
        [JsonProperty(Order = 104)] public string Body;
        [JsonProperty(Order = 101)] public DateTime Time;
        [JsonProperty(Order = 103)] public LogTagLevel TagLevel;
        [JsonIgnore] public dynamic _AnalysisExtra = new ExpandoObject(); // To avoid some repeated calculations during analysis

        public LogLine(string tag, string body, DateTime time, LogLineList parent)
        {
            ParentLogLineList = parent;

            Tag = tag;
            Body = body;
            Time = time;

            if (Tag.Contains("/INFO"))
                TagLevel = LogTagLevel.INFO;
            else if (Tag.Contains("/WARN"))
                TagLevel = LogTagLevel.WARN;
            else if (Tag.Contains("/ERROR"))
                TagLevel = LogTagLevel.ERROR;
        }

        // Returns this log line in the format used by the minecraft log files
        // Example line:
        // [19:42:16] [Server thread/INFO]: Server permissions file permissions.yml is empty, ignoring it
        // Anatomy:
        // [Time] [Tag]: Message
        public string ToSourceFormatString()
        {
            string hour = Time.Hour.ToString();
            if (hour.Length == 1)
                hour = "0" + hour;
            string minute = Time.Minute.ToString();
            if (minute.Length == 1)
                minute = "0" + minute;
            string second = Time.Second.ToString();
            if (second.Length == 1)
                second = "0" + second;
            return "[" + hour + ":" + minute + ":" + second + "] [" + Tag + "]: " + Body;
        }
    }
}
