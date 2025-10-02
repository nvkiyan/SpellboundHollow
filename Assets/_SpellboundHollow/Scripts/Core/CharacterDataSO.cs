using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    /// <summary>
    /// ScriptableObject, хранящий статичные данные о персонаже, такие как имя и портрет.
    /// Используется для централизованного управления информацией о персонажах.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Spellbound Hollow/Character Data")]
    public class CharacterDataSO : ScriptableObject
    {
        [Tooltip("Имя персонажа, которое будет отображаться в диалоговом окне.")]
        public string characterName;

        [Tooltip("Спрайт с портретом персонажа для отображения в UI.")]
        public Sprite characterPortrait;
    }
}