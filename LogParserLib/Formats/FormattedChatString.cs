using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Simple consolidator of a string and some formatting
    public class FormattedChatString
    {
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals


        //================| Serializable fields |================//

        [JsonProperty(Order = 1001)] public string Text;
            public bool ShouldSerializeText() { return !E_Options.FormattedChatString_UseHTMLFormatting; }
        [JsonProperty(Order = 1002)] public ChatColorType Formatting;
            public bool ShouldSerializeFormatting() { return !E_Options.FormattedChatString_UseHTMLFormatting; }

        [JsonProperty(Order = 1003)] public string TextHTML;
            public bool ShouldSerializeTextHTML() { return E_Options.FormattedChatString_UseHTMLFormatting; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public FormattedChatString(string text, ChatColorType formatting)
        {
            Text = text;
            Formatting = formatting;

            TextHTML = "";
        }
        
        public void BuildHTMLText(string htmlTag, string cssClassForMagic)
        {
            string css = "";
            foreach (ChatColorType cct in PlayerChatHelper.HTMLColorMappings.Keys)
                htmlAddColor(cct, ref css);

            if (Formatting.HasFlag(ChatColorType.BOLD))
                css += "font-weight: bold;";
            
            if (Formatting.HasFlag(ChatColorType.ITALIC))
                css += "font-style: italic;";

            string lines = "text-decoration:";
            if (Formatting.HasFlag(ChatColorType.UNDERLINE) || Formatting.HasFlag(ChatColorType.STRIKETHROUGH))
            {
                if (Formatting.HasFlag(ChatColorType.UNDERLINE))
                    lines += " underline";
                if (Formatting.HasFlag(ChatColorType.STRIKETHROUGH))
                    lines += " line-through";
                lines += ";";
                css += lines;
            }

            string magic = "";
            if (Formatting.HasFlag(ChatColorType.MAGIC))
                magic = "class=\"" + cssClassForMagic + "\"";

            TextHTML = "<" + htmlTag + magic + " style=\"" + css + "\">" + Text + "</" + htmlTag + ">";
        }
        private void htmlAddColor(ChatColorType cct, ref string css)
        {
            if (Formatting.HasFlag(cct))
                css += "color: #" + PlayerChatHelper.HTMLColorMappings[cct] + ";";
        }
    }
}
