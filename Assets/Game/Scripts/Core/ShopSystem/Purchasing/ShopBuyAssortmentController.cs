using System;
using System.Linq;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.InventorySystem;
using GamePush;
using LitMotion;
using UnityEngine;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyAssortmentController : IInitializable, IDisposable
    {
        private const float REFRESH_TIME_SEC = 300f;
        
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyModel m_model;
        private readonly SeedsConfig m_seedsConfig;
        private readonly ItemFactory m_itemFactory;

        private IDisposable m_disposable;
        private MotionHandle m_handle;

        public ShopBuyAssortmentController(ShopBuyModel model, SeedsConfig seedsConfig, ItemFactory itemFactory, ShopBuyWindow window)
        {
            m_model = model;
            m_seedsConfig = seedsConfig;
            m_itemFactory = itemFactory;
            m_window = window;
        }

        public void Initialize()
        {
            StartTimer();
            RestockAssortment();
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        public void RestockAndRestart()
        {
            m_handle.Cancel();
            
            StartTimer();
            RestockAssortment();

            ShowNotification();
        }

        private void StartTimer()
        {
            m_handle = LMotion.Create(REFRESH_TIME_SEC, 0, REFRESH_TIME_SEC)
                .WithScheduler(MotionScheduler.TimeUpdateRealtime)
                .WithLoops(-1)
                .WithOnLoopComplete(_ => RestockAndRestart())
                .Bind(SetTimerText);

            m_disposable = m_handle.ToDisposable();
        }

        private void RestockAssortment()
        {
            m_model.AssortmentList.Value =
                m_seedsConfig.DataList.Select(data => CreateItem(data.Id)).ToList();
        }

        private void SetTimerText(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)MathF.Ceiling(time - minutes * 60f);
            
            if (minutes > 0)
            {
                m_window.SetHeaderText(LocalizationUtils.GetLocalized(
                    $"Restock in {minutes}m {seconds}s",
                    $"Обновление через {minutes}м {seconds}с"));
            }
            else
            {
                m_window.SetHeaderText(LocalizationUtils.GetLocalized(
                    $"Restock in {seconds}s",
                    $"Обновление через {seconds}с"));
            }
        }

        private SeedItem CreateItem(int seedId)
        {
            var item = m_itemFactory.CreateSeed(seedId);
            item.Amount = GetAmount(item.Rarity);
            
            return item;
        }

        private int GetAmount(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    return Random.Range(5, 15);
                
                case Rarity.Uncommon:
                    return Random.value < 0.7f ? Random.Range(2, 8) : 0;
                
                case Rarity.Rare:
                    return Random.value < 0.5f ? Random.Range(1, 6) : 0;
                
                case Rarity.Epic:
                    return Random.value < 0.3f ? Random.Range(1, 4) : 0;
            }

            return 0;
        }
        
        private void ShowNotification()
        {
            Notification.Instance.Show(LocalizationUtils.GetLocalized(
                    $"The store's assortment has been updated",
                    $"Ассортимент магазина обновлен"),
                Color.forestGreen);
        }
    }
}