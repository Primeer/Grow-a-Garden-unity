using System;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.UseSystem
{
    public class UseButtonController : IInitializable, IDisposable
    {
        private readonly UseButton m_button;
        private readonly UsableObjectSelector m_usableObjectSelector;

        private IDisposable m_disposable;

        public UseButtonController(UseButton button, UsableObjectSelector usableObjectSelector)
        {
            m_button = button;
            m_usableObjectSelector = usableObjectSelector;
        }

        public void Initialize()
        {
            m_disposable = m_usableObjectSelector.SelectedObject.Subscribe(OnObjectSelected);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnObjectSelected(UsableObject obj)
        {
            m_button.SetTarget(obj?.transform);
        }
    }
}