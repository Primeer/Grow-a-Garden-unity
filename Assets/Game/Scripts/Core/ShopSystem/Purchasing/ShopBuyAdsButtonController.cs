using System;
using Game.Scripts.Core.InventorySystem;
using GamePush;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyAdsButtonController : IInitializable, IDisposable
    {
        private const string ANALYTICS_PURCHASE_KEY = "PURCHASE_ADS";
        private const string ANALYTICS_PURCHASE_SUCCESSFUL_KEY = "PURCHASE_ADS_SUCCESSFUL";
        
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyModel m_model;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ItemFactory m_itemFactory;
        
        private IDisposable m_disposable;

        public ShopBuyAdsButtonController(ShopBuyWindow window, ShopBuyModel model, InventoryRepository inventoryRepository, ItemFactory itemFactory)
        {
            m_window = window;
            m_model = model;
            m_inventoryRepository = inventoryRepository;
            m_itemFactory = itemFactory;
        }

        public void Initialize()
        {
            var d1 = m_window.AdsButton.Button.OnClickAsObservable().Subscribe(_ => OnAdsPerformed());
            
            m_disposable = Disposable.Combine(d1);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }
        
        private void OnAdsPerformed()
        {
            if (m_inventoryRepository.IsSeedsFull())
            {
                Notification.Instance.ShowInventoryIsFull();
                return;
            }
            
            var item = m_model.SelectedItem.CurrentValue;
            
            if (item == null)
                return;

            // Debug.Log("<color=blue>Ads</color> performed");
            GP_Ads.ShowRewarded(item.Tag, OnRewardedReward);
            
            GP_Analytics.Goal(ANALYTICS_PURCHASE_KEY, item.Tag);
        }
        
        private void OnRewardedReward(string tag)
        {
            // Debug.Log("<color=blue>Ads</color> rewarded");
            
            var newItem = m_itemFactory.CreateSeed(tag);
            m_inventoryRepository.TryAddItem(newItem);
            
            Notification.Instance.ShowItemAdded(newItem);
            GP_Analytics.Goal(ANALYTICS_PURCHASE_SUCCESSFUL_KEY, tag);
        }
    }
}