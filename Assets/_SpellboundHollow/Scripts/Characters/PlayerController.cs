using _SpellboundHollow.Scripts.Core;
using _SpellboundHollow.Scripts.Gameplay;
using _SpellboundHollow.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace _SpellboundHollow.Scripts.Characters
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Interaction Settings")]
        [SerializeField] private LayerMask interactableLayerMask;

        [Header("Cursor Settings")]
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D interactableCursor;
        
        [Header("Footstep Settings")]
        [SerializeField] private AudioSource footstepAudioSource;
        [SerializeField] private float footstepRaycastDistance = 0.5f;
        [SerializeField] private LayerMask surfaceLayerMask;
        [SerializeField] private List<SurfaceSoundDataSO> surfaceSounds;
        
        private PlayerControls _playerControls;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Camera _mainCamera;
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.down;
        private IInteractable _currentInteractable;
        private bool _isPointerOverUI;
        private string _previousSurfaceTag;
        
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IdleHorizontal = Animator.StringToHash("IdleHorizontal");
        private static readonly int IdleVertical = Animator.StringToHash("IdleVertical");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _mainCamera = Camera.main;
            _playerControls = new PlayerControls();
            _playerControls.Player.Enable();
            _playerControls.Player.Move.performed += OnMovePerformed;
            _playerControls.Player.Move.canceled += OnMoveCanceled;
            _playerControls.Player.PrimaryAction.performed += OnPrimaryAction;
            _playerControls.Player.OpenGrimoire.performed += OnOpenGrimoirePerformed;
        }

        private void OnDestroy()
        {
            if (_playerControls == null) return;
            _playerControls.Player.Move.performed -= OnMovePerformed;
            _playerControls.Player.Move.canceled -= OnMoveCanceled;
            _playerControls.Player.PrimaryAction.performed -= OnPrimaryAction;
            _playerControls.Player.OpenGrimoire.performed -= OnOpenGrimoirePerformed;
            _playerControls.Player.Disable();
        }

        private void Start() 
        { 
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); 
        }

        private void Update() 
        {
            _isPointerOverUI = EventSystem.current.IsPointerOverGameObject(); 
            
            bool isGameplay = GameManager.Instance.CurrentState == GameState.Gameplay;
            
            if (isGameplay) 
            { 
                UpdateAnimator(); 
                HandleFootstepSounds(); 
            } 
            else 
            { 
                _rb.linearVelocity = Vector2.zero; 
                _animator.SetFloat(Speed, 0); 
                if (footstepAudioSource != null && footstepAudioSource.isPlaying) 
                {
                    footstepAudioSource.Stop(); 
                }
            } 
            
            HandleCursor(); 
        }
        
        private void FixedUpdate() 
        { 
            if (GameManager.Instance.CurrentState == GameState.Gameplay) 
            { 
                HandleMovement(); 
            } 
        }
        
        private void HandleMovement() 
        { 
            _rb.linearVelocity = _moveInput * moveSpeed; 
        }
        
        private void UpdateAnimator() 
        { 
            _animator.SetFloat(Speed, _moveInput.sqrMagnitude); 
            if (_moveInput.sqrMagnitude > 0.01f) 
            { 
                _lastMoveDirection = _moveInput.normalized; 
                _animator.SetFloat(Horizontal, _moveInput.x); 
                _animator.SetFloat(Vertical, _moveInput.y); 
            } 
            else 
            { 
                _animator.SetFloat(IdleHorizontal, _lastMoveDirection.x); 
                _animator.SetFloat(IdleVertical, _lastMoveDirection.y); 
            } 
        }
        
        private void HandleFootstepSounds() 
        { 
            if (footstepAudioSource == null) return; 
            
            bool isMovingNow = _moveInput.sqrMagnitude > 0.01f; 
            
            if (isMovingNow) 
            { 
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, footstepRaycastDistance, surfaceLayerMask); 
                string currentSurfaceTag = hit.collider != null ? hit.collider.tag : null; 
                
                if (currentSurfaceTag != _previousSurfaceTag) 
                { 
                    _previousSurfaceTag = currentSurfaceTag; 
                    footstepAudioSource.Stop(); 
                    if (currentSurfaceTag != null) 
                    { 
                        SurfaceSoundDataSO soundData = surfaceSounds.FirstOrDefault(s => s.surfaceTag == currentSurfaceTag); 
                        if (soundData != null && soundData.footstepSound != null) 
                        { 
                            footstepAudioSource.clip = soundData.footstepSound; 
                            footstepAudioSource.Play(); 
                        } 
                    } 
                } 
                else if (!footstepAudioSource.isPlaying && currentSurfaceTag != null) 
                { 
                    footstepAudioSource.Play(); 
                } 
            } 
            else 
            { 
                if (footstepAudioSource.isPlaying) 
                { 
                    footstepAudioSource.Stop(); 
                } 
                _previousSurfaceTag = null; 
            } 
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context) 
        { 
            _moveInput = context.ReadValue<Vector2>(); 
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context) 
        { 
            _moveInput = Vector2.zero; 
        }
        
        private void HandleCursor() 
        { 
            if (_isPointerOverUI) 
            { 
                if (_currentInteractable != null) 
                { 
                    Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); 
                    _currentInteractable = null; 
                } 
                return; 
            } 
            
            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, Mathf.Infinity, interactableLayerMask); 
            
            if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactable)) 
            { 
                if (interactable != _currentInteractable) 
                { 
                    Cursor.SetCursor(interactableCursor, Vector2.zero, CursorMode.Auto); 
                    _currentInteractable = interactable; 
                } 
            } 
            else 
            { 
                if (_currentInteractable != null) 
                { 
                    Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); 
                    _currentInteractable = null; 
                } 
            } 
        }
        
        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            // ПРАВИЛЬНЫЙ ПОРЯДОК ПРОВЕРОК:
            
            // 1. Проверяем состояние диалога ПЕРЕД проверкой UI.
            //    Если идет диалог, PlayerController не должен ничего делать.
            //    Клик будет обработан в DialogueManager.Update().
            if (GameManager.Instance.CurrentState == GameState.Dialogue)
            {
                return;
            }
            
            // 2. Если игра НЕ в диалоге, тогда проверяем, не над UI ли курсор.
            //    Это нужно для других элементов интерфейса (инвентарь, меню паузы).
            if (_isPointerOverUI)
            {
                return;
            }

            // 3. Если все проверки пройдены, значит, мы в состоянии Gameplay
            //    и можем взаимодействовать с миром.
            HandleInteraction();
        }

        private void HandleInteraction()
        {
            Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, interactableLayerMask);
            
            if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactableObject))
            {
                if (Vector2.Distance(transform.position, hit.collider.transform.position) <= interactableObject.InteractionRadius)
                {
                    interactableObject.Interact(transform);
                }
            }
        }
        
        private void OnOpenGrimoirePerformed(InputAction.CallbackContext context) 
        { 
            if (GrimoireUI.Instance != null) 
            {
                GrimoireUI.Instance.Toggle(); 
            } 
        }
        
        private void OnDrawGizmos() 
        { 
            if (Application.isPlaying && _mainCamera != null) 
            { 
                RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, Mathf.Infinity, interactableLayerMask); 
                if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactable)) 
                { 
                    Gizmos.color = new Color(1, 0.92f, 0.016f, 0.25f); 
                    Gizmos.DrawSphere(transform.position, interactable.InteractionRadius); 
                } 
            } 
        }
    }
}