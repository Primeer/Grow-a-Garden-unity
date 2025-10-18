using JetBrains.Annotations;
using UnityEngine;

namespace Game.Scripts.Core.UseSystem
{
    public class UseButton : MonoBehaviour
    {
        [Header("Настройки")]
        public RectTransform m_buttonRectTransform;
        public GameObject m_anotherButton;
        public Canvas m_canvas;
        public RectTransform m_canvasTransform;
        public Camera m_mainCamera;
    
        [Header("Смещение")]
        public Vector3 m_worldOffset = Vector3.zero;
        public Vector2 m_screenOffset = Vector2.zero;
        
        private Transform m_targetObject;
            
        public void SetTarget([CanBeNull] Transform newTarget)
        {
            m_targetObject = newTarget;
            gameObject.SetActive(newTarget);
            m_anotherButton.SetActive(newTarget);
        }
        
        void Update()
        {
            if (m_targetObject == null)
                return;
        
            m_buttonRectTransform.anchoredPosition = GetCanvasPosition(m_targetObject.position);
        }

        private Vector2 GetCanvasPosition(Vector3 worldPosition)
        {
            // Конвертируем world position в screen position
            Vector3 screenPosition = m_mainCamera.WorldToScreenPoint(worldPosition + m_worldOffset);
        
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
    }
}