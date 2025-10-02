using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    /// <summary>
    /// Перечисление, определяющее глобальное состояние игры.
    /// Позволяет централизованно управлять тем, какие действия доступны в данный момент.
    /// </summary>
    public enum GameState
    {
        Gameplay,   // Обычный игровой процесс: игрок может двигаться и взаимодействовать.
        Dialogue,   // Игрок находится в диалоге или читает UI: движение заблокировано.
        Paused      // Игра на паузе (например, открыто меню).
    }

    /// <summary>
    /// Главный синглтон, управляющий всеми остальными менеджерами и общим состоянием игры.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- ВОССТАНОВЛЕННЫЕ ССЫЛКИ НА ВСЕ МЕНЕДЖЕРЫ ---
        // Эти свойства предоставляют глобальную точку доступа к ключевым системам игры.
        public DialogueManager DialogueManager { get; private set; }
        public TimeManager TimeManager { get; private set; }
        public InventoryManager InventoryManager { get; private set; }
        public GrimoireManager GrimoireManager { get; private set; }
        // Добавьте сюда другие менеджеры по мере их появления...
        
        /// <summary>
        /// Текущее состояние игры. Доступно только для чтения извне.
        /// </summary>
        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // --- КОРРЕКТНАЯ ИНИЦИАЛИЗАЦИЯ ВСЕХ МЕНЕДЖЕРОВ ---
            // Мы предполагаем, что все менеджеры являются дочерними объектами GameManager.
            // GetComponentInChildren найдет их, даже если они вложены.
            DialogueManager = GetComponentInChildren<DialogueManager>();
            TimeManager = GetComponentInChildren<TimeManager>();
            InventoryManager = GetComponentInChildren<InventoryManager>();
            GrimoireManager = GetComponentInChildren<GrimoireManager>();

            // Проверка, чтобы убедиться, что все менеджеры были найдены.
            // Это поможет избежать будущих ошибок NullReferenceException.
            if (DialogueManager == null) Debug.LogError("GameManager: DialogueManager не найден!");
            if (TimeManager == null) Debug.LogError("GameManager: TimeManager не найден!");
            if (InventoryManager == null) Debug.LogError("GameManager: InventoryManager не найден!");
            if (GrimoireManager == null) Debug.LogError("GameManager: GrimoireManager не найден!");
        }

        private void Start()
        {
            CurrentState = GameState.Gameplay;
        }

        /// <summary>
        /// Устанавливает новое состояние игры.
        /// </summary>
        /// <param name="newState">Новое состояние.</param>
        public void SetGameState(GameState newState)
        {
            CurrentState = newState;
        }
    }
}