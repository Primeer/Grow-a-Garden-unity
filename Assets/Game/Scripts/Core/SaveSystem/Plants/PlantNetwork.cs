using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.Plants;
using Game.Scripts.Core.UseSystem;
using Photon.Pun;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem.Plants
{
    public class PlantNetwork : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private Growth m_growth;
        private UsableObject m_usableObject;
        private PhotonView m_photonView;

        private void Awake()
        {
            m_growth = GetComponent<Growth>();
            m_usableObject = GetComponent<UsableObject>();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            m_photonView = info.photonView;
            object[] data = info.photonView.InstantiationData;
            
            SetupScale((Vector3)data[0]);
            SetupGrowth((float)data[1], (long)data[2]);

            if (m_photonView.IsMine)
            {
                SetupUsableObject();
            }
        }

        private void SetupScale(Vector3 scale)
        {
            // сетаем lossyScale - это допустимо, т.к. еще не присвоен parent
            transform.localScale = scale;
        }

        private void SetupGrowth(float timer, long saveTimeTicks)
        {
            float progressTime = TimeUtils.CalculateProgressTime(timer, saveTimeTicks);
            
            m_growth.SetProgressTime(progressTime);
        }

        private void SetupUsableObject()
        {
            if (!m_usableObject)
                return;
            
            if (m_growth.IsGrowthCompleted)
            {
                m_usableObject.EnableUsageTracking();
            }
            else
            {
                m_growth.GrowthCompleted += m_usableObject.EnableUsageTracking;
            }
        }

        private void OnDestroy()
        {
            if (m_photonView && !m_photonView.IsMine) 
                return;
            
            m_usableObject?.DisableUsageTracking();
        }
    }
}