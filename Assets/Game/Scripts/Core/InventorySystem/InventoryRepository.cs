using JetBrains.Annotations;
using ObservableCollections;

namespace Game.Scripts.Core.InventorySystem
{
    public class InventoryRepository
    {
        private const int MAX_ITEMS_COUNT = 36;
        
        private readonly ObservableList<SeedItem> m_seeds = new();
        private readonly ObservableList<PlantItem> m_plants = new();
        
        public IReadOnlyObservableList<SeedItem> Seeds => m_seeds;
        public IReadOnlyObservableList<PlantItem> Plants => m_plants;
        
        public bool TryAddItem([NotNull] Item item)
        {
            return item switch
            {
                SeedItem seed => TryAddSeed(seed),
                PlantItem plant => TryAddPlant(plant),
                _ => false
            };
        }

        private bool TryAddSeed(SeedItem seed)
        {
            if (m_seeds.Count == MAX_ITEMS_COUNT)
                return false;
            
            m_seeds.Add(seed);
            return true;
        }
        
        private bool TryAddPlant(PlantItem plant)
        {
            if (m_plants.Count == MAX_ITEMS_COUNT)
                return false;
            
            m_plants.Add(plant);
            return true;
        }

        public void RemoveItem([NotNull] Item item)
        {
            switch (item)
            {
                case SeedItem seed:
                    RemoveSeed(seed);
                    break;
                
                case PlantItem plant:
                    RemovePlant(plant);
                    break;
            }
        }

        private void RemoveSeed(SeedItem seed)
        {
            m_seeds.Remove(seed);
        }
        
        private void RemovePlant(PlantItem plant)
        {
            m_plants.Remove(plant);
        }

        public bool HasItem(Item item)
        {
            return item switch
            {
                SeedItem seed => m_seeds.Contains(seed),
                PlantItem plant => m_plants.Contains(plant),
                _ => false
            };
        }

        public bool IsSeedsFull() => m_seeds.Count >= MAX_ITEMS_COUNT;
        public bool IsPlantsFull() => m_plants.Count >= MAX_ITEMS_COUNT;
    }
}