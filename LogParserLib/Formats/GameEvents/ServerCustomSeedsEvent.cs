using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ServerCustomSeedsEvent : GameEvent
    {
        // 1.13.2
        public string SeedVillage = "";
        public string SeedDesert = "";
        public string SeedIgloo = "";
        public string SeedJungle = "";
        public string SeedSwamp = "";
        public string SeedMonument = "";
        public string SeedOcean = "";
        public string SeedShipwreck = "";
        public string SeedSlime = "";

        // 1.9.2
        //            SeedVillage
        public string SeedFeature = "";

        private readonly char[] numerals = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private readonly List<char> numeralsList = new List<char>(){ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public ServerCustomSeedsEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            // Real example from 1.13.2:
            // Custom Map Seeds:  Village: 10387312 Desert: 14357617 Igloo: 14357618 Jungle: 14357619 Swamp: 14357620 Monument: 10387313Ocean: 14357621 Shipwreck: 165745295 Slime: 987234911
            // Note the lack of a space between the Monument seed value and the "Ocean:" key

            string check = Source.Body;
            SeedVillage = getSeed(check, "Village: ");
            SeedDesert = getSeed(check, "Desert: ");
            SeedIgloo = getSeed(check, "Igloo: ");
            SeedJungle = getSeed(check, "Jungle: ");
            SeedSwamp = getSeed(check, "Swamp: ");
            SeedMonument = getSeed(check, "Monument: ");
            SeedOcean = getSeed(check, "Ocean: ");
            SeedShipwreck = getSeed(check, "Shipwreck: ");
            SeedSlime = getSeed(check, "Slime: ");

            SeedFeature = getSeed(check, "Feature: ");
        }

        private string getSeed(string check, string key)
        {
            int spot = check.IndexOf(key);
            if (spot == -1)
                return "";
            spot += key.Length;
            int spot2 = check.IndexOfAny(numerals, spot + 1);
            while ((spot2 < check.Length) && numeralsList.Contains(check[spot2]))
                spot2++;
            return check.Substring(spot, spot2 - spot);
        }
    }
}
