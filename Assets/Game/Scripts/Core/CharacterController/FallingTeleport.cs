using Game.Scripts.Common.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.Core.CharacterController
{
    public class FallingTeleport : MonoBehaviour
    {
        [Header(Constants.SETTINGS)]
        [SerializeField] private float m_minY;
        
        [Header(Constants.REFERENCES)]
        [SerializeField] private Transform m_teleportPoint;
        [SerializeField] private CinemachineCamera m_camera;
        [SerializeField] private CinemachineOrbitalFollow m_orbitalFollow;

        private Transform PlayerTransform => PlayerContext.Instance.PlayerTransform;

        private void FixedUpdate()
        {
            if (PlayerTransform && PlayerTransform.position.y < m_minY)
            {
                TeleportPlayer();
            }
        }

        private void TeleportPlayer()
        {
            // 1) считаем дельту до телепорта
            var oldPos = PlayerTransform.position;

            // 2) телепортируем
            PlayerTransform.SetPositionAndRotation(m_teleportPoint.position, m_teleportPoint.rotation);

            // 3) сообщаем CM про варп и убираем дампинг на кадр
            var delta = PlayerTransform.position - oldPos;
            m_camera.OnTargetObjectWarped(PlayerTransform, delta); // важно!
            m_camera.PreviousStateIsValid = false;

            // 4) мгновенно рецентрируем орбиту
            m_orbitalFollow.HorizontalAxis.TriggerRecentering();
            m_orbitalFollow.VerticalAxis.TriggerRecentering();
        }
    }
}