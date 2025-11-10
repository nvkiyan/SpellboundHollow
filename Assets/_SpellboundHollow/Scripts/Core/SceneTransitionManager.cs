using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using _SpellboundHollow.Scripts.Characters;
using _SpellboundHollow.Scripts.Gameplay;

namespace _SpellboundHollow.Scripts.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;
        
        private bool _isTransitioning;
        private static string _targetEntryPointId;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this); 
                return;
            }
            Instance = this;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void TransitionToScene(string sceneName, string entryPointId)
        {
            if (_isTransitioning) return;
            _targetEntryPointId = entryPointId;
            StartCoroutine(TransitionRoutine(sceneName));
        }

        private IEnumerator TransitionRoutine(string sceneName)
        {
            _isTransitioning = true;
            GameManager.Instance.SetGameState(GameState.Paused);

            yield return StartCoroutine(Fade(1f));

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone) { yield return null; }

            yield return StartCoroutine(Fade(0f));
            
            GameManager.Instance.SetGameState(GameState.Gameplay);
            _isTransitioning = false;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (string.IsNullOrEmpty(_targetEntryPointId)) return;
            
            // Используем современный, более производительный метод FindObjectsByType.
            // FindObjectsSortMode.None указывает, что нам не важен порядок, что делает поиск еще быстрее.
            SceneEntryTrigger[] entryPoints = FindObjectsByType<SceneEntryTrigger>(FindObjectsSortMode.None);
            SceneEntryTrigger targetPoint = entryPoints.FirstOrDefault(p => p.EntryId == _targetEntryPointId);
            
            if (targetPoint == null)
            {
                Debug.LogError($"Не удалось найти точку входа с ID: '{_targetEntryPointId}' в сцене '{scene.name}'!");
                _targetEntryPointId = null;
                return;
            }
            
            // Используем современный метод FindFirstObjectByType, который заменяет устаревший FindObjectOfType.
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                var playerRb = player.GetComponent<Rigidbody2D>();
                playerRb.simulated = false;
                player.transform.position = targetPoint.transform.position;
                playerRb.simulated = true;
            }
            else 
            { 
                Debug.LogError("SceneTransitionManager: PlayerController не найден на новой сцене!"); 
            }
            
            // Сбрасываем ID, чтобы он не использовался при следующей загрузке сцены.
            _targetEntryPointId = null;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            float time = 0;
            float startAlpha = fadeCanvasGroup.alpha;
            while (time < fadeDuration)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                time += Time.deltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = targetAlpha;
        }
    }
}