using Game.Scripts.Common.Misc;
using Game.Scripts.Core.InventorySystem.UI.Views;
using R3;

namespace Game.Scripts.Core.InventorySystem
{
    public class InventoryModel
    {
        public BiDictionary<UIItemSlot, Item> Slots { get; } = new();
        public ReactiveProperty<UIItemSlot> SelectedSlot { get; } = new();
        
        public bool IsPlantFilter { get; set; }
        
        public void Clear()
        {
            Slots.Clear();
            SelectedSlot.Value = null;
        }
    }
}