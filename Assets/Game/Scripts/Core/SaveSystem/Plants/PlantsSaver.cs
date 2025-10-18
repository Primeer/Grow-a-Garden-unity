using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.Plants;
using GamePush;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem.Plants
{
    public class PlantsSaver
    {
        public PlantsSaveData GetSaveData() =>
            new()
            {
                Plants = CollectActivePlants(),
                SaveTimeTicks = GP_Server.Time().Ticks
            };

        private List<PlantSaveData> CollectActivePlants()
        {
            var plantDataList = new List<PlantSaveData>();

            foreach (Transform plantTransform in PlayerContext.Instance.PlantsContainer)
            {
                if (!IsActivePlant(plantTransform))
                    continue;

                var plant = plantTransform.GetComponent<Plant>();
                var plantData = CollectPlantData(plant);
                plantDataList.Add(plantData);
            }

            return plantDataList;
        }

        private bool IsActivePlant(Transform plantTransform)
        {
            return plantTransform.gameObject.activeSelf &&
                   plantTransform.GetComponent<Plant>() != null;
        }

        private PlantSaveData CollectPlantData(Plant plant)
        {
            var growth = plant.Growth;

            return new PlantSaveData
            {
                PlantId = plant.Id,
                TransformData = TransformData.Create(plant.transform),
                Step = growth.CurrentStep,
                Timer = growth.TotalTime,
                Children = plant.GetComponentsInChildren<ChildSpawner>()
                    .Select(CollectChildSpawnerData)
                    .ToArray()
            };
        }

        private PlantChildSaveData CollectChildSpawnerData(ChildSpawner spawner)
        {
            var plant = spawner.Child;
            bool hasSpawnedChild = plant != null;

            return hasSpawnedChild ? 
                new PlantChildSaveData
                {
                    HasChild = true,
                    TransformData = TransformData.Create(plant.transform),
                    Step = plant.Growth.CurrentStep,
                    Timer = plant.Growth.TotalTime
                } : 
                new PlantChildSaveData
                {
                    HasChild = false,
                    TimeToSpawn = spawner.TimeToSpawn
                };
        }
    }
}