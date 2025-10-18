using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.PlantingSystem;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core.Plants
{
    public class ChildSpawner : MonoBehaviour
    {
        private float m_spawnDelayMin;
        private float m_spawnDelayMax;
        
        private Plant m_plantPrefab;
        private bool m_cycleStarted;
        private Plant m_currentChild;
        private MotionHandle m_handle;
        private Growth m_parent;
        private ChildrenSettings m_settings;

        [ShowInInspector]
        public float TimeToSpawn { get; private set; }
        public Plant Child => m_currentChild;

        private void Awake()
        {
            m_parent = GetComponentInParent<Growth>();
            m_settings = GetComponentInParent<ChildrenSettings>();
        }

        private void OnEnable() => m_parent.GrowthCompleted += OnParentGrowthCompleted;

        private void OnDisable() => m_parent.GrowthCompleted -= OnParentGrowthCompleted;

        private void OnParentGrowthCompleted()
        {
            Setup();
            StartGrowth();
        }

        public void SetLoadedData(Plant child)
        {
            Setup();
            SetCurrentChild(child);
            m_cycleStarted = true;
        }
        
        public void SetLoadedData(float timeToSpawn)
        {
            Setup();
            StartGrowth(timeToSpawn);
        }

        private void StartGrowth()
        {
            float delay = Random.Range(m_spawnDelayMin, m_spawnDelayMax);
            StartGrowth(delay);
        }

        private void StartGrowth(float delay)
        {
            if (m_cycleStarted)
                return;
            
            m_cycleStarted = true;
            
            StartGrowthCycle(delay);
        }
        
        private void StartGrowthCycle(float delay)
        {
            m_handle = LMotion.Create(delay, 0f, delay)
                .WithOnComplete(SpawnChild)
                .Bind(x => TimeToSpawn = x)
                .AddTo(gameObject);
        }

        private void SpawnChild()
        {
            var scale = RandomUtils.NormalScale();

            var rotation = RandomUtils.RandomYRotation(transform.rotation);

            var plantGo = PlantSpawner.Instance.SpawnPlant(
                m_plantPrefab.name,
                transform,
                transform.position,
                rotation,
                scale);
            
            m_currentChild = plantGo.GetComponent<Plant>();
            
            m_currentChild.UsableObject.UsePerformed += OnChildHarvested;
        }

        private void OnChildHarvested()
        {
            m_currentChild = null;
            float delay = Random.Range(m_spawnDelayMin, m_spawnDelayMax);
            StartGrowthCycle(delay);
        }

        private void Setup()
        {
            m_plantPrefab = m_settings.ChildPrefab;
            m_spawnDelayMin = m_settings.ChildSpawnDelayMin;
            m_spawnDelayMax = m_settings.ChildSpawnDelayMax;
        }

        private void SetCurrentChild(Plant child)
        {
            m_currentChild = child;
            
            if (m_currentChild is null)
                return;

            m_currentChild.UsableObject.UsePerformed += OnChildHarvested;
        }
        
        private void OnDestroy()
        {
            m_handle.TryCancel();
        }
    }
}
