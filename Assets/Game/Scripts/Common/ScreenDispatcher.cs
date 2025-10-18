using Game.Scripts.Core.CharacterController;
using Game.Scripts.Core.InventorySystem.UI.Views;
using Game.Scripts.Core.ShopSystem.Purchasing;
using Game.Scripts.Core.ShopSystem.Selling;

namespace Game.Scripts.Common
{
    public class ScreenDispatcher
    {
        private readonly ShopSellWindow m_shopSellWindow;
        private readonly ShopBuyWindow m_shopBuyWindow;
        private readonly InventoryWindow m_inventoryWindow;
        private readonly CharacterMovement m_movement;

        public ScreenDispatcher(ShopSellWindow shopSellWindow, ShopBuyWindow shopBuyWindow, InventoryWindow inventoryWindow, CharacterMovement movement)
        {
            m_shopSellWindow = shopSellWindow;
            m_shopBuyWindow = shopBuyWindow;
            m_inventoryWindow = inventoryWindow;
            m_movement = movement;
        }

        public void ShopSell(bool isActive)
        {
            m_shopSellWindow.gameObject.SetActive(isActive);
            m_inventoryWindow.gameObject.SetActive(!isActive);
            m_movement.SetActive(!isActive);

            if (isActive)
            {
                m_shopBuyWindow.gameObject.SetActive(false);
            }
        }
        
        public void ShopBuy(bool isActive)
        {
            m_shopBuyWindow.gameObject.SetActive(isActive);
            m_movement.SetActive(!isActive);

            if (isActive)
            {
                m_shopSellWindow.gameObject.SetActive(false);
            }
        }
    }
}