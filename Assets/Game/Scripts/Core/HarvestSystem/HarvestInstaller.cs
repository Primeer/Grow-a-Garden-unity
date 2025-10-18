using Game.Scripts.Core.UseSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.HarvestSystem
{
    public static class HarvestInstaller
    {
        public static void RegisterHarvestSystem(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<HarvestController>();
        }
    }
}