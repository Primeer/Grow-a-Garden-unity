using System;
using Game.Scripts.Common;
using R3;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.Core.CharacterController
{
    public class TelepotButtons : MonoBehaviour
    {
        [SerializeField] private UIButton m_gardenButton;
        [SerializeField] private UIButton m_seedsButton;
        [SerializeField] private UIButton m_sellButton;
        [SerializeField] private Transform m_seedsPoint;
        [SerializeField] private Transform m_sellPoint;
        [SerializeField] private CinemachineCamera m_camera;
        [SerializeField] private CinemachineOrbitalFollow m_orbitalFollow;

        private IDisposable m_disposable;
        private Transform m_targetPoint;

        private void OnEnable()
        {
            var d1 = m_gardenButton.Button.OnClickAsObservable().Subscribe(_ => SetTeleportTarget(PlayerContext.Instance.GardenPoint));
            var d2 = m_seedsButton.Button.OnClickAsObservable().Subscribe(_ => SetTeleportTarget(m_seedsPoint));
            var d3 = m_sellButton.Button.OnClickAsObservable().Subscribe(_ => SetTeleportTarget(m_sellPoint));

            m_disposable = Disposable.Combine(d1, d2, d3);
        }

        private void OnDisable()
        {
            m_disposable.Dispose();
        }

        private void SetTeleportTarget(Transform point)
        {
            if (!PlayerContext.Instance.PlayerTransform || !point)
                return;
            
            m_targetPoint = point;
        }

        private void FixedUpdate()
        {
            if (m_targetPoint)
            {
                var player = PlayerContext.Instance.PlayerTransform;
                
                // 1) считаем дельту до телепорта
                var oldPos = player.position;

                // 2) телепортируем
                player.SetPositionAndRotation(m_targetPoint.position, m_targetPoint.rotation);

                // 3) сообщаем CM про варп и убираем дампинг на кадр
                var delta = player.position - oldPos;
                m_camera.OnTargetObjectWarped(player, delta); // важно!
                m_camera.PreviousStateIsValid = false;

                // 4) мгновенно рецентрируем орбиту
                m_orbitalFollow.HorizontalAxis.TriggerRecentering();
                m_orbitalFollow.VerticalAxis.TriggerRecentering();

                m_targetPoint = null;
            }
        }
    }
}