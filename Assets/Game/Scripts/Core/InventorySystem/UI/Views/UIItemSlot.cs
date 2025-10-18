using System;
using Game.Scripts.Common.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.InventorySystem.UI.Views
{
    public class UIItemSlot : MonoBehaviour
    {
        [SerializeField] private GameObject m_selectedObject;
        [SerializeField] private Button m_button;
        [SerializeField] private TMP_Text m_text;

        private UIItemWeight m_weightView;

        public event Action<UIItemSlot> SlotClicked;

        private void Awake()
        {
            m_weightView = GetComponentInChildren<UIItemWeight>(true);
        }

        private void OnEnable()
        {
            m_button.onClick.AddListener(() => SlotClicked?.Invoke(this));
        }

        private void OnDisable()
        {
            m_button.onClick.RemoveAllListeners();
        }

        public void SetItem(Item item)
        {
            m_text.text = LocalizationUtils.GetLocalized(item.En, item.Ru);
            TrySetWeight(item);
        }

        private void TrySetWeight(Item item)
        {
            if (item is PlantItem plantItem)
            {
                m_weightView.gameObject.SetActive(true);
                m_weightView.SetWeight(plantItem.Weight);
            }
            else
            {
                m_weightView.gameObject.SetActive(false);
            }
        }

        public void SetSelected(bool isSelected)
        {
            m_selectedObject.SetActive(isSelected);
        }
    }
}