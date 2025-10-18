using Game.Scripts.Core.Plants;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem.Migrators
{
    public static class SaveMigrator1To2
    {
        public static string GetMigrator(string json)
        {
            var config = Resources.Load<PlantsConfig>(nameof(PlantsConfig));
            Vector3 plantsPositionOffset = new Vector3(30, 0, 32);
            
            var data = JsonUtility.FromJson<SavingService.Data>(json);

            for (var i = 0; i < data.Plants.Plants.Count; i++)
            {
                var plant = data.Plants.Plants[i];
                
                // Глобальные позиции - Локальные позиции растений
                if (plant.TransformData.Position.x >= 19 || plant.TransformData.Position.z >= 20)
                {
                    plant.TransformData.Position -= plantsPositionOffset;
                }

                // Шаги + таймер - Полный таймер
                var stepDuration = config.GetPlantData(plant.PlantId).Prefab.Growth.StepDuration;
                plant.Timer += plant.Step * stepDuration;
                
                data.Plants.Plants[i] = plant;
            }

            return JsonUtility.ToJson(data);
        }
    }
}