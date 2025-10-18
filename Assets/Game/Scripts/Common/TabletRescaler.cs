using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Common
{
    public class TabletRescaler : MonoBehaviour
    {
        [SerializeField] private float m_minAspect;
        [SerializeField] private CanvasScaler m_canvasScaler;

        private int m_screenWidth;
        private int m_screenHeight;

        private void Update()
        {
            if (Screen.width != m_screenWidth || Screen.height != m_screenHeight)
            {
                m_screenWidth = Screen.width;
                m_screenHeight = Screen.height;
                
                m_canvasScaler.matchWidthOrHeight = (float)m_screenWidth / m_screenHeight < m_minAspect ? 0f : 1f;
            }
        }

        private void OnValidate()
        {
            m_canvasScaler ??= GetComponent<CanvasScaler>();
        }
    }
}