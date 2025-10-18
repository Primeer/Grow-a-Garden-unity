using Game.Scripts.Common.Utilities;
using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    public class Plate : MonoBehaviour
    {
        [SerializeField] private float m_lifetime;

        private float m_time;

        // private void Update()
        // {
        //     m_time += Time.deltaTime;
        //     
        //     if (m_time >= m_lifetime)
        //     {
        //         CommonUtils.DestroyGameObject(gameObject);
        //     }
        // }
    }
}