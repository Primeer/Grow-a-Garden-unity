using UnityEngine;

namespace Game.Scripts.Core.CharacterController
{
    public class InputSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_mobileInputObjects;
        [SerializeField] private GameObject[] m_pcInputObjects;

        private void Start()
        {
            if (Application.isEditor)
            {
                SetInputType(false);
                return;
            }

            SetInputType(GamePush.GP_Device.IsMobile());
        }

        private void SetInputType(bool isMobile)
        {
            foreach (var go in m_mobileInputObjects)
            {
               go.SetActive(isMobile); 
            }
            
            foreach (var go in m_pcInputObjects)
            {
                go.SetActive(!isMobile); 
            }
        }
    }
}
