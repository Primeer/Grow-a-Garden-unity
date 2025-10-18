using System;
using System.Collections.Generic;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem.UI.Views;
using UnityEngine;

namespace Game.Scripts.Core.InventorySystem
{
    public class ItemsView : MonoBehaviour
    {
        [Header(Constants.REFERENCES)]
        [SerializeField] private Transform m_container;
        [SerializeField] private UIItemSlot m_slotPrefab;
        
        private readonly List<UIItemSlot> m_slots = new();
        private UIItemSlot m_selectedSlot;
        
        public event Action<UIItemSlot> SlotSelected;

        public UIItemSlot SpawnItem(Item item)
        {
            var slot = Instantiate(m_slotPrefab, m_container);
            slot.SetItem(item);
            slot.SlotClicked += OnSlotClicked;
                
            m_slots.Add(slot);

            return slot;
        }
        
        public void RemoveItem(UIItemSlot slot)
        {
            DeselectSlot();

            m_slots.Remove(slot);
            
            CommonUtils.DestroyGameObject(slot.gameObject);
        }

        public void Clear()
        {
            DeselectSlot();

            foreach (var slot in m_slots)
            {
                CommonUtils.DestroyGameObject(slot.gameObject);
            }
            
            m_slots.Clear();
        }
        
        private void OnSlotClicked(UIItemSlot slot)
        {
            SoundsManager.Instance.PlayInventorySlot();
            
            if (slot == m_selectedSlot)
            {
                DeselectSlot();
                return;
            }
            
            if (m_selectedSlot != null)
            {
                m_selectedSlot.SetSelected(false);
            }
            
            slot.SetSelected(true);
            
            m_selectedSlot = slot;
            SlotSelected?.Invoke(slot);
        }

        private void DeselectSlot()
        {
            if (m_selectedSlot != null)
            {
                m_selectedSlot.SetSelected(false);
            }

            m_selectedSlot = null;
            SlotSelected?.Invoke(null);
        }
    }
}