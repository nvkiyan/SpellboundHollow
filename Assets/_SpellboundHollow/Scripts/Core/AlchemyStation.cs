namespace _SpellboundHollow.Scripts.Gameplay
{
    using UnityEngine;
    using Core;
    using UI;
    using Characters;
    using System.Collections;

    [RequireComponent(typeof(Animator), typeof(Collider2D))]
    public class AlchemyStation : MonoBehaviour
    {
        [Header("Конфигурация Рецепта")]
        [Tooltip("Рецепт, который будет создаваться на этой станции.")]
        [SerializeField] private AlchemyRecipeSO recipeToCraft;
        [Tooltip("Общая длительность анимации крафта в секундах.")]
        [SerializeField] private float craftingDuration = 3f;

        [Header("Настройки Мини-Игры")]
        [Tooltip("Центр зоны успеха в процентах от 0 до 100.")]
        [Range(0, 100)] [SerializeField] private float successZoneCenter = 50f;
        [Tooltip("Размер зоны успеха в процентах (чем меньше, тем сложнее).")]
        [Range(5, 50)] [SerializeField] private float successZoneSize = 30f;

        // --- Ссылки на компоненты и системы ---
        private Animator _animator;
        private AlchemyMinigameUI _minigameUI;
        private PlayerController _playerController;
        
        // Флаг, чтобы избежать повторного запуска крафта, пока идет предыдущий
        private bool _isCrafting = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Находим контроллеры других систем при старте сцены
            _minigameUI = FindFirstObjectByType<AlchemyMinigameUI>();
            _playerController = FindFirstObjectByType<PlayerController>();
            
            if (_minigameUI != null)
            {
                // Подписываемся на событие завершения мини-игры, чтобы знать, когда она закончилась
                _minigameUI.OnMinigameFinished += HandleMinigameResult;
            }
        }

        // Важно отписываться от событий, когда объект уничтожается
        private void OnDestroy()
        {
            if (_minigameUI != null)
            {
                _minigameUI.OnMinigameFinished -= HandleMinigameResult;
            }
        }

        // Главный метод взаимодействия, вызываемый из PlayerController
        public void Interact()
        {
            // Игнорируем вызов, если крафт уже идет
            if (_isCrafting) return;
            
            // Проверяем, есть ли у игрока все необходимые ингредиенты
            bool canCraft = CheckIngredients();

            if (canCraft)
            {
                StartCoroutine(CraftingRoutine());
            }
        }

        // Проверяет наличие ингредиентов в инвентаре
        private bool CheckIngredients()
        {
            foreach (var ingredient in recipeToCraft.ingredients)
            {
                if (!GameManager.Instance.InventoryManager.HasItems(ingredient.item, ingredient.quantity))
                {
                    Debug.Log($"Недостаточно ингредиентов! Нужно: {ingredient.item.itemName} x{ingredient.quantity}");
                    return false; // Возвращаем false, если хоть одного ингредиента не хватает
                }
            }
            return true; // Возвращаем true, если все на месте
        }

        // Корутина, управляющая процессом варки зелья
        private IEnumerator CraftingRoutine()
        {
            _isCrafting = true;
            
            // Отключаем управление игроком на время крафта
            _playerController.enabled = false;
            
            // Сразу списываем ингредиенты
            foreach (var ingredient in recipeToCraft.ingredients)
            {
                GameManager.Instance.InventoryManager.RemoveItem(ingredient.item, ingredient.quantity);
            }
            
            // Запускаем анимацию котла
            _animator.SetTrigger("StartCrafting");
            
            // Ждем окончание анимации
            yield return new WaitForSeconds(craftingDuration);

            // После анимации запускаем мини-игру
            _minigameUI.StartMinigame(successZoneCenter, successZoneSize);
        }

        // Этот метод вызывается, когда мини-игра сообщает о своем завершении
        private void HandleMinigameResult(bool success)
        {
            if (success)
            {
                // Если мини-игра пройдена успешно, добавляем результат в инвентарь
                GameManager.Instance.InventoryManager.AddItem(recipeToCraft.outputItem, recipeToCraft.outputQuantity);
                Debug.Log($"УСПЕХ! Вы сварили: {recipeToCraft.outputItem.itemName}");
            }
            else
            {
                // Если провал, ингредиенты уже списаны
                Debug.Log("Провал! Ингредиенты потрачены впустую.");
            }
            
            // Возвращаем управление игроку
            _playerController.enabled = true;
            _isCrafting = false;
        }
    }
}