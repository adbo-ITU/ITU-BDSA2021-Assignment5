namespace GildedRose
{
    public class Conjured : Item
    {
        protected override void UpdateQuality() => Quality -= SellIn > -1 ? 2 : 4;
    }
}