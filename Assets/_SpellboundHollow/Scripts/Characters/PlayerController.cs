using Unity.VisualScripting;

namespace _SpellboundHollow.Scripts.Characters
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Core;
    using Gameplay;
    using UI;

    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        // --- ID для Animator ---
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        
        [Header("Настройки Движения")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Настройки Взаимодействия")]
        [SerializeField] private float interactionDistance = 1.5f;

        // --- Компоненты и Внутреннее Состояние ---
        private Rigidbody2D _rb;
        private Animator _animator;
        private PlayerControls _playerControls;
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.down;
        private bool _canMove = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _playerControls.Player.Enable();
            _playerControls.Player.Move.performed += OnMovePerformed;
            _playerControls.Player.Move.canceled += OnMoveCanceled;
            _playerControls.Player.Interact.performed += OnInteractPerformed;
            _playerControls.Player.OpenGrimoire.performed += OnOpenGrimoirePerformed;
        }

        private void OnDisable()
        {
            _playerControls.Player.Disable();
            // ... (все отписки -=) ...
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                _rb.MovePosition(_rb.position + _moveInput.normalized * (moveSpeed * Time.fixedDeltaTime));
            }
            else
            {
                // Используем новый, правильный способ для остановки
                _rb.linearVelocity = Vector2.zero;
            }
        
            HandleAnimation();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // --- ИСПРАВЛЕНО: Обращаемся к DialogueManager через GameManager ---
            if (GameManager.Instance.DialogueManager != null && GameManager.Instance.DialogueManager.IsDialogueActive())
            {
                GameManager.Instance.DialogueManager.DisplayNextLine();
                return;
            }
            
            int playerLayer = LayerMask.NameToLayer("Player");
            LayerMask layerMask = ~(1 << playerLayer);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _lastMoveDirection, interactionDistance, layerMask);
            
            if (hit.collider == null) return;

            if (hit.collider.TryGetComponent(out DialogueTrigger dialogueTrigger)) { dialogueTrigger.TriggerDialogue(); }
            else if (hit.collider.TryGetComponent(out AlchemyStation alchemyStation)) { alchemyStation.Interact(); }
            else if (hit.collider.TryGetComponent(out CollectibleItem collectible)) { collectible.Collect(); }
        }
        
        private void OnOpenGrimoirePerformed(InputAction.CallbackContext context)
        {
            // --- ИСПРАВЛЕНО: Обращаемся к GrimoireUI через GameManager (или Instance, если UI - синглтон) ---
            // Для UI синглтоны допустимы, так как они часто уникальны. Оставим Instance для UI.
            if (GrimoireUI.Instance != null) GrimoireUI.Instance.Toggle();
        }
        
        public void DisableMovement() => _canMove = false;
        public void EnableMovement() => _canMove = true;
        
        private void HandleAnimation()
        {
            bool isMoving = _canMove && _moveInput != Vector2.zero;
            _animator.SetBool(IsMoving, isMoving);

            if (isMoving)
            {
                _lastMoveDirection = _moveInput.normalized;
            }

            _animator.SetFloat(MoveX, _lastMoveDirection.x);
            _animator.SetFloat(MoveY, _lastMoveDirection.y);
        }
    }
}