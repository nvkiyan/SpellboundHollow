using _SpellboundHollow.Scripts.Core;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    /// <summary>
    /// Запускает указанный диалог автоматически при старте сцены.
    /// Полезен для стартовых монологов или сюжетных событий при входе в локацию.
    /// </summary>
    public class SceneEntryTrigger : MonoBehaviour
    {
        [Tooltip("Диалог, который будет запущен при загрузке этого объекта.")]
        [SerializeField] private DialogueDataSO entryDialogue;

        [Tooltip("Если true, диалог сработает только один раз. Требует наличия уникального ID.")]
        [SerializeField] private bool triggerOnce = true;
        
        [Tooltip("Уникальный идентификатор для этого триггера. Нужен, чтобы игра 'помнила', что он уже сработал.")]
        [SerializeField] private string triggerId;

        private void Start()
        {
            if (entryDialogue == null)
            {
                Debug.LogWarning("SceneEntryTrigger: Диалог не назначен!", this);
                return;
            }

            // Проверяем, нужно ли сработать только один раз.
            if (triggerOnce)
            {
                if (string.IsNullOrEmpty(triggerId))
                {
                    Debug.LogError("SceneEntryTrigger: Установлен флаг 'triggerOnce', но не указан уникальный ID!", this);
                    return;
                }
                
                // Проверяем, был ли этот триггер уже активирован (например, через систему сохранений)
                // ЗАГЛУШКА: Здесь будет проверка в GameManager или SaveManager
                // if (GameManager.Instance.ProgressManager.IsTriggerCompleted(triggerId))
                // {
                //     return;
                // }
            }

            // Запускаем диалог
            GameManager.Instance.DialogueManager.StartDialogue(entryDialogue);
            
            // Если нужно, помечаем триггер как выполненный
            // ЗАГЛУШКА: Здесь будет вызов GameManager.Instance.ProgressManager.CompleteTrigger(triggerId);
        }
    }
}