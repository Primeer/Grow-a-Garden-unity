using System;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.PlantingSystem;
using Game.Scripts.Core.Plants;
using Game.Scripts.Core.UseSystem;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Scripts.Core.HarvestSystem
{
    public class HarvestController : IInitializable, IDisposable
    {
        private readonly UsableObjectSelector m_usableObjectSelector;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly PlantSpawner m_plantSpawner;
        private readonly ItemFactory m_itemFactory;
        private readonly InputAction m_useInput;

        public HarvestController(
            InventoryRepository inventoryRepository, 
            UsableObjectSelector usableObjectSelector, 
            PlantSpawner plantSpawner, 
            ItemFactory itemFactory)
        {
            m_inventoryRepository = inventoryRepository;
            m_usableObjectSelector = usableObjectSelector;
            m_plantSpawner = plantSpawner;
            m_itemFactory = itemFactory;

            m_useInput = InputSystem.actions.FindAction("Use");
        }

        public void Initialize()
        {
            m_useInput.performed += OnInputPerformed;
        }

        public void Dispose()
        {
            m_useInput.performed -= OnInputPerformed;
        }

        private void OnInputPerformed(InputAction.CallbackContext _)
        {
            var selectedObject = m_usableObjectSelector.SelectedObject.CurrentValue;

            if (selectedObject == null)
                return;

            var plant = selectedObject.GetComponent<Plant>();
            
            if (plant == null)
                return;

            var item = m_itemFactory.CreatePlant(plant.Id, plant.transform.lossyScale.x);

            if (m_inventoryRepository.TryAddItem(item))
            {
                m_plantSpawner.DespawnPlant(plant);
                
                SoundsManager.Instance.PlayHarvest();
            }
            else
            {
                Notification.Instance.ShowInventoryIsFull();
            }
        }
    }
}