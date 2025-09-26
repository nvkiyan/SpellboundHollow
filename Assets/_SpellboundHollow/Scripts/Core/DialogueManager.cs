namespace _SpellboundHollow.Scripts.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using Characters;
    using TMPro;
    using UnityEngine;

    public class DialogueManager : MonoBehaviour
    {
        [Header("Компоненты UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI dialogueLineText;
        [SerializeField] private GameObject continueIndicator;
        
        [Header("Настройки Печати Текста")]
        [SerializeField] private float typingSpeed = 0.04f;

        // --- Внутреннее состояние ---
        private readonly Queue<DialogueLine> _lines = new Queue<DialogueLine>();
        private PlayerController _playerController;
        private bool _isDialogueActive = false;
        private bool _isTyping = false; // Флаг, который показывает, идет ли сейчас печать текста
        private string _fullLineText; // Хранит полный текст текущей реплики для быстрого отображения

        private void Start()
        {
            // Находим контроллер игрока при старте
            _playerController = FindFirstObjectByType<PlayerController>();
            // Убедимся, что панель изначально выключена
            dialoguePanel.SetActive(false);
        }
        
        /// <summary>
        /// Возвращает true, если диалоговое окно в данный момент активно.
        /// </summary>
        public bool IsDialogueActive() => _isDialogueActive;

        /// <summary>
        /// Запускает новый диалог.
        /// </summary>
        public void StartDialogue(DialogueDataSO dialogue)
        {
            if (_isDialogueActive) return;

            _isDialogueActive = true; 
            _playerController.DisableMovement(); // Отключаем управление игроком
            dialoguePanel.SetActive(true);
            
            // Очищаем очередь от возможных старых реплик
            _lines.Clear();

            // Заполняем очередь новыми репликами из нашего ScriptableObject
            foreach (var line in dialogue.lines)
            {
                _lines.Enqueue(line);
            }
            
            // Начинаем показывать первую реплику
            DisplayNextLine();
        }
        
        /// <summary>
        /// Отображает следующую реплику из очереди или завершает диалог.
        /// </summary>
        public void DisplayNextLine()
        {
            // Если сейчас идет печать текста, мы сначала мгновенно показываем всю реплику до конца
            if (_isTyping)
            {
                Debug.Log("[DialogueManager] Пытаемся показать следующую реплику.");
                StopAllCoroutines();
                dialogueLineText.text = _fullLineText;
                _isTyping = false;
                // Показываем индикатор, только если есть еще реплики
                if (_lines.Count > 0)
                {
                    continueIndicator.SetActive(true);
                }
                return; // Выходим, чтобы следующее нажатие показало новую реплику
            }
            
            // Если реплики в очереди закончились, завершаем диалог
            if (_lines.Count == 0)
            {
                Debug.Log("[DialogueManager] Реплики закончились. Вызываем EndDialogue().");
                EndDialogue();
                return;
            }

            // Достаем следующую реплику из очереди
            DialogueLine currentLine = _lines.Dequeue();
            
            // Запускаем корутину, которая напечатает текст по буквам
            StopAllCoroutines(); // На всякий случай останавливаем предыдущие корутины
            StartCoroutine(TypeLine(currentLine));
        }

        // Корутина для "эффекта пишущей машинки"
        private IEnumerator TypeLine(DialogueLine line)
        {
            _isTyping = true;
            _fullLineText = line.text; // Сохраняем полный текст для возможного "проматывания"
            
            characterNameText.text = line.characterName;
            dialogueLineText.text = "";
            continueIndicator.SetActive(false);
            
            foreach (char letter in line.text.ToCharArray())
            {
                dialogueLineText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            _isTyping = false;

            // Показываем индикатор "продолжить", только если в очереди ЕСТЬ еще реплики
            if (_lines.Count > 0)
            {
                continueIndicator.SetActive(true);
            }
        }

        // Завершает диалог и возвращает управление игроку
        private void EndDialogue()
        {
            if (!_isDialogueActive) return;
            
            Debug.Log("[DialogueManager] Завершаем диалог. Возвращаем управление игроку.");

            _isDialogueActive = false;
            dialoguePanel.SetActive(false);
            _playerController.EnableMovement();
        }
    }
}