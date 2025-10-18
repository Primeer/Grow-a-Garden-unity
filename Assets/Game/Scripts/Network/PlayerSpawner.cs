using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Network
{
    public class PlayerSpawner : MonoBehaviour
    {
        const string K_AVATAR_VIEW_ID = "avatarViewId";
        [SerializeField] private string m_playerPrefabPath = "Player";
        [SerializeField] private CinemachineCamera m_cinemachineCamera;

        private GameObject m_player;

        public UnityEvent<GameObject> PlayerSpawned;

        private void Start()
        {
            m_player = SpawnPlayerLocal();
        }

        public void OnContextInitialized()
        {
            SetupPlayerAsync().Forget();
        }

        private async UniTask SetupPlayerAsync()
        {
            await UniTask.WaitForEndOfFrame();

            if (m_player)
            {
                CommonUtils.DestroyGameObject(m_player);
                m_player = null;
            }
            
            m_player = SpawnPlayerNetwork();
            
            // if (TryGetExistedPlayer(out var playerGo))
            // {
            //     SetPlayer(playerGo);
            // }
            // else
            // {
            //     SpawnPlayer();
            // }
        }

        private bool TryGetExistedPlayer(out GameObject playerGo)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(K_AVATAR_VIEW_ID, out object boxed) &&
                boxed is int viewId)
            {
                var pv = PhotonView.Find(viewId);
                
                if (pv)
                {
                    playerGo = pv.gameObject;
                    return true;
                }
            }

            playerGo = null;
            return false;
        }
        
        private GameObject SpawnPlayerLocal()
        {
            var playerGo = InstantiatePlayerLocal();
            SetPlayer(playerGo);

            return playerGo;
        }

        private GameObject SpawnPlayerNetwork()
        {
            var playerGo = InstantiatePlayerNetwork();
            // SavePlayerViewId(playerGo.GetComponent<PhotonView>());
            SetPlayer(playerGo);

            return playerGo;
        }

        private GameObject InstantiatePlayerLocal()
        {
            var playerPrefab = Resources.Load<GameObject>(m_playerPrefabPath);
            return Instantiate(playerPrefab);
        }

        private GameObject InstantiatePlayerNetwork()
        {
            var position = PlayerContext.Instance.GardenPoint.position;
            var rotation = PlayerContext.Instance.GardenPoint.rotation;
            
            var playerGo = PhotonNetwork.Instantiate(m_playerPrefabPath, position, rotation);
            
            return playerGo;
        }
        
        private void SavePlayerViewId(PhotonView pv)
        {
            var props = new Hashtable { {K_AVATAR_VIEW_ID, pv.ViewID} };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void SetPlayer(GameObject playerGo)
        {
            m_cinemachineCamera.Target = new CameraTarget() { TrackingTarget = playerGo.transform };
            PlayerSpawned?.Invoke(playerGo);
        }
    }
}