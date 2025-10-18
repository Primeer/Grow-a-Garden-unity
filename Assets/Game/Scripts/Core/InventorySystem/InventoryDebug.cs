using Game.Scripts.Core.MoneySystem;
using Game.Scripts.Core.Plants;
using VContainer.Unity;

namespace Game.Scripts.Core.InventorySystem
{
    public class InventoryDebug : IStartable
    {
        private readonly SeedsConfig m_seedsConfig;
        private readonly InventoryRepository m_repository;
        private readonly MoneyRepository m_moneyRepository;
        private readonly ItemFactory m_factory;

        public InventoryDebug(InventoryRepository repository, ItemFactory factory, MoneyRepository moneyRepository, SeedsConfig seedsConfig)
        {
            m_repository = repository;
            m_factory = factory;
            m_moneyRepository = moneyRepository;
            m_seedsConfig = seedsConfig;
        }

        public void Start()
        {
            // m_moneyRepository.Money.Value = 1000;
            
            // m_repository.TryAddItem(m_factory.CreateSeed(8));
            // m_repository.TryAddItem(m_factory.CreateSeed(8));
            // m_repository.TryAddItem(m_factory.CreateSeed(8));
            // m_repository.TryAddItem(m_factory.CreateSeed(0));
            // m_repository.TryAddItem(m_factory.CreateSeed(0));
            // m_repository.TryAddItem(m_factory.CreateSeed(0));
            
            // foreach (var data in m_seedsConfig.DataList)
            // {
            //     m_repository.TryAddItem(m_factory.CreateSeed(data.Id));
            // }
        }
    }
}