using UnityEngine;

namespace Game.Scripts.Core
{
    public class RandomDisabler : MonoBehaviour
    {
        [SerializeField] private float m_factor = 0.5f;
        [SerializeField] private GameObject[] m_objects;

        private void Awake()
        {
            foreach (var obj in m_objects)
            {
                obj.SetActive(Random.value < m_factor);
            }
        }
    }
}
