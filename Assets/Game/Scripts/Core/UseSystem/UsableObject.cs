using System;
using UnityEngine;

namespace Game.Scripts.Core.UseSystem
{
    [RequireComponent(typeof(ObjectEffects))]
    public class UsableObject : MonoBehaviour
    {
        private ObjectEffects m_effects;
        private bool m_shouldRegister;
        private bool m_isRegistered;

        public event Action UsePerformed;
        public event Action Destroyed;

        private void Awake()
        {
            m_effects = GetComponent<ObjectEffects>();
        }

        private void OnEnable() => TryRegister();

        private void OnDisable() => TryUnregister();

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }

        public void PerformUse()
        {
            UsePerformed?.Invoke();
        }

        public void SetHighlighted(bool isHighlighted)
        {
            m_effects.SetHighlighted(isHighlighted);
        }

        public void EnableUsageTracking()
        {
            m_shouldRegister = true;
            TryRegister();
        }

        public void DisableUsageTracking()
        {
            m_shouldRegister = false;
            TryUnregister();
        }

        private void TryRegister()
        {
            if (!m_shouldRegister || m_isRegistered || !isActiveAndEnabled)
                return;

            var container = UsableObjectsContainer.Instance;
            if (container == null)
                return;

            container.Register(this);
            m_isRegistered = true;
        }

        private void TryUnregister()
        {
            if (!m_isRegistered)
                return;

            var container = UsableObjectsContainer.Instance;
            if (container != null)
            {
                container.Unregister(this);
            }

            m_isRegistered = false;
        }
    }
}
