using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Simple consolidation class, not meant to have substantial data on a world (since the log files provide virtually nothing)
    public class ServerLevel
    {
        public string LoadedName; // Name as provided by the server when loading the world. This will be 0, 1, 2, etc. for older minecraft versions.
        public string Seed;
        //public Dictionary<string, string> CustomMapSeeds = new Dictionary<string, string>();

        public ServerLevel(string loadedName, string seed)
        {
            LoadedName = loadedName;
            Seed = seed;
        }
    }
}
