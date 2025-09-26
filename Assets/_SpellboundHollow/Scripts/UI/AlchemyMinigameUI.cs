namespace _SpellboundHollow.Scripts.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class AlchemyMinigameUI : MonoBehaviour
    {
        [Header("Компоненты UI")]
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private Image successZoneImage;
        [SerializeField] private Image pulsingCircleImage;

        [Header("Настройки Мини-Игры")]
        [SerializeField] private float pulseSpeed = 1f;
        [Tooltip("Через сколько секунд игра автоматически завершится провалом, если игрок бездействует.")]
        [SerializeField] private float timeLimit = 5f; // <-- НОВОЕ ПОЛЕ: Таймер

        // --- Состояние ---
        private PlayerControls _playerControls;
        private bool _isGameActive = false;
        private float _successZoneStart;
        private float _successZoneEnd;
        private float _timer; // <-- НОВОЕ ПОЛЕ: Наш таймер

        public event System.Action<bool> OnMinigameFinished;

        private void Awake()
        {
            _playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _playerControls.Minigame.PrimaryAction.performed += context => CheckForSuccess();
        }

        private void OnDisable()
        {
            _playerControls.Minigame.PrimaryAction.performed -= context => CheckForSuccess();
        }

        private void Update()
        {
            if (!_isGameActive) return;

            // --- ОБНОВЛЕННАЯ ЛОГИКА ---
            // 1. Обновляем пульсацию
            float currentValue = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            pulsingCircleImage.fillAmount = currentValue;
            
            // 2. Обновляем таймер
            _timer -= Time.deltaTime;
            
            // 3. Если таймер истек, засчитываем провал
            if (_timer <= 0f)
            {
                Debug.Log("Время вышло! Провал.");
                FinishMinigame(false);
            }
        }

        public void StartMinigame(float successZoneCenter, float successZoneSize)
        {
            minigamePanel.SetActive(true);
            _playerControls.Minigame.Enable();
            
            // --- НОВОЕ: Сбрасываем таймер при старте ---
            _timer = timeLimit;

            // ... (остальной код метода StartMinigame без изменений) ...
            float sizeNormalized = successZoneSize / 100f;
            float centerNormalized = successZoneCenter / 100f;
            _successZoneStart = centerNormalized - (sizeNormalized / 2);
            _successZoneEnd = centerNormalized + (sizeNormalized / 2);
            successZoneImage.fillAmount = sizeNormalized;
            successZoneImage.transform.localEulerAngles = new Vector3(0, 0, -centerNormalized * 360f);

            _isGameActive = true;
        }

        // CheckForSuccess теперь вызывается без аргументов, но с проверкой
        private void CheckForSuccess()
        {
            if (!_isGameActive) return;
            
            float currentValue = pulsingCircleImage.fillAmount;
            bool success = currentValue >= _successZoneStart && currentValue <= _successZoneEnd;
            Debug.Log(success ? "УСПЕХ в мини-игре!" : "ПРОВАЛ в мини-игре!");

            FinishMinigame(success);
        }

        private void FinishMinigame(bool success)
        {
            // Убедимся, что игра точно неактивна, чтобы Update перестал работать
            if (!_isGameActive) return;
            
            _isGameActive = false;
            minigamePanel.SetActive(false);
            _playerControls.Minigame.Disable();
            
            OnMinigameFinished?.Invoke(success);
        }
    }
}