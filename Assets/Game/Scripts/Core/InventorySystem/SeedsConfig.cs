using System;
using System.Linq;
using Game.Scripts.Core.ShopSystem.Purchasing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core.InventorySystem
{
    [CreateAssetMenu(fileName = nameof(SeedsConfig), menuName = nameof(SeedsConfig), order = 0)]
    public class SeedsConfig : ScriptableObject
    {
        [TableList]
        [SerializeField] private Data[] m_dataList;

        public Data[] DataList => m_dataList;
        
        public Data GetSeedData(string tag)
        {
            return m_dataList.First(d => d.Tag == tag);
        }

        public Data GetSeedData(int id)
        {
            return m_dataList.First(d => d.Id == id);
        }

        private void OnValidate()
        {
            for (var i = 0; i < m_dataList.Length; i++)
            {
                m_dataList[i].Id = i;
            }
        }

        [Serializable]
        public class Data
        {
            [VerticalGroup("Data")]
            [ReadOnly]
            public int Id;

            [VerticalGroup("Data")]
            public string Ru;
            
            [VerticalGroup("Data")]
            public string En;
            
            [VerticalGroup("Data")]
            public int Cost;
            
            [VerticalGroup("Data")]
            public int PlantId;

            [VerticalGroup("Data")]
            public Rarity Rarity;
            
            [VerticalGroup("Data")]
            public bool Ads;
            
            [VerticalGroup("Data")]
            public string Tag;
            
            [TableColumnWidth(50, false)]
            [PreviewField, HideLabel]
            public Sprite Sprite;
        }
    }
}