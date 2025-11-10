using _SpellboundHollow.Scripts.Core;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    /// <summary>
    /// Запускает указанный диалог автоматически при старте сцены и служит точкой входа в сцену.
    /// Полезен для стартовых монологов или сюжетных событий при входе в локацию.
    /// </summary>
    public class SceneEntryTrigger : MonoBehaviour
    {
        [Header("Entry Point Settings")]
        [Tooltip("Уникальный идентификатор этой точки входа. Используется дверями для телепортации игрока.")]
        [SerializeField] private string entryId;
        
        [Header("Dialogue Settings")]
        [Tooltip("Диалог, который будет запущен при загрузке этого объекта.")]
        [SerializeField] private DialogueDataSO entryDialogue;

        [Tooltip("Если true, диалог сработает только один раз. Требует наличия уникального ID.")]
        [SerializeField] private bool triggerOnce = true;
        
        [Tooltip("Уникальный идентификатор для этого триггера. Нужен, чтобы игра 'помнила', что он уже сработал.")]
        [SerializeField] private string triggerId;

        // Публичное свойство, чтобы SceneTransitionManager мог прочитать ID этой точки входа.
        public string EntryId => entryId;

        private void Start()
        {
            if (entryDialogue == null)
            {
                // Это сообщение не должно быть ошибкой, так как точка входа может и не иметь диалога.
                // Debug.LogWarning("SceneEntryTrigger: Диалог не назначен!", this);
                return;
            }

            if (triggerOnce)
            {
                if (string.IsNullOrEmpty(triggerId))
                {
                    Debug.LogError("SceneEntryTrigger: Установлен флаг 'triggerOnce', но не указан уникальный ID для диалога!", this);
                    return;
                }
                
                // ЗАГЛУШКА: Здесь будет проверка в GameManager или SaveManager
                // if (GameManager.Instance.ProgressManager.IsTriggerCompleted(triggerId))
                // {
                //     return;
                // }
            }

            GameManager.Instance.DialogueManager.StartDialogue(entryDialogue);
            
            // ЗАГЛУШКА: Здесь будет вызов GameManager.Instance.ProgressManager.CompleteTrigger(triggerId);
        }
    }
}