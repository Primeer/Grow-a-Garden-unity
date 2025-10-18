using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Common.Misc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace Game.Scripts.Common
{
    public class MeshCombiner : MonoSingleton<MeshCombiner>
    {
        [SerializeField] private List<GameObject> m_parentObjects = new();
        [SerializeField] private bool m_isOnlyActiveObjects = true;
        
        [Button]
        public void ProcessCombineGroups()
        {
            foreach (GameObject parent in m_parentObjects)
            {
                if (parent == null) 
                    continue;

                // Получаем все ProBuilder меши в дочерних объектах
                ProBuilderMesh[] childMeshes = parent.GetComponentsInChildren<ProBuilderMesh>();

                List<ProBuilderMesh> meshesToCombine = childMeshes
                    .Where(m => !m_isOnlyActiveObjects || m.gameObject.activeInHierarchy)
                    .ToList();

                if (meshesToCombine.Count < 2)
                {
                    Debug.Log($"[MeshCombiner] Недостаточно мешей в {parent.name}");
                    continue;
                }

                CombineMeshGroup(meshesToCombine, parent.name);
            }
        }

        private void CombineMeshGroup(List<ProBuilderMesh> meshes, string groupName)
        {
            try
            {
                ProBuilderMesh target = meshes[0];
                List<ProBuilderMesh> result = CombineMeshes.Combine(meshes, target);

                // Удаляем остальные меши
                for (int i = 1; i < meshes.Count; i++)
                {
                    DestroyImmediate(meshes[i].gameObject);
                }

                target.ToMesh();
                target.Refresh();

                Debug.Log($"[MeshCombiner] Группа '{groupName}': объединено {meshes.Count} → {result.Count} меш(ей)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[MeshCombiner] Ошибка в группе '{groupName}': {e.Message}");
            }
        }
    }
}