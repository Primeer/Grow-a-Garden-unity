using System;
using Game.Scripts.Core.InventorySystem;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyItemsController : IInitializable, IDisposable
    {
        private readonly ShopBuyWindow m_window;
        private readonly ShopBuyModel m_model;

        private IDisposable m_disposable;
        private bool m_isWindowVisible;

        public ShopBuyItemsController(ShopBuyWindow window, ShopBuyModel model)
        {
            m_window = window;
            m_model = model;
        }

        public void Initialize()
        {
            m_window.Showed += OnWindowShowed;
            m_window.Hided += OnWindowHided;
            m_window.ItemSelected += OnItemSelected;
            m_disposable = m_model.AssortmentList.Subscribe(_ => OnAssortmentChanged());
        }

        public void Dispose()
        {
            m_window.Showed -= OnWindowShowed;
            m_window.Hided -= OnWindowHided;
            m_window.ItemSelected -= OnItemSelected;
            
            m_disposable.Dispose();
        }

        private void OnWindowShowed()
        {
            m_isWindowVisible = true;
            
            SpawnItems();
        }

        private void OnWindowHided()
        {
            ClearItems();
            
            m_isWindowVisible = false;
        }

        private void OnItemSelected(SeedItem item)
        {
            m_model.SelectedItem.Value = item;
        }

        private void OnAssortmentChanged()
        {
            if (m_isWindowVisible == false)
                return;
            
            ClearItems();
            SpawnItems();
        }

        private void SpawnItems()
        {
            foreach (var seedItem in m_model.AssortmentList.CurrentValue)
            {
                var uiShopItem = m_window.SpawnItem(seedItem);
                m_model.Items.Add(uiShopItem, seedItem);
            }
        }

        private void ClearItems()
        {
            m_window.ClearItems();
            m_model.Items.Clear();
            m_model.SelectedItem.Value = null;
        }
    }
}