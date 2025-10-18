using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core.CharacterController
{
    public class CinemachineInputController : InputAxisControllerBase<CinemachineInputController.Reader>
    {
        private void Update()
        {
            if (Application.isPlaying)
            {
                UpdateControllers();
            }
        }

        [Serializable]
        public sealed class Reader : IInputAxisReader
        {
            public float TouchGain = 1;
            public float MouseGain = 1;
            public float WebGLCorrection = 0.125f; //не удалять
            
            public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                if (context is not CinemachineInputController controller) 
                    return 0f;

                if (TryGetTouchValue(hint, out float touchValue))
                {
                    return touchValue * TouchGain / Time.unscaledDeltaTime;
                }

                if (TryGetMouseValue(hint, out float mouseValue))
                {
                    return mouseValue * MouseGain / Time.unscaledDeltaTime;
                }

                return 0f;
            }
            
            private bool TryGetTouchValue(IInputAxisOwner.AxisDescriptor.Hints hint, out float result)
            {
                if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0)
                {
                    result = 0f;
                    return false;
                }
        
                // Проверяем первое касание
                if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue()))
                {
                    // Если первое касание на UI, проверяем второе
                    if (Touchscreen.current.touches.Count > 1 && Touchscreen.current.touches[1].isInProgress)
                    {
                        if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[1].touchId.ReadValue()))
                        {
                            result = 0f;
                            return false;
                        }
                
                        // Используем второе касание для управления камерой
                        Vector2 touchDelta = Touchscreen.current.touches[1].delta.ReadValue();
                        result = hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? touchDelta.y : touchDelta.x;
                        return true;
                    }
                }
                else
                {
                    // Используем первое касание для управления камерой
                    if (Touchscreen.current.touches[0].isInProgress)
                    {
                        Vector2 touchDelta = Touchscreen.current.touches[0].delta.ReadValue();
                        result = hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? touchDelta.y : touchDelta.x;
                        return true;
                    }
                }

                result = 0f;
                return false;
            }

            private bool TryGetMouseValue(IInputAxisOwner.AxisDescriptor.Hints hint, out float result)
            {
                if (Mouse.current == null)
                {
                    result = 0f;
                    return false;
                }

                if (Mouse.current.rightButton.IsPressed() == false)
                {
                    result = 0f;
                    return true;
                }

                var pointerDelta = Mouse.current.delta.value;
                result = hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? pointerDelta.y : pointerDelta.x;

#if UNITY_WEBGL && !UNITY_EDITOR
                    result = result * WebGLCorrection;
#endif

                return true;
            }
        }
    }
}