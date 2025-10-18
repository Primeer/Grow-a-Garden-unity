using System;
using System.Linq;
using Game.Scripts.Common.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    [CreateAssetMenu(fileName = nameof(PlantsConfig), menuName = nameof(PlantsConfig), order = 0)]
    public class PlantsConfig : ScriptableObject
    {
        [Header(Constants.DATA)]
        [TableList]
        [SerializeField] private Data[] m_dataList;

        public Data[] DataList => m_dataList;
        
        public Data GetPlantData(int id)
        {
            return m_dataList.First(d => d.Id == id);
        }

        private void OnValidate()
        {
            for (var i = 0; i < m_dataList.Length; i++)
            {
                m_dataList[i].Id = i;
                m_dataList[i].Prefab.SetId(i);
            }
        }

        [Serializable]
        public class Data
        {
            [VerticalGroup("Left")]
            [ReadOnly, HideLabel]
            public int Id;
            
            [VerticalGroup("Left")]
            public string Ru;
            
            [VerticalGroup("Left")]
            public string En;
            
            [VerticalGroup("Left")]
            [HideLabel]
            public Plant Prefab;
            
            [VerticalGroup("Right")]
            [ToggleLeft]
            public bool IsHarvestable;
            
            [VerticalGroup("Right")]
            [ShowIf("IsHarvestable")]
            public int Cost;
            
            [VerticalGroup("Right")]
            [ShowIf("IsHarvestable")]
            public float Weight;
            
            [ShowIf("IsHarvestable")]
            [TableColumnWidth(50, false)]
            [PreviewField, HideLabel]
            public Sprite Icon;
        }
    }
}