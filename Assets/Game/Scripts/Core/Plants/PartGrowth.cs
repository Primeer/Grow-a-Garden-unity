using LitMotion;
using LitMotion.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core.Plants
{
    public class PartGrowth : MonoBehaviour
    {
        [SerializeField] private Vector3 m_startPosition = Vector3.zero;

        private MotionHandle m_handle;

        [Button]
        private void FillStartPosition()
        {
            m_startPosition = transform.localPosition;
        }

        [Button]
        public void PlayAnimation(float duration = 2f)
        {
            m_handle = CreateAnimation(Vector3.zero, m_startPosition, duration).Run().AddTo(gameObject);
        }

        public void PlayAnimationPartly(float duration, float skippedTime)
        {
            var remainTime = duration - skippedTime;
            var progress = skippedTime / duration;
            var startScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            var startPosition = Vector3.Lerp(m_startPosition, transform.localPosition, progress);
            
            m_handle = CreateAnimation(startScale, startPosition, remainTime).Run().AddTo(gameObject);
        }

        private MotionSequenceBuilder CreateAnimation(Vector3 startScale, Vector3 startPosition, float time)
        {
            return LSequence.Create()
                .Append(LMotion.Create(startScale, Vector3.one, time).BindToLocalScale(transform))
                .Join(LMotion.Create(startPosition, transform.localPosition, time).BindToLocalPosition(transform));
        }

        private void OnDestroy()
        {
            m_handle.TryCancel();
        }
    }
}