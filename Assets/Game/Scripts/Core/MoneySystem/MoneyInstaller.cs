using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.MoneySystem
{
    public static class MoneyInstaller
    {
        public static void RegisterMoney(this IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MoneyView>();
            builder.RegisterEntryPoint<MoneyController>();
            builder.Register<MoneyRepository>(Lifetime.Singleton);
            builder.Register<ItemSellingService>(Lifetime.Singleton);
        }
    }
}