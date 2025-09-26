// Инвентарь - это тоже базовая система
namespace _SpellboundHollow.Scripts.Core
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InventoryManager : MonoBehaviour
    {
        private readonly Dictionary<StudyItemDataSO, int> _inventory = new Dictionary<StudyItemDataSO, int>();

        public void AddItem(StudyItemDataSO item, int quantity = 1)
        {
            if (_inventory.ContainsKey(item))
            {
                _inventory[item] += quantity;
            }
            else
            {
                _inventory.Add(item, quantity);
            }
            Debug.Log($"Добавлено в инвентарь: {item.itemName} x{quantity}. Всего: {_inventory[item]}");
        }

        public bool HasItems(StudyItemDataSO item, int quantity)
        {
            return _inventory.ContainsKey(item) && _inventory[item] >= quantity;
        }

        public void RemoveItem(StudyItemDataSO item, int quantity = 1)
        {
            if (HasItems(item, quantity))
            {
                _inventory[item] -= quantity;
                Debug.Log($"Удалено из инвентаря: {item.itemName} x{quantity}. Осталось: {_inventory[item]}");
            }
        }
    }
}