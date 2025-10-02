using _SpellboundHollow.Scripts.Core;
using _SpellboundHollow.Scripts.UI;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    public class InteractionTrigger : MonoBehaviour, IInteractable
    {
        // Enum теперь публичный, так как используется в публичных полях, и с корректным регистром
        public enum InteractionType
        {
            DoNothing,
            DialogueSo, // Исправлен регистр
            InlineDialogue,
            Thought,
            RepeatInitial 
        }

        // --- ИМЕНА ПОЛЕЙ ИСПРАВЛЕНЫ В СООТВЕТСТВИИ СО СТАНДАРТОМ UNITY ---
        [Header("Primary Interaction")]
        [Tooltip("Действие, которое произойдет при первом взаимодействии.")]
        [SerializeField] private InteractionType initialAction = InteractionType.Thought;
        [SerializeField] private DialogueDataSO initialDialogueSo;
        [SerializeField] private DialogueLine[] initialInlineDialogue;
        [Tooltip("Варианты текста для облачка мыслей. Если их несколько, будет выбран случайный.")]
        [SerializeField] private string[] initialThoughtTexts;
        
        [Header("Subsequent Interaction")]
        [Tooltip("Действие при всех последующих взаимодействиях.")]
        [SerializeField] private InteractionType subsequentAction = InteractionType.RepeatInitial;
        [SerializeField] private DialogueDataSO subsequentDialogueSo;
        [SerializeField] private DialogueLine[] subsequentInlineDialogue;
        [Tooltip("Варианты текста для облачка мыслей. Если их несколько, будет выбран случайный.")]
        [SerializeField] private string[] subsequentThoughtTexts;

        [Header("Trigger Settings")]
        [SerializeField] private float interactionRadius = 2.5f;
        public float InteractionRadius => interactionRadius;
        [SerializeField] private string triggerId;
        private bool _hasBeenTriggered;

        public void Interact(Transform playerTransform)
        {
            if (!_hasBeenTriggered)
            {
                ExecuteInteraction(initialAction, playerTransform, initialDialogueSo, initialInlineDialogue, initialThoughtTexts);
                _hasBeenTriggered = true;
            }
            else
            {
                var actionToExecute = subsequentAction == InteractionType.RepeatInitial ? initialAction : subsequentAction;
                var dialogueToExecute = subsequentAction == InteractionType.RepeatInitial ? initialDialogueSo : subsequentDialogueSo;
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
                case InteractionType.DoNothing:
                case InteractionType.RepeatInitial:
                default:
                    break;
            }
        }
    }
}