using System;
using Game.Scripts.Common;
using Game.Scripts.Core.Audio;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellWindowController : IInitializable, IDisposable
    {
        private readonly ShopSellWindow m_shopSellWindow;
        private readonly ScreenDispatcher m_screenDispatcher;

        public ShopSellWindowController(ShopSellWindow shopSellWindow, ScreenDispatcher screenDispatcher)
        {
            m_shopSellWindow = shopSellWindow;
            m_screenDispatcher = screenDispatcher;
        }

        public void Initialize()
        {
            m_shopSellWindow.UsableObject.UsePerformed += OnUse;
            m_shopSellWindow.ClosePerformed += OnClosePerformed;
        }

        public void Dispose()
        {
            m_shopSellWindow.UsableObject.UsePerformed -= OnUse;
            m_shopSellWindow.ClosePerformed -= OnClosePerformed;
        }

        private void OnUse()
        {
            ShowShop();
        }

        private void OnClosePerformed()
        {
            HideShop();
        }

        private void ShowShop()
        {
            m_screenDispatcher.ShopSell(true);
            
            SoundsManager.Instance.PlayShopOpen();
        }

        private void HideShop()
        {
            m_screenDispatcher.ShopSell(false);
        
            SoundsManager.Instance.PlayShopClose();
        }
    }
}