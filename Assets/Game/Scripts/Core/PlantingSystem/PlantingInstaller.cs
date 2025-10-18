using Game.Scripts.Core.Plants;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Core.PlantingSystem
{
    public static class PlantingInstaller
    {
        public static void RegisterPlantingSystem(this IContainerBuilder builder)
        {
            var config = Resources.Load<PlantsConfig>(nameof(PlantsConfig));
            builder.RegisterInstance(config);
            
            builder.RegisterComponentInHierarchy<PlantingInput>();
            builder.RegisterComponentInHierarchy<PlantSpawner>();

            builder.RegisterEntryPoint<PlantingController>();
        }
    }
}