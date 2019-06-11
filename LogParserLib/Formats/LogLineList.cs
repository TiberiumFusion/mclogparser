using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Basic programmatic format of the list of lines format that the minecraft logs come in
    public class LogLineList
    {
        [JsonIgnore] public DecoratedLog ParentDecoratedLog;
        

        //================| Serializable fields |================//

        [JsonProperty(Order = 1002)] public string[] RawLines { get; private set; }
            public bool ShouldSerializeRawLines() { return ParentDecoratedLog.E_Options.DecoratedLog_IncludeRawLogLines; }

        [JsonProperty(Order = 1001)] public List<LogLine> DissectedLines = new List<LogLine>();
            public bool ShouldSerializeDissectedLines() { return ParentDecoratedLog.E_Options.DecoratedLog_IncludeDissectedLogLines; }



        public LogLineList(string[] lines, DateTime filenameDate, DecoratedLog parent)
        {
            ParentDecoratedLog = parent;

            // All log lines generally respect a rule of "no newlines", except for exception traces, which have unescaped newlines
            // Additionally, some plugins and components log outputs with newlines in them
            // As such, we must check for these traces. They are then concat'd into a single line

            // Example line:
            // [19:42:16] [Server thread/INFO]: Server permissions file permissions.yml is empty, ignoring it
            // Key anatomy: 3 colons before the message, and a "] [" between the time and tag
            
            RawLines = lines;

            for (int i = 0; i < RawLines.Length; i++)
            {
                string line = RawLines[i];
                
                // Dissect a bit
                int colonSpot = line.IndexOf(':'); // First colon
                colonSpot = line.IndexOf(':', colonSpot + 1); // Second colon
                colonSpot = line.IndexOf(':', colonSpot + 1); // Third colon

                // Extract message
                string message = line.Substring(colonSpot + 2);

                // Dissect further and extract tag
                int tagSepSpot = line.IndexOf("] [") + 3;
                string tag = line.Substring(tagSepSpot, colonSpot - tagSepSpot - 1);

                // Ditto for time
                int dateStart = line.IndexOf('[') + 1;
                int dateEnd = line.IndexOf(']');
                string date = line.Substring(dateStart, dateEnd - dateStart);
                string[] dateCuts = date.Split(':');
                DateTime time = new DateTime(filenameDate.Year, filenameDate.Month, filenameDate.Day, int.Parse(dateCuts[0]), int.Parse(dateCuts[1]), int.Parse(dateCuts[2]));

                // Check if this line is the start of a "multiline" log line
                bool multiline = false;
                int multilineType = -1;
                string next = null;
                if (i < lines.Length - 1)
                {
                    next = lines[i + 1];
                    multiline = checkLineMultiline(next, out multilineType);
                }

                // If uniline, simply assemble the line and move to the next
                if (!multiline)
                    DissectedLines.Add(new LogLine(tag, message, time, this));
                // If multiline, seek forward until the next uniline or EOF
                else
                {
                    i++;
                    if (multilineType == 0)
                    {
                        // "Proper" mulitline, no tag to remove
                        message += Environment.NewLine + next;
                    }
                    else if (multilineType == 1)
                    {
                        // Remove tag
                        int spot = line.IndexOf(':');
                        spot = line.IndexOf(':', spot + 1);
                        spot = line.IndexOf(':', spot + 1);
                        message += Environment.NewLine + next.Substring(spot + 2);
                    }

                    while (i < lines.Length - 1)
                    {
                        i++;
                        next = lines[i];
                        if (checkLineMultiline(next, out multilineType))
                        {
                            if (multilineType == 0)
                            {
                                message += Environment.NewLine + next;
                            }
                            else if (multilineType == 1)
                            {
                                int spot = line.IndexOf(':');
                                spot = line.IndexOf(':', spot + 1);
                                spot = line.IndexOf(':', spot + 1);
                                message += Environment.NewLine + next.Substring(spot + 2);
                            }
                        }
                        else
                        {
                            i--;
                            break;
                        }
                    }

                    // Assemble now
                    DissectedLines.Add(new LogLine(tag, message, time, this));
                }
            }

            // Once all lines are read, perform a finishing pass which will nearly ensure no two lines have the same time
            // Each line only stores time to the nearest second, so it very common for multiple lines to have the same time
            // To maintain order of the log lines using only their DateTimes, the milliseconds field of the DT is used to count occurences of the same HH:MM:SS time. Hopefully, no more than 999 lines will occur in the second.
            DateTime last = new DateTime(1, 1, 1);
            int lastCount = 0;
            foreach (LogLine line in DissectedLines)
            {
                if (line.Time == last)
                {
                    lastCount++;
                    DateTime dt = line.Time;
                    line.Time = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, lastCount);
                }
                else
                {
                    last = line.Time;
                    lastCount = 0;
                }
            }
        }

        private bool checkLineMultiline(string line, out int type)
        {
            type = -1;

            if (line.Length >= 10) // All "uniline" log lines start with the 10 char date part: e.g. [19:42:17]
            {
                // Multiline log writes by plugins may use [:] chars, so the best identifier we have is to check for the 10 char date part
                if (line[0] != '[' || line[3] != ':' || line[6] != ':' || line[9] != ']')
                {
                    // If the line doesnt match each of these 4 char checks, it's 99% likely to be from an exception trace of multiline plugin write
                    type = 0;
                    return true;
                }
                else if (line.Contains(": \tat"))
                {
                    // Not all exceptions are properly multline, some have each newline prefixed with the time and thread server tag. Thankfully, they use tabs for distinct ID'ing
                    type = 1;
                    return true;
                }
            }
            else
            {
                type = 0;
                return true; // If the line is less than 10 chars, it's too short to have the 10 char date part and is 99% likely a multline
            }

            return false;
        }
    }
}
