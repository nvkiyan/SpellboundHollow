using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _SpellboundHollow.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueUIController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image portraitImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("Typing Effect Settings")]
        [SerializeField] private float typingSpeed = 0.04f;
        public bool IsTyping { get; private set; }
        
        private CanvasGroup _canvasGroup;
        private Coroutine _typingCoroutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void ShowDialoguePanel()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void HideDialoguePanel()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void DisplayLine(Core.DialogueLine line)
        {
            if (line.characterData != null)
            {
                nameText.text = line.characterData.characterName;
                portraitImage.sprite = line.emotionalPortrait != null ? line.emotionalPortrait : line.characterData.characterPortrait;
                portraitImage.enabled = portraitImage.sprite != null;
            }
            else
            {
                nameText.text = string.Empty;
                portraitImage.enabled = false;
            }

            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeLine(line.text));
        }

        public void CompleteLine()
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
            IsTyping = false;
        }
        
        private IEnumerator TypeLine(string line)
        {
            IsTyping = true;
            dialogueText.text = line;
            dialogueText.maxVisibleCharacters = 0;
            yield return null; 

            int totalVisibleCharacters = dialogueText.textInfo.characterCount;
            for (int i = 0; i < totalVisibleCharacters; i++)
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            IsTyping = false;
        }
    }
}