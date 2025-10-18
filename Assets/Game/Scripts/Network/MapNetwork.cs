using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Game.Scripts.Common;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

namespace Game.Scripts.Network
{
    public class MapNetwork : MonoBehaviourPunCallbacks
    {
        private const string ROOM_SLOTS_KEY = "room_slots";
        private const string PLAYER_SLOT_KEY = "player_slot";
        private const int SET_PROPERTIES_ATTEMPTS_COUNT = 3;

        private int m_cachedSlotIndex;
        private int m_setRoomPropsAttempt;
        private CancellationTokenSource m_cts = new ();
        private Action m_cachedOperation;
        
        public UnityEvent<int> JoinedRoom;
        
        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.OpResponseReceived += NetworkingClientOnOpResponseReceived;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.OpResponseReceived -= NetworkingClientOnOpResponseReceived;
        }
        
        public override void OnJoinedRoom()
        {
            if (TryGetSlotIndex(PhotonNetwork.LocalPlayer.CustomProperties, out int slotIndex))
            {
                Logger.Info($"{nameof(MapNetwork)}.{nameof(OnJoinedRoom)}:: Player property slot found, slot {slotIndex}");
                
                JoinedRoom?.Invoke(slotIndex);
                return;
            }
            
            CacheAndInvoke(FillEmptySlot);
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!PhotonNetwork.IsMasterClient || otherPlayer.IsInactive)
                return;

            CacheAndInvoke(() => ReleaseSlot(otherPlayer));
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (TryGetSlotIndex(PhotonNetwork.LocalPlayer.CustomProperties, out int slotIndex) && slotIndex == m_cachedSlotIndex) 
                return;
            
            SetPlayerSlotProperty(m_cachedSlotIndex);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!targetPlayer.IsLocal || !TryGetSlotIndex(changedProps, out int slotIndex))
                return;

            JoinedRoom?.Invoke(slotIndex);
        }
        
        private void NetworkingClientOnOpResponseReceived(OperationResponse opResponse)
        {
            if (opResponse.OperationCode != OperationCode.SetProperties ||
                opResponse.ReturnCode != ErrorCode.InvalidOperation) 
                return;
            
            Logger.Error(
                $"{nameof(MapNetwork)}.{nameof(NetworkingClientOnOpResponseReceived)}:: Attempt was failed to set room slots");

            if (m_setRoomPropsAttempt++ >= SET_PROPERTIES_ATTEMPTS_COUNT)
            {
                PhotonNetwork.ConnectUsingSettings();
                return;
            }
                
            InvokeCachedOperationDelayedAsync().Forget();
        }

        private void CacheAndInvoke(Action action)
        {
            ResetAttemptCount();
            
            m_cachedOperation = action;
            action?.Invoke();
        }

        private void FillEmptySlot()
        {
            bool[] slots = GetRoomSlotsOrDefault();
            m_cachedSlotIndex = FindEmptySlot(slots);
            
            slots[m_cachedSlotIndex] = true;

            SetRoomSlots(slots);
            
            Logger.Info($"{nameof(MapNetwork)}.{nameof(FillEmptySlot)}:: Attempt to fill empty slot, slot {m_cachedSlotIndex}");
        }

        private void ReleaseSlot(Player player)
        {
            if (TryGetSlotIndex(player.CustomProperties, out int slotIndex) == false)
                return;
            
            bool[] slots = GetRoomSlotsOrDefault();

            slots[slotIndex] = false;
            
            SetRoomSlots(slots);
        
            Logger.Info($"{nameof(MapNetwork)}.{nameof(ReleaseSlot)}:: Attempt to release slot, slot {slotIndex}");
        }
        
        private void SetRoomSlots(bool[] slots)
        {
            var propertiesToSet = new Hashtable {{ ROOM_SLOTS_KEY, slots }};
            
            if (TryGetRoomSlots(PhotonNetwork.CurrentRoom.CustomProperties, out bool[] currentSlots))
            {
                var expectedProperties = new Hashtable {{ ROOM_SLOTS_KEY, currentSlots }};
                PhotonNetwork.CurrentRoom.SetCustomProperties(propertiesToSet, expectedProperties);
                Logger.Info($"{nameof(MapNetwork)}.{nameof(SetRoomSlots)}:: Room slots changed");
            }
            else
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(propertiesToSet);
                Logger.Info($"{nameof(MapNetwork)}.{nameof(SetRoomSlots)}:: Room slots created");
            }
        }
        
        private bool[] GetRoomSlotsOrDefault()
        {
            if (TryGetRoomSlots(PhotonNetwork.CurrentRoom.CustomProperties, out bool[] slots))
            {
                return slots.ToArray();
            }

            return new bool[6];
        }

        private bool TryGetRoomSlots(Hashtable properties, out bool[] slots)
        {
            if (properties.TryGetValue(ROOM_SLOTS_KEY, out object value) && value is bool[] slotsArray)
            {
                slots = slotsArray;
                return true;
            }

            slots = null;
            return false;
        }
        
        private bool TryGetSlotIndex(Hashtable properties, out int slotIndex)
        {
            if (properties.TryGetValue(PLAYER_SLOT_KEY, out object value) && value is int slot)
            {
                slotIndex = slot;
                return true;
            }

            slotIndex = -1;
            return false;
        }

        private void SetPlayerSlotProperty(int idx)
        {
            var playerProperties = new Hashtable { {PLAYER_SLOT_KEY, idx} };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            
            Logger.Info($"{nameof(MapNetwork)}.{nameof(SetPlayerSlotProperty)}:: Player slot set, slot {idx}");
        }
        
        private int FindEmptySlot(bool[] slots)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (slots[i] == false)
                    return i;
            }

            return 5;
        }

        private async UniTask InvokeCachedOperationDelayedAsync()
        {
            await UniTask.WaitForSeconds(0.5f * m_setRoomPropsAttempt, cancellationToken: m_cts.Token);
            
            m_cachedOperation?.Invoke();
        }

        private void ResetAttemptCount() => m_setRoomPropsAttempt = 0;

        private void OnDestroy()
        {
            m_cts?.Cancel();
            m_cts?.Dispose();
            m_cts = null;
        }
    }
}