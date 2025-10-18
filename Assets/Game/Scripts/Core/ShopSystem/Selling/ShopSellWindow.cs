using System;
using Game.Scripts.Common;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.UseSystem;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellWindow : MonoBehaviour
    {
        [SerializeField] private Button[] m_closeButtons;
        [SerializeField] private UIButton m_sellButton;
        [SerializeField] private UIButton m_sellAllButton;
        [SerializeField] private ItemsView m_itemsView;
        [SerializeField] private UsableObject m_usableObject;

        public ItemsView ItemsView => m_itemsView;
        public UsableObject UsableObject => m_usableObject;
        public UIButton SellButton => m_sellButton;
        public UIButton SellAllButton => m_sellAllButton;

        public event Action ClosePerformed;
        public event Action Showed;
        public event Action Hided;

        private void OnEnable()
        {
            m_closeButtons.ForEach(button => button.onClick.AddListener(OnCloseButtonClicked));
            
            Showed?.Invoke();
        }

        private void OnDisable()
        {
            m_closeButtons.ForEach(button => button.onClick.RemoveAllListeners());
      
            Hided?.Invoke();
        }

        private void OnCloseButtonClicked()
        {
            ClosePerformed?.Invoke();
        }
    }
}