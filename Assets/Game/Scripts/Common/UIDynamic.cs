using UnityEngine;

namespace Game.Scripts.Common
{
    public class UIDynamic : MonoBehaviour
    {
        [SerializeField] private Vector2 m_screenOffset = Vector2.zero;
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private RectTransform m_canvasTransform;
        [SerializeField] private Camera m_camera;

        protected Camera Camera => m_camera;
        
        
        protected Vector2 ScreenToRectPosition(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_canvasTransform,
                screenPosition,
                m_canvas.worldCamera,
                out var canvasPosition
            );

            canvasPosition += m_screenOffset;

            return canvasPosition;
        }
    }
}