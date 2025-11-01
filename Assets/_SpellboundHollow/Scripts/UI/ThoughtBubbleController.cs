using System.Collections;
using TMPro;
using UnityEngine;

namespace _SpellboundHollow.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ThoughtBubbleController : MonoBehaviour
    {
        public static ThoughtBubbleController Instance { get; private set; }

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI thoughtText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Sound Settings")]
        [SerializeField] private AudioClip appearSound;

        private Coroutine _activeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }
        
        private void Start()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // --- ЛОГИКА СЛЕДОВАНИЯ ЗА ПЕРСОНАЖЕМ ПОЛНОСТЬЮ УДАЛЕНА ---
        // Метод LateUpdate() больше не нужен.

        /// <summary>
        /// Показывает панель с мыслью. Теперь она не требует позиции, так как закреплена на экране.
        /// </summary>
        /// <param name="text">Текст для отображения.</param>
        /// <param name="playerTransform">Этот параметр больше не используется, но оставлен для обратной совместимости, чтобы не ломать InteractionTrigger.</param>
        /// <param name="duration">Длительность отображения.</param>
        public void ShowThought(string text, Transform playerTransform, float duration)
        {
            Core.AudioManager.Instance.PlaySFX(appearSound);

            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
            _activeCoroutine = StartCoroutine(ShowThoughtRoutine(text, duration));
        }

        private IEnumerator ShowThoughtRoutine(string text, float duration)
        {
            thoughtText.text = text;
            
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
            
            _activeCoroutine = null;
        }
    }
}