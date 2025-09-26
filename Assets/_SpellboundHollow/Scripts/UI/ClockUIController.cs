namespace _SpellboundHollow.Scripts.UI
{
    using Core; // Подключаем, чтобы видеть GameManager
    using TMPro;
    using UnityEngine;

    public class ClockUIController : MonoBehaviour
    {
        [Header("UI Элементы")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI moonPhaseText;
        
        [Header("Настройки")]
        [SerializeField] private TimeSettingsSO timeSettings;
        
        // OnEnable/OnDisable - правильное место для подписки/отписки на события
        private void OnEnable()
        {
            // Подписываемся на события через GameManager
            GameManager.Instance.TimeManager.OnDateTimeChanged += UpdateDateTimeText;
            GameManager.Instance.TimeManager.OnMoonPhaseChanged += UpdateMoonPhaseText;
        }

        private void OnDisable()
        {
            // Важно отписаться, чтобы избежать утечек памяти
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TimeManager.OnDateTimeChanged -= UpdateDateTimeText;
                GameManager.Instance.TimeManager.OnMoonPhaseChanged -= UpdateMoonPhaseText;
            }
        }

        // Этот метод будет вызываться событием из TimeManager
        private void UpdateDateTimeText(GameTimestamp timestamp)
        {
            timeText.text = $"{timestamp.hour:D2}:{timestamp.minute:D2}";
            string seasonName = timeSettings.seasonNames[timestamp.season];
            dateText.text = $"{seasonName}, День {timestamp.day}";
        }

        // Этот метод будет вызываться событием из TimeManager
        private void UpdateMoonPhaseText(MoonPhase phase)
        {
            moonPhaseText.text = ConvertMoonPhaseToString(phase);
        }
        
        private string ConvertMoonPhaseToString(MoonPhase phase)
        {
            switch (phase)
            {
                case MoonPhase.NewMoon:  return "Новолуние";
                case MoonPhase.Waxing:   return "Растущая Луна";
                case MoonPhase.FullMoon: return "Полнолуние";
                case MoonPhase.Waning:   return "Убывающая Луна";
                default:                 return "Неизвестная фаза";
            }
        }
    }
}