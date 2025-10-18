using System;
using UnityEngine;

namespace Game.Scripts.Archive
{
    public class CenterScreenRaycast : MonoBehaviour
    {
        [Header("Настройки луча")]
        public float m_rayDistance = 100f;           // Максимальная дистанция луча
        public LayerMask m_layerMask = -1;           // Слои для проверки (по умолчанию все)
        public Camera m_targetCamera;                // Камера для луча (по умолчанию основная)
    
        [Header("Настройки поиска")]
        public bool m_continuousUpdate = true;       // Постоянно обновлять поиск
        public bool m_useSphereCast;         // Использовать SphereCast вместо Raycast
        public float m_sphereRadius = 0.5f;          // Радиус сферы для SphereCast
    
        [Header("Фильтры")]
        public string[] m_ignoreTags;                // Теги объектов, которые нужно игнорировать
        public bool m_onlyWithColliders = true;      // Учитывать только объекты с коллайдерами
    
        [Header("Отладка")]
        public bool m_showDebugRay = true;           // Показывать луч в Scene view
        public Color m_rayColor = Color.red;         // Цвет луча
    
        // Текущий ближайший объект
        public GameObject NearestObject { get; private set; }
        public float NearestDistance { get; private set; }
        public Vector3 HitPoint { get; private set; }
        public Vector3 HitNormal { get; private set; }
    
        // События
        public Action<GameObject> OnNearestObjectChanged;
        public Action<RaycastHit> OnObjectHit;
        public Action OnNoObjectFound;
    
        private GameObject m_previousNearestObject;
        private Ray m_currentRay;
    
        void Start()
        {
            // Получаем камеру, если не назначена
            if (m_targetCamera == null)
                m_targetCamera = Camera.main;
            
            if (m_targetCamera == null)
                m_targetCamera = FindObjectOfType<Camera>();
        }
    
        void Update()
        {
            if (m_continuousUpdate)
            {
                FindNearestObject();
            }
        }
    
        /// <summary>
        /// Найти ближайший объект к лучу из центра экрана
        /// </summary>
        public GameObject FindNearestObject()
        {
            if (m_targetCamera == null)
            {
                Debug.LogWarning("Камера не найдена!");
                return null;
            }
        
            // Создаем луч из центра экрана
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            m_currentRay = m_targetCamera.ScreenPointToRay(screenCenter);
        
            GameObject nearestObject = null;
            float nearestDistance = float.MaxValue;
            RaycastHit nearestHit = new RaycastHit();
        
            // Выполняем raycast или spherecast
            RaycastHit[] hits;
        
            if (m_useSphereCast)
            {
                hits = Physics.SphereCastAll(m_currentRay, m_sphereRadius, m_rayDistance, m_layerMask);
            }
            else
            {
                hits = Physics.RaycastAll(m_currentRay, m_rayDistance, m_layerMask);
            }
        
            // Проходим по всем попаданиям
            foreach (RaycastHit hit in hits)
            {
                // Проверяем фильтры
                if (!IsValidTarget(hit.collider.gameObject))
                    continue;
                
                // Проверяем, является ли этот объект ближайшим
                if (hit.distance < nearestDistance)
                {
                    nearestDistance = hit.distance;
                    nearestObject = hit.collider.gameObject;
                    nearestHit = hit;
                }
            }
        
            // Обновляем свойства
            NearestObject = nearestObject;
            NearestDistance = nearestDistance == float.MaxValue ? 0f : nearestDistance;
        
            if (nearestObject != null)
            {
                HitPoint = nearestHit.point;
                HitNormal = nearestHit.normal;
                OnObjectHit?.Invoke(nearestHit);
            }
            else
            {
                HitPoint = Vector3.zero;
                HitNormal = Vector3.zero;
                OnNoObjectFound?.Invoke();
            }
        
            // Проверяем, изменился ли ближайший объект
            if (nearestObject != m_previousNearestObject)
            {
                m_previousNearestObject = nearestObject;
                OnNearestObjectChanged?.Invoke(nearestObject);
            }
        
            return nearestObject;
        }
    
        /// <summary>
        /// Проверить, подходит ли объект под наши критерии
        /// </summary>
        private bool IsValidTarget(GameObject obj)
        {
            // Проверяем коллайдер
            if (m_onlyWithColliders && obj.GetComponent<Collider>() == null)
                return false;
            
            // Проверяем игнорируемые теги
            if (m_ignoreTags != null)
            {
                foreach (string tag in m_ignoreTags)
                {
                    if (obj.CompareTag(tag))
                        return false;
                }
            }
        
            return true;
        }
    
        /// <summary>
        /// Найти ближайший объект среди всех объектов на сцене (без raycast)
        /// </summary>
        public GameObject FindNearestObjectInScene()
        {
            if (m_targetCamera == null)
                return null;
            
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = m_targetCamera.ScreenPointToRay(screenCenter);
        
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            GameObject nearestObject = null;
            float nearestDistance = float.MaxValue;
        
            foreach (GameObject obj in allObjects)
            {
                if (!IsValidTarget(obj))
                    continue;
                
                // Вычисляем расстояние от луча до объекта
                Vector3 closestPoint = GetClosestPointOnRay(ray, obj.transform.position);
                float distance = Vector3.Distance(closestPoint, obj.transform.position);
            
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = obj;
                }
            }
        
            return nearestObject;
        }
    
        /// <summary>
        /// Получить ближайшую точку на луче к заданной позиции
        /// </summary>
        private Vector3 GetClosestPointOnRay(Ray ray, Vector3 point)
        {
            Vector3 rayToPoint = point - ray.origin;
            float projection = Vector3.Dot(rayToPoint, ray.direction);
        
            // Ограничиваем проекцию положительными значениями
            projection = Mathf.Max(0f, projection);
        
            return ray.origin + ray.direction * projection;
        }
    
        /// <summary>
        /// Получить информацию о попадании в объект
        /// </summary>
        public RaycastHit? GetHitInfo(GameObject targetObject)
        {
            if (m_targetCamera == null || targetObject == null)
                return null;
            
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = m_targetCamera.ScreenPointToRay(screenCenter);
        
            RaycastHit hit;
            if (targetObject.GetComponent<Collider>().Raycast(ray, out hit, m_rayDistance))
            {
                return hit;
            }
        
            return null;
        }
    
        /// <summary>
        /// Установить новую камеру
        /// </summary>
        public void SetCamera(Camera newCamera)
        {
            m_targetCamera = newCamera;
        }
    
        /// <summary>
        /// Получить текущий луч
        /// </summary>
        public Ray GetCurrentRay()
        {
            return m_currentRay;
        }
    
        void OnDrawGizmos()
        {
            if (!m_showDebugRay || m_targetCamera == null)
                return;
            
            // Рисуем луч в Scene view
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = m_targetCamera.ScreenPointToRay(screenCenter);
        
            Gizmos.color = m_rayColor;
            Gizmos.DrawRay(ray.origin, ray.direction * m_rayDistance);
        
            // Рисуем сферу, если используем SphereCast
            if (m_useSphereCast)
            {
                Gizmos.color = m_rayColor * 0.3f;
                Gizmos.DrawWireSphere(ray.origin, m_sphereRadius);
                Gizmos.DrawWireSphere(ray.origin + ray.direction * m_rayDistance, m_sphereRadius);
            }
        
            // Рисуем точку попадания
            if (NearestObject != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(HitPoint, 0.2f);
            
                // Рисуем нормаль
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(HitPoint, HitNormal);
            }
        }
    }
}