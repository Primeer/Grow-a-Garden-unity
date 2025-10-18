using Game.Scripts.Common.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Common
{
    public class Localization : MonoBehaviour
    {
        [SerializeField] private string m_ru;
        [SerializeField] private string m_en;
        [SerializeField] private TMP_Text m_text;

        private void Start()
        {
            m_text.text = LocalizationUtils.GetLocalized(m_en, m_ru);
        }

        private void OnValidate()
        {
            m_text ??= GetComponent<TMP_Text>();
            m_ru ??= m_text.text;
        }
    }
}