namespace GildedRose
{
    public class Conjured : Item
    {
        protected override void UpdateQuality() => Quality -= SellIn > 0 ? 2 : 4;
    }
}