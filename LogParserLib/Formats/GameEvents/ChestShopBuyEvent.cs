using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents
{
    public class ChestShopBuyEvent : GameEvent
    {
        public NameWithUUID PurchasingPlayer;
        public Location ShopLocation;
        public string ShopName;
        public string ItemName;
        public string ItemCount;
        public double SalePrice;

        public ChestShopBuyEvent(LogLine source) : base(source) { }

        protected override void parse()
        {
            string check = Source.Body;

            // Example: [ChestShop] LegitPlayer bought 51 Stone:1 for 7.97 from Admin Shop at [world] 100, 200, 300

            int spot = check.IndexOf(' ');
            spot++;
            int spot2 = check.IndexOf(' ', spot + 1);
            PurchasingPlayer.Name = check.Substring(spot, spot2 - spot);

            spot = Source.Body.IndexOf("bought ");
            spot += 7;
            string main = Source.Body.Substring(spot, Source.Body.Length - spot);

            spot = main.IndexOf(' ');
            ItemCount = main.Substring(0, spot);

            spot++;
            spot2 = main.IndexOf(' ', spot + 1);
            ItemName = main.Substring(spot, spot2 - spot);

            spot = main.IndexOf("for ");
            spot += 4;
            spot2 = main.IndexOf(' ', spot + 1);
            SalePrice = double.Parse(main.Substring(spot, spot2 - spot));

            spot = main.IndexOf("from ", spot2);
            spot += 5;
            spot2 = main.IndexOf(" at", spot + 1);
            ShopName = main.Substring(spot, spot2 - spot);
            
            spot = main.IndexOf('[', spot2 + 1);
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
            string UUID = analyzedData.FindBestUUIDMatchFor(PurchasingPlayer.Name, Source.Time);
            PurchasingPlayer.UUID = (UUID != null) ? UUID : "";
        }
    }
}
