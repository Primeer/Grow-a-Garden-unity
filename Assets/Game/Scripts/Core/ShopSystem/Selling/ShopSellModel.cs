using Game.Scripts.Common.Misc;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.InventorySystem.UI.Views;
using R3;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellModel
    {
        public BiDictionary<UIItemSlot, Item> Slots { get; } = new();

        public ReactiveProperty<UIItemSlot> SelectedSlot { get; } = new();

        public void Clear()
        {
            Slots.Clear();
            SelectedSlot.Value = null;
        }
    }
}