using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Common.Utilities
{
    public static class CommonUtils
    {
        public static void DestroyGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            Object.Destroy(gameObject);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }
        
        public static bool TryAdd<T>(this List<T> list, T item)
        {
            if (list.Contains(item))
            {
                return false;
            }

            list.Add(item);
            return false;
        }

        public static bool TryRemove<T>(this List<T> list, T item)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
                return true;
            }

            return false;
        }
        
        public static void SwitchGo(this GameObject[] array, int idx)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].SetActive(i == idx);
            }
        }

        public static void AddRange<T>(this List<T> list, params T[] elements) => list.AddRange(elements);
    }
}