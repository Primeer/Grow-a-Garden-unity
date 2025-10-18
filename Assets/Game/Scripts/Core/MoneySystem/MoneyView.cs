using Game.Scripts.Common.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Core.MoneySystem
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        
        public void SetMoney(int money)
        {
            m_text.text = StringsUtils.FormatMoney(money);
        }
    }
}