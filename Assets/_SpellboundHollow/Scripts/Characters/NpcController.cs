using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace _SpellboundHollow.Scripts.Characters
{
    [RequireComponent(typeof(Animator))]
    public class NpcController : MonoBehaviour
    {
        public enum BehaviorType { Stationary, Patrol }

        [Header("Behavior")]
        [SerializeField] private BehaviorType behavior = BehaviorType.Patrol;
        
        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float waitTime = 1f;
        
        private int _currentPointIndex = 0;
        private bool _isWaiting;
        private float _waitTimer;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>(); 
        }

        private void Update()
        {
            if (behavior == BehaviorType.Patrol) HandlePatrolBehavior();
            else UpdateAnimator(Vector2.zero);
        }

        private void HandlePatrolBehavior()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= waitTime)
                {
                    _isWaiting = false;
                    _currentPointIndex = (_currentPointIndex + 1) % patrolPoints.Length;
                }
                UpdateAnimator(Vector2.zero);
                return;
            }

            Transform targetPoint = patrolPoints[_currentPointIndex];
            Vector2 direction = (targetPoint.position - transform.position).normalized;

            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            UpdateAnimator(direction);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                _isWaiting = true;
                _waitTimer = 0f;
            }
        }

        private void UpdateAnimator(Vector2 direction)
        {
            _animator.SetFloat(Speed, direction.magnitude);
            _animator.SetFloat(Horizontal, direction.x);
            _animator.SetFloat(Vertical, direction.y);
            
            if (Mathf.Abs(direction.x) > 0.5f)
            {
                _spriteRenderer.flipX = direction.x < 0;
            }
        }
    }
}