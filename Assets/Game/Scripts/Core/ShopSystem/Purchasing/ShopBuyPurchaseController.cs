using System;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.MoneySystem;
using GamePush;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyPurchaseController : IInitializable, IDisposable
    {
        private const string ANALYTICS_KEY = "PURCHASE_SOFT";
        
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyModel m_model;
        private readonly MoneyRepository m_moneyRepository;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ItemFactory m_itemFactory;

        private IDisposable m_disposable;

        public ShopBuyPurchaseController(ShopBuyWindow window, ShopBuyModel model, MoneyRepository moneyRepository, InventoryRepository inventoryRepository, ItemFactory itemFactory)
        {
            m_window = window;
            m_model = model;
            m_moneyRepository = moneyRepository;
            m_inventoryRepository = inventoryRepository;
            m_itemFactory = itemFactory;
        }

        public void Initialize()
        {
            var d1 = m_window.SoftButton.Button.OnClickAsObservable().Subscribe(_ => OnPurchasePerformed());

            m_disposable = Disposable.Combine(d1);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnPurchasePerformed()
        {
            var item = m_model.SelectedItem.CurrentValue;
            
            if (item == null)
                return;
            
            if (m_moneyRepository.Money.Value < item.Cost)
            {
                Notification.Instance.ShowNotEnoughMoney();
                return;
            }

            var newItem = m_itemFactory.CreateSeed(item.Tag);
            
            if (m_inventoryRepository.TryAddItem(newItem))
            {
                m_moneyRepository.Money.Value -= item.Cost;
                item.Amount--;
                m_model.Items[item].SetItem(item);
                
                SoundsManager.Instance.PlayMoney();
                Notification.Instance.ShowItemAdded(newItem);
                GP_Analytics.Goal(ANALYTICS_KEY, item.Tag);
                
                if (item.Amount > 0)
                    return;
                
                DisableSoftButton();
            }
            else
            {
                Notification.Instance.ShowInventoryIsFull();
            }
        }
        
        private void DisableSoftButton()
        {
            m_window.SoftButton.Button.interactable = false;
            m_window.SoftButton.SetText("Нет");
        }
    }
}