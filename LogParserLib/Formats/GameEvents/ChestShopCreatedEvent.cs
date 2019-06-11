using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ChestShopCreatedEvent : GameEvent
    {
        public NameWithUUID CreatedByPlayer;
        public Location ShopLocation;
        public string ShopName;
        public string ItemName;
        public string ItemCount;
        public double BuyPrice;
        public double SellPrice;

        public ChestShopCreatedEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;

            // Example: [ChestShop] LegitPlayer created an Admin Shop - 1 Stone:1 - B 1:S 1 - at [world] 100, 200, 300
            // But I've seen this as well: [ChestShop] LegitPlayer created an Admin Shop - 1 Stone:1 - B 1:1 S - at [world] 100, 200, 300
            // Also, shops can be either buy/sell and not both. i.e. [ChestShop] LegitPlayer created an Admin Shop - 1 Stone:1 - 1 S - at [world] 100, 200, 300
            
            int spot = check.IndexOf(' ');
            spot++;
            int spot2 = check.IndexOf(' ', spot + 1);
            CreatedByPlayer.Name = check.Substring(spot, spot2 - spot);

            spot2 = Source.Body.IndexOf("created ");
            spot2 += 8;
            spot2 = Source.Body.IndexOf(' ', spot2 + 1);
            spot2++;
            spot = Source.Body.IndexOf('-');
            spot--;
            ShopName = Source.Body.Substring(spot2, spot - spot2);

            spot += 3;
            string main = Source.Body.Substring(spot, Source.Body.Length - spot);

            spot = main.IndexOf(' ');
            ItemCount = main.Substring(0, spot);

            spot++;
            spot2 = main.IndexOf('-', spot + 1);
            spot2--;
            ItemName = main.Substring(spot, spot2 - spot);

            spot = main.IndexOf('-', spot2 + 1);
            spot++;
            spot2 = main.IndexOf('-', spot + 1);
            string buysell = main.Substring(spot, spot2 - spot);
            string[] cuts = buysell.Split(':');
            foreach (string cut in cuts)
            {
                if (cut.Contains("B"))
                    BuyPrice = double.Parse(new string(cut.Where(x => char.IsDigit(x)).ToArray()));
                else if (cut.Contains("S"))
                    SellPrice = double.Parse(new string(cut.Where(x => char.IsDigit(x)).ToArray()));
            }
            
            spot = main.IndexOf('[', spot2);
            spot++;
            spot2 = main.IndexOf(']', spot + 1);
            ShopLocation.World = main.Substring(spot, spot2 - spot);

            string[] rest = main.Substring(spot2 + 2).Split(' ');
            ShopLocation.X = double.Parse(rest[0].Replace(",", ""));
            ShopLocation.Y = double.Parse(rest[1].Replace(",", ""));
            ShopLocation.Z = double.Parse(rest[2].Replace(",", ""));
        }

        public override void UUIDPass(AnalyzedData analyzedData)
        {
            string UUID = analyzedData.FindBestUUIDMatchFor(CreatedByPlayer.Name, Source.Time);
            CreatedByPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
