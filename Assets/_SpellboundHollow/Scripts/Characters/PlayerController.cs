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

        private void Start()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

        private void OnDestroy()
        {
            _playerControls.Player.Move.performed -= OnMovePerformed;
            _playerControls.Player.Move.canceled -= OnMoveCanceled;
            _playerControls.Player.PrimaryAction.performed -= OnPrimaryAction;
            _playerControls.Player.OpenGrimoire.performed -= OnOpenGrimoirePerformed;
        }

        private void Update()
        {
            _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
            
            if (GameManager.Instance.CurrentState == GameState.Gameplay)
            {
                UpdateAnimator();
                HandleFootstepSounds();
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
                _animator.SetFloat(Speed, 0);
                if (footstepAudioSource != null && footstepAudioSource.isPlaying) footstepAudioSource.Stop();
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

        private void HandleCursor()
        {
            if (_isPointerOverUI)
            {
                if (_currentInteractable != null) { Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); _currentInteractable = null; }
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != _currentInteractable) { Cursor.SetCursor(interactableCursor, Vector2.zero, CursorMode.Auto); _currentInteractable = interactable; }
            }
            else
            {
                if (_currentInteractable != null) { Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto); _currentInteractable = null; }
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
        
        // ЗАМЕНИТЕ ЭТОТ МЕТОД ЦЕЛИКОМ
    private void HandleFootstepSounds()
    {
        if (footstepAudioSource == null) return;

        bool isMovingNow = _moveInput.sqrMagnitude > 0.01f;

        if (isMovingNow)
        {
            // Debug.Log("Player IS MOVING."); // Раскомментируйте, если нужно проверить самое начало
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, footstepRaycastDistance, surfaceLayerMask);

            if (hit.collider != null)
            {
                string currentSurfaceTag = hit.collider.tag;
                Debug.Log($"[Footsteps] Raycast HIT: '{hit.collider.name}', Tag: '{currentSurfaceTag}'");
                
                if (currentSurfaceTag != _previousSurfaceTag)
                {
                    Debug.Log($"[Footsteps] Surface CHANGED to: '{currentSurfaceTag}'. Searching for sound...");
                    _previousSurfaceTag = currentSurfaceTag;
                    footstepAudioSource.Stop();

                    if (currentSurfaceTag != null)
                    {
                        SurfaceSoundDataSO soundData = surfaceSounds.FirstOrDefault(s => s.surfaceTag == currentSurfaceTag);
                        if (soundData != null && soundData.footstepSound != null)
                        {
                            Debug.Log($"[Footsteps] SUCCESS: Sound found for '{currentSurfaceTag}'. Playing clip '{soundData.footstepSound.name}'.");
                            footstepAudioSource.clip = soundData.footstepSound;
                            footstepAudioSource.Play();
                        }
                        else
                        {
                            Debug.LogWarning($"[Footsteps] Sound data NOT FOUND for tag '{currentSurfaceTag}'.");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("[Footsteps] Raycast hit NOTHING. Player is in the air?");
                if (footstepAudioSource.isPlaying) footstepAudioSource.Stop();
                _previousSurfaceTag = null;
            }
        }
        else // Если игрок остановился
        {
            if (footstepAudioSource.isPlaying)
            {
                Debug.Log("[Footsteps] Player STOPPED. Stopping sound.");
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

        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.CurrentState != GameState.Gameplay) return;
            if (_isPointerOverUI) return;

            Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider == null) return;
            
            if (hit.collider.TryGetComponent(out IInteractable interactableObject))
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
            if (Application.isPlaying)
            {
                RaycastHit2D hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
                if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    Gizmos.color = new Color(1, 0.92f, 0.016f, 0.25f);
                    Gizmos.DrawSphere(transform.position, interactable.InteractionRadius);
                }
            }
        }
    }
}