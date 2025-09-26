namespace _SpellboundHollow.Scripts.Core
{
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public TimeManager TimeManager { get; private set; }
        public GrimoireManager GrimoireManager { get; private set; }
        public DialogueManager DialogueManager { get; private set; }
        public InventoryManager InventoryManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Ищем компоненты в СВОИХ дочерних объектах
            TimeManager = GetComponentInChildren<TimeManager>();
            GrimoireManager = GetComponentInChildren<GrimoireManager>();
            DialogueManager = GetComponentInChildren<DialogueManager>();
            InventoryManager = GetComponentInChildren<InventoryManager>();
        }
    }
}