using System.Collections.Generic;
using Game.Scripts.Common.Misc;
using Game.Scripts.Core.SaveSystem;
using Photon.Realtime;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Core
{
    public class PlayerContext : MonoSingleton<PlayerContext>
    {
        [SerializeField] private Garden[] m_gardens;
        
        public Transform PlayerTransform { get; private set; }
        public int GardenIndex { get; private set; }
        public Garden[] Gardens => m_gardens;
        public Garden Garden { get; private set; }
        public Transform GardenPoint { get; private set; }
        public Transform PlantsContainer { get; private set; }
        public SavingService.Data? SaveData { get; set; }
        public List<Player> OtherPlayers { get; set; }
        public ReactiveProperty<bool> PlantsSpawned { get; set; } = new();

        public UnityEvent Initialized;
        
        public void OnJoinedRoom(int gardenIndex)
        {
            GardenIndex = gardenIndex;
            Garden = m_gardens[gardenIndex];
            PlantsContainer = Garden.PlantsContainer;
            GardenPoint = Garden.TeleportPoint;

            SetGardens(gardenIndex);
            
            Initialized?.Invoke();
        }

        public void OnPlayerSpawned(GameObject go)
        {
            PlayerTransform = go.transform;
        }

        private void SetGardens(int gardenIndex)
        {
            for (var i = 0; i < Gardens.Length; i++)
            {
                Gardens[i].SetPlanes(i == gardenIndex);
            }
        }
    }
}