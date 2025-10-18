using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class Garden : MonoBehaviour
    {
        [SerializeField] private Transform m_plantsContainer;
        [SerializeField] private Transform m_teleportPoint;
        [SerializeField] private TMP_Text m_billboardText;
        [SerializeField] private GameObject[] m_planes;

        public Transform PlantsContainer => m_plantsContainer;
        public Transform TeleportPoint => m_teleportPoint;
        public TMP_Text BillboardText => m_billboardText;

        public void SetPlanes(bool isActive)
        {
            m_planes.ForEach(plane => 
                plane.layer = isActive ? 
                    LayerMask.NameToLayer("Ground") : 
                    LayerMask.NameToLayer("Default"));
        }
    }
}