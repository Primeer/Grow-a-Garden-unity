using UnityEngine;

namespace Game.Scripts.Core.InventorySystem
{
    public class Item
    {
        public int TypeId { get; set; }
        public string Ru { get; set; }
        public string En { get; set; }
        public Sprite Icon { get; set; }
        public int Cost { get; set; }
    }
}