using com.tiberiumfusion.minecraft.logparserlib.Formats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Utility for working with player chat messages
    public static class PlayerChatHelper
    {
        //////////////////////////////////////////// FIELDS ////////////////////////////////////////////
        
        // From https://minecraft.gamepedia.com/Formatting_codes
        public static readonly Dictionary<ChatColorType, string> HTMLColorMappings = new Dictionary<ChatColorType, string>()
        {
            { ChatColorType.AQUA, "153F3F" },
            { ChatColorType.BLACK, "000000" },
            { ChatColorType.BLUE, "15153F" },
            { ChatColorType.DARK_AQUA, "002A2A" },
            { ChatColorType.DARK_BLUE, "00002A" },
            { ChatColorType.DARK_GRAY, "151515" },
            { ChatColorType.DARK_GREEN, "002A00" },
            { ChatColorType.DARK_PURPLE, "2A002A" },
            { ChatColorType.DARK_RED, "2A0000" },
            { ChatColorType.GOLD, "2A2A00" },
            { ChatColorType.GRAY, "2A2A2A" },
            { ChatColorType.GREEN, "153F15" },
            { ChatColorType.LIGHT_PURPLE, "3F153F" },
            { ChatColorType.RED, "3F1515" },
            { ChatColorType.WHITE, "3F3F3F" }, // I never noticed this wasn't pure white
            { ChatColorType.YELLOW, "3F3F15" }
        };

        // The relation between chatcolor char groups and human-readable types is provided in a variety of formats for convenience and performance
        public const string CCT_AQUA = "\x1B[0;36;1m";
        public const string CCT_BLACK = "\x1B[0;30;22m";
        public const string CCT_BLUE = "\x1B[0;34;1m";
        public const string CCT_BOLD = "\x1B[21m";
        public const string CCT_DARK_AQUA = "\x1B[0;36;22m";
        public const string CCT_DARK_BLUE = "\x1B[0;34;22m";
        public const string CCT_DARK_GRAY = "\x1B[0;30;1m";
        public const string CCT_DARK_GREEN = "\x1B[0;32;22m";
        public const string CCT_DARK_PURPLE = "\x1B[0;35;22m";
        public const string CCT_DARK_RED = "\x1B[0;31;22m";
        public const string CCT_GOLD = "\x1B[0;33;22m";
        public const string CCT_GRAY = "\x1B[0;37;22m";
        public const string CCT_GREEN = "\x1B[0;32;1m";
        public const string CCT_ITALIC = "\x1B[3m";
        public const string CCT_LIGHT_PURPLE = "\x1B[0;35;1m";
        public const string CCT_MAGIC = "\x1B[5m";
        public const string CCT_RED = "\x1B[0;31;1m";
        public const string CCT_RESET = "\x1B[m";
        public const string CCT_STRIKETHROUGH = "\x1B[9m";
        public const string CCT_UNDERLINE = "\x1B[4m";
        public const string CCT_WHITE = "\x1B[0;37;1m";
        public const string CCT_YELLOW = "\x1B[0;33;1m";
        public const string CCT_COLOR_CHAR = "\x1B";
        public static readonly Dictionary<string, ChatColorType> ChatColorMappings = new Dictionary<string, ChatColorType>()
        {
            { CCT_AQUA, ChatColorType.AQUA },
            { CCT_BLACK, ChatColorType.BLACK },
            { CCT_BLUE, ChatColorType.BLUE },
            { CCT_BOLD, ChatColorType.BOLD },
            { CCT_DARK_AQUA, ChatColorType.DARK_AQUA },
            { CCT_DARK_BLUE, ChatColorType.DARK_BLUE },
            { CCT_DARK_GRAY, ChatColorType.DARK_GRAY },
            { CCT_DARK_GREEN, ChatColorType.DARK_GREEN },
            { CCT_DARK_PURPLE, ChatColorType.DARK_PURPLE },
            { CCT_DARK_RED, ChatColorType.DARK_RED },
            { CCT_GOLD, ChatColorType.GOLD },
            { CCT_GRAY, ChatColorType.GRAY },
            { CCT_GREEN, ChatColorType.GREEN },
            { CCT_ITALIC, ChatColorType.ITALIC },
            { CCT_LIGHT_PURPLE, ChatColorType.LIGHT_PURPLE },
            { CCT_MAGIC, ChatColorType.MAGIC },
            { CCT_RED, ChatColorType.RED },
            { CCT_RESET, ChatColorType.RESET },
            { CCT_STRIKETHROUGH, ChatColorType.STRIKETHROUGH },
            { CCT_UNDERLINE, ChatColorType.UNDERLINE },
            { CCT_WHITE, ChatColorType.WHITE },
            { CCT_YELLOW, ChatColorType.YELLOW },
            { CCT_COLOR_CHAR, ChatColorType.COLOR_CHAR }
        };
        public static readonly string[] ChatColorStrings = ChatColorMappings.Keys.ToArray();
        public static readonly ChatColorType[] ChatColorTypes = ChatColorMappings.Values.ToArray();

        public const char asciiDC1 = '\x11';       // These two chars are used like the brackets on an <xml> tag to denote the start of a chatcolor
        public const char asciiDC2 = '\x12';       // Because parsing things like [0;37;1m[[0;33;1mA chat message[0;37;1m] sucks
        // These "TF chatcolor type" consts use the spec from: https://minecraft.gamepedia.com/Formatting_codes (minus the '§' char)
        public static readonly string TFCCT_AQUA = asciiDC1 + "b" + asciiDC2;
        public static readonly string TFCCT_BLACK = asciiDC1 + "0" + asciiDC2;
        public static readonly string TFCCT_BLUE = asciiDC1 + "9" + asciiDC2;
        public static readonly string TFCCT_BOLD = asciiDC1 + "l" + asciiDC2;
        public static readonly string TFCCT_DARK_AQUA = asciiDC1 + "3" + asciiDC2;
        public static readonly string TFCCT_DARK_BLUE = asciiDC1 + "1" + asciiDC2;
        public static readonly string TFCCT_DARK_GRAY = asciiDC1 + "8" + asciiDC2;
        public static readonly string TFCCT_DARK_GREEN = asciiDC1 + "2" + asciiDC2;
        public static readonly string TFCCT_DARK_PURPLE = asciiDC1 + "5" + asciiDC2;
        public static readonly string TFCCT_DARK_RED = asciiDC1 + "4" + asciiDC2;
        public static readonly string TFCCT_GOLD = asciiDC1 + "6" + asciiDC2;
        public static readonly string TFCCT_GRAY = asciiDC1 + "7" + asciiDC2;
        public static readonly string TFCCT_GREEN = asciiDC1 + "a" + asciiDC2;
        public static readonly string TFCCT_ITALIC = asciiDC1 + "o" + asciiDC2;
        public static readonly string TFCCT_LIGHT_PURPLE = asciiDC1 + "d" + asciiDC2;
        public static readonly string TFCCT_MAGIC = asciiDC1 + "k" + asciiDC2;
        public static readonly string TFCCT_RED = asciiDC1 + "c" + asciiDC2;
        public static readonly string TFCCT_RESET = asciiDC1 + "r" + asciiDC2;
        public static readonly string TFCCT_STRIKETHROUGH = asciiDC1 + "m" + asciiDC2;
        public static readonly string TFCCT_UNDERLINE = asciiDC1 + "n" + asciiDC2;
        public static readonly string TFCCT_WHITE = asciiDC1 + "f" + asciiDC2;
        public static readonly string TFCCT_YELLOW = asciiDC1 + "e" + asciiDC2;
        public static readonly string TFCCT_COLOR_CHAR = asciiDC1 + "X" + asciiDC2; // I'd rather be consistent with the above and use a printing char for all of these than use the esc char for this one
        public static readonly Dictionary<string, ChatColorType> TFChatColorMappings = new Dictionary<string, ChatColorType>()
        {
            { TFCCT_AQUA, ChatColorType.AQUA },
            { TFCCT_BLACK, ChatColorType.BLACK },
            { TFCCT_BLUE, ChatColorType.BLUE },
            { TFCCT_BOLD, ChatColorType.BOLD },
            { TFCCT_DARK_AQUA, ChatColorType.DARK_AQUA },
            { TFCCT_DARK_BLUE, ChatColorType.DARK_BLUE },
            { TFCCT_DARK_GRAY, ChatColorType.DARK_GRAY },
            { TFCCT_DARK_GREEN, ChatColorType.DARK_GREEN },
            { TFCCT_DARK_PURPLE, ChatColorType.DARK_PURPLE },
            { TFCCT_DARK_RED, ChatColorType.DARK_RED },
            { TFCCT_GOLD, ChatColorType.GOLD },
            { TFCCT_GRAY, ChatColorType.GRAY },
            { TFCCT_GREEN, ChatColorType.GREEN },
            { TFCCT_ITALIC, ChatColorType.ITALIC },
            { TFCCT_LIGHT_PURPLE, ChatColorType.LIGHT_PURPLE },
            { TFCCT_MAGIC, ChatColorType.MAGIC },
            { TFCCT_RED, ChatColorType.RED },
            { TFCCT_RESET, ChatColorType.RESET },
            { TFCCT_STRIKETHROUGH, ChatColorType.STRIKETHROUGH },
            { TFCCT_UNDERLINE, ChatColorType.UNDERLINE },
            { TFCCT_WHITE, ChatColorType.WHITE },
            { TFCCT_YELLOW, ChatColorType.YELLOW },
            { TFCCT_COLOR_CHAR, ChatColorType.COLOR_CHAR }
        };
        public static readonly string[] TFChatColorStrings = TFChatColorMappings.Keys.ToArray();
        public static readonly ChatColorType[] TFChatColorTypes = TFChatColorMappings.Values.ToArray();

        public static readonly ChatColorType AllChatColors = ChatColorType.AQUA |
                                                             ChatColorType.BLACK |
                                                             ChatColorType.BLUE |
                                                             ChatColorType.DARK_AQUA |
                                                             ChatColorType.DARK_BLUE |
                                                             ChatColorType.DARK_GRAY |
                                                             ChatColorType.DARK_GREEN |
                                                             ChatColorType.DARK_PURPLE |
                                                             ChatColorType.DARK_RED |
                                                             ChatColorType.GOLD |
                                                             ChatColorType.GRAY |
                                                             ChatColorType.LIGHT_PURPLE |
                                                             ChatColorType.RED |
                                                             ChatColorType.WHITE |
                                                             ChatColorType.YELLOW;
        public static readonly ChatColorType AllChatFormats = ChatColorType.BOLD |
                                                              ChatColorType.ITALIC |
                                                              ChatColorType.MAGIC |
                                                              ChatColorType.STRIKETHROUGH |
                                                              ChatColorType.UNDERLINE;


        //////////////////////////////////////////// PROCEDURES ////////////////////////////////////////////

        public static string RemoveChatColorsFrom(string message)
        {
            for (int i = 0; i < ChatColorStrings.Length; i++)
                message = message.Replace(ChatColorStrings[i], "");

            return message;
        }

        public static string RemoveTFChatColorsFrom(string message)
        {
            for (int i = 0; i < TFChatColorStrings.Length; i++)
                message = message.Replace(TFChatColorStrings[i], "");

            return message;
        }

        public static string ReEncodeChatColors(string message)
        {
            return message.Replace(CCT_AQUA, TFCCT_AQUA)
                          .Replace(CCT_BLACK, TFCCT_BLACK)
                          .Replace(CCT_BLUE, TFCCT_BLUE)
                          .Replace(CCT_BOLD, TFCCT_BOLD)
                          .Replace(CCT_DARK_AQUA, TFCCT_DARK_AQUA)
                          .Replace(CCT_DARK_BLUE, TFCCT_DARK_BLUE)
                          .Replace(CCT_DARK_GRAY, TFCCT_DARK_GRAY)
                          .Replace(CCT_DARK_GREEN, TFCCT_DARK_GREEN)
                          .Replace(CCT_DARK_PURPLE, TFCCT_DARK_PURPLE)
                          .Replace(CCT_DARK_RED, TFCCT_DARK_RED)
                          .Replace(CCT_GOLD, TFCCT_GOLD)
                          .Replace(CCT_GRAY, TFCCT_GRAY)
                          .Replace(CCT_GREEN, TFCCT_GREEN)
                          .Replace(CCT_ITALIC, TFCCT_ITALIC)
                          .Replace(CCT_LIGHT_PURPLE, TFCCT_LIGHT_PURPLE)
                          .Replace(CCT_MAGIC, TFCCT_MAGIC)
                          .Replace(CCT_RED, TFCCT_RED)
                          .Replace(CCT_RESET, TFCCT_RESET)
                          .Replace(CCT_STRIKETHROUGH, TFCCT_STRIKETHROUGH)
                          .Replace(CCT_UNDERLINE, TFCCT_UNDERLINE)
                          .Replace(CCT_WHITE, TFCCT_WHITE)
                          .Replace(CCT_YELLOW, TFCCT_YELLOW)
                          .Replace(CCT_COLOR_CHAR, TFCCT_COLOR_CHAR); // COLOR_CHAR should always be last (to catch any stray escape chars and not ruin regular color groups)

        }

        // Both tag and message must use the TFCCT colorchars
        public static void CreateFormattedGroupsForMessage(string tag, string body, out List<FormattedChatString> formattedTag, out List<FormattedChatString> formattedBody)
        {
            List<FormattedChatString> fTag = new List<FormattedChatString>();
            List<FormattedChatString> fBody = new List<FormattedChatString>();

            int lastIndex = 0;
            ChatColorType currentFormat = ChatColorType.None;
            for (int i = 0; i < tag.Length; i++)
            {
                // Walk string until chatcolor is hit
                if (tag[i] == asciiDC1)
                {
                    string ccraw = tag.Substring(i, 3);
                    ChatColorType chatcolor = TFChatColorMappings[ccraw];

                    // Group up all chars walked past with the old format
                    if (i - lastIndex > 0)
                        fTag.Add(new FormattedChatString(tag.Substring(lastIndex, i - lastIndex), currentFormat));

                    // Update the current format
                    currentFormat = CompositeChatColor(currentFormat, chatcolor);

                    // Advance cursor
                    i += 2;
                    lastIndex = i + 1;
                }
            }
            // Close fTag list
            if (lastIndex < tag.Length)
                fTag.Add(new FormattedChatString(tag.Substring(lastIndex, tag.Length - lastIndex), currentFormat));
            lastIndex = 0;

            // Now repeat for the body, carrying over the formatting from the tag
            for (int i = 0; i < body.Length; i++)
            {
                if (body[i] == asciiDC1)
                {
                    string ccraw = body.Substring(i, 3);
                    ChatColorType chatcolor = TFChatColorMappings[ccraw];

                    if (i - lastIndex > 0)
                        fBody.Add(new FormattedChatString(body.Substring(lastIndex, i - lastIndex), currentFormat));

                    currentFormat = CompositeChatColor(currentFormat, chatcolor);

                    i += 2;
                    lastIndex = i + 1;
                }
            }
            if (lastIndex < body.Length)
                fBody.Add(new FormattedChatString(body.Substring(lastIndex, body.Length - lastIndex), currentFormat));

            formattedTag = fTag;
            formattedBody = fBody;
        }
        private static ChatColorType CompositeChatColor(ChatColorType source, ChatColorType addition)
        {
            ChatColorType temp = source;

            if (addition == ChatColorType.RESET)
                return ChatColorType.None;
            else
            {
                if ((addition & AllChatFormats) > 0) // Colors reset formatting
                    temp = temp & ~AllChatFormats;

                if ((addition & AllChatColors) > 0) // Text cant have more than one color
                    return (temp & ~AllChatColors) | addition;
                else                                // But it can have more than one style formatter (i.e. bold, italic, magic, etc.) (I think)
                    return temp | addition;
            }
        }
    }
}