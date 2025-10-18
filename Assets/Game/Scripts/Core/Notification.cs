using Game.Scripts.Common.Misc;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.InventorySystem;
using LitMotion.Animation;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class Notification : MonoSingleton<Notification>
    {
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private LitMotionAnimation m_animation;

        public void ShowItemAdded(SeedItem seedItem)
        {
            Show(LocalizationUtils.GetLocalized(
                    $"Got {seedItem.En} x{seedItem.Amount}",
                    $"Получено {seedItem.Ru} х{seedItem.Amount}"),
                Color.forestGreen);
        }
        
        public void ShowNotEnoughMoney()
        {
            Show(LocalizationUtils.GetLocalized(
                "Not enough money",
                "Недостаточно денег"),
                Color.red);
        }
        
        public void ShowInventoryIsFull()
        {
            Show(LocalizationUtils.GetLocalized(
                "Inventory is full",
                "Инвентарь заполнен"),
                Color.red);
        }
        
        public void Show(string text, Color color)
        {
            m_text.text = text;
            m_text.color = color;
            m_animation.Restart();
        }

        private void OnValidate()
        {
            m_text ??= GetComponentInChildren<TMP_Text>();
            m_animation ??= GetComponentInChildren<LitMotionAnimation>();
        }
    }
}
