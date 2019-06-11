using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // Simple Location consolidator, like that of the Bukkit API
    public struct Location
    {
        public double X;
        public double Y;
        public double Z;
        public string World;

        public Location(double x, double y, double z, string world)
        {
            X = x;
            Y = y;
            Z = z;
            World = world;
        }
    }
}
