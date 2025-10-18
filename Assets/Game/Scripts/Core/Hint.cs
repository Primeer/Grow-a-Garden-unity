using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Plants;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Core
{
    public class Hint : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private RectTransform m_rectTransform;

        [SerializeField] private Canvas m_canvas;
        [SerializeField] private RectTransform m_canvasTransform;
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private Camera m_camera;
        [SerializeField] private LayerMask m_ignoreLayerMask;
        [SerializeField] private PlantsConfig m_plantsConfig;

        [Header("Смещение")]
        [SerializeField] private Vector2 m_screenOffset = Vector2.zero;
        [SerializeField] private float m_rayDistance;

        public void Setup(bool isActive, string text)
        {
            m_text.text = text;
            m_text.gameObject.SetActive(isActive);
        }

        private void Update()
        {
            m_rectTransform.anchoredPosition = GetCanvasPosition(Pointer.current.position.value);
        }

        private void FixedUpdate()
        {
            Setup(false, null);
            
            if (DoRaycast(Pointer.current.position.value, out var hit))
            {
                var plant = hit.collider.GetComponentInParent<Plant>();
                var data = m_plantsConfig.GetPlantData(plant.Id);
                var plantName = LocalizationUtils.GetLocalized(data.En, data.Ru);
                var isGrowthCompleted = plant.Growth.IsGrowthCompleted;

                if (isGrowthCompleted)
                {
                    Setup(true, $"{plantName}");
                }
                else
                {
                    var growthProgress = plant.Growth.GrowthProgress;
                    Setup(true, $"{plantName} {growthProgress:P1}");
                }
            }
        }

        private Vector2 GetCanvasPosition(Vector2 screenPosition)
        {
            // Конвертируем screen position в позицию на канвасе
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_canvasTransform,
                screenPosition,
                m_canvas.worldCamera,
                out var canvasPosition
            );

            // Применяем смещение на экране
            canvasPosition += m_screenOffset;

            return canvasPosition;
        }
        
        private bool DoRaycast(Vector2 screenPoint, out RaycastHit hit)
        {
            var ray = m_camera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(ray, out hit, m_rayDistance, ~m_ignoreLayerMask);
        }
    }
}