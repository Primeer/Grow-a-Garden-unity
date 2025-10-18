using System;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.Plants;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.PlantingSystem
{
    public class PlantingController : IInitializable, IDisposable
    {
        private readonly PlantingInput m_input;
        private readonly PlantSpawner m_spawner;
        private readonly PlantsConfig m_config;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly InventoryModel m_inventoryModel;

        public PlantingController(
            PlantingInput input, 
            InventoryRepository inventoryRepository, 
            InventoryModel inventoryModel, 
            PlantSpawner spawner, 
            PlantsConfig config)
        {
            m_input = input;
            m_inventoryRepository = inventoryRepository;
            m_inventoryModel = inventoryModel;
            m_spawner = spawner;
            m_config = config;
        }

        public void Initialize()
        {
            m_input.Performed += OnInputPerformed;
        }

        public void Dispose()
        {
            m_input.Performed -= OnInputPerformed;
        }

        private void OnInputPerformed(Vector3 position)
        {
            var selectedSlot = m_inventoryModel.SelectedSlot.Value;
            
            if (selectedSlot == null)
                return;
            
            var selectedItem = m_inventoryModel.Slots[selectedSlot];

            if (selectedItem is not SeedItem seed) 
                return;
            
            var plantPrefab = m_config.GetPlantData(seed.PlantId).Prefab;
            var scale = RandomUtils.NormalScale();
            SpawnPlant(plantPrefab, position, scale);
            m_inventoryRepository.RemoveItem(selectedItem);
            
            SoundsManager.Instance.PlayPlanting();
        }

        private void SpawnPlant(Plant plantPrefab, Vector3 worldPosition, Vector3 scale)
        {
            m_spawner.SpawnPlate(worldPosition);
            
            var rotation = RandomUtils.RandomYRotation();

            m_spawner.SpawnPlant(
                plantPrefab.name,
                PlayerContext.Instance.PlantsContainer,
                worldPosition,
                rotation,
                scale);
        }
    }
}
