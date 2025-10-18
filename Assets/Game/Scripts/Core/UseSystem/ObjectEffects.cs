using System.Linq;
using Game.Scripts.Common.Utilities;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Scripts.Core.UseSystem
{
    public class ObjectEffects : MonoBehaviour
    {
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        
        [Header(Constants.SETTINGS)]
        [SerializeField] private float m_outlineWidth;
        
        private OutlineObject[] m_outlineObjects;

        private void Awake()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);

            m_outlineObjects = renderers.Select(r => 
                    new OutlineObject()
                    {
                        Renderer = r, 
                        PropertyBlock = new()
                    })
                .ToArray();
        }

        public void SetHighlighted(bool isSelected)
        {
            m_outlineObjects.ForEach(obj => SetOutlineWidth(obj, isSelected));
        }
        
        private void SetOutlineWidth(OutlineObject obj, bool isEnabled)
        {
            obj.Renderer.GetPropertyBlock(obj.PropertyBlock);
            obj.PropertyBlock.SetFloat(OutlineWidth, isEnabled ? m_outlineWidth : 0f);
            obj.Renderer.SetPropertyBlock(obj.PropertyBlock);
        }

        private class OutlineObject
        {
            public Renderer Renderer;
            public MaterialPropertyBlock PropertyBlock;
        }
    }
}