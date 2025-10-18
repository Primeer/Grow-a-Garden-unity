using System.Collections.Generic;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core;
using Photon.Pun;
using Photon.Realtime;

namespace Game.Scripts.Network
{
    public class PlayersCacher : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            PlayerContext.Instance.OtherPlayers = new List<Player>(PhotonNetwork.PlayerListOthers);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.IsLocal)
                return;
            
            PlayerContext.Instance.OtherPlayers.TryAdd(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerContext.Instance.OtherPlayers.TryRemove(otherPlayer);
        }
    }
}