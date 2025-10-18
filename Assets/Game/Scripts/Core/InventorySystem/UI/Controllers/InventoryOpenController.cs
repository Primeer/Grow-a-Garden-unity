using System;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem.UI.Views;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Scripts.Core.InventorySystem.UI.Controllers
{
    public class InventoryOpenController : IInitializable, IDisposable
    {
        private readonly InventoryAnimation m_animationView;

        private InputAction m_input;

        public InventoryOpenController(InventoryAnimation animationView)
        {
            m_animationView = animationView;
        }

        public void Initialize()
        {
            m_input = InputSystem.actions.FindAction("Inventory");
            
            m_input.performed += OnInventoryInput;
        }

        public void Dispose()
        {
            m_input.performed -= OnInventoryInput;
        }

        private void OnInventoryInput(InputAction.CallbackContext _)
        {
            m_animationView.SwitchOpenState();
            
            SoundsManager.Instance.PlayInventory();
        }
    }
}