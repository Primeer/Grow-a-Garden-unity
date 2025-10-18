using Game.Scripts.Common.Utilities;
using LitMotion;
using UnityEngine;

namespace Game.Scripts.Core.InventorySystem.UI.Views
{
    public class InventoryAnimation : MonoBehaviour
    {
        [Header(Constants.SETTINGS)]
        [SerializeField] private float m_hidePosition;
        [SerializeField] private float m_time;
        [SerializeField] private Ease m_ease;
        
        [Header(Constants.REFERENCES)]
        [SerializeField] private RectTransform m_transform;

        private MotionHandle m_handle;
        private bool m_isOpened;
        
        public void SwitchOpenState()
        {
            if (m_isOpened)
            {
                Hide();
            }
            else
            {
                Show();
            }

            m_isOpened = !m_isOpened;
        }

        private void Show()
        {
            float currentPosition = GetCurrentPosition();
            float time = GetRemainTime(currentPosition, m_transform.sizeDelta.y);

            m_handle.TryCancel();
            m_handle = LMotion.Create(currentPosition, m_transform.sizeDelta.y, time)
                .WithEase(m_ease)
                .Bind(SetPosition);
        }

        private void Hide()
        {
            float currentPosition = GetCurrentPosition();
            float time = GetRemainTime(currentPosition, m_hidePosition);
            
            m_handle.TryCancel();
            m_handle = LMotion.Create(currentPosition, m_hidePosition, time)
                .WithEase(m_ease)
                .Bind(SetPosition)
                .AddTo(gameObject);
        }

        private void SetPosition(float pos)
        {
            var position = m_transform.anchoredPosition;
            position.y = pos;
            m_transform.anchoredPosition = position;
        }

        private float GetCurrentPosition()
        {
            return m_transform.anchoredPosition.y;
        }

        private float GetRemainTime(float currentPosition, float targetPosition)
        {
            float distance = m_transform.sizeDelta.y - m_hidePosition;
            return Mathf.Abs(targetPosition - currentPosition) / distance * m_time;
        }
    }
}