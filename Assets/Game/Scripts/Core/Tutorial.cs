using System;
using Game.Scripts.Common.Utilities;
using GamePush;
using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Core
{
    public class Tutorial : MonoBehaviour
    {
        private const string SAVE_KEY = "tutorial_movement";
        private const string ANALYTICS_KEY = "TUTORIAL_COMPLETED";
        
        [SerializeField] private float m_timer;
        [SerializeField] private Button m_closeButton;
        [SerializeField] private TMP_Text m_timerText;
        [SerializeField] private GameObject m_panelObject;
        
        private IDisposable m_disposable;
        private MotionHandle m_handle;
        
        private void Start()
        {
            GP_Game.GameReady();
            
            bool isTutorCompleted = GP_Player.GetBool(SAVE_KEY);

            if (isTutorCompleted == false)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            m_panelObject.SetActive(true);
            
            m_timerText.text = LocalizationUtils.GetLocalized(
                "Tap to continue",
                "Нажмите, чтобы продолжить");
            
            m_closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void Hide()
        {
            m_panelObject.SetActive(false);
            GP_Game.GameplayStart();
        }

        private void OnCloseButtonClicked()
        {
            GP_Player.Set(SAVE_KEY, true);
            GP_Player.Sync();
            
            GP_Analytics.Goal(ANALYTICS_KEY, "MOVEMENT");
            
            Hide();
        }
        
        private void OnDestroy()
        {
            m_closeButton.onClick.RemoveAllListeners();
            
            GP_Game.GameplayStop();
        }
    }
}