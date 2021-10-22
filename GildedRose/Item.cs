using System;

namespace GildedRose
{
    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }

        public virtual void Update()
        {
            UpdateQuality();
            UpdateSellIn();

            Quality = Math.Max(Math.Min(Quality, 50), 0);
        }

        protected virtual void UpdateQuality()
        {
            Quality -= SellIn <= 0 ? 2 : 1;
        }

        protected virtual void UpdateSellIn()
        {
            SellIn--;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Item)obj;
            return Name == other.Name && SellIn == other.SellIn && Quality == other.Quality;
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}
