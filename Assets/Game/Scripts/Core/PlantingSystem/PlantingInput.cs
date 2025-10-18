using System;
using Game.Scripts.Common.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Core.PlantingSystem
{
    public class PlantingInput : MonoBehaviour
    {
        [SerializeField]
        private float m_rayDistance;
        [SerializeField]
        private LayerMask m_ignoreLayerMask;
        [SerializeField] 
        private Camera m_camera;

        private InputAction m_plantInput;
        private int m_groundLayer;

        public event Action<Vector3> Performed; 
        
        private void Awake()
        {
            m_plantInput = InputSystem.actions.FindAction("Plant");
            m_groundLayer = LayerMask.NameToLayer("Ground");
        }

        private void OnEnable()
        {
            m_plantInput.performed += OnPlantInput;
        }

        private void OnDisable()
        {
            m_plantInput.performed -= OnPlantInput;
        }

        private void OnPlantInput(InputAction.CallbackContext context)
        {
            var pointerPosition = Pointer.current.position.value;

            if (InputUtils.IsPointerOverUI(pointerPosition))
                return;
            
            if (DoRaycast(pointerPosition, m_rayDistance, out RaycastHit hit) && CheckLayer(hit))
            {
                Performed?.Invoke(hit.point);
            }
        }

        private bool DoRaycast(Vector2 pointerPosition, float distance, out RaycastHit hit)
        {
            return Physics.Raycast(m_camera.ScreenPointToRay(pointerPosition), out hit, distance, ~m_ignoreLayerMask);
        }

        private bool CheckLayer(RaycastHit hit)
        {
            return hit.collider.gameObject.layer == m_groundLayer;
        }
    }
}