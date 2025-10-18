using Game.Scripts.Common.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core.ShopSystem.Purchasing
{
    public class UIItemRarity : MonoBehaviour
    {
        [SerializeField] private Color m_commonColor;
        [SerializeField] private Color m_uncommonColor;
        [SerializeField] private Color m_rareColor;
        [SerializeField] private Color m_epicColor;
        [SerializeField] private Color m_legendaryColor;
        [SerializeField] private Color m_mythicalColor;
        
        [Space]
        [SerializeField] private Image m_backgroundImage;
        [SerializeField] private TMP_Text m_text;

        public void SetRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    m_backgroundImage.color = m_commonColor;
                    m_text.text = LocalizationUtils.GetLocalized("Common", "Обычные");
                    break;
                
                case Rarity.Uncommon:
                    m_backgroundImage.color = m_uncommonColor;
                    m_text.text = LocalizationUtils.GetLocalized("Uncommon", "Необычные");
                    break;
                
                case Rarity.Rare:
                    m_backgroundImage.color = m_rareColor;
                    m_text.text = LocalizationUtils.GetLocalized("Rare", "Редкие");
                    break;
                
                case Rarity.Epic:
                    m_backgroundImage.color = m_epicColor;
                    m_text.text = LocalizationUtils.GetLocalized("Epic", "Эпические");
                    break;
                
                case Rarity.Legendary:
                    m_backgroundImage.color = m_legendaryColor;
                    m_text.text = LocalizationUtils.GetLocalized("Legendary", "Легендарные");
                    break;
                
                case Rarity.Mythical:
                    m_backgroundImage.color = m_mythicalColor;
                    m_text.text = LocalizationUtils.GetLocalized("Mythical", "Мифические");
                    break;
            }
        }

        private void OnValidate()
        {
            m_backgroundImage ??= GetComponent<Image>();
            m_text ??= GetComponentInChildren<TMP_Text>();
        }
    }
}