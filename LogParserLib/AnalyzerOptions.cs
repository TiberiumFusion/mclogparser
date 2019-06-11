using com.tiberiumfusion.minecraft.logparserlib.Formats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Used to determine what method the analyzer uses to ID chat messages
    public enum ChatAnalysisMethodType
    {
        GeneralAnalysisOnly,            // General-purpose algorithm. Requires no configuration. May produce false positives (and/or miss chat messages when StrictChatMessageAnalysis = true)
        UserRegexesOnly,                // Match potential chat messages against a set of Regexes that you provide. Will always & only be as accurate as your regexes are.
        UserRegexesAndGeneralAnalysis,  // A combination of both of the above messages. An ultimate positive ID is made if EITHER method produces a positive ID. User regexes are tested first.
    }

    // Grouper of options for the analyzer
    public class AnalyzerOptions
    {
        //////////////////////////////////////////// General Options ////////////////////////////////////////////

        // Enabling to *disables* the check for magic message tags (i.e. Async Chat Thread) for chat message analysis.
        // Only affects the GeneralAnalysis chat ID algorithm.
        public bool StrictChatMessageAnalysis;

        // Enable to use a parallel processing strategy of each log file's lines (instead of serial). See notes in Analyzer.cs
        public bool ProcessLogLinesInParallel;

        // Enable to *disable* the checks for the common magic phrases like "Server thread" in message tags
        public bool IgnoreCommonTags;


        //////////////////////////////////////////// Chat Analysis specific things ////////////////////////////////////////////

        // Enables the general purpose chat heuristic for vanilla chat format
        public bool GeneralChatAnalysisCheckVanilla;
        // Enables the general purpose chat heuristic for the mineversechat chat format
        public bool GeneralChatAnalysisCheckMineverse;

        // Determines how chat messages are analyzed for a positive ID
        public ChatAnalysisMethodType ChatAnalysisMethod;

        // Used by UserRegexesOnly and UserRegexesAndGeneralAnalysis
        // This is a set of regexes that you provide. You know the structure of your server's chat messages! (hopefully it is a distinct one).
        // A positive ID is made if any of these match.
        // Within your regex, you can use non-printing ascii char 26 (substitute) to wildcard any playernames collected by the analyzer *at the time of analyzing the chat message*. Be advised that this may slow analysis considerably.
        // If you use this wildcard, you must set a null Regex object as the key's value in the dictionary.
        // If you do not use this wildcard, you must create a Regex object from that string as the key's value in the dictionary.
        public List<ChatAnalysisRegexSet> ChatAnalysisUserRegexes = new List<ChatAnalysisRegexSet>();

        // If true, the Analyzer will throw out all UnrecognizedEvents (as opposed to including them in the analyzed data)
        public bool SkipUnrecognizedEvents;

        // List of GameEvent types to ignore when analyzing the LogLines
        public List<Type> ExcludedGameEvents = new List<Type>();

        // Counts up how many of each kind of Game Event there are
        public bool CountGameEventTotals;


        // Defaults
        public AnalyzerOptions()
        {
            StrictChatMessageAnalysis = false;
            ProcessLogLinesInParallel = false;
            IgnoreCommonTags = false;

            GeneralChatAnalysisCheckVanilla = true;
            GeneralChatAnalysisCheckMineverse = true;

            ChatAnalysisMethod = ChatAnalysisMethodType.GeneralAnalysisOnly;

            SkipUnrecognizedEvents = false;

            ExcludedGameEvents.Clear();

            CountGameEventTotals = true;
        }
        // Creating from json model from user file
        public AnalyzerOptions(AnalyzerOptionsJsonModel jsonModel)
        {
            StrictChatMessageAnalysis = jsonModel.StrictChatMessageAnalysis;
            ProcessLogLinesInParallel = jsonModel.ProcessLogLinesInParallel;
            IgnoreCommonTags = jsonModel.IgnoreCommonTags;

            ChatAnalysisMethod = (ChatAnalysisMethodType)Enum.Parse(typeof(ChatAnalysisMethodType), jsonModel.ChatAnalysisMethod);

            GeneralChatAnalysisCheckVanilla = jsonModel.GeneralChatAnalysisCheckVanilla;
            GeneralChatAnalysisCheckMineverse = jsonModel.GeneralChatAnalysisCheckMineverse;

            foreach (ChatAnalysisRegexSetJsonModel rSetModel in jsonModel.ChatAnalysisUserRegexes)
                ChatAnalysisUserRegexes.Add(new ChatAnalysisRegexSet(rSetModel));

            SkipUnrecognizedEvents = jsonModel.SkipUnrecognizedEvents;

            foreach (string type in jsonModel.ExcludedGameEvents)
            {
                string test = type;
                if (!test.Contains('.'))
                    test = "com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents." + test;
                ExcludedGameEvents.Add(Type.GetType(test));
            }

            CountGameEventTotals = jsonModel.CountGameEventTotals;
        }
    }
    
    // Because we don't directly serialize enums or regexes
    public class AnalyzerOptionsJsonModel
    {
        public bool StrictChatMessageAnalysis;
        public bool ProcessLogLinesInParallel;
        public bool IgnoreCommonTags;
        
        public string ChatAnalysisMethod;

        public bool GeneralChatAnalysisCheckVanilla;
        public bool GeneralChatAnalysisCheckMineverse;

        public List<ChatAnalysisRegexSetJsonModel> ChatAnalysisUserRegexes;

        public bool SkipUnrecognizedEvents;

        public List<string> ExcludedGameEvents;

        public bool CountGameEventTotals;
    }
}
