using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.SaveSystem;
using GamePush;
using Sirenix.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class PurchaseConsumer : IInitializable, IDisposable
    {
        private readonly SavingService m_savingService;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ItemFactory m_itemFactory;

        public PurchaseConsumer(InventoryRepository inventoryRepository, ItemFactory itemFactory, SavingService savingService)
        {
            m_inventoryRepository = inventoryRepository;
            m_itemFactory = itemFactory;
            m_savingService = savingService;
        }

        public void Initialize()
        {
            m_savingService.GameStateLoaded += OnGameStateLoaded;
        }

        public void Dispose()
        {
            m_savingService.GameStateLoaded -= OnGameStateLoaded;
            GP_Payments.OnFetchPlayerPurchases -= OnFetchPlayerPurchases;
        }

        private void OnGameStateLoaded()
        {
            OnFetchPlayerPurchases(GP_Payments.Purchases);
            
            GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
        }

        private void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purchases)
        {
            if (purchases.IsNullOrEmpty())
                return;

            List<SeedItem> items = new List<SeedItem>(purchases.Count);
            
            var sbEn = new StringBuilder("Got ");
            var sbRu = new StringBuilder("Получено ");
            
            foreach (var purchase in purchases)
            {
                var item = Consume(purchase);
                items.Add(item);
            }
            
            sbEn.AppendJoin(", ", items.Select(item => $"{item.En} x{item.Amount}"));
            sbRu.AppendJoin(", ", items.Select(item => $"{item.Ru} х{item.Amount}"));
            
            Notification.Instance.Show(LocalizationUtils.GetLocalized(
                    sbEn.ToString(),
                    sbRu.ToString()),
                Color.forestGreen);
            
            SoundsManager.Instance.PlayMoney();
        }
        
        public SeedItem Consume(FetchPlayerPurchases purchase)
        {
            Debug.Log($"<color=red>Unconsumed Purchase</color> {purchase.tag}");
                
            var newItem = m_itemFactory.CreateSeed(purchase.tag);
            m_inventoryRepository.TryAddItem(newItem);

            GP_Payments.Consume(purchase.tag, OnConsumeSuccess);

            return newItem;
        }

        private void OnConsumeSuccess(string tag)
        {
            Debug.Log($"<color=green>Consume</color> success {tag}");
        }
    }
}