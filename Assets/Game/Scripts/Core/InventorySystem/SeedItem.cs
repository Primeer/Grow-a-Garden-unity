using Game.Scripts.Core.ShopSystem.Purchasing;

namespace Game.Scripts.Core.InventorySystem
{
    public class SeedItem : Item
    {
        public int Amount { get; set; }
        public int PlantId { get; set; }
        public Rarity Rarity { get; set; }
        public string Tag { get; set; }
    }
}