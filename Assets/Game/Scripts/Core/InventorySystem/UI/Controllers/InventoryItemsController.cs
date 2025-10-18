using System;
using Game.Scripts.Core.InventorySystem.UI.Views;
using ObservableCollections;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.InventorySystem.UI.Controllers
{
    public class InventoryItemsController : IInitializable, IDisposable
    {
        private readonly InventoryRepository m_inventoryRepository;
        private readonly InventoryWindow m_inventoryWindow;
        private readonly InventoryModel m_inventoryModel;

        private IDisposable m_disposable;

        public InventoryItemsController(
            InventoryRepository inventoryRepository,
            InventoryWindow inventoryWindow,
            InventoryModel inventoryModel)
        {
            m_inventoryRepository = inventoryRepository;
            m_inventoryWindow = inventoryWindow;
            m_inventoryModel = inventoryModel;
        }

        public void Initialize()
        {
            var d1 =  m_inventoryRepository.Seeds.ObserveAdd().Subscribe(s => OnItemAdded(s.Value));
            var d2 =  m_inventoryRepository.Seeds.ObserveRemove().Subscribe(s => OnItemRemoved(s.Value));
            
            var d3 =  m_inventoryRepository.Plants.ObserveAdd().Subscribe(s => OnItemAdded(s.Value));
            var d4 =  m_inventoryRepository.Plants.ObserveRemove().Subscribe(s => OnItemRemoved(s.Value));
            
            var d5 = Observable.FromEvent<UIItemSlot>(
                    h => m_inventoryWindow.ItemsView.SlotSelected += h,
                    h => m_inventoryWindow.ItemsView.SlotSelected -= h)
                .Subscribe(OnSlotSelected);

            var d6 = m_inventoryWindow.SeedToggle.OnValueChangedAsObservable().Where(toggled => toggled).Subscribe(_ => OnSeedFilterPerformed());
            var d7 = m_inventoryWindow.PlantToggle.OnValueChangedAsObservable().Where(toggled => toggled).Subscribe(_ => OnPlantFilterPerformed());

            m_disposable = Disposable.Combine(d1, d2, d3, d4, d5, d6, d7);

            m_inventoryModel.IsPlantFilter = true;
            OnSeedFilterPerformed();
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnItemAdded(Item item)
        {
            if (CheckItemsFilter(item) == false)
                return;
            
            var slot = m_inventoryWindow.ItemsView.SpawnItem(item);
            m_inventoryModel.Slots.Add(slot, item);
        }

        private void OnItemRemoved(Item item)
        {
            if (CheckItemsFilter(item) == false)
                return;
            
            var slot = m_inventoryModel.Slots[item];
            m_inventoryWindow.ItemsView.RemoveItem(slot);
            m_inventoryModel.Slots.Remove(slot);
        }

        private void OnSlotSelected(UIItemSlot slot)
        {
            m_inventoryModel.SelectedSlot.Value = slot;
        }

        private void OnSeedFilterPerformed()
        {
            if(m_inventoryModel.IsPlantFilter == false)
                return;
            
            m_inventoryModel.IsPlantFilter = false;
            
            m_inventoryWindow.ItemsView.Clear();
            m_inventoryModel.Clear();
            
            foreach (var seed in m_inventoryRepository.Seeds)
            {
                OnItemAdded(seed);
            }
        }
        
        private void OnPlantFilterPerformed()
        {
            if(m_inventoryModel.IsPlantFilter)
                return;
            
            m_inventoryModel.IsPlantFilter = true;
            
            m_inventoryWindow.ItemsView.Clear();
            m_inventoryModel.Clear();
            
            foreach (var plant in m_inventoryRepository.Plants)
            {
                OnItemAdded(plant);
            }
        }

        private bool CheckItemsFilter(Item item)
        {
            return item switch
            {
                SeedItem => m_inventoryModel.IsPlantFilter == false,
                PlantItem => m_inventoryModel.IsPlantFilter,
                _ => false
            };
        }
    }
}