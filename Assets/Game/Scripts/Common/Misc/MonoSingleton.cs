using UnityEngine;

namespace Game.Scripts.Common.Misc
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            Instance = this as T;
            
            AwakeInternal();
        }

        protected virtual void AwakeInternal() { }
    }
}
