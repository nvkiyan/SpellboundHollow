namespace _SpellboundHollow.Scripts.Core
{
    using UnityEngine;

    // Эта структура описывает одну-единственную реплику в диалоге
    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName; // Имя говорящего
        [TextArea(2, 4)]
        public string text; // Текст реплики
    }

    // Это ScriptableObject, который служит "контейнером" или "сценарием" для целого диалога
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Spellbound Hollow/Dialogue")]
    public class DialogueDataSO : ScriptableObject
    {
        // Массив всех реплик, которые принадлежат этому диалогу
        public DialogueLine[] lines;
    }
}