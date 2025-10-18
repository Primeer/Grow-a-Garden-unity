using Game.Scripts.Core;
using Game.Scripts.Core.CharacterController;
using Game.Scripts.Core.HarvestSystem;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.MoneySystem;
using Game.Scripts.Core.PlantingSystem;
using Game.Scripts.Core.SaveSystem;
using Game.Scripts.Core.ShopSystem;
using Game.Scripts.Core.UseSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Common
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterPlantingSystem();
            builder.RegisterHarvestSystem();
            builder.RegisterUsableSystem();
            builder.RegisterInventory();
            builder.RegisterMoney();
            builder.RegisterShop();
            builder.RegisterSaveSystem();

            builder.Register<ScreenDispatcher>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<CharacterMovement>();
            builder.RegisterComponentInHierarchy<Tutorial>();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            // builder.RegisterEntryPoint<InventoryDebug>();
#endif
        }
    }
}