using GamePush;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Scripts.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] m_sources;
        private AudioSource m_playerSource;

        public void OnPlayerSpawned(GameObject playerGo)
        {
            m_playerSource = playerGo.GetComponent<AudioSource>();
            m_playerSource.mute = GP_Sounds.IsMuted();
        }
        
        private void OnEnable()
        {
            GP_Sounds.OnMute += OnMute;
            GP_Sounds.OnUnmute += OnUnmute;
        }

        private void OnDisable()
        {
            GP_Sounds.OnMute -= OnMute;
            GP_Sounds.OnUnmute -= OnUnmute;
        }

        private void OnMute()
        {
            m_sources.ForEach(s => s.mute = true);
            m_playerSource.mute = true;
        }

        private void OnUnmute()
        {
            m_sources.ForEach(s => s.mute = false);
            m_playerSource.mute = false;
        }
    }
}