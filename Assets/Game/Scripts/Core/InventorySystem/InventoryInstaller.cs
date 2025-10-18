using Game.Scripts.Core.InventorySystem.UI.Controllers;
using Game.Scripts.Core.InventorySystem.UI.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.InventorySystem
{
    public static class InventoryInstaller
    {
        public static void RegisterInventory(this IContainerBuilder builder)
        {
            var config = Resources.Load<SeedsConfig>(nameof(SeedsConfig));
            builder.RegisterInstance(config);
            
            builder.Register<ItemFactory>(Lifetime.Singleton);
            builder.Register<InventoryRepository>(Lifetime.Singleton);
            builder.Register<InventoryModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<InventoryItemsController>();
            builder.RegisterEntryPoint<InventoryOpenController>();
            builder.RegisterComponentInHierarchy<InventoryWindow>();
            builder.RegisterComponentInHierarchy<InventoryAnimation>();
        }
    }
}