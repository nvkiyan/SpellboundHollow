using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    public enum GameState { Gameplay, Dialogue, Paused }

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    
                    if (_instance == null)
                    {
                        var gameManagerPrefab = Resources.Load<GameManager>("GameManager");
                        if (gameManagerPrefab != null)
                        {
                            _instance = Instantiate(gameManagerPrefab);
                        }
                        else
                        {
                            Debug.LogError("GameManager prefab not found in Resources folder! Please ensure it exists at 'Assets/Resources/GameManager.prefab'");
                        }
                    }
                }
                return _instance;
            }
        }

        [Header("System Managers")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private GrimoireManager grimoireManager;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;
        [SerializeField] private AudioManager audioManager;
        
        // --- ДОБАВЛЕНО ---
        [Header("UI References")]
        [Tooltip("Ссылка на Transform основного 'бессмертного' Canvas. Нужна для создания UI-элементов.")]
        [SerializeField] private Transform mainCanvasTransform;
        // --- КОНЕЦ ДОБАВЛЕНИЯ ---
        
        public DialogueManager DialogueManager => dialogueManager;
        public TimeManager TimeManager => timeManager;
        public InventoryManager InventoryManager => inventoryManager;
        public GrimoireManager GrimoireManager => grimoireManager;
        public SceneTransitionManager SceneTransitionManager => sceneTransitionManager;
        public AudioManager AudioManager => audioManager;
        
        // --- ДОБАВЛЕНО ---
        public Transform MainCanvasTransform => mainCanvasTransform;
        // --- КОНЕЦ ДОБАВЛЕНИЯ ---
        
        public GameState CurrentState { get; private set; }
        
        private void Awake()
        {
            if (_instance != null && _instance != this) 
            {
                // --- ДИАГНОСТИЧЕСКАЯ СТРОКА ---
                Debug.Break(); // Эта команда немедленно поставит редактор на паузу.
        
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
    
            CurrentState = GameState.Gameplay;
        }
        
        public void SetGameState(GameState newState) 
        { 
            CurrentState = newState; 
        }
    }
}