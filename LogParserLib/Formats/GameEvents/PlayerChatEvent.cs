using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    // This is perhaps the hardest event to ID because of how ambiguous chat formats are. The analysis test for this should always be performed after all other event tests.
    public class PlayerChatEvent : GameEvent
    {
        public NameWithUUID SendingPlayer;

        // Exactly as it appears in the log file
        public string RawFullMessage;
            public bool ShouldSerializeRawFullMessage() { return E_Options.PlayerChatEvent_IncludeRawFullMessage; }

        // Chatcolor strings removed
        public string PlainFullMessage;
            public bool ShouldSerializePlainFullMessage() { return E_Options.PlayerChatEvent_IncludePlainFullMessage; }
        public string PlainTag;
            public bool ShouldSerializePlainTag() { return E_Options.PlayerChatEvent_IncludePlainTag; }
        public string PlainBody;
            public bool ShouldSerializePlainBody() { return E_Options.PlayerChatEvent_IncludePlainBody; }

        // Chatcolor strings replaced with much more manageable ones
        public string ReEncodedFullMessage;
            public bool ShouldSerializeReEncodedFullMessage() { return E_Options.PlayerChatEvent_IncludeReEncodedFullMessage; }
        public string ReEncodedTag;
            public bool ShouldSerializeReEncodedTag() { return E_Options.PlayerChatEvent_IncludeReEncodedTag; }
        public string ReEncodedBody;
            public bool ShouldSerializeReEncodedBody() { return E_Options.PlayerChatEvent_IncludeReEncodedBody; }

        // This will be true if the ReEncoded version could not be processed and is thus null (FormatGroupedTag and FormatGroupedBody will also be null)
        public bool PlainProcessedOnly = false;

        // Message sectioned into a new group at every format change (what gets exported, useful for rendering this message)
        public List<FormattedChatString> FormatGroupedTag;
            public bool ShouldSerializeFormatGroupedTag() { return E_Options.PlayerChatEvent_IncludeFormatGroupedTag; }
        public List<FormattedChatString> FormatGroupedBody;
            public bool ShouldSerializeFormatGroupedBody() { return E_Options.PlayerChatEvent_IncludeFormatGroupedBody; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public PlayerChatEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            // Raw
            RawFullMessage = Source.Body;

            ///// Dissection method switch
            if (Source._AnalysisExtra.MethodUsed == 0) // 0 = General analysis gave a positive ID
            {
                // Sender
                SendingPlayer.Name = Source._AnalysisExtra.Username;

                // Full message prep
                PlainFullMessage = Source._AnalysisExtra.FullCleaned;
                ReEncodedFullMessage = PlayerChatHelper.ReEncodeChatColors(RawFullMessage);

                int type = Source._AnalysisExtra.Type;
                if (type == 0) // Vanilla format
                {
                    // Plain message (no chatcolors)
                    int spot = PlainFullMessage.IndexOf('<');
                    int spot2 = PlainFullMessage.IndexOf('>', spot) + 2;
                    PlainTag = PlainFullMessage.Substring(spot, spot2 - spot);
                    PlainBody = PlainFullMessage.Substring(spot2, PlainFullMessage.Length - spot2);

                    // Re-encoded message (chatcolors in TFCCT format)
                    spot = ReEncodedFullMessage.IndexOf('<');
                    spot2 = ReEncodedFullMessage.IndexOf('>', spot) + 2;
                    ReEncodedTag = ReEncodedFullMessage.Substring(spot, spot2 - spot);
                    ReEncodedBody = ReEncodedFullMessage.Substring(spot2, ReEncodedFullMessage.Length - spot2);
                }
                else if (type == 1) // Mineverse format
                {
                    int usernameColon = Source._AnalysisExtra.UsernameColon;

                    // Plain message
                    string[] cuts = PlainFullMessage.Split(':');
                    PlainTag = "";
                    PlainBody = "";
                    for (int i = 0; i < cuts.Length; i++)
                    {
                        if (i < cuts.Length - 1)
                            cuts[i] += ':';

                        // Hop the space (and any formatting) after the colon in {{tag}}: {{message}} from the cut after the colon to the end of the one preceeding it
                        if (i == usernameColon)
                        {
                            string next = cuts[i + 1];
                            int spot = next.IndexOf(' ') + 1;
                            string slice = next.Substring(0, spot);
                            cuts[i] += slice;
                            cuts[i + 1] = next.Substring(spot, next.Length - spot);
                        }

                        if (i <= usernameColon)
                            PlainTag += cuts[i];
                        else
                            PlainBody += cuts[i];
                    }

                    // Re-encoded message
                    cuts = ReEncodedFullMessage.Split(':');
                    ReEncodedTag = "";
                    ReEncodedBody = "";
                    for (int i = 0; i < cuts.Length; i++)
                    {
                        if (i < cuts.Length - 1)
                            cuts[i] += ':';

                        if (i == usernameColon)
                        {
                            string next = cuts[i + 1];
                            int spot = next.IndexOf(' ') + 1;
                            string slice = next.Substring(0, spot);
                            cuts[i] += slice;
                            cuts[i + 1] = next.Substring(spot, next.Length - spot);
                        }

                        if (i <= usernameColon)
                            ReEncodedTag += cuts[i];
                        else
                            ReEncodedBody += cuts[i];
                    }
                }

                // Create formatted text groups
                PlayerChatHelper.CreateFormattedGroupsForMessage(ReEncodedTag, ReEncodedBody, out FormatGroupedTag, out FormatGroupedBody);
            }
            else if (Source._AnalysisExtra.MethodUsed == 1) // 1 = a user regex gave a positive ID
            {
                PlainFullMessage = PlayerChatHelper.RemoveChatColorsFrom(RawFullMessage);
                ReEncodedFullMessage = PlayerChatHelper.ReEncodeChatColors(RawFullMessage);

                // The ID'ing user regex has ID'd this log line, but we still need to dissect it. The Tag user regex will be used to find the split location betwen message tag and Body.
                ChatAnalysisRegexSet rSet = Source._AnalysisExtra.RegexSet;
                Regex dyn;
                if (rSet.MessageTagLocationRegex == null)
                {
                    // If Source._AnalysisExtra.MatchedUsername == null (i.e. the InitialID tests did not use the playername wildcard), but the
                    // upcoming MessageTagLocation test does use the wildcard (for whatever reason), we will just strip it from the regex
                    if (Source._AnalysisExtra.MatchedUsername == null)
                        dyn = new Regex(rSet.MessageTagLocationSource.Replace("\x1A", ""));

                    else
                        dyn = new Regex(rSet.MessageTagLocationSource.Replace("\x1A", Regex.Escape(Source._AnalysisExtra.MatchedUsername)));
                }
                else
                    dyn = rSet.MessageTagLocationRegex;

                string textToTest = ReEncodedFullMessage;
                if (rSet.CleanForMessageTagLocationTest)
                    textToTest = PlainFullMessage;
                
                Match matchAlpha = dyn.Matches(textToTest)[0]; // Regex should yield the first match (if more than one) to be the tag's location
                int splitSpot = matchAlpha.Index + matchAlpha.Length;

                if (rSet.CleanForMessageTagLocationTest)
                {
                    // If the user regex is designed to test the simpler, cleaned version of the text, there's no easy way to extrapolate that to the version with chatcolors
                    // So, the fields containing the formatted version of the text will be empty
                    PlainProcessedOnly = true;

                    PlainTag = ReEncodedFullMessage.Substring(0, splitSpot);
                    PlainBody = ReEncodedFullMessage.Substring(splitSpot, ReEncodedFullMessage.Length - splitSpot);
                }
                else
                {
                    ReEncodedTag = ReEncodedFullMessage.Substring(0, splitSpot);
                    ReEncodedBody = ReEncodedFullMessage.Substring(splitSpot, ReEncodedFullMessage.Length - splitSpot);
                    PlainTag = PlayerChatHelper.RemoveTFChatColorsFrom(ReEncodedTag);
                    PlainBody = PlayerChatHelper.RemoveTFChatColorsFrom(ReEncodedBody);

                    // Create formatted text groups
                    PlayerChatHelper.CreateFormattedGroupsForMessage(ReEncodedTag, ReEncodedBody, out FormatGroupedTag, out FormatGroupedBody);
                }
            }
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(SendingPlayer.Name, Source.Time);
            SendingPlayer.UUID = (UUID != null) ? UUID : "";
        }

        public override void PropogateExportOptions()
        {
            foreach (FormattedChatString fsc in FormatGroupedTag)
                fsc.E_Options = this.E_Options;
            foreach (FormattedChatString fsc in FormatGroupedBody)
                fsc.E_Options = this.E_Options;
        }

        public void BuildHTMLText(string htmlTag, string cssClassForMagic)
        {
            foreach (FormattedChatString fsc in FormatGroupedTag)
                fsc.BuildHTMLText(htmlTag, cssClassForMagic);
            foreach (FormattedChatString fsc in FormatGroupedBody)
                fsc.BuildHTMLText(htmlTag, cssClassForMagic);
        }
    }
}
