using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    /// ScriptableObject, который хранит последовательность реплик, составляющих один диалог.
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Spellbound Hollow/Dialogue Data")]
    public class DialogueDataSO : ScriptableObject
    {
        public DialogueLine[] lines;
    }
    
    /// Структура, представляющая одну реплику в диалоге.
    /// Содержит данные о говорящем персонаже, сам текст реплики,
    //  а также опциональный спрайт для передачи специфической эмоции.
    [System.Serializable]
    public struct DialogueLine
    {
        [Tooltip("Данные о персонаже, который произносит эту реплику (имя, портрет по умолчанию).")]
        public CharacterDataSO characterData;

        [Tooltip("Текст реплики. Поддерживает теги Rich Text от TextMeshPro.")]
        [TextArea(3, 10)]
        public string text;
        
        [Tooltip("(Опционально) Укажите здесь спрайт, чтобы переопределить портрет по умолчанию для этой конкретной реплики.")]
        public Sprite emotionalPortrait;
    }
}