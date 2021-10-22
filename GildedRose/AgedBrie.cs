namespace GildedRose
{
    public class AgedBrie : Item
    {
        protected override void UpdateQuality() => Quality += SellIn >= 0 ? 1 : 2;
    }
}
