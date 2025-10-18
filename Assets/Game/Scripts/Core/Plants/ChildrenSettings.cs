using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    public class ChildrenSettings : MonoBehaviour
    {
        [field: SerializeField] public Plant ChildPrefab { get; private set; }
        [field: SerializeField] public float ChildSpawnDelayMin { get; private set; }
        [field: SerializeField] public float ChildSpawnDelayMax { get; private set; }

        private void OnValidate()
        {
            var growth = GetComponent<Growth>();
            ChildSpawnDelayMin = growth.GrowthDuration * 0.05f;
            ChildSpawnDelayMax = growth.GrowthDuration * 0.15f;
        }
    }
}