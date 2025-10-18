using Game.Scripts.Common;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public class MeshCombinePreprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"[MeshCombiner] Запуск для {report.summary.platform}");
            
            MeshCombiner.Instance.ProcessCombineGroups();
            Object.DestroyImmediate(MeshCombiner.Instance.gameObject);
        }
    }
}
