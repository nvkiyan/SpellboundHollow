namespace _SpellboundHollow.Scripts.Gameplay
{
    using UnityEngine;
    using Core; // "Знакомим" с Core, где лежат DialogueDataSO и DialogueManager

    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Данные Диалога")]
        [Tooltip("Сценарий диалога, который будет запущен при взаимодействии.")]
        [SerializeField] private DialogueDataSO dialogue;

        public void TriggerDialogue()
        {
            if (dialogue != null)
            {
                GameManager.Instance.DialogueManager.StartDialogue(dialogue);
            }
            else
            {
                Debug.LogWarning($"На объекте {gameObject.name} отсутствует DialogueDataSO!", this);
            }
        }
    }
}