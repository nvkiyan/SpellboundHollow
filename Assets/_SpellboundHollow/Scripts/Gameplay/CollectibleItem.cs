namespace _SpellboundHollow.Scripts.Gameplay
{
    using UnityEngine;
    using _SpellboundHollow.Scripts.Core;

    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private StudyItemDataSO itemData;
        [SerializeField] private int quantity = 1;
        
        public void Collect()
        {
            // Проверяем, был ли этот предмет уже изучен
            if (!GameManager.Instance.GrimoireManager.HasStudied(itemData))
            {
                // Если нет - добавляем запись в Гримуар
                GameManager.Instance.GrimoireManager.AddStudiedItem(itemData);
            }
            
            // В любом случае (и в первый раз, и в последующие) добавляем предмет в инвентарь
            GameManager.Instance.InventoryManager.AddItem(itemData, quantity);
            
            Destroy(gameObject); // Уничтожаем объект после сбора
        }
    }
}