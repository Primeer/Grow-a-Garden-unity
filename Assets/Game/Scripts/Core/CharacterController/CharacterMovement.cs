using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Core.CharacterController
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 2.0f;
        [SerializeField] private float jumpForce = 1.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float turnSpeed = 5.0f;
        [SerializeField] private float m_animationThreshold;
        [SerializeField] private float m_soundDelay = 0.3f;
   
        [Header("Component References")]
        [SerializeField] private UnityEngine.CharacterController characterController;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterAnimation m_animation;
        [SerializeField] private AudioSource m_audioSource;
    
        private InputAction m_movementInput;
        private InputAction m_jumpInput;
        private Vector3 m_verticalVelocity;
        private bool m_isActive = true;
        private bool m_isSoundStarted;

        public void SetPlayer(GameObject go)
        {
            characterController = go.GetComponent<UnityEngine.CharacterController>();
            m_animation = go.GetComponent<CharacterAnimation>();
            m_audioSource = go.GetComponent<AudioSource>();
        }
        
        public void SetActive(bool active)
        {
            m_isActive = active;
        }

        private void Start()
        {
            InitializeInputActions();
        }

        private void InitializeInputActions()
        {
            m_movementInput = InputSystem.actions.FindAction("Move");
            m_jumpInput = InputSystem.actions.FindAction("Jump");
        }

        private void Update()
        {
            if (!m_isActive || !characterController) 
                return;
            
            bool isGrounded = IsCharacterGrounded();
            Vector2 inputVector = GetMovementInput();
            Vector3 movementDirection = CalculateMovementDirection(inputVector);
            
            HandleGroundedState(isGrounded);
            HandleJump(isGrounded);
            ApplyGravity();
            MoveCharacter(movementDirection);
            RotateCharacter(movementDirection);
        }

        private bool IsCharacterGrounded()
        {
            return characterController && characterController.isGrounded;
        }

        private Vector2 GetMovementInput()
        {
            return m_movementInput.ReadValue<Vector2>();
        }

        private void HandleGroundedState(bool isGrounded)
        {
            if (isGrounded && m_verticalVelocity.y < 0)
            {
                m_verticalVelocity.y = 0f;
            }
        }

        private Vector3 CalculateMovementDirection(Vector2 inputVector)
        {
            Vector3 cameraForward = GetCameraForwardDirection();
            Vector3 cameraRight = GetCameraRightDirection();
            
            return cameraForward * inputVector.y + cameraRight * inputVector.x;
        }

        private Vector3 GetCameraForwardDirection()
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            return forward.normalized;
        }

        private Vector3 GetCameraRightDirection()
        {
            Vector3 right = cameraTransform.right;
            right.y = 0f;
            return right.normalized;
        }

        private void HandleJump(bool isGrounded)
        {
            if (m_jumpInput.IsPressed() && isGrounded)
            {
                m_verticalVelocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
                
                SoundsManager.Instance.PlayJump();
            }
        }

        private void ApplyGravity()
        {
            m_verticalVelocity.y += gravity * Time.deltaTime;
        }

        private void MoveCharacter(Vector3 movementDirection)
        {
            Vector3 finalMovement = (movementDirection * movementSpeed) + (m_verticalVelocity.y * Vector3.up);
            characterController?.Move(finalMovement * Time.deltaTime);

            if (movementDirection.sqrMagnitude > m_animationThreshold)
            {
                m_animation?.PlayMovement();

                if (IsCharacterGrounded())
                {
                    PlayFootstep();
                }
            }
            else
            {
                m_animation?.StopMovement();
            }
        }

        private void RotateCharacter(Vector3 movementDirection)
        {
            if (movementDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                characterController.transform.rotation = Quaternion.Slerp(characterController.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        private void PlayFootstep()
        {
            if (m_isSoundStarted)
                return;

            m_isSoundStarted = true;
            
            PlayAsync().Forget();
        }

        private async UniTaskVoid PlayAsync()
        {
            m_audioSource?.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(m_soundDelay));

            m_isSoundStarted = false;
        }
    }
}
