using Game.Scripts.Common;
using Game.Scripts.Common.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.InventorySystem.UI.Views
{
    public class InventoryWindow : MonoBehaviour
    {
        [Header(Constants.REFERENCES)]
        [SerializeField] private Toggle m_seedToggle;
        [SerializeField] private Toggle m_plantToggle;
        [SerializeField] private ItemsView m_itemsView;
        
        public ItemsView ItemsView => m_itemsView;
        public Toggle SeedToggle => m_seedToggle;
        public Toggle PlantToggle => m_plantToggle;
    }
}