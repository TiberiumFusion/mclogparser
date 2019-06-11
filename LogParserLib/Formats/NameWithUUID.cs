using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Simple player name and UUID pair
    public struct NameWithUUID
    {
        public string Name;
        public string UUID;

        public static NameWithUUID Empty { get; } = new NameWithUUID("", "");

        public NameWithUUID(string name, string uuid)
        {
            Name = name;
            UUID = uuid;
        }
    }
}
