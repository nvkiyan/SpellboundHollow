using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    public enum GameState { Gameplay, Dialogue, Paused }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private GrimoireManager grimoireManager;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;
        [SerializeField] private AudioManager audioManager; // <-- ДОБАВЛЕНО
        
        public DialogueManager DialogueManager => dialogueManager;
        public TimeManager TimeManager => timeManager;
        public InventoryManager InventoryManager => inventoryManager;
        public GrimoireManager GrimoireManager => grimoireManager;
        public SceneTransitionManager SceneTransitionManager => sceneTransitionManager;
        public AudioManager AudioManager => audioManager; // <-- ДОБАВЛЕНО
        
        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance != null) 
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() 
        { 
            CurrentState = GameState.Gameplay; 
        }
        
        public void SetGameState(GameState newState) 
        { 
            CurrentState = newState; 
        }
    }
}