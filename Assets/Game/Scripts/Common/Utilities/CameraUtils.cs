using UnityEngine;

namespace Game.Scripts.Common.Utilities
{
    public static class CameraUtils
    {
        public static bool IsPositionVisibleInViewport(Vector3 worldPosition, Camera camera)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPosition);
            bool isVisible = viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
                             viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
                             viewportPoint.z > 0;

            return isVisible;
        }
        
        /// <summary>
        /// Получить ближайшую точку на луче к заданной позиции
        /// </summary>
        public static Vector3 GetClosestPointOnRay(Ray ray, Vector3 point)
        {
            Vector3 rayToPoint = point - ray.origin;
            float projection = Vector3.Dot(rayToPoint, ray.direction);
        
            // Ограничиваем проекцию положительными значениями
            projection = Mathf.Max(0f, projection);
        
            return ray.origin + ray.direction * projection;
        }
    }
}