using Game.Scripts.Core.SaveSystem.Plants;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.SaveSystem
{
    public static class SaveSystemInstaller
    {
        public static void RegisterSaveSystem(this IContainerBuilder builder)
        {
            // SaveVersionManager.CheckAndUpgradeAllSaves();
            
            builder.RegisterEntryPoint<MoneySaver>();
            builder.Register<PlantsSaver>(Lifetime.Singleton);
            builder.Register<InventorySaver>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SavingService>().AsSelf();
            builder.RegisterEntryPoint<PlantsSaveSpawner>();
        }
    }
}