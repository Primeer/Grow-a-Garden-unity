using System.Collections.Generic;
using Game.Scripts.Common.Misc;
using Game.Scripts.Core.InventorySystem;
using R3;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyModel
    {
        public BiDictionary<UIShopItem, SeedItem> Items { get; } = new();
        public ReactiveProperty<SeedItem> SelectedItem { get; } = new();
        public ReactiveProperty<List<SeedItem>> AssortmentList { get; } = new();
    }
}