using _SpellboundHollow.Scripts.Core;
using _SpellboundHollow.Scripts.UI;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    public class InteractionTrigger : MonoBehaviour, IInteractable
    {
        public enum InteractionType
        {
            DoNothing,
            DialogueSo,
            InlineDialogue,
            Thought,
            RepeatInitial,
            ShowUIPrefab
        }

        [Header("Primary Interaction")]
        [SerializeField] private InteractionType initialAction = InteractionType.Thought;
        [SerializeField] private DialogueDataSO initialDialogueSo;
        [SerializeField] private DialogueLine[] initialInlineDialogue;
        [SerializeField] private string[] initialThoughtTexts;
        [SerializeField] private GameObject uiPrefabToShow; 

        [Header("Subsequent Interaction")]
        [SerializeField] private InteractionType subsequentAction = InteractionType.RepeatInitial;
        [SerializeField] private DialogueDataSO subsequentDialogueSo;
        [SerializeField] private DialogueLine[] subsequentInlineDialogue;
        [SerializeField] private string[] subsequentThoughtTexts;

        [Header("Trigger Settings")]
        [SerializeField] private float _interactionRadius = 2.5f;
        [Tooltip("Если true, объект будет уничтожен после первого успешного взаимодействия.")]
        [SerializeField] private bool destroyAfterInteraction; 
        [SerializeField] private string triggerId;
        private bool _hasBeenTriggered;

        public float InteractionRadius => _interactionRadius;

        public void Interact(Transform playerTransform)
        {
            if (!_hasBeenTriggered)
            {
                ExecuteInteraction(initialAction, playerTransform, initialDialogueSo, initialInlineDialogue, initialThoughtTexts);
                _hasBeenTriggered = true;
                
                if (destroyAfterInteraction)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                var actionToExecute = subsequentAction == InteractionType.RepeatInitial ? initialAction : subsequentAction;
                var dialogueToExecute = subsequentAction == InteractionType.RepeatInitial ? subsequentDialogueSo : subsequentDialogueSo;
                var inlineToExecute = subsequentAction == InteractionType.RepeatInitial ? initialInlineDialogue : subsequentInlineDialogue;
                var thoughtsToExecute = subsequentAction == InteractionType.RepeatInitial ? initialThoughtTexts : subsequentThoughtTexts;
                ExecuteInteraction(actionToExecute, playerTransform, dialogueToExecute, inlineToExecute, thoughtsToExecute);
            }
        }

        private void ExecuteInteraction(InteractionType type, Transform playerTransform, DialogueDataSO dialogueData, DialogueLine[] inlineDialogue, string[] thoughts)
        {
            switch (type)
            {
                case InteractionType.DialogueSo:
                    if (dialogueData != null) GameManager.Instance.DialogueManager.StartDialogue(dialogueData);
                    break;
                case InteractionType.InlineDialogue:
                    if (inlineDialogue != null && inlineDialogue.Length > 0)
                    {
                        var tempData = ScriptableObject.CreateInstance<DialogueDataSO>();
                        tempData.lines = inlineDialogue;
                        GameManager.Instance.DialogueManager.StartDialogue(tempData);
                    }
                    break;
                case InteractionType.Thought:
                    if (thoughts != null && thoughts.Length > 0)
                    {
                        string textToShow = thoughts[Random.Range(0, thoughts.Length)];
                        ThoughtBubbleController.Instance.ShowThought(textToShow, playerTransform, 4f);
                    }
                    break;
                case InteractionType.ShowUIPrefab:
                    if (uiPrefabToShow != null)
                    {
                        Transform canvasTransform = GameManager.Instance.MainCanvasTransform;
                        if (canvasTransform != null)
                        {
                            Instantiate(uiPrefabToShow, canvasTransform);
                            GameManager.Instance.SetGameState(GameState.Paused);
                        }
                        else
                        {
                            Debug.LogError("MainCanvasTransform не назначен в GameManager! Невозможно создать UI.", this);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("InteractionType.ShowUIPrefab, но префаб UI не назначен!", this);
                    }
                    break;
                case InteractionType.DoNothing:
                case InteractionType.RepeatInitial:
                default:
                    break;
            }
        }
    }
}