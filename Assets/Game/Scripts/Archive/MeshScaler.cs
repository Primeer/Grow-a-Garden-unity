using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Common.Utilities;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.ProBuilder;
using Math = UnityEngine.ProBuilder.Math;

namespace Game.Scripts.Archive
{
    public class MeshScaler : MonoBehaviour
    {
        [Header(Constants.SETTINGS)]
        [SerializeField] private int[] m_faceIndexes;
       
        [Header(Constants.REFERENCES)]
        [SerializeField] private ProBuilderMesh m_mesh;
        
        private MeshFilter m_meshFilter;
        private Vertex[] m_vertices;
        private AnimationData[] m_animationData;
        
        private MotionHandle m_handle;
        
        private void Awake()
        {
            m_meshFilter = m_mesh.GetComponent<MeshFilter>();
            m_vertices = m_mesh.GetVertices();

            FillAnimationData();
        }

        [Button]
        public void PlayAnimationDebug(float duration)
        {
            m_meshFilter = m_mesh.GetComponent<MeshFilter>();
            m_vertices = m_mesh.GetVertices();

            FillAnimationData();
            SetInitialFlatState();
            
            m_handle = LMotion.Create(0f, 1f, duration)
                .WithScheduler(MotionScheduler.FixedUpdate)
                .Bind(ChangeMeshSize);
        }

        public void PlayAnimation(float duration)
        {
            SetInitialFlatState();
            
            m_handle = LMotion.Create(0f, 1f, duration)
                .WithScheduler(MotionScheduler.FixedUpdate)
                .Bind(ChangeMeshSize);
        }

        private void ChangeMeshSize(float t)
        {
            foreach(var data in m_animationData)
            {
                for(var i = 0; i < data.CoincidentVertices.Count; i++)
                {
                    int vertexIndex = data.CoincidentVertices[i];
                    var startPos = data.InitialVerticesPositions[i];
                    var targetPos = data.TargetVerticesPositions[i];
                    
                    // Интерполируем между начальной (плоской) и целевой позицией
                    m_vertices[vertexIndex].position = Vector3.Lerp(startPos, targetPos, t);
                }
            }
            
            m_mesh.SetVertices(m_vertices);
            m_mesh.ToMesh();
            m_mesh.Refresh();
            
            var umesh = m_meshFilter.sharedMesh;
            MeshUtility.CollapseSharedVertices(umesh);
        }

        private void SetInitialFlatState()
        {
            foreach(var data in m_animationData)
            {
                for(var i = 0; i < data.CoincidentVertices.Count; i++)
                {
                    int vertexIndex = data.CoincidentVertices[i];
                    m_vertices[vertexIndex].position = data.InitialVerticesPositions[i];
                }
            }
            
            m_mesh.SetVertices(m_vertices);
            m_mesh.ToMesh();
            m_mesh.Refresh();
        }

        private Vector3 CalculateOppositeFaceCenter(List<Vector3> vertices)
        {
            // Находим центр всех вершин, не входящих в анимируемую грань
            var allVertices = m_vertices.Select(v => v.position).ToList();
            var animatedVertices = vertices.ToHashSet();
            
            var oppositeCenter = Vector3.zero;
            var count = 0;
            
            foreach(var vertex in allVertices)
            {
                if (!animatedVertices.Contains(vertex))
                {
                    oppositeCenter += vertex;
                    count++;
                }
            }
            
            return count > 0 ? oppositeCenter / count : Vector3.zero;
        }

        private Vector3 ProjectVertexToOpposite(Vector3 vertex, Vector3 oppositeCenter, Vector3 normal)
        {
            // Проецируем вершину на плоскость, определяемую противоположным центром и нормалью
            var toVertex = vertex - oppositeCenter;
            float projectionDistance = Vector3.Dot(toVertex, normal);
            
            // Возвращаем позицию близко к противоположной стороне
            return vertex - normal * (projectionDistance * 0.95f); // 0.95f чтобы избежать полного наложения
        }

        private void FillAnimationData()
        {
            m_animationData = new AnimationData[m_faceIndexes.Length];
            
            for (var i = 0; i < m_animationData.Length; i++)
            {
                var face = m_mesh.faces[m_faceIndexes[i]];  
                var normal = Math.Normal(m_mesh, face);
                var coincidentVertices = m_mesh.GetCoincidentVertices(face.indexes);
                var targetVerticesPositions = coincidentVertices.Select(idx => m_vertices[idx].position).ToList();

                m_animationData[i] = new AnimationData()
                {
                    Normal = normal,
                    CoincidentVertices = coincidentVertices,
                    TargetVerticesPositions = targetVerticesPositions,
                    InitialVerticesPositions = CalculateInitialVerticesPositions(coincidentVertices, targetVerticesPositions, normal),
                };
            }
        }

        private List<Vector3> CalculateInitialVerticesPositions(List<int> coincidentVertices, List<Vector3> targetVerticesPositions, Vector3 normal)
        {
            var result = new List<Vector3>();
            
            // Находим противоположную грань для расчета плоской позиции
            var oppositeCenter = CalculateOppositeFaceCenter(targetVerticesPositions);
                
            for(var i = 0; i < coincidentVertices.Count; i++)
            {
                var originalPos = targetVerticesPositions[i];
                    
                // Проецируем вершину на плоскость противоположной грани
                var flatPos = ProjectVertexToOpposite(originalPos, oppositeCenter, normal);
                    
                result.Add(flatPos);
            }

            return result;
        }
        
        private class AnimationData
        {
            public Vector3 Normal;
            public List<int> CoincidentVertices;
            public List<Vector3> TargetVerticesPositions; // Финальные позиции (оригинальные)
            public List<Vector3> InitialVerticesPositions; // Начальные позиции (плоские)
        }

        private void OnDestroy()
        {
            if (m_handle.IsActive())
                m_handle.Cancel();
        }
    }
}