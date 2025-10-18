using System;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using GamePush;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyRealButtonController : IInitializable, IDisposable
    {
        private const string ANALYTICS_PURCHASE_KEY = "PURCHASE_REAL";
        private const string ANALYTICS_PURCHASE_SUCCESSFUL_KEY = "PURCHASE_REAL_SUCCESSFUL";
        private const string ANALYTICS_PURCHASE_FAILED_KEY = "PURCHASE_REAL_FAILED";
        
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyModel m_model;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ItemFactory m_itemFactory;
        
        private IDisposable m_disposable;

        public ShopBuyRealButtonController(ShopBuyWindow window, InventoryRepository inventoryRepository, ItemFactory itemFactory, ShopBuyModel model)
        {
            m_window = window;
            m_inventoryRepository = inventoryRepository;
            m_itemFactory = itemFactory;
            m_model = model;
        }

        public void Initialize()
        {
            var d1 = m_window.RealButton.Button.OnClickAsObservable().Subscribe(_ => OnRealPurchasePerformed());

            m_disposable = Disposable.Combine(d1);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnRealPurchasePerformed()
        {
            if (m_inventoryRepository.IsSeedsFull())
            {
                Notification.Instance.ShowInventoryIsFull();
                return;
            }
            
            var item = m_model.SelectedItem.CurrentValue;
            
            if (item == null)
                return;
            
            // Debug.Log($"<color=green>Purchase</color> performed {item.Tag}");
            
            // 1. Производится покупка
            GP_Payments.Purchase(item.Tag, OnPurchaseSuccess, OnPurchaseError);
            
            GP_Analytics.Goal(ANALYTICS_PURCHASE_KEY, item.Tag);
        }

        private void OnPurchaseSuccess(string tag)
        {
            // Debug.Log($"<color=green>Purchase</color> success {tag}");
            GP_Analytics.Goal(ANALYTICS_PURCHASE_SUCCESSFUL_KEY, tag);
            
            // 2. Начисляется награда
            var newItem = m_itemFactory.CreateSeed(tag);
            m_inventoryRepository.TryAddItem(newItem);
            
            // 3. Игрок сохраняется на сервере
            // автоматически
            // GP_Player.Sync();
            
            SoundsManager.Instance.PlayMoney();
            Notification.Instance.ShowItemAdded(newItem);
            
            // 4. Покупка консумируется (расходуется)
            GP_Payments.Consume(tag, OnConsumeSuccess);
        }
        
        private void OnPurchaseError() => GP_Analytics.Goal(ANALYTICS_PURCHASE_KEY, "FAILED");

        private void OnConsumeSuccess(string tag)
        {
            Debug.Log($"<color=green>Consume</color> success {tag}");
        }
    }
}