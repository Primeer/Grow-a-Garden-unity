using System;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.InventorySystem.UI.Views;
using ObservableCollections;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellItemsController : IInitializable, IDisposable
    {
        private readonly ShopSellWindow m_shopSellWindow;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ShopSellModel m_shopModel;

        private IDisposable m_disposable;

        public ShopSellItemsController(ShopSellWindow shopSellWindow, InventoryRepository inventoryRepository, ShopSellModel shopModel)
        {
            m_shopSellWindow = shopSellWindow;
            m_inventoryRepository = inventoryRepository;
            m_shopModel = shopModel;
        }

        public void Initialize()
        {
            m_shopSellWindow.Showed += OnSellWindowShowed;
            m_shopSellWindow.Hided += OnSellWindowHided;
        }

        public void Dispose()
        {
            m_shopSellWindow.Showed -= OnSellWindowShowed;
            m_shopSellWindow.Hided -= OnSellWindowHided;
        }

        private void OnSellWindowShowed()
        {
            SpawnItems();

            var d1 = m_inventoryRepository.Plants.ObserveAdd().Subscribe(s => OnItemAdded(s.Value));
            var d2 = m_inventoryRepository.Plants.ObserveRemove().Subscribe(s => OnItemRemoved(s.Value));
         
            m_shopSellWindow.ItemsView.SlotSelected += OnSlotSelected;

            m_disposable = Disposable.Combine(d1, d2);
        }
        
        private void OnSellWindowHided()
        {
            m_disposable.Dispose();
        
            m_shopSellWindow.ItemsView.SlotSelected -= OnSlotSelected;
            
            m_shopSellWindow.ItemsView.Clear();
            m_shopModel.Clear();
        }

        private void SpawnItems()
        {
            foreach (var item in m_inventoryRepository.Plants)
            {
                OnItemAdded(item);
            }
        }
        
        private void OnItemAdded(Item item)
        {
            var slot = m_shopSellWindow.ItemsView.SpawnItem(item);
            m_shopModel.Slots.Add(slot, item);
        }
        
        private void OnItemRemoved(Item item)
        {
            var slot = m_shopModel.Slots[item];
            m_shopSellWindow.ItemsView.RemoveItem(slot);
            m_shopModel.Slots.Remove(slot);
        }
        
        private void OnSlotSelected(UIItemSlot slot) => m_shopModel.SelectedSlot.Value = slot;
    }
}