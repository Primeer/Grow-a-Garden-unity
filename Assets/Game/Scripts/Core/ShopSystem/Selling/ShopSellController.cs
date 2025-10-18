using System;
using System.Collections.Generic;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.MoneySystem;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellController : IInitializable, IDisposable
    {
        private readonly ShopSellWindow m_shopSellWindow;
        private readonly ShopSellModel m_shopModel;
        private readonly ItemSellingService m_sellingService;

        private IDisposable m_disposable;

        public ShopSellController(ShopSellWindow shopSellWindow, ShopSellModel shopModel, ItemSellingService sellingService)
        {
            m_shopSellWindow = shopSellWindow;
            m_shopModel = shopModel;
            m_sellingService = sellingService;
        }

        public void Initialize()
        {
            var d1 = m_shopSellWindow.SellButton.Button.OnClickAsObservable().Subscribe(_ => OnSellPerformed());
            var d2 = m_shopSellWindow.SellAllButton.Button.OnClickAsObservable().Subscribe(_ => OnSellAllPerformed());

            m_disposable = Disposable.Combine(d1, d2);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnSellPerformed()
        {
            var selectedSlot = m_shopModel.SelectedSlot.Value;

            if (selectedSlot == null)
                return;

            var item = m_shopModel.Slots[selectedSlot];

            m_sellingService.SellItem(item);
            
            SoundsManager.Instance.PlayMoney();
        }

        private void OnSellAllPerformed()
        {
            var items = new List<Item>(m_shopModel.Slots.Values);

            foreach (var item in items)
            {
                m_sellingService.SellItem(item);
            }
            
            SoundsManager.Instance.PlayMoney();
        }
    }
}