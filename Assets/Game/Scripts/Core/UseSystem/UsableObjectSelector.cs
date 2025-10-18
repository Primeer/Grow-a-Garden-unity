using System.Linq;
using Game.Scripts.Common.Utilities;
using R3;
using UnityEngine;

namespace Game.Scripts.Core.UseSystem
{
    public class UsableObjectSelector : MonoBehaviour
    {
        [SerializeField] private float m_radius;
        [SerializeField] private Camera m_camera;
        
        private readonly ReactiveProperty<UsableObject> m_selectedObject = new();
        private float m_sqrRadius;
        private Transform PlayerTransform => PlayerContext.Instance.PlayerTransform;

        public ReadOnlyReactiveProperty<UsableObject> SelectedObject => m_selectedObject;

        private void Awake()
        {
            m_sqrRadius = m_radius * m_radius;
        }

        private void FixedUpdate()
        {
            if (!PlayerTransform)
                return;
            
            var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            var ray = m_camera.ScreenPointToRay(screenCenter);
            
            var usableObjects = UsableObjectsContainer.Instance.GetObjectsInArea(PlayerTransform.position, m_sqrRadius).Where(FilterObjects);
            var minDistance = float.MaxValue;
            
            UsableObject closestObject = null;
            
            foreach (var plant in usableObjects)
            {
                var closestPointOnRay = CameraUtils.GetClosestPointOnRay(ray, plant.transform.position);
                float distance = (closestPointOnRay - plant.transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObject = plant;
                }
            }

            var selectedObject = m_selectedObject.Value;

            if (closestObject != selectedObject)
            {
                m_selectedObject.Value = closestObject;
                
                selectedObject?.SetHighlighted(false);
                closestObject?.SetHighlighted(true);

                if (closestObject != null)
                {
                    closestObject.Destroyed += () => m_selectedObject.Value = null;
                }
            }
        }

        private bool FilterObjects(UsableObject obj)
        {
            return CameraUtils.IsPositionVisibleInViewport(obj.transform.position, m_camera);
        }
    }
}