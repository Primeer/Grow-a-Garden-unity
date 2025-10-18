using System;
using Game.Scripts.Common.Misc;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Plants;
using Photon.Pun;
using UnityEngine;

namespace Game.Scripts.Core.PlantingSystem
{
    public class PlantSpawner : MonoSingleton<PlantSpawner>
    {
        [Header(Constants.REFERENCES)]
        [SerializeField]
        private Plate m_platePrefab;

        public event Action<Plant> PlantSpawned;
        public event Action<Plant> PlantDespawned;

        public void SpawnPlate(Vector3 worldPosition)
        {
            Instantiate(m_platePrefab, worldPosition, Quaternion.identity, transform);
        }

        public GameObject SpawnPlant(
            string prefabResourcePath,
            Transform parent,
            Vector3 worldPosition,
            Quaternion worldRotation,
            Vector3 scale)
        {
            var plantGo = PhotonNetwork.Instantiate(prefabResourcePath, worldPosition, worldRotation, 0, new object[]
            {
                scale,
                0f,
                TimeUtils.CurrentTimeTicks()
            });
            plantGo.transform.parent = parent;
            
            PlantSpawned?.Invoke(plantGo.GetComponent<Plant>());
            return plantGo;
        }
        
        public GameObject SpawnSavedPlant(
            string prefabResourcePath,
            Transform parent,
            Vector3 worldPosition,
            Quaternion worldRotation,
            Vector3 scale,
            float savedTimer,
            long saveTimeTicks)
        {
            var plantGo = PhotonNetwork.Instantiate(prefabResourcePath, worldPosition, worldRotation, 0, new object[]
            {
                scale,
                savedTimer,
                saveTimeTicks
            });
            plantGo.transform.parent = parent;
            
            PlantSpawned?.Invoke(plantGo.GetComponent<Plant>());
            return plantGo;
        }

        public void DespawnPlant(Plant plant)
        {
            plant.gameObject.SetActive(false);
            PhotonNetwork.Destroy(plant.gameObject);
            // CommonUtils.DestroyGameObject(plant.gameObject);
            PlantDespawned?.Invoke(plant);
        }
    }
}