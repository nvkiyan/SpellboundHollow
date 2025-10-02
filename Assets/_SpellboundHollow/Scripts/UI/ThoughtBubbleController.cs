using System.Collections;
using TMPro;
using UnityEngine;

namespace _SpellboundHollow.Scripts.UI
{
    /// <summary>
    /// Управляет отображением UI-элемента "Облачко Мыслей".
    /// Является синглтоном для легкого доступа из любого места в коде.
    /// </summary>
    public class ThoughtBubbleController : MonoBehaviour
    {
        public static ThoughtBubbleController Instance { get; private set; }

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI thoughtText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Vector3 positionOffset;

        private Coroutine _activeCoroutine;
        private Camera _mainCamera;
        private Transform _targetToFollow;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Проверка на случай, если CanvasGroup не был назначен в инспекторе.
            // Это предотвратит ошибки и поможет в отладке.
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    Debug.LogError("ThoughtBubbleController: CanvasGroup component is missing!", this);
                    enabled = false; // Выключаем компонент, чтобы избежать дальнейших ошибок.
                }
            }
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;
            // Изначально облачко полностью невидимо и не блокирует клики.
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false; // Эта строка — ключевое исправление.
        }

        private void LateUpdate()
        {
            if (_targetToFollow != null)
            {
                transform.position = _mainCamera.WorldToScreenPoint(_targetToFollow.position + positionOffset);
            }
        }

        public void ShowThought(string text, Transform target, float duration)
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
            }
            _activeCoroutine = StartCoroutine(ShowThoughtRoutine(text, target, duration));
        }

        private IEnumerator ShowThoughtRoutine(string text, Transform target, float duration)
        {
            _targetToFollow = target;
            thoughtText.text = text;
            
            // Во время показа мы можем временно блокировать рейкасты, если это понадобится,
            // но для облачка мыслей это не нужно.
            // canvasGroup.blocksRaycasts = true;

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            yield return new WaitForSeconds(duration);

            timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            
            // canvasGroup.blocksRaycasts = false; // Возвращаем обратно, если включали
            _targetToFollow = null; 
            _activeCoroutine = null;
        }
    }
}