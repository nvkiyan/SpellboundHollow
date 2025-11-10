using System.Collections;
using System.Collections.Generic;
using _SpellboundHollow.Scripts.Core;
using _SpellboundHollow.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem; // <-- ВАЖНО: Добавляем using для новой системы ввода

namespace _SpellboundHollow.Scripts.Core
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueUIController dialogueUIController;
        
        private Queue<DialogueLine> _linesQueue;
        private bool _isDialogueActive;
        private DialogueLine _currentLine;
        
        [Header("Sound Settings")]
        [Tooltip("Звук, который проигрывается один раз при открытии диалогового окна.")]
        [SerializeField] private AudioClip openDialogueSound;

        public bool IsDialogueActive => _isDialogueActive;

        private void Awake()
        {
            _linesQueue = new Queue<DialogueLine>();
        }

        // Возвращаем метод Update для обработки кликов во время диалога.
        private void Update()
        {
            // Метод работает только если игра в состоянии диалога.
            if (GameManager.Instance.CurrentState != GameState.Dialogue) return;
            
            // Используем новую систему ввода, чтобы избежать конфликтов.
            // Mouse.current.leftButton.wasPressedThisFrame - это современный аналог Input.GetMouseButtonDown(0).
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Если в данный момент печатается строка, завершаем ее досрочно.
                if (dialogueUIController.IsTyping)
                {
                    dialogueUIController.CompleteLine();
                }
                // Если строка уже напечатана, показываем следующую.
                else
                {
                    DisplayNextLine();
                }
            }
        }
        
        public void StartDialogue(DialogueDataSO dialogueData)
        {
            if (_isDialogueActive) return;
            
            AudioManager.Instance.PlaySFX(openDialogueSound);

            GameManager.Instance.SetGameState(GameState.Dialogue);
            _isDialogueActive = true;
            
            dialogueUIController.ShowDialoguePanel();

            _linesQueue.Clear();
            foreach (DialogueLine line in dialogueData.lines)
            {
                _linesQueue.Enqueue(line);
            }
            
            DisplayNextLine();
        }

        public void EndDialogue()
        {
            if (!_isDialogueActive) return;

            _isDialogueActive = false;
            
            if (dialogueUIController != null)
            {
                dialogueUIController.HideDialoguePanel();
            }
            
            if(GameManager.Instance.CurrentState == GameState.Dialogue)
            {
                GameManager.Instance.SetGameState(GameState.Gameplay);
            }
        }

        private void DisplayNextLine()
        {
            if (_linesQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            _currentLine = _linesQueue.Dequeue();
            dialogueUIController.DisplayLine(_currentLine);
        }
    }
}