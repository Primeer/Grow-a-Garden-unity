using System;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.PlantingSystem;
using Game.Scripts.Core.Plants;
using Sirenix.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.SaveSystem.Plants
{
    public class PlantsSaveSpawner : IStartable, IDisposable
    {
        private readonly PlantsConfig m_plantsConfig;

        public PlantsSaveSpawner(PlantsConfig plantsConfig)
        {
            m_plantsConfig = plantsConfig;
        }

        public void Start() => PlayerContext.Instance.Initialized.AddListener(OnContextInitialized);
        public void Dispose() => PlayerContext.Instance.Initialized.RemoveListener(OnContextInitialized);

        private void OnContextInitialized()
        {
            if (!PlayerContext.Instance.SaveData.HasValue)
            {
                PlayerContext.Instance.PlantsSpawned.Value = true;
                return;
            }

            SpawnPlantsFromSaveData(PlayerContext.Instance.SaveData.Value.Plants);

            PlayerContext.Instance.PlantsSpawned.Value = true;
        }

        private void SpawnPlantsFromSaveData(PlantsSaveData savePlantsSaveData)
        {
            foreach (var plantData in savePlantsSaveData.Plants)
            {
                SpawnPlant(plantData, savePlantsSaveData.SaveTimeTicks);
            }
        }

        private void SpawnPlant(PlantSaveData plantSaveData, long saveTimeTicks)
        {
            var plantGo = SpawnPlantFromData(plantSaveData, saveTimeTicks);

            if (plantGo is null)
                return;

            if (!plantGo.TryGetComponent<ChildrenSettings>(out _))
                return;

            var plant = plantGo.GetComponent<Plant>();

            if (!plant.Growth.IsGrowthCompleted)
                return;

            RestoreChildrenSpawners(plant, plantSaveData, saveTimeTicks);
        }

        private GameObject SpawnPlantFromData(PlantSaveData plantSaveData, long saveTimeTicks)
        {
            var plantPrefabData = m_plantsConfig.GetPlantData(plantSaveData.PlantId);

            if (plantPrefabData.Prefab is null)
                return null;

            var position = PlayerContext.Instance.PlantsContainer.TransformPoint(plantSaveData.TransformData.Position);

            var plantGo = PlantSpawner.Instance.SpawnSavedPlant(
                plantPrefabData.Prefab.name,
                PlayerContext.Instance.PlantsContainer,
                position,
                plantSaveData.TransformData.Rotation,
                plantSaveData.TransformData.Scale,
                plantSaveData.Timer,
                saveTimeTicks);

            return plantGo;
        }

        private void RestoreChildrenSpawners(Plant plant, PlantSaveData plantSaveData, long saveTimeTicks)
        {
            var childSpawners = plant.GetComponentsInChildren<ChildSpawner>();

            if (plantSaveData.Children.IsNullOrEmpty())
            {
                SpawnChildrenWithoutData(childSpawners, plant.ChildrenSettings, plant.Growth, plantSaveData,
                    saveTimeTicks);
                return;
            }

            int matchedCount = RestoreMatchedSpawners(childSpawners, plantSaveData.Children, plant.ChildrenSettings,
                saveTimeTicks);
            SpawnRemainingSpawners(childSpawners, matchedCount, plant.ChildrenSettings, plant.Growth, plantSaveData,
                saveTimeTicks);
        }

        private int RestoreMatchedSpawners(
            ChildSpawner[] spawners,
            PlantChildSaveData[] savedChildren,
            ChildrenSettings settings,
            long saveTimeTicks)
        {
            int availableSpawnerCount = spawners.Length;
            int savedChildrenCount = savedChildren.Length;
            int matchedCount = Mathf.Min(availableSpawnerCount, savedChildrenCount);

            for (var i = 0; i < matchedCount; i++)
            {
                var savedChild = savedChildren[i];
                var spawner = spawners[i];

                RestoreChildSpawner(spawner, savedChild, settings, saveTimeTicks);
            }

            return matchedCount;
        }

        private void SpawnRemainingSpawners(
            ChildSpawner[] spawners,
            int matchedCount,
            ChildrenSettings settings,
            Growth growth,
            PlantSaveData plantSaveData,
            long saveTimeTicks)
        {
            int remainingCount = spawners.Length - matchedCount;

            if (remainingCount <= 0)
                return;

            var unusedSpawners = SliceSpawners(spawners, matchedCount);
            SpawnChildrenWithoutData(unusedSpawners, settings, growth, plantSaveData, saveTimeTicks);
        }

        private ChildSpawner[] SliceSpawners(ChildSpawner[] spawners, int startIndex)
        {
            int length = spawners.Length - startIndex;

            if (length <= 0)
                return Array.Empty<ChildSpawner>();

            var result = new ChildSpawner[length];
            Array.Copy(spawners, startIndex, result, 0, length);
            return result;
        }

        private void SpawnChildrenWithoutData(
            ChildSpawner[] spawners,
            ChildrenSettings settings,
            Growth growth,
            PlantSaveData plantSaveData,
            long saveTimeTicks)
        {
            float timeAfterGrowth = TimeUtils.CalculateProgressTime(plantSaveData.Timer, saveTimeTicks) - growth.GrowthDuration;

            foreach (var spawner in spawners)
            {
                float timeToSpawn = settings.ChildSpawnDelayMax;

                if (timeAfterGrowth >= timeToSpawn)
                {
                    float timer = -(growth.GrowthDuration - plantSaveData.Timer + timeToSpawn);
                    SpawnChild(spawner, settings, timer, saveTimeTicks);
                }
                else
                {
                    float remainTimeToSpawn = timeToSpawn - timeAfterGrowth;
                    spawner.SetLoadedData(remainTimeToSpawn);
                }
            }
        }

        private void RestoreChildSpawner(
            ChildSpawner spawner, 
            PlantChildSaveData plantChildSaveData,
            ChildrenSettings settings, 
            long saveTimeTicks)
        {
            if (plantChildSaveData.HasChild)
            {
                var position = spawner.transform.TransformPoint(plantChildSaveData.TransformData.Position);

                var childPlantGo = PlantSpawner.Instance.SpawnSavedPlant(
                    settings.ChildPrefab.name,
                    spawner.transform,
                    position,
                    plantChildSaveData.TransformData.Rotation,
                    plantChildSaveData.TransformData.Scale,
                    plantChildSaveData.Timer,
                    saveTimeTicks);

                spawner.SetLoadedData(childPlantGo.GetComponent<Plant>());
            }
            else
            {
                float deltaSeconds = TimeUtils.GetDeltaSeconds(saveTimeTicks);

                if (deltaSeconds >= plantChildSaveData.TimeToSpawn)
                {
                    float timer = -plantChildSaveData.TimeToSpawn;
                    SpawnChild(spawner, settings, timer, saveTimeTicks);
                }
                else
                {
                    float timeToSpawn = plantChildSaveData.TimeToSpawn - deltaSeconds;
                    spawner.SetLoadedData(timeToSpawn);
                }
            }
        }

        private void SpawnChild(ChildSpawner spawner, ChildrenSettings settings, float timer, long saveTimeTicks)
        {
            var position = spawner.transform.position;
            var rotation = RandomUtils.RandomYRotation(spawner.transform.rotation);
            var scale = RandomUtils.NormalScale();

            var childPlantGo = PlantSpawner.Instance.SpawnSavedPlant(
                settings.ChildPrefab.name,
                spawner.transform,
                position,
                rotation,
                scale,
                timer,
                saveTimeTicks);

            spawner.SetLoadedData(childPlantGo.GetComponent<Plant>());
        }
    }
}