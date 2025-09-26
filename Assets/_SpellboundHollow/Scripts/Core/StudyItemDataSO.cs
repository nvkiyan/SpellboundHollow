namespace _SpellboundHollow.Scripts.Core
{
    using UnityEngine;

    // Этот ScriptableObject - наш шаблон для любого предмета, который можно изучить.
    // Растения, существа, камни - все они будут использовать этот шаблон.
    [CreateAssetMenu(fileName = "NewStudyItem", menuName = "Spellbound Hollow/Study Item Data")]

    public class StudyItemDataSO : ScriptableObject
    {
        [Header("Основная Информация")]
        public string itemName; // Название, например "Лунная Ромашка"
    
        [TextArea(3, 5)] // Делает текстовое поле в инспекторе больше
        public string description; // Описание, которое появится в Гримуаре
    
        // В будущем мы добавим сюда иконку, тип предмета (растение/существо), и т.д.
    }   
}