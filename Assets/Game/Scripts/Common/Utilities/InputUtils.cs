using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Common.Utilities
{
    public static class InputUtils
    {
        public static bool IsPointerOverUI(Vector2 screenPosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = screenPosition;
    
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
    
            return results.Count > 0;
        }
    }
}