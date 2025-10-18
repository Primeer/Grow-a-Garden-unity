using Game.Scripts.Core.InventorySystem;

namespace Game.Scripts.Core.MoneySystem
{
    public class ItemSellingService
    {
        private readonly InventoryRepository m_inventoryRepository;
        private readonly MoneyRepository m_moneyRepository;

        public ItemSellingService(InventoryRepository inventoryRepository, MoneyRepository moneyRepository)
        {
            m_inventoryRepository = inventoryRepository;
            m_moneyRepository = moneyRepository;
        }

        public void SellItem(Item item)
        {
            if (m_inventoryRepository.HasItem(item) == false)
                return;

            m_inventoryRepository.RemoveItem(item);
            m_moneyRepository.Money.Value += item.Cost;
        }
    }
}