using System;
using Game.Scripts.Common;
using Game.Scripts.Core.Audio;
using Sirenix.Utilities;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyWindowController : IInitializable, IDisposable
    {
        private readonly ShopBuyWindow m_window;
        private readonly ScreenDispatcher m_screenDispatcher;

        public ShopBuyWindowController(ShopBuyWindow window, ScreenDispatcher screenDispatcher)
        {
            m_window = window;
            m_screenDispatcher = screenDispatcher;
        }

        public void Initialize()
        {
            m_window.UsableObject.UsePerformed += OnUse;
            m_window.CloseButtons.ForEach(b => b.onClick.AddListener(OnClosePerformed));
        }

        public void Dispose()
        {
            m_window.UsableObject.UsePerformed -= OnUse;
            m_window.CloseButtons.ForEach(b => b.onClick.RemoveAllListeners());
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
            m_screenDispatcher.ShopBuy(true);
            
            SoundsManager.Instance.PlayShopOpen();
        }

        private void HideShop()
        {
            m_screenDispatcher.ShopBuy(false);
            
            SoundsManager.Instance.PlayShopClose();
        }
    }
}