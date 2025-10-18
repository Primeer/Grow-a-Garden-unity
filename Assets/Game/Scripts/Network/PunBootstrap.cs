using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using GamePush;
using Logger = Game.Scripts.Common.Logger;

namespace Game.Scripts.Network
{
    /// <summary>
    /// Handles connection to Photon Cloud, matchmaking and player instantiation.
    /// </summary>
    public class PunBootstrap : MonoBehaviourPunCallbacks
    {
        [Tooltip("Game version used by Photon to separate clients")]
        [SerializeField] private string gameVersion = "2";
        
        private void Start()
        {
            ApplyIdentityFromGamePush();

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;

            Logger.Info($"{nameof(PunBootstrap)}.{nameof(Start)}:: Connecting to server started");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnConnectedToMaster)}:: Connected to master");
            Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnConnectedToMaster)}:: Joining random room");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnJoinRandomFailed)}:: Joining random room failed, code {returnCode}, message {message}");
            CreateRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnJoinRoomFailed)}:: Joining room failed, code {returnCode}, message {message}");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnDisconnected)}:: Disconnected from server, cause {cause}");
            
            switch (cause)
            {
                case DisconnectCause.DisconnectByClientLogic:
                case DisconnectCause.DisconnectByOperationLimit:
                case DisconnectCause.DisconnectByDisconnectMessage:
                case DisconnectCause.ApplicationQuit:
                case DisconnectCause.None:
                    return;
                
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.ClientTimeout:
                    Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnDisconnected)}:: Connect Using Settings");
                    PhotonNetwork.ConnectUsingSettings();
                    // Logger.Info($"{nameof(PunBootstrap)}.{nameof(OnDisconnected)}:: Reconnect and rejoin");
                    // PhotonNetwork.ReconnectAndRejoin();
                    return;
            }
        }
        
        private void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 6,
                // PlayerTtl = 30000,
                PlayerTtl = 0,
                // EmptyRoomTtl = 30000,
                EmptyRoomTtl = 0,
            };

            Logger.Info($"{nameof(PunBootstrap)}.{nameof(CreateRoom)}:: Creating room");
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
        
        private void ApplyIdentityFromGamePush()
        {
            string userId = GP_Player.GetID().ToString();
            string nickName = GP_Player.GetName();

            PhotonNetwork.AuthValues = new AuthenticationValues(userId);
            PhotonNetwork.NickName = string.IsNullOrEmpty(nickName) ? "Guest" : nickName;

            Logger.Info($"{nameof(PunBootstrap)}.{nameof(ApplyIdentityFromGamePush)}:: Identity applying");
        }
    }
}
