using System;
using System.Linq;
using Game.Scripts.Core.InventorySystem;

namespace Game.Scripts.Core.SaveSystem
{
    public class InventorySaver
    {
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ItemFactory m_itemFactory;
        
        public InventorySaver(InventoryRepository inventoryRepository, ItemFactory itemFactory)
        {
            m_inventoryRepository = inventoryRepository;
            m_itemFactory = itemFactory;
        }
        
        public Data GetSaveData()
        {
            return new Data()
            {
                Seeds = m_inventoryRepository.Seeds.Select(item => item.TypeId).ToArray(),
                Plants = m_inventoryRepository.Plants.Select(item => new PlantData() {Id = item.TypeId, WeightFactor = item.WeightFactor}).ToArray(),
            };
        }
        
        public void LoadSaveData(Data data)
        {
            foreach (int seedId in data.Seeds)
            {
                var seedItem = m_itemFactory.CreateSeed(seedId);
                m_inventoryRepository.TryAddItem(seedItem);
            }
            
            foreach (var plant in data.Plants)
            {
                var plantItem = m_itemFactory.CreatePlant(plant.Id, plant.WeightFactor);
                m_inventoryRepository.TryAddItem(plantItem);
            }
        }
        
        [Serializable]
        public struct Data
        {
            public int[] Seeds;
            public PlantData[] Plants;
        }

        [Serializable]
        public struct PlantData
        {
            public int Id;
            public float WeightFactor;
        }
    }
}