using System;
using Game.Scripts.Common.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    [RequireComponent(typeof(Plant))]
    public class Growth : MonoBehaviour
    {
        [Header(Constants.SETTINGS)]
        [SerializeField] private float m_growthDuration;
        [SerializeField] private float m_stepDuration;

        [Header(Constants.REFERENCES)]
        [SerializeField] private GameObject[] m_steps;

        public bool IsGrowthCompleted { get; private set; }
        public int CurrentStep { get; private set; }
        public float StepTime { get; private set; }
        public float TotalTime { get; private set; }

        public float GrowthDuration => m_growthDuration;
        public int StepsCount => m_steps.Length;
        [ShowInInspector]
        public float GrowthProgress => TotalTime / GrowthDuration;
        public float StepDuration => m_stepDuration;
        public event Action GrowthCompleted;
        
        private void Update()
        {
            if (IsGrowthCompleted)
                return;

            StepTime += Time.deltaTime;
            TotalTime += Time.deltaTime;

            if (StepTime < StepDuration) 
                return;
            
            if (CurrentStep == StepsCount)
            {
                IsGrowthCompleted = true;
                GrowthCompleted?.Invoke();
                return;
            }

            StepTime = 0;
            SetNextStep(++CurrentStep);
        }

        private void SetNextStep(int step)
        {
            var stepGo = m_steps[step - 1];
            stepGo.SetActive(true);
            stepGo.GetComponentsInChildren<PartGrowth>().ForEach(p => 
                p.PlayAnimation(StepDuration));
        }
        
        public void SetProgressTime(float progressTime)
        {
            if (progressTime >= GrowthDuration)
            {
                IsGrowthCompleted = true;
                TotalTime = GrowthDuration;
                CurrentStep = StepsCount;
                StepTime = StepDuration;
                return;
            }
            
            TotalTime = progressTime;
            CurrentStep = Mathf.FloorToInt(progressTime / StepDuration);
            StepTime = progressTime - CurrentStep * StepDuration;
            
            for (var i = 0; i < m_steps.Length; i++)
            {
                m_steps[i].SetActive(i < CurrentStep);
            }

            if (CurrentStep > 0)
            {
                m_steps[CurrentStep - 1].GetComponentsInChildren<PartGrowth>().ForEach(p => 
                    p.PlayAnimationPartly(StepDuration, StepTime));
            }
        }
        
        #region Editor
        
        [Button]
        private void FillSteps()
        {
            m_steps = new GameObject[transform.childCount];

            var i = 0;
            foreach (Transform child in transform)
            {
                m_steps[i++] = child.gameObject;
            }
        }

        [Button]
        private void SetStepDebug(int stepIndex)
        {
            for (var i = 0; i < m_steps.Length; i++)
            {
                m_steps[i].SetActive(i < stepIndex);
            }
        }

        private void OnValidate()
        {
            m_stepDuration = GrowthDuration / (StepsCount + 1);
        }

        #endregion
    }
}