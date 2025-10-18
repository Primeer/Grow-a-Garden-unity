using Game.Scripts.Core.ShopSystem.Purchasing;
using Game.Scripts.Core.ShopSystem.Selling;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem
{
    public static class ShopInstaller
    {
        public static void RegisterShop(this IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ShopSellWindow>();
            builder.RegisterEntryPoint<ShopSellWindowController>();
            builder.RegisterEntryPoint<ShopSellItemsController>();
            builder.RegisterEntryPoint<ShopSellController>();
            builder.RegisterEntryPoint<ShopSellButtonsController>();
            builder.Register<ShopSellModel>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<ShopBuyWindow>();
            builder.RegisterEntryPoint<ShopBuyWindowController>();
            builder.RegisterEntryPoint<ShopBuyItemsController>();
            builder.RegisterEntryPoint<ShopBuyPurchaseController>();
            builder.RegisterEntryPoint<ShopBuyButtonsController>();
            builder.RegisterEntryPoint<ShopBuyAssortmentController>().AsSelf();
            builder.RegisterEntryPoint<ShopBuyAdsButtonController>();
            builder.RegisterEntryPoint<ShopBuyRealButtonController>();
            builder.RegisterEntryPoint<ShopBuyRestockButtonController>();
            builder.Register<ShopBuyModel>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<PurchaseConsumer>();
        }
    }
}