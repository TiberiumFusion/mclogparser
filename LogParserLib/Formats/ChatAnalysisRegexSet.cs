using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Provides the required data for trying to ID a log line as a chat event using a user regex set
    public class ChatAnalysisRegexSet
    {
        ///// A null Regex indicates that the playername wildcard (ascii 26) is used and thus the regexes must be generated dynamically per-analysis
        ///// For the case of the initial ID, a null string indicates that the regex isn't used. (e.g. using just one regex to match the line's body and ignoring the line's tag)

        // Matched against the tag portion of the log line (i.e. "[Server Thread/INFO]")
        public Regex InitialIDLineTagRegex = null;
        public string InitialIDLineTagSource;

        // Matched against the body portion of the log line (i.e. "<ExplodingTNT> I smell cheese")
        public Regex InitialIDLineBodyRegex = null;
        public string InitialIDLineBodySource;
        public bool CleanForLineBodyTest; // If false, the regex will be tested on the text will all chatcolor intact. If true, it will be tested on a version will all chatcolors removed.

        // If a successful ID is made from the above two regexes, this one is used to find where the message tag (i.e. "<ExplodingTNT> ") is located
        public Regex MessageTagLocationRegex = null;
        public string MessageTagLocationSource;
        public bool CleanForMessageTagLocationTest; // If false, the regex will be tested on the text will all chatcolor intact. If true, it will be tested on a version will all chatcolors removed.
                                                    // Note that, if true, the ReEncoded variant of the message will be empty in the created PlayerChatEvent. Only the Plain variant will be parsed.

        // If true, both the InitialIDLineTag and InitialIDLineBody regex must match for a positive ID. If false, only one needs to match.
        // This is automatically set to false if only one InitialID regex is provided (for example, you only care about matching against the log line's body)
        public bool RequireMatchOnBothInitialID = true;


        public ChatAnalysisRegexSet(string initialIDLineTag, string initialIDLineBody, string messageTagLocation,
                                       bool bothIDMustMatch, bool cleanForLineBodyTest, bool cleanForMessageTagLocationTest)
        {
            init(initialIDLineTag, initialIDLineBody, messageTagLocation,
                 bothIDMustMatch, cleanForLineBodyTest, cleanForMessageTagLocationTest);
        }
        public ChatAnalysisRegexSet(ChatAnalysisRegexSetJsonModel jsonModel)
        {
            init(jsonModel.InitialIDLineTag, jsonModel.InitialIDLineBody, jsonModel.MessageTagLocation,
                 jsonModel.RequireBothInitialIDToMatch, jsonModel.CleanForLineBodyTest, jsonModel.CleanForMessageTagLocationTest);
        }
        private void init(string initialIDLineTag, string initialIDLineBody, string messageTagLocation,
                          bool bothIDMustMatch, bool cleanForLineBodyTest, bool cleanForMessageTagLocationTest)
        {
            RequireMatchOnBothInitialID = bothIDMustMatch;
            CleanForLineBodyTest = cleanForLineBodyTest;
            CleanForMessageTagLocationTest = cleanForMessageTagLocationTest;

            InitialIDLineTagSource = initialIDLineTag;
            if (InitialIDLineTagSource != "")
            {
                if (!InitialIDLineTagSource.Contains('\x1A'))
                    InitialIDLineTagRegex = new Regex(InitialIDLineTagSource);
            }
            else
                RequireMatchOnBothInitialID = false;

            InitialIDLineBodySource = initialIDLineBody;
            if (InitialIDLineBodySource != "")
            {
                if (!InitialIDLineBodySource.Contains('\x1A'))
                    InitialIDLineBodyRegex = new Regex(InitialIDLineBodySource);
            }
            else
                RequireMatchOnBothInitialID = false;

            MessageTagLocationSource = messageTagLocation;
            if (!MessageTagLocationSource.Contains('\x1A'))
                MessageTagLocationRegex = new Regex(MessageTagLocationSource);

            if (InitialIDLineTagSource == "")
                InitialIDLineTagSource = null;
            if (InitialIDLineBodySource == "")
                InitialIDLineBodySource = null;
        }
    }

    // For more distinct serialization
    public class ChatAnalysisRegexSetJsonModel
    {
        public string InitialIDLineTag;
        public string InitialIDLineBody;
        public string MessageTagLocation;
        public bool RequireBothInitialIDToMatch;
        public bool CleanForLineBodyTest;
        public bool CleanForMessageTagLocationTest;
    }
}
