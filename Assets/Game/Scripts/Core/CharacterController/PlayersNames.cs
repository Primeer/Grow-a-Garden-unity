using System.Collections.Generic;
using Game.Scripts.Common;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Core.CharacterController
{
    public class PlayersNames : UIDynamic
    {
        [SerializeField] private float m_distance1 = 5f;
        [SerializeField] private Vector3 m_worldOffset = Vector3.zero;
        [SerializeField] private TMP_Text[] m_texts;
        [SerializeField] private List<Transform> m_targets = new();

        private float m_sqrDistance;

        private void Awake()
        {
            DisableTexts();
            m_sqrDistance = m_distance1 * m_distance1;
        }

        private void FixedUpdate()
        {
            DisableTexts();
            m_targets.Clear();
            
            if (PhotonNetwork.InRoom == false)
                return;
            
            var localPlayerPosition = PlayerContext.Instance.PlayerTransform.position;
            var players = PlayerContext.Instance.OtherPlayers;
            
            for (var i = 0; i < players.Count; i++)
            {
                if (CheckPlayer(players[i], localPlayerPosition, out var playerTransform))
                {
                    m_texts[i].text = players[i].NickName;
                    m_texts[i].gameObject.SetActive(true);
                    
                    m_targets.Add(playerTransform);
                }
            }
        }

        private void Update()
        {
            for (var i = 0; i < m_targets.Count; i++)
            {
                SetTextPosition(m_texts[i], m_targets[i]);
            }
        }

        private bool CheckPlayer(Player player, Vector3 localPlayerPosition, out Transform playerTransform)
        {
            playerTransform = null;
            var playerGo = (GameObject)player.TagObject;

            if (playerGo is null)
                return false;

            playerTransform = playerGo.transform;
            
            var direction = playerGo.transform.position - localPlayerPosition;
            float distance = Vector3.SqrMagnitude(direction);
            return distance <= m_sqrDistance;
        }

        private void SetTextPosition(TMP_Text text, Transform target)
        {
            var worldPosition = target.position + m_worldOffset;
            var screenPosition = Camera.WorldToScreenPoint(worldPosition);
            text.rectTransform.anchoredPosition = ScreenToRectPosition(screenPosition);
        }

        private void DisableTexts() => m_texts.ForEach(t => t.gameObject.SetActive(false));
    }
}