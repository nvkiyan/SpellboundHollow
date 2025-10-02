using _SpellboundHollow.Scripts.Core;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    // Убеждаемся, что класс РЕАЛИЗУЕТ IInteractable
    public class CollectibleItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private StudyItemDataSO itemData;
        [SerializeField] private int quantity = 1;
        
        [Tooltip("Радиус, в пределах которого игрок должен находиться для взаимодействия.")]
        [SerializeField] private float interactionRadius = 1.5f;
        
        // Реализация свойства из интерфейса
        public float InteractionRadius => interactionRadius;

        // Реализация метода из интерфейса
        public void Interact(Transform playerTransform)
        {
            Collect();
        }
        
        public void Collect()
        {
            if (itemData == null)
            {
                Debug.LogError("CollectibleItem: ItemData не назначен!", this);
                return;
            }
            
            // Здесь ваша логика добавления в инвентарь и гримуар
            GameManager.Instance.InventoryManager.AddItem(itemData, quantity);
            if (!GameManager.Instance.GrimoireManager.HasStudied(itemData))
            {
                GameManager.Instance.GrimoireManager.AddStudiedItem(itemData);
            }
            
            // Уничтожаем объект после сбора
            Destroy(gameObject);
        }
    }
}