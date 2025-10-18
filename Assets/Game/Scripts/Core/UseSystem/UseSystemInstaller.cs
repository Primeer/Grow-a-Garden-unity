using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.UseSystem
{
    public static class UseSystemInstaller
    {
        public static void RegisterUsableSystem(this IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<UseButton>();
            builder.RegisterEntryPoint<UseButtonController>();
            
            builder.RegisterComponentInHierarchy<UsableObjectSelector>();
            
            builder.RegisterEntryPoint<UsableObjectController>();
        }
    }
}