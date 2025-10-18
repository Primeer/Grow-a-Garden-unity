using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.UseSystem;
using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    public class Plant : MonoBehaviour
    {
        [Header(Constants.SETTINGS)]
        [SerializeField] private int m_id;

        private Growth m_growth;
        private ChildrenSettings m_childrenSettings;
        private UsableObject m_usableObject;

        public int Id => m_id;
        public Growth Growth => m_growth ??= GetComponent<Growth>();
        public ChildrenSettings ChildrenSettings => m_childrenSettings ??= GetComponent<ChildrenSettings>();
        public UsableObject UsableObject => m_usableObject ??= GetComponent<UsableObject>();

        public void SetId(int id) => m_id = id;
    }
}