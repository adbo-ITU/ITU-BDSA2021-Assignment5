using GildedRose;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GildedRose.Tests
{
    public class TestAssemblyTests
    {
        Program _app;

        public TestAssemblyTests()
        {
            _app = new Program()
            {
                Items = new List<Item>
                {
                    new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                    new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                    new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                    new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new Item
                        {
                            Name = "Backstage passes to a TAFKAL80ETC concert",
                            SellIn = 15,
                            Quality = 20
                        },
                    new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 10}
                }
            };
        }

        [Fact]
        public void BadTest()
        {
            // This test is just to ignore Main... Please forgive us.
            Program.Main(new string[] { });
        }

        [Fact]
        public void UpdateQuality_after_1_updates()
        {
            // Act
            _app.UpdateQuality();

            // Assert
            var expected = new List<Item>
            {
                new Item { Name = "+5 Dexterity Vest", SellIn = 9, Quality = 19 },
                new Item { Name = "Aged Brie", SellIn = 1, Quality = 1 },
                new Item { Name = "Elixir of the Mongoose", SellIn = 4, Quality = 6 },
                new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 14, Quality = 21 },
                new Item { Name = "Conjured Mana Cake", SellIn = 2, Quality = 8 }
            };

            Assert.Equal(expected, _app.Items);
        }

        [Fact]
        public void UpdateQuality_after_30_updates()
        {
            for (int i = 0; i < 30; i++)
            {
                _app.UpdateQuality();
            }

            var expected = new List<Item>
            {
                new Item() { Name = "+5 Dexterity Vest", SellIn = -20, Quality = 0 },
                new Item() { Name = "Aged Brie", SellIn = -28, Quality = 50 },
                new Item() { Name = "Elixir of the Mongoose", SellIn = -25, Quality = 0 },
                new Item() { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
                new Item() { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = -15, Quality = 0 },
                new Item() { Name = "Conjured Mana Cake", SellIn = -27, Quality = 0 }
            };

            Assert.Equal(expected, _app.Items);
        }

        [Fact]
        public void After_SellDate_Quality_Degrades_twice()
        {
            for (int i = 0; i < 10; i++)
            {
                _app.UpdateQuality();
            }

            Assert.Equal(10, _app.Items[0].Quality);

            for (int i = 0; i < 5; i++)
            {
                _app.UpdateQuality();
            }

            Assert.Equal(0, _app.Items[0].Quality);
        }

        [Fact]
        public void Quality_never_more_than_50()
        {
            for (int i = 0; i < 100; i++)
            {
                _app.UpdateQuality();
            }

            foreach (Item i in _app.Items)
            {
                if (i.Name == "Sulfuras, Hand of Ragnaros")
                {
                    Assert.Equal(80, i.Quality);
                }
                else
                {
                    Assert.InRange(i.Quality, 0, 50);
                }
            }
        }

        [Fact]
        public void UpdateQuality_item_quality_is_never_negative()
        {
            // Arrange
            _app = new Program()
            {
                Items = new List<Item>
                {
                    new Item { Name = "Not negative", SellIn = 0, Quality = 0},
                }
            };

            // Act
            // This should decrease quality to below 0
            _app.UpdateQuality();

            // Assert
            Assert.Equal(0, _app.Items[0].Quality);
        }

        [Fact]
        public void UpdateQuality_sulfura_never_has_to_be_sold_or_decreases()
        {
            // Arrange
            Func<Item> GetSulfura = () => _app.Items.Where(item => item.Name == "Sulfuras, Hand of Ragnaros").First();
            var quality = GetSulfura().Quality;
            var sellIn = GetSulfura().SellIn;

            // Act
            for (int i = 0; i < 100; i++)
            {
                _app.UpdateQuality();
            }

            // Assert
            Assert.Equal(80, quality);
            Assert.Equal(0, sellIn);

            quality = GetSulfura().Quality;
            sellIn = GetSulfura().SellIn;

            Assert.Equal(80, quality);
            Assert.Equal(0, sellIn);
        }

        [Fact]
        public void UpdateQuality_aged_brie_increases_in_quality_with_time()
        {
            // Arrange
            Func<Item> GetBrie = () => _app.Items.Where(item => item.Name == "Aged Brie").First();
            var prev = GetBrie().Quality;
            Action AssertBrieIsHigherQualityThanBefore = () =>
            {
                var cur = GetBrie().Quality;
                Assert.True(cur > prev, "new quality was not greater than previous quality");
                prev = cur;
            };

            // Act + Assert
            for (int i = 0; i < 10; i++)
            {
                _app.UpdateQuality();
                AssertBrieIsHigherQualityThanBefore();
            }
        }

        [Fact]
        public void UpdateQuality_backstage_pass_quality_increases_until_sellin()
        {
            // Arrange
            Func<Item> GetBackstagePass = () => _app.Items.Where(item => item.Name == "Backstage passes to a TAFKAL80ETC concert").First();
            var prev = GetBackstagePass().Quality;
            Action AssertBackstagePassIsHigherQualityThanBefore = () =>
            {
                var cur = GetBackstagePass().Quality;
                Assert.True(cur > prev, "new quality was not greater than previous quality");
                prev = cur;
            };

            // Act + Assert
            for (int i = 0; i < 10; i++)
            {
                _app.UpdateQuality();
                AssertBackstagePassIsHigherQualityThanBefore();
            }
        }

        [Fact]
        public void UpdateQuality_backstage_pass_11_days_before_sellin_increases_wit_1()
        {
            var pass = _app.Items.Where(item => item.Name == "Backstage passes to a TAFKAL80ETC concert").First();
            var originalQuality = pass.Quality;
            pass.SellIn = 11;
            _app.UpdateQuality();
            Assert.Equal(originalQuality + 1, pass.Quality);
        }

        [Fact]
        public void UpdateQuality_backstage_pass_10_days_before_sellin_increases_with_2()
        {
            var pass = _app.Items.Where(item => item.Name == "Backstage passes to a TAFKAL80ETC concert").First();
            var originalQuality = pass.Quality;
            pass.SellIn = 10;
            _app.UpdateQuality();
            Assert.Equal(originalQuality + 2, pass.Quality);
        }

        [Fact]
        public void UpdateQuality_backstage_pass_5_days_before_sellin_increases_with_3()
        {
            var pass = _app.Items.Where(item => item.Name == "Backstage passes to a TAFKAL80ETC concert").First();
            var originalQuality = pass.Quality;
            pass.SellIn = 5;
            _app.UpdateQuality();
            Assert.Equal(originalQuality + 3, pass.Quality);
        }

        [Fact]
        public void UpdateQuality_backstage_pass_negative_sellin_has_quality_0()
        {
            var pass = _app.Items.Where(item => item.Name == "Backstage passes to a TAFKAL80ETC concert").First();
            var originalQuality = pass.Quality;
            pass.SellIn = -1;
            _app.UpdateQuality();
            Assert.Equal(0, pass.Quality);
        }

        [Fact]
        public void UpdateQuality_conjured_items_degrade_with_2_before_sellin()
        {
            var item = _app.Items.Where(item => item.Name.StartsWith("Conjured")).First();
            var originalQuality = item.Quality;
            _app.UpdateQuality();
            Assert.Equal(originalQuality - 2, item.Quality);
            _app.UpdateQuality();
            Assert.Equal(originalQuality - 4, item.Quality);
            _app.UpdateQuality();
            Assert.Equal(originalQuality - 6, item.Quality);
        }

        [Fact]
        public void UpdateQuality_conjured_items_degrade_with_4_after_sellin()
        {
            var item = _app.Items.Where(item => item.Name.StartsWith("Conjured")).First();
            _app.UpdateQuality();
            _app.UpdateQuality();
            _app.UpdateQuality();
            Assert.Equal(0, item.SellIn);

            var originalQuality = item.Quality;
            _app.UpdateQuality();
            Assert.Equal(originalQuality - 4, item.Quality);
        }
    }
}
