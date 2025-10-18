using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Common.Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core.UseSystem
{
    public class UsableObjectsContainer : MonoSingleton<UsableObjectsContainer>
    {
        [SerializeField] private UsableObject[] m_prespawnedObjects;
        
        [ShowInInspector]
        private readonly List<UsableObject> m_objects = new ();

        public IReadOnlyList<UsableObject> Objects => m_objects;

        private void Start()
        {
            foreach (var obj in m_prespawnedObjects)
            {
                if (obj == null)
                    continue;

                obj.EnableUsageTracking();
            }
        }

        internal void Register(UsableObject obj)
        {
            if (obj == null || m_objects.Contains(obj))
                return;

            m_objects.Add(obj);
        }

        internal void Unregister(UsableObject obj)
        {
            if (obj == null)
                return;

            m_objects.Remove(obj);
        }

        public IEnumerable<UsableObject> GetObjectsInArea(Vector3 origin, float sqrRadius)
        {
            return m_objects.Where(obj => (obj.transform.position - origin).sqrMagnitude <= sqrRadius);
        }
    }
}
