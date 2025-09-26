namespace _SpellboundHollow.Scripts.UI
{
    using System.Collections.Generic;
    using Core; // Подключаем наш Core, чтобы видеть GrimoireManager
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GrimoireUI : MonoBehaviour
    {
        public static GrimoireUI Instance { get; private set; }

        [Header("Компоненты UI")]
        [SerializeField] private GameObject grimoirePanel; // Главная панель Гримуара
        [SerializeField] private Transform contentParent; // Объект "Content" внутри Scroll View
        [SerializeField] private GameObject studyItemButtonPrefab; // Наш префаб кнопки
        
        [Header("Правая страница")]
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            // Изначально Гримуар должен быть закрыт
            grimoirePanel.SetActive(false);
        }

        // Start вызывается после того, как ВСЕ Awake() в сцене были выполнены.
        // Это гарантирует, что GrimoireManager.Instance уже существует.
        private void Start()
        {
            // Переносим подписку сюда
            GameManager.Instance.GrimoireManager.OnGrimoireUpdated += UpdateUI;
        }

        private void OnDisable()
        {
            // Обязательно отписываемся, чтобы избежать утечек памяти
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GrimoireManager.OnGrimoireUpdated -= UpdateUI;
            }
        }
        
        /// <summary>
        /// Показывает или скрывает интерфейс Гримуара.
        /// </summary>
        public void Toggle()
        {
            bool isActive = !grimoirePanel.activeSelf;
            grimoirePanel.SetActive(isActive);

            // Обновляем UI каждый раз, когда открываем его
            if (isActive)
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// Обновляет список изученных предметов на левой странице.
        /// </summary>
        private void UpdateUI()
        {
            // 1. Очищаем старый список
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // 2. Получаем актуальный список из менеджера, ВЫЗЫВАЯ ЕГО МЕТОД GetStudiedItems()
            List<StudyItemDataSO> studiedItems = GameManager.Instance.GrimoireManager.GetStudiedItems();

            // 3. Создаем кнопки для каждого изученного предмета
            foreach (var itemData in studiedItems)
            {
                GameObject buttonGO = Instantiate(studyItemButtonPrefab, contentParent);
                buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = itemData.itemName;
                buttonGO.GetComponent<Button>().onClick.AddListener(() => DisplayItemDetails(itemData));
            }
        
            // Опционально: Показываем описание первого предмета в списке
            if (studiedItems.Count > 0)
            {
                DisplayItemDetails(studiedItems[0]);
            }
            else
            {
                // Если список пуст, очищаем правую страницу
                DisplayItemDetails(null);
            }
        }

        /// <summary>
        /// Отображает информацию о выбранном предмете на правой странице.
        /// </summary>
        private void DisplayItemDetails(StudyItemDataSO itemData)
        {
            itemNameText.text = itemData.itemName;
            itemDescriptionText.text = itemData.description;
        }
    }
}