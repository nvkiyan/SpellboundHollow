namespace _SpellboundHollow.Scripts.UI
{
    using UnityEngine;
    using TMPro;
    using Core;

    // Этот компонент можно повесить на любой GameObject с TextMeshPro.
    // Он автоматически найдет текст, возьмет его шаблон и отформатирует при запуске.
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FormattedText : MonoBehaviour
    {
        // В это поле мы будем вписывать наш текст с токенами прямо в инспекторе.
        [TextArea(5, 10)]
        [SerializeField] private string textTemplate;
        
        private TextMeshProUGUI _textComponent;

        // Awake вызывается раньше Start.
        private void Awake()
        {
            // Получаем ссылку на компонент текста на этом же объекте.
            _textComponent = GetComponent<TextMeshProUGUI>();
            
            // Форматируем и отображаем текст.
            UpdateText();
        }
        
        // Публичный метод, если нам понадобится обновить текст из другого скрипта в будущем
        public void UpdateText()
        {
            if (_textComponent != null && !string.IsNullOrEmpty(textTemplate))
            {
                // Пропускаем наш шаблон через универсальный форматер.
                _textComponent.text = TextFormatter.Format(textTemplate);
            }
        }
    }
}