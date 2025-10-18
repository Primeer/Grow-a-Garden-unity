using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Common
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        [SerializeField] private Button m_button;
        [SerializeField] private TMP_Text m_text;
        
        public Button Button => m_button;
        
        public void SetText(string text) => m_text.text = text;

        private void OnValidate()
        {
            m_button ??= GetComponent<Button>();
            m_text ??= GetComponentInChildren<TMP_Text>();
        }
    }
}