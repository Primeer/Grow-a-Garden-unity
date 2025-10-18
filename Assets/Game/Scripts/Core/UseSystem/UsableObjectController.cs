using System;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Scripts.Core.UseSystem
{
    public class UsableObjectController : IInitializable, IDisposable
    {
        private readonly UsableObjectSelector m_objectSelector;
        private readonly InputAction m_useInput;
        
        public UsableObjectController(UsableObjectSelector objectSelector)
        {
            m_objectSelector = objectSelector;
            m_useInput = InputSystem.actions.FindAction("Use");
        }

        public void Initialize()
        {
            m_useInput.performed += OnInputPerformed;
        }

        public void Dispose()
        {
            m_useInput.performed -= OnInputPerformed;
        }

        private void OnInputPerformed(InputAction.CallbackContext _)
        {
            var selectedObject = m_objectSelector.SelectedObject.CurrentValue;
            
            if (selectedObject == null)
                return;
            
            selectedObject.PerformUse();
        }
    }
}