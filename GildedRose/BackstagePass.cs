namespace GildedRose
{
    public class BackstagePass : Item
    {
        protected override void UpdateQuality()
        {
            if (SellIn < 0) Quality = 0;
            else if (SellIn <= 5) Quality += 3;
            else if (SellIn <= 10) Quality += 2;
            else Quality++;
        }
    }
}
