using System;
using System.Collections.Generic;
using Game.Scripts.Common;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Audio;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.UseSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class ShopBuyWindow : MonoBehaviour
    {
        [SerializeField] private Button[] m_closeButtons;
        [SerializeField] private TMP_Text m_headerText;
        [SerializeField] private Transform m_container;
        [SerializeField] private UIShopItem m_itemPrefab;
        [SerializeField] private Transform m_buttonsPanel;
        [SerializeField] private UIButton m_softButton;
        [SerializeField] private UIButton m_realButton;
        [SerializeField] private UIButton m_adsButton;
        [SerializeField] private UIButton m_restockButton;
        [SerializeField] private UsableObject m_usableObject;
        
        private readonly List<UIShopItem> m_items = new();

        public Button[] CloseButtons => m_closeButtons;
        public UIButton SoftButton => m_softButton;
        public UIButton RealButton => m_realButton;
        public UIButton AdsButton => m_adsButton;
        public UIButton RestockButton => m_restockButton;
        public UsableObject UsableObject => m_usableObject;
        public event Action Showed;
        public event Action Hided;
        public event Action<SeedItem> ItemSelected; 

        private void OnEnable() => Showed?.Invoke();
        private void OnDisable() => Hided?.Invoke();

        public void SetHeaderText(string text)
        {
            m_headerText.text = text;
        }

        public UIShopItem SpawnItem(SeedItem item)
        {
            var uiItem = Instantiate(m_itemPrefab, m_container);
            uiItem.SetItem(item);
            uiItem.Button.onClick.AddListener(() => OnItemClicked(uiItem, item));
                
            m_items.Add(uiItem);
            
            m_buttonsPanel.gameObject.SetActive(false);

            return uiItem;
        }

        public void ClearItems()
        {
            foreach (var uiShopItem in m_items)
            {
                uiShopItem.Button.onClick.RemoveAllListeners();
                CommonUtils.DestroyGameObject(uiShopItem.gameObject);
            }
            
            m_items.Clear();
        }

        private void OnItemClicked(UIShopItem uiItem, SeedItem item)
        {
            SoundsManager.Instance.PlayShopSlot();
            
            int index = m_items.IndexOf(uiItem);
            m_buttonsPanel.gameObject.SetActive(true);
            m_buttonsPanel.SetSiblingIndex(index + 1);
            
            ItemSelected?.Invoke(item);
        }
        
    }
}