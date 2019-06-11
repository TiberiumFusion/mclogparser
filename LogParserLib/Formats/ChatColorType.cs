using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Using the Bukkit API names (as of 1.12.2)
    [Flags]
    public enum ChatColorType
    {
        None = 0,
        AQUA = 1,
        BLACK = 2,
        BLUE = 4,
        BOLD = 8,
        DARK_AQUA = 16,
        DARK_BLUE = 32,
        DARK_GRAY = 64,
        DARK_GREEN = 128,
        DARK_PURPLE = 256,
        DARK_RED = 512,
        GOLD = 1024,
        GRAY = 2048,
        GREEN = 4096,
        ITALIC = 8192,
        LIGHT_PURPLE = 16384,
        MAGIC = 32768,
        RED = 65536,
        RESET = 131072,
        STRIKETHROUGH = 262144,
        UNDERLINE = 524288,
        WHITE = 1048576,
        YELLOW = 2097152,
        COLOR_CHAR = 4194304,
    }
}
