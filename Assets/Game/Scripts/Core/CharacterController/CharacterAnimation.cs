using System;
using System.Collections.Generic;
using Game.Scripts.Common.Utilities;
using LitMotion;
using Photon.Pun;
using UnityEngine;

namespace Game.Scripts.Core.CharacterController
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private float m_duration;
        [SerializeField] private float m_angle;
        
        [SerializeField] private Transform m_leftLeg;
        [SerializeField] private Transform m_rightLeg;
        [SerializeField] private Transform m_leftArm;
        [SerializeField] private Transform m_rightArm;

        private PhotonView m_photonView;
        private float m_halfDuration;
        private readonly List<MotionHandle> m_inHandles = new ();
        private readonly List<MotionHandle> m_loopHandles = new ();
        private readonly List<MotionHandle> m_outHandles = new ();

        private bool m_isPlaying;

        private void Awake()
        {
            m_photonView = GetComponent<PhotonView>();
            m_halfDuration = m_duration / 2f;
        }

        public void PlayMovement()
        {
            if (m_isPlaying)
                return;

            m_isPlaying = true;

            if (PhotonNetwork.InRoom)
            {
                m_photonView.RPC(nameof(PlayMovement_Rpc), RpcTarget.All);
            }
            else
            {
                PlayMovement_Rpc();
            }
        }
        
        public void StopMovement()
        {
            if (m_isPlaying == false)
                return;

            m_isPlaying = false;

            if (PhotonNetwork.InRoom)
            {
                m_photonView.RPC(nameof(StopMovement_Rpc), RpcTarget.All);
            }
            else
            {
                StopMovement_Rpc();
            }
        }

        [PunRPC]
        public void PlayMovement_Rpc()
        {
            CancelAndClear(m_outHandles);
            
            PlayIn(PlayLoop);
        }

        [PunRPC]
        public void StopMovement_Rpc()
        {
            CancelAndClear(m_inHandles);
            CancelAndClear(m_loopHandles);
            
            PlayOut();
        }

        private void PlayIn(Action onComplete)
        {
            var h1 = LMotion.Create(0, m_angle, m_halfDuration)
                .WithOnComplete(onComplete)
                .Bind(t => SetRotation(t, m_rightLeg))
                .AddTo(gameObject);
            
            var h2 = LMotion.Create(0, -m_angle, m_halfDuration)
                .Bind(t => SetRotation(t, m_leftLeg))
                .AddTo(gameObject);
            
            var h3 = LMotion.Create(0, -m_angle, m_halfDuration)
                .Bind(t => SetRotation(t, m_rightArm))
                .AddTo(gameObject);
            
            var h4 = LMotion.Create(0, m_angle, m_halfDuration)
                .Bind(t => SetRotation(t, m_leftArm))
                .AddTo(gameObject);

            m_inHandles.AddRange(h1, h2, h3, h4);
        }

        private void PlayLoop()
        {
            var h1 = LMotion.Create(m_angle, -m_angle, m_duration)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(t => SetRotation(t, m_rightLeg))
                .AddTo(gameObject);
            
            var h2 = LMotion.Create(-m_angle, m_angle, m_duration)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(t => SetRotation(t, m_leftLeg))
                .AddTo(gameObject);
            
            var h3 = LMotion.Create(-m_angle, m_angle, m_duration)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(t => SetRotation(t, m_rightArm))
                .AddTo(gameObject);
            
            var h4 = LMotion.Create(m_angle, -m_angle, m_duration)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(t => SetRotation(t, m_leftArm))
                .AddTo(gameObject);
            
            m_loopHandles.AddRange(h1, h2, h3, h4);
        }

        private void PlayOut()
        {
            var h1 = LMotion.Create(ClampAngle(m_rightLeg.rotation.eulerAngles.x), 0, m_halfDuration)
                .Bind(t => SetRotation(t, m_rightLeg))
                .AddTo(gameObject);
            
            var h2 = LMotion.Create(ClampAngle(m_leftLeg.rotation.eulerAngles.x), 0, m_halfDuration)
                .Bind(t => SetRotation(t, m_leftLeg))
                .AddTo(gameObject);
            
            var h3 = LMotion.Create(ClampAngle(m_rightArm.rotation.eulerAngles.x), 0, m_halfDuration)
                .Bind(t => SetRotation(t, m_rightArm))
                .AddTo(gameObject);
            
            var h4 = LMotion.Create(ClampAngle(m_leftArm.rotation.eulerAngles.x), 0, m_halfDuration)
                .Bind(t => SetRotation(t, m_leftArm))
                .AddTo(gameObject);
            
            m_outHandles.AddRange(h1, h2, h3, h4);
        }

        private void SetRotation(float value, Transform target)
        {
            var rotationEuler = new Vector3(value, 0f);
            var rotation = target.localRotation;
            rotation.eulerAngles = rotationEuler;
            target.localRotation = rotation;
        }

        private float ClampAngle(float value) => value > 180 ? value - 360 : value;
        
        private static void CancelAndClear(List<MotionHandle> handles)
        {
            handles.ForEach(h => h.TryCancel());
            handles.Clear();
        }
    }
}