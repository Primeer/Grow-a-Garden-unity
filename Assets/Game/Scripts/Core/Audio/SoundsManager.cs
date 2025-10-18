using Game.Scripts.Common.Misc;
using UnityEngine;

namespace Game.Scripts.Core.Audio
{
    public class SoundsManager : MonoSingleton<SoundsManager>
    {
        [SerializeField] private AudioClip m_jumpClip;
        [SerializeField] private AudioClip m_plantingClip;
        [SerializeField] private AudioClip m_harvestClip;
        [SerializeField] private AudioClip m_inventoryClip;
        [SerializeField] private AudioClip m_inventorySlotClip;
        [SerializeField] private AudioClip m_shopSlotClip;
        [SerializeField] private AudioClip m_shopOpenClip;
        [SerializeField] private AudioClip m_shopCloseClip;
        [SerializeField] private AudioClip m_moneyClip;
        [SerializeField] private AudioSource m_audioSource;

        public void PlayJump() => m_audioSource.PlayOneShot(m_jumpClip);
        public void PlayPlanting() => m_audioSource.PlayOneShot(m_plantingClip);
        public void PlayHarvest() => m_audioSource.PlayOneShot(m_harvestClip);
        public void PlayInventory() => m_audioSource.PlayOneShot(m_inventoryClip);
        public void PlayInventorySlot() => m_audioSource.PlayOneShot(m_inventorySlotClip);
        public void PlayShopSlot() => m_audioSource.PlayOneShot(m_shopSlotClip);
        public void PlayShopOpen() => m_audioSource.PlayOneShot(m_shopOpenClip);
        public void PlayShopClose() => m_audioSource.PlayOneShot(m_shopCloseClip);
        public void PlayMoney() => m_audioSource.PlayOneShot(m_moneyClip);

        private void OnValidate()
        {
            m_audioSource ??= GetComponent<AudioSource>();
        }
    }
}
