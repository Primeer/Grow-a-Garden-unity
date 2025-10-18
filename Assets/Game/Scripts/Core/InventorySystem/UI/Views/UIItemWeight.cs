using TMPro;
using UnityEngine;

namespace Game.Scripts.Core.InventorySystem.UI.Views
{
    public class UIItemWeight : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;

        public void SetWeight(string weight)
        {
            m_text.text = weight;
        }
    }
}