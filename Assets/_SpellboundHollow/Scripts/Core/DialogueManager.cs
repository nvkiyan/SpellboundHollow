using System.Collections;
using System.Collections.Generic;
using _SpellboundHollow.Scripts.Core;
using _SpellboundHollow.Scripts.UI;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueUIController dialogueUIController;
        
        private Queue<DialogueLine> _linesQueue;
        private bool _isDialogueActive;
        private DialogueLine _currentLine;
        private bool _isAcceptingInput;

        public bool IsDialogueActive => _isDialogueActive;

        private void Awake()
        {
            _linesQueue = new Queue<DialogueLine>();
        }
        
        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Dialogue || !_isAcceptingInput) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                if (dialogueUIController.IsTyping)
                {
                    dialogueUIController.CompleteLine();
                }
                else
                {
                    DisplayNextLine();
                }
            }
        }
        
        public void StartDialogue(DialogueDataSO dialogueData)
        {
            if (_isDialogueActive) return;

            GameManager.Instance.SetGameState(GameState.Dialogue);
            _isDialogueActive = true;
            
            dialogueUIController.ShowDialoguePanel();

            _linesQueue.Clear();
            foreach (DialogueLine line in dialogueData.lines)
            {
                _linesQueue.Enqueue(line);
            }
            
            StartCoroutine(EnableInputAfterFrame());
            DisplayNextLine();
        }

        public void EndDialogue()
        {
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

        private IEnumerator EnableInputAfterFrame()
        {
            _isAcceptingInput = false;
            yield return null; 
            _isAcceptingInput = true;
        }
    }
}