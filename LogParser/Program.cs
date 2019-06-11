using com.tiberiumfusion.minecraft.logparserlib;
using com.tiberiumfusion.minecraft.logparserlib.Formats;
using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tiberiumfusion.minecraft.logparser
{
    internal class Program
    {
        private static string arg_Path = null;
        private static bool arg_Rewriteall = false;
        private static string arg_Rewritetags = "IWE"; // Default value
        private static bool arg_Analyze = false;
        private static bool arg_Export = false;
        private static string arg_ExportPath = "export.json"; // Default value
        private static bool arg_Silent = false;
        private static string arg_Compress = "0"; // Default value
        private static AnalyzerOptions argParsed_AnalyzerOpts; // Default value
        private static ExporterOptions argParsed_ExporterOpts; // Default value
        private static bool arg_WaitOnFinish = false;

        private static List<WorkerReport> workerReports = new List<WorkerReport>();
        private static long lastWorkerReportNum = 0;

        static void Main(string[] args)
        {
            //////////////////////////////////////////// INIT ////////////////////////////////////////////
            // Gather launch arguments
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                string argLower = arg.ToLowerInvariant();

                if (argLower == "/silent")
                    arg_Silent = true;

                else if (argLower == "/rewriteall")
                    arg_Rewriteall = true;

                else if (argLower == "/analyze")
                    arg_Analyze = true;

                else if (argLower == "/waitonfinish")
                    arg_WaitOnFinish = true;

                else if (arg.Length >= 13 && argLower.Substring(0, 13) == "/rewritetags:")
                    arg_Rewritetags = arg.Substring(13);

                else if (arg.Length >= 12 && argLower.Substring(0, 12) == "/analyzeopt:")
                {
                    try
                    {
                        string read = File.ReadAllText(arg.Substring(12));
                        argParsed_AnalyzerOpts = new AnalyzerOptions(JsonConvert.DeserializeObject<AnalyzerOptionsJsonModel>(read));
                    }
                    catch (Exception e)
                    {
                        ConsoleWrite("Warning: Provided analyze options json file could not be read/parsed. Details:\n " + e.ToString() + "\n");
                        if (!arg_Silent)
                        {
                            ConsoleWrite("--- Press any key to continue with the default analyzer options ---");
                            Console.ReadKey(true);
                        }
                    }
                }

                else if (arg.Length >= 11 && argLower.Substring(0, 11) == "/exportopt:")
                {
                    try
                    {
                        string read = File.ReadAllText(arg.Substring(11));
                        argParsed_ExporterOpts = ExporterOptions.CreateFromJson(read);
                    }
                    catch (Exception e)
                    {
                        ConsoleWrite("Warning: Provided exporter options json file could not be read/parsed. Details:\n " + e.ToString() + "\n");
                        if (!arg_Silent)
                        {
                            ConsoleWrite("--- Press any key to continue with the default exporter options ---");
                            Console.ReadKey(true);
                        }
                    }
                }

                else if (arg.Length >= 10 && argLower.Substring(0, 10) == "/compress:")
                {
                    arg_Compress = arg.Substring(10);
                    string[] valid = { "0", "1", "2", "3" };
                    if (!valid.ToList().Contains(arg_Compress))
                    {
                        ConsoleWrite("Warning: Provided /compress level: \"" + arg_Compress + "\" is invalid. Defaulting to 0.");
                        arg_Compress = "0";
                    }
                }

                else if (arg.Length >= 7 && argLower.Substring(0, 7) == "/export")
                {
                    arg_Export = true;
                    if (arg.Length >= 8 && argLower.Substring(0, 8) == "/export:")
                        arg_ExportPath = arg.Substring(8);
                }

                else if (arg.Length >= 6 && argLower.Substring(0, 6) == "/path:")
                {
                    arg_Path = arg.Substring(6);
                    if (!Directory.Exists(arg_Path))
                    {
                        ConsoleWrite("Error: The provided /path does not exist");
                        return;
                    }
                }
            }
            if (arg_Path == null)
            {
                ConsoleWrite("Error: Invalid/missing/incomplete /path argument. Valid argument is required.");
                Thread.Sleep(765);
                return;
            }

            // Get file paths
            string[] files = Directory.GetFiles(arg_Path);
            if (files.Length == 0)
            {
                ConsoleWrite("Error: No files in the provided /path");
                Thread.Sleep(765);
                return;
            }

            //////////////////////////////////////////// PROCESS ////////////////////////////////////////////
            AnalyzedData analyzedData = null;
            // Using a BackgroundWorker to help prevent IO-blocking
            BackgroundWorker parseWorker = new BackgroundWorker();
            parseWorker.WorkerSupportsCancellation = true;
            parseWorker.WorkerReportsProgress = true;
            parseWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                Parser.ParseFromFilepaths(files, out analyzedData, parseWorker);
            };
            parseWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                lock (workerReports)
                {
                    WorkerReport? report = e.UserState as WorkerReport?;
                    if (report != null)
                    {
                        WorkerReport rep = (WorkerReport)report;
                        rep.Message = (rep.WithLead ? "[Parser] " : "") + rep.Message;
                        workerReports.Add(rep);
                    }
                }
            };
            parseWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                // Not used at present
            };
            ConsoleWrite("Parsing log files within provided directory (/path)");
            workerReports.Clear();
            parseWorker.RunWorkerAsync();

            // Idle until the worker finishes
            while (parseWorker.IsBusy)
            {
                Thread.Sleep(1);
                flushWorkerReports();
            }
            flushWorkerReports();

            //////////////////////////////////////////// PARSING COMPLETE ////////////////////////////////////////////
            ConsoleWrite("Parsing complete");

            //////////////////////////////////////////// REWRITING ////////////////////////////////////////////
            if (arg_Rewriteall)
            {
                ConsoleNewLine();
                ConsoleWrite("Now rewriting all parsed log files (/rewriteall)");

                bool includeINFOs = false;
                bool includeWARNs = false;
                bool includeERRORs = false;
                if (arg_Rewritetags.Contains('I'))
                    includeINFOs = true;
                if (arg_Rewritetags.Contains('W'))
                    includeWARNs = true;
                if (arg_Rewritetags.Contains('E'))
                    includeERRORs = true;

                string rewriteFilenameTag = "";
                if (includeINFOs)
                    rewriteFilenameTag += "I";
                if (includeWARNs)
                    rewriteFilenameTag += "W";
                if (includeERRORs)
                    rewriteFilenameTag += "E";

                ConsoleWrite("Rewrite tags: " + arg_Rewritetags + " (/rewritetags)");

                foreach (DecoratedLog dlog in analyzedData.DecoratedLogs)
                {
                    string rewriteDir = Path.Combine(dlog.SourceDirectory, "rewrite");
                    string filename = Path.Combine(rewriteDir, dlog.SourceFilenameNoExt + "_rewrite" + rewriteFilenameTag + ".log");

                    ConsoleWrite("Rewriting " + dlog.SourceFilename + " to " + filename + " ... ", false);

                    try
                    {
                        Directory.CreateDirectory(rewriteDir);
                        dlog.WriteToFile(filename, includeINFOs, includeWARNs, includeERRORs);
                    }
                    catch (Exception e)
                    {
                        ConsoleWrite("failed", true, false);
                        ConsoleWrite("Unexpected error occurred while trying to rewrite log: " + e.ToString());
                        continue;
                    }

                    ConsoleWrite("success", true, false);
                }
            }

            //////////////////////////////////////////// ANALYSIS ////////////////////////////////////////////
            bool doAnalysis = false;
            if (arg_Export)
            {
                ConsoleNewLine();
                ConsoleWrite("Preparing to analyze and export the parsed data from all read log files (required for /export)");
                doAnalysis = true;
            }
            else if (arg_Analyze)
            {
                ConsoleNewLine();
                ConsoleWrite("Preparing to analyze the parsed data from all read log files");
                doAnalysis = true;
            }
            if (doAnalysis)
            {
                if (argParsed_AnalyzerOpts != null)
                    ConsoleWrite("Using analyzer options provided from /analyzeopt argument");
                else
                {
                    ConsoleWrite("No valid /analyzeopt configuration file was provided. Using defaults.");
                    argParsed_AnalyzerOpts = new AnalyzerOptions();
                }

                // Using a BackgroundWorker (again) to help prevent IO-blocking and similar
                BackgroundWorker analysisWorker = new BackgroundWorker();
                analysisWorker.WorkerSupportsCancellation = true;
                analysisWorker.WorkerReportsProgress = true;
                analysisWorker.DoWork += (object sender, DoWorkEventArgs e) =>
                {
                    Analyzer.ProcessData(analyzedData, argParsed_AnalyzerOpts, analysisWorker);
                };
                analysisWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
                {
                    lock (workerReports)
                    {
                        WorkerReport? report = e.UserState as WorkerReport?;
                        if (report != null)
                        {
                            WorkerReport rep = (WorkerReport)report;
                            rep.Message = (rep.WithLead ? "[Analyzer] " : "") + rep.Message;
                            workerReports.Add(rep);
                        }
                    }
                };
                analysisWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                {
                    // Not used at present
                };
                ConsoleWrite("Performing analysis");
                workerReports.Clear();
                analysisWorker.RunWorkerAsync();

                // Idle until the worker finishes
                while (analysisWorker.IsBusy)
                {
                    Thread.Sleep(1);
                    flushWorkerReports();
                }
                flushWorkerReports();
            }

            //////////////////////////////////////////// EXPORT ////////////////////////////////////////////
            if (arg_Export)
            {
                ConsoleNewLine();
                ConsoleWrite("Preparing analyzed data for export");

                if (argParsed_ExporterOpts != null)
                    ConsoleWrite("Using exporter options provided from /exportopt argument");
                else
                {
                    ConsoleWrite("No valid /exportopt configuration file was provided. Using defaults.");
                    argParsed_ExporterOpts = new ExporterOptions();
                }

                ExportData exportData = new ExportData(argParsed_ExporterOpts);

                // Using a BackgroundWorker (yet again) to help prevent IO-blocking and similar
                BackgroundWorker exportPrepWorker = new BackgroundWorker();
                exportPrepWorker.WorkerSupportsCancellation = true;
                exportPrepWorker.WorkerReportsProgress = true;
                exportPrepWorker.DoWork += (object sender, DoWorkEventArgs e) =>
                {
                    Exporter.ProcessData(analyzedData, exportData, exportPrepWorker);
                };
                exportPrepWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
                {
                    lock (workerReports)
                    {
                        WorkerReport? report = e.UserState as WorkerReport?;
                        if (report != null)
                        {
                            WorkerReport rep = (WorkerReport)report;
                            rep.Message = (rep.WithLead ? "[ExportPrep] " : "") + rep.Message;
                            workerReports.Add(rep);
                        }
                    }
                };
                exportPrepWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                {
                    // Not used at present
                };
                workerReports.Clear();
                exportPrepWorker.RunWorkerAsync();

                // Idle until the worker finishes
                while (exportPrepWorker.IsBusy)
                {
                    Thread.Sleep(1);
                    flushWorkerReports();
                }
                flushWorkerReports();

                //////////////////////////////////////////// Serialize and write ////////////////////////////////////////////
                ConsoleNewLine();
                string epath = Path.GetFileNameWithoutExtension(arg_ExportPath);
                string temp = "Exporting data";
                if (arg_Compress == "0")
                    temp += " to \"" + epath + "\" (no compression)";
                else if (arg_Compress == "1")
                    temp += " to \"" + epath + ".zip\" (no compression)";
                else if (arg_Compress == "2")
                    temp += " to \"" + epath + ".zip\" (standard compression)";
                else if (arg_Compress == "3")
                    temp += " to \"" + epath + ".zip\" (best compression)";
                temp += "...";
                ConsoleWrite(temp);
                
                try
                {
                    DateTime start = DateTime.Now;
                    if (arg_Compress == "0")
                    {
                        ConsoleWrite("Serializing & writing to disk ...", false);

                        using (StreamWriter output = new StreamWriter(arg_ExportPath, false, Encoding.UTF8))
                        {
                            Task.Run(() =>
                            {
                                double size = 0;
                                while (output.BaseStream != null)
                                {
                                    try
                                    {
                                        size = (double)output.BaseStream.Position;
                                        ConsoleWrite("Serializing & writing to disk ... " + formatBytes(size) + "        ", false, true, 0);
                                        Thread.Sleep(50);
                                    }
                                    catch (Exception e) { } // Just swallow it
                                }
                                ConsoleClearLine();
                                ConsoleWrite("Serializing & writing to disk ... " + formatBytes(size), true, true, 0);
                            });

                            JsonSerializer js = new JsonSerializer();
                            js.Formatting = exportData.Options.IndentationAndFormatting ? Formatting.Indented : Formatting.None;
                            js.NullValueHandling = NullValueHandling.Include;
                            js.Serialize(output, exportData);
                        }

                        Thread.Sleep(101);
                    }
                    else
                    {
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            using (ZipArchive archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
                            {
                                CompressionLevel clevel = CompressionLevel.NoCompression;
                                if (arg_Compress == "1") clevel = CompressionLevel.NoCompression;
                                else if (arg_Compress == "2") clevel = CompressionLevel.Fastest;
                                else if (arg_Compress == "3") clevel = CompressionLevel.Optimal;

                                var jsonfile = archive.CreateEntry(Path.GetFileName(arg_ExportPath), clevel);

                                ConsoleWrite("Serializing & compressing ... ", false);
                                
                                using (Stream entryStream = jsonfile.Open())
                                using (StreamWriter output = new StreamWriter(entryStream))
                                {
                                    Task.Run(() =>
                                    {
                                        double raw = 0;
                                        double compressed = 0;
                                        while (output.BaseStream != null)
                                        {
                                            try
                                            {
                                                raw = (double)output.BaseStream.Position;
                                                compressed = (double)memStream.Position;
                                                ConsoleWrite("Serializing & compressing ... " + formatBytes(raw) + " / " + formatBytes(compressed) +
                                                             "   (" + Math.Max(0d, (100d - (compressed / raw) * 100d)).ToString("F") + "%)       ", false, true, 0);
                                                Thread.Sleep(50);
                                            }
                                            catch (Exception e) { } // Just swallow it
                                        }
                                        ConsoleClearLine();
                                        ConsoleWrite("Serializing & compressing ... " + formatBytes(raw) + " / " + formatBytes(compressed) +
                                                     "   (" + Math.Max(0d, (100d - (compressed / raw) * 100d)).ToString("F") + "%)       ", true, true, 0);
                                    });

                                    JsonSerializer js = new JsonSerializer();
                                    js.Formatting = Formatting.Indented;
                                    js.NullValueHandling = NullValueHandling.Include;
                                    js.Serialize(output, exportData);
                                }
                                
                                Thread.Sleep(101);
                            }

                            ConsoleWrite("Writing " + formatBytes((double)memStream.Length) + " to disk...");

                            using (FileStream fileStream = new FileStream(arg_ExportPath + ".zip", FileMode.Create))
                            {
                                memStream.Seek(0, SeekOrigin.Begin);
                                memStream.CopyTo(fileStream);
                            }
                        }
                    }

                    ConsoleWrite("Export completed in " + (DateTime.Now - start));
                }
                catch (Exception e)
                {
                    ConsoleWrite("Unexpected error occurred during serialization: " + e.ToString());
                }

                if (arg_WaitOnFinish)
                {
                    ConsoleNewLine();
                    ConsoleWrite("Press any key to exit (/waitonfinish)");
                    Console.ReadKey(true);
                }
                else
                    Thread.Sleep(765); // Help user notice final message
            }
        }

        private static string formatBytes(double bytes)
        {
            double temp = bytes;
            string unit = "B";
            if (temp > 1024d) { temp /= 1024d; unit = "KB"; }
            if (temp > 1024d) { temp /= 1024d; unit = "MB"; }
            if (temp > 1024d) { temp /= 1024d; unit = "GB"; }
            if (temp > 1024d) { temp /= 1024d; unit = "TB"; }
            return temp.ToString("F") + unit;
        }

        public static void ConsoleWrite(string message, bool withNewline = true, bool withLead = true, int cursorLeft = -1)
        {
            if (arg_Silent)
                return;

            if (cursorLeft > -1)
                Console.CursorLeft = cursorLeft;

            string lead = "";
            if (withLead)
                lead = " >  ";
            if (withNewline)
                Console.WriteLine(lead + message);
            else
                Console.Write(lead + message);
        }
        public static void ConsoleNewLine()
        {
            Console.WriteLine("");
        }
        public static void ConsoleClearLine()
        {
            int oldPos = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, oldPos);
        }

        private static void flushWorkerReports()
        {
            lock (workerReports)
            {
                try
                {
                    workerReports = workerReports.OrderBy(x => x.ReportNum).ToList();
                    while (workerReports.Count > 0)
                    {
                        WorkerReport rep = workerReports[0];
                        if (rep.ReportNum == lastWorkerReportNum)
                        {
                            ConsoleWrite(rep.Message, rep.WithNewline, rep.WithLead, rep.CursorLeft);
                            workerReports.RemoveAt(0);
                            lastWorkerReportNum++;
                        }
                        else if (workerReports.Count > 10)
                            lastWorkerReportNum = workerReports[0].ReportNum;
                        else
                            break;
                    }
                }
                catch (IndexOutOfRangeException e) { } // Swallow race condition (should diagnose and resolve later)
            }
        }
    }
}
