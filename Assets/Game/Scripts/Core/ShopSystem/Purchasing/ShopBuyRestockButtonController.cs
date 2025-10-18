using System;
using GamePush;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyRestockButtonController : IInitializable, IDisposable
    {
        private const string ADS_TAG = "ads_restock";
        
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyAssortmentController m_assortmentController;
        
        private IDisposable m_disposable;

        public ShopBuyRestockButtonController(ShopBuyWindow window, ShopBuyAssortmentController assortmentController)
        {
            m_window = window;
            m_assortmentController = assortmentController;
        }

        public void Initialize()
        {
            m_window.Showed += OnWindowShowed;
            m_window.Hided += OnWindowHided;
        }

        public void Dispose()
        {
            m_window.Showed -= OnWindowShowed;
            m_window.Hided -= OnWindowHided;
        }
        
        private void OnWindowShowed()
        {
            bool isAdsAvailable = GP_Ads.IsRewardedAvailable();
            
            m_window.RestockButton.gameObject.SetActive(isAdsAvailable);
            
            if (isAdsAvailable == false)
                return;
            
            m_disposable = m_window.RestockButton.Button.OnClickAsObservable().Subscribe(_ => OnButtonPerformed());
        }

        private void OnWindowHided()
        {
            m_disposable?.Dispose();
        }

        private void OnButtonPerformed()
        {
            Debug.Log("<color=purple>Assortment restock</color> performed");
            GP_Ads.ShowRewarded(ADS_TAG, OnRewardedReward);
        }
        
        private void OnRewardedReward(string tag)
        {
            if (tag != ADS_TAG)
                return;
            
            Debug.Log("<color=purple>Assortment restock</color> rewarded");
            m_assortmentController.RestockAndRestart();
        }
    }
}