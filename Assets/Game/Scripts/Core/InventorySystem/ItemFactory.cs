using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Plants;
using UnityEngine;

namespace Game.Scripts.Core.InventorySystem
{
    public class ItemFactory
    {
        private readonly SeedsConfig m_seedsConfig;
        private readonly PlantsConfig m_plantsConfig;

        public ItemFactory(SeedsConfig seedsConfig, PlantsConfig plantsConfig)
        {
            m_seedsConfig = seedsConfig;
            m_plantsConfig = plantsConfig;
        }
        
        public SeedItem CreateSeed(string tag)
        {
            var data = m_seedsConfig.GetSeedData(tag);
            return CreateSeed(data);
        }

        public SeedItem CreateSeed(int seedId)
        {
            var data = m_seedsConfig.GetSeedData(seedId);
            return CreateSeed(data);
        }

        private SeedItem CreateSeed(SeedsConfig.Data data)
        {
            return new SeedItem()
            {
                TypeId = data.Id,
                Ru = data.Ru,
                En = data.En,
                Icon = data.Sprite,
                Cost = data.Cost,
                Amount = 1,
                PlantId = data.PlantId,
                Rarity = data.Rarity,
                Tag = data.Tag,
            };
        }

        public PlantItem CreatePlant(int plantId, float weightFactor)
        {
            var data = m_plantsConfig.GetPlantData(plantId);
            float weight = data.Weight * weightFactor;

            return new PlantItem()
            {
                TypeId = data.Id,
                Ru = data.Ru,
                En = data.En,
                Icon = data.Icon,
                Cost = Mathf.RoundToInt(data.Cost * weightFactor),
                WeightFactor = weightFactor,
                Weight = LocalizationUtils.GetLocalized($"{weight:F}kg", $"{weight:F}кг"),
            };
        }
    }
}