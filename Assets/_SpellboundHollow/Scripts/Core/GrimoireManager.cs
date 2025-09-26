namespace _SpellboundHollow.Scripts.Core
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GrimoireManager : MonoBehaviour
    {
        // --- СОБЫТИЕ ЭКЗЕМПЛЯРА ---
        // Нет 'static'. Это событие принадлежит конкретному объекту GrimoireManager.
        public event System.Action OnGrimoireUpdated;

        // --- БАЗА ЗНАНИЙ ---
        private readonly HashSet<StudyItemDataSO> _studiedItems = new HashSet<StudyItemDataSO>();
        
        // --- ПУБЛИЧНЫЕ МЕТОДЫ ---
        public void AddStudiedItem(StudyItemDataSO itemData)
        {
            // Метод Add у HashSet возвращает true, если элемент был успешно добавлен
            if (_studiedItems.Add(itemData))
            {
                // Если добавили новый, уникальный предмет...
                Debug.Log($"Новая запись в Гримуаре: {itemData.itemName}");
                
                // ...вызываем событие, чтобы уведомить UI.
                // Мы вызываем его напрямую, без 'Instance' и 'static'.
                OnGrimoireUpdated?.Invoke();
            }
            else
            {
                Debug.Log($"Предмет '{itemData.itemName}' уже был изучен.");
            }
        }
        
        public List<StudyItemDataSO> GetStudiedItems()
        {
            return new List<StudyItemDataSO>(_studiedItems);
        }

        public bool HasStudied(StudyItemDataSO itemData)
        {
            return _studiedItems.Contains(itemData);
        }
    }
}