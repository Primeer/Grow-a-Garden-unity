using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Common
{
    public class SafeArea : MonoBehaviour
    {
        private Rect m_safeRect;

        private void Update()
        {
            if (Screen.safeArea != m_safeRect)
            {
                Recalculate();
            }
        }

        [Button]
        private void Recalculate()
        {
            m_safeRect = Screen.safeArea;
            
            var rectTransform = GetComponent<RectTransform>();
            
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = m_safeRect.min;
            rectTransform.offsetMax = m_safeRect.max - new Vector2(Screen.width, Screen.height);
        }
    }
}