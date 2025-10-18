using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class UIShopItem : MonoBehaviour
    {
        [SerializeField] private Color m_priceGreenColor;
        [SerializeField] private Color m_priceRedColor;
        
        [SerializeField] private TMP_Text m_nameText;
        [SerializeField] private TMP_Text m_amountText;
        [SerializeField] private TMP_Text m_priceText;
        [SerializeField] private Image m_icon;
        [SerializeField] private UIItemRarity m_itemRarity;
        [SerializeField] private Button m_button;

        public Button Button => m_button;

        public void SetItem(SeedItem item)
        {
            m_nameText.text = LocalizationUtils.GetLocalized(item.En, item.Ru);
            m_amountText.text = $"X{item.Amount}";
            
            m_priceText.text = item.Amount > 0 ? 
                StringsUtils.FormatMoney(item.Cost) : 
                LocalizationUtils.GetLocalized("No stock", "Нет в продаже");
            
            m_priceText.color = item.Amount > 0 ? m_priceGreenColor : m_priceRedColor;
            m_icon.sprite = item.Icon;
            
            m_itemRarity.SetRarity(item.Rarity);
        }
    }
}