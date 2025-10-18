using Sirenix.Utilities;
using UnityEngine;

namespace Game.Scripts.Common
{
    public class DebugSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_prefab;
        
#if DEVELOPMENT_BUILD
        private void Start()
        {
            m_prefab.ForEach(prefab => Instantiate(prefab, transform));
        }
#endif
    }
}