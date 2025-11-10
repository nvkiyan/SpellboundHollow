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
        
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private GrimoireManager grimoireManager;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;
        [SerializeField] private AudioManager audioManager;
        
        public DialogueManager DialogueManager => dialogueManager;
        public TimeManager TimeManager => timeManager;
        public InventoryManager InventoryManager => inventoryManager;
        public GrimoireManager GrimoireManager => grimoireManager;
        public SceneTransitionManager SceneTransitionManager => sceneTransitionManager;
        public AudioManager AudioManager => audioManager;
        
        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this) 
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Устанавливаем начальное состояние здесь, в Awake.
            // Awake() гарантированно выполнится до любого Start().
            // Это предотвращает "гонку состояний", когда SceneEntryTrigger.Start()
            // устанавливает состояние Dialogue, а GameManager.Start() его перезаписывает.
            CurrentState = GameState.Gameplay;
        }

        // Метод Start() больше не нужен для установки состояния, так как это сделано в Awake.
        // private void Start() { }
        
        public void SetGameState(GameState newState) 
        { 
            CurrentState = newState; 
        }
    }
}