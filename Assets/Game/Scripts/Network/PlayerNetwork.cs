using Game.Scripts.Core;
using Photon.Pun;
using UnityEngine;

namespace Game.Scripts.Network
{
    /// <summary>
    /// Synchronizes the position and rotation of the player across the network.
    /// Also disables local control on remote players.
    /// </summary>
    public class PlayerNetwork : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
    {
        // Target position and rotation received from the network
        private Vector3 m_networkPosition;
        private Quaternion m_networkRotation;

        private void Start()
        {
            // Initialize network targets with current transform to prevent snapping.
            m_networkPosition = transform.position;
            m_networkRotation = transform.rotation;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // The local player sends its current position and rotation.
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                // Network player receives position and rotation and caches them.
                m_networkPosition = (Vector3)stream.ReceiveNext();
                m_networkRotation = (Quaternion)stream.ReceiveNext();
            }
        }

        private void Update()
        {
            if (PhotonNetwork.InRoom == false)
                return;
            
            // Smoothly move remote players toward their network position.
            if (!photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, m_networkPosition, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_networkRotation, Time.deltaTime * 10f);
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            gameObject.name = $"{info.Sender.UserId}_{info.Sender.NickName}";
            info.Sender.TagObject = gameObject;
            
            if (info.Sender.CustomProperties.TryGetValue("player_slot", out object value) && value is int slotIndex)
            {
                PlayerContext.Instance.Gardens[slotIndex].BillboardText.text = info.Sender.NickName;
            }
        }
    }
}
