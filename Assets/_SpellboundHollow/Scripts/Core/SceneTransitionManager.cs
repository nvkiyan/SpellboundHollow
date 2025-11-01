using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using _SpellboundHollow.Scripts.Characters;

namespace _SpellboundHollow.Scripts.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;
        
        private bool _isTransitioning;

        private void Awake()
        {
            // Этот скрипт живет на "бессмертном" GameManager,
            // поэтому ему не нужен собственный DontDestroyOnLoad.
            if (Instance != null)
            {
                // Этого не должно происходить при правильной настройке, но это защита.
                Destroy(this); 
                return;
            }
            Instance = this;
        }

        public void TransitionToScene(string sceneName, Vector3 targetPosition)
        {
            if (_isTransitioning) return;
            StartCoroutine(TransitionRoutine(sceneName, targetPosition));
        }

        private IEnumerator TransitionRoutine(string sceneName, Vector3 targetPosition)
        {
            _isTransitioning = true;
            GameManager.Instance.SetGameState(GameState.Paused);

            yield return StartCoroutine(Fade(1f));

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone) { yield return null; }

            // Ждем один кадр, чтобы все объекты на новой сцене успели инициализироваться.
            yield return null; 

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.GetComponent<Rigidbody2D>().simulated = false;
                player.transform.position = targetPosition;
                player.GetComponent<Rigidbody2D>().simulated = true;
            }
            else 
            { 
                Debug.LogError("SceneTransitionManager: PlayerController не найден на новой сцене!"); 
            }

            yield return StartCoroutine(Fade(0f));
            
            GameManager.Instance.SetGameState(GameState.Gameplay);
            _isTransitioning = false;
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