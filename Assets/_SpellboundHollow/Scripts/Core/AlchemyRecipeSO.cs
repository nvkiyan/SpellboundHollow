// Помещаем его в наш Core, т.к. это фундаментальный тип данных
namespace _SpellboundHollow.Scripts.Core
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [System.Serializable]
    public struct Ingredient
    {
        public StudyItemDataSO item;
        public int quantity;
    }

    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Spellbound Hollow/Alchemy Recipe")]
    public class AlchemyRecipeSO : ScriptableObject
    {
        [Header("Ингредиенты")]
        public List<Ingredient> ingredients;

        [Header("Результат")]
        public StudyItemDataSO outputItem;
        public int outputQuantity;
    }
}