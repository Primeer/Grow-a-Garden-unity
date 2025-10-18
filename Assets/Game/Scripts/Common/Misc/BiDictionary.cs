using System;
using System.Collections.Generic;

namespace Game.Scripts.Common.Misc
{
    public class BiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_keyToValue = new();
        private readonly Dictionary<TValue, TKey> m_valueToKey = new();

        public Dictionary<TKey,TValue>.KeyCollection Keys => m_keyToValue.Keys;
        public Dictionary<TKey,TValue>.ValueCollection Values => m_keyToValue.Values;

        public void Add(TKey key, TValue value)
        {
            if (m_keyToValue.ContainsKey(key) || m_valueToKey.ContainsKey(value))
                throw new ArgumentException("Ключ или значение уже существует");
        
            m_keyToValue[key] = value;
            m_valueToKey[value] = key;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_keyToValue.TryGetValue(key, out value);
        }

        public bool TryGetKey(TValue value, out TKey key)
        {
            return m_valueToKey.TryGetValue(value, out key);
        }

        public TValue GetByKey(TKey key) => m_keyToValue[key];
        public TKey GetByValue(TValue value) => m_valueToKey[value];
        
        public TValue this[TKey key] => m_keyToValue[key];
        public TKey this[TValue value] => m_valueToKey[value];

        public bool Remove(TKey key)
        {
            if (m_keyToValue.Remove(key, out var value))
            {
                m_valueToKey.Remove(value);
                return true;
            }
            return false;
        }
        
        public bool Remove(TValue value)
        {
            if (m_valueToKey.Remove(value, out var key))
            {
                m_keyToValue.Remove(key);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            m_keyToValue.Clear();
            m_valueToKey.Clear();
        }
    }
}