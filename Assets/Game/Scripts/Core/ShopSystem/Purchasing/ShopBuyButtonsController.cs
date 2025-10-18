using System;
using System.Linq;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.InventorySystem;
using GamePush;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyButtonsController : IInitializable, IDisposable
    {
        private readonly ShopBuyWindow m_window;
        private readonly SeedsConfig m_seedsConfig;

        public ShopBuyButtonsController(ShopBuyWindow window, SeedsConfig seedsConfig)
        {
            m_window = window;
            m_seedsConfig = seedsConfig;
        }

        public void Initialize()
        {
            m_window.ItemSelected += OnItemSelected;
        }

        public void Dispose()
        {
            m_window.ItemSelected -= OnItemSelected;
        }

        private void OnItemSelected(SeedItem item)
        {
            SetupSoftButton(item);
            SetupRealButton(item);
            SetupAdsButton(item);
        }

        private void SetupSoftButton(SeedItem item)
        {
            bool isAvailable = item.Amount > 0;
            
            m_window.SoftButton.Button.interactable = isAvailable;

            string text = isAvailable ? 
                StringsUtils.FormatMoney(item.Cost) : 
                LocalizationUtils.GetLocalized("No stock", "Нет в продаже");
            
            m_window.SoftButton.SetText(text);
        }
        
        private void SetupRealButton(SeedItem item)
        {
            var product = GP_Payments.Products.FirstOrDefault(product => product.tag == item.Tag);
            bool hasProduct = product != null;
            
            m_window.RealButton.gameObject.SetActive(hasProduct);

            if (hasProduct == false)
                return;
            
            m_window.RealButton.SetText($"{product.price.InsertSpaces()} {product.currencySymbol}");
        }

        private void SetupAdsButton(SeedItem item)
        {
            bool isAdsAvailable = GP_Ads.IsRewardedAvailable();
            bool hasAdsButton = m_seedsConfig.GetSeedData(item.TypeId).Ads && isAdsAvailable;
            m_window.AdsButton.gameObject.SetActive(hasAdsButton);

#if UNITY_EDITOR
            m_window.AdsButton.gameObject.SetActive(true);
#endif
        }
    }
}