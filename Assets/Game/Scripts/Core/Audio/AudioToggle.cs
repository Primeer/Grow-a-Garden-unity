using GamePush;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.Audio
{
    [RequireComponent(typeof(Toggle))]
    public class AudioToggle : MonoBehaviour
    {
        private Toggle m_toggle;

        private void Awake()
        {
            m_toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            m_toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnDisable()
        {
            m_toggle.onValueChanged.RemoveAllListeners();
        }

        private void OnToggle(bool isToggled)
        {
            if (isToggled)
                GP_Sounds.Mute();
            else
                GP_Sounds.Unmute();
        }
    }
}