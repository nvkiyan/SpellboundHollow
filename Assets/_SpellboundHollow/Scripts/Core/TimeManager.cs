namespace _SpellboundHollow.Scripts.Core
{

    using UnityEngine;
    using System;

    public class TimeManager : MonoBehaviour
    {
        // --- СОБЫТИЯ ---
        // Другие скрипты могут подписаться на эти события, чтобы реагировать на изменения времени
        // Это гораздо производительнее, чем проверять время в Update() каждого скрипта.
        public event Action<GameTimestamp> OnDateTimeChanged;
        public event Action<MoonPhase> OnMoonPhaseChanged;
        public event Action<int> OnDayChanged;

        [Header("Настройки Времени")] [SerializeField]
        private TimeSettingsSO timeSettings;

        // --- ТЕКУЩЕЕ ВРЕМЯ ---
        private GameTimestamp _currentTime;
        private MoonPhase _currentMoonPhase;

        // Внутренний таймер
        private float _timeOfDay;

        private void Start()
        {
            // Инициализация времени из настроек
            _currentTime = new GameTimestamp(
                timeSettings.startingYear,
                timeSettings.startingSeason,
                timeSettings.startingDay,
                timeSettings.startingHour,
                timeSettings.startingMinute
            );

            _timeOfDay = _currentTime.hour * 60f + _currentTime.minute;

            UpdateMoonPhase();

            // Уведомляем всех подписчиков о начальном времени
            OnDateTimeChanged?.Invoke(_currentTime);
            OnMoonPhaseChanged?.Invoke(_currentMoonPhase);
        }

        private void Update()
        {
            // Проверяем, есть ли настройки
            if (timeSettings == null) return;

            // Двигаем время вперед
            float previousTimeOfDay = _timeOfDay;
            _timeOfDay += Time.deltaTime * (1440 / timeSettings.secondsInDay); // 1440 минут в дне

            // --- ПРОВЕРКА ИЗМЕНЕНИЙ ---

            // Проверяем, не перешли ли мы на следующий день
            if (_timeOfDay >= 1440)
            {
                _timeOfDay = 0;
                AdvanceDay();
            }

            // Обновляем текущие час и минуту
            _currentTime.hour = (int)(_timeOfDay / 60);
            _currentTime.minute = (int)(_timeOfDay % 60);

            // Вызываем событие, если время изменилось
            if ((int)previousTimeOfDay != (int)_timeOfDay)
            {
                OnDateTimeChanged?.Invoke(_currentTime);
            }
        }

        private void AdvanceDay()
        {
            _currentTime.day++;
            if (_currentTime.day > timeSettings.daysInSeason)
            {
                _currentTime.day = 1;
                _currentTime.season++;
                if (_currentTime.season >= timeSettings.seasonNames.Length)
                {
                    _currentTime.season = 0;
                    _currentTime.year++;
                }
            }

            // Обновляем фазу луны каждый новый день
            UpdateMoonPhase();

            // Уведомляем подписчиков о смене дня
            OnDayChanged?.Invoke(_currentTime.day);
        }

        private void UpdateMoonPhase()
        {
            MoonPhase newPhase;
            int dayInSeason = _currentTime.day; // Используем день в сезоне (от 1 до 28)

            // Новая, упрощенная логика на 4 фазы
            if (dayInSeason <= 3) // Дни 1-3
            {
                newPhase = MoonPhase.NewMoon;
            }
            else if (dayInSeason <= 14) // Дни 4-14 (11 дней)
            {
                newPhase = MoonPhase.Waxing;
            }
            else if (dayInSeason <= 17) // Дни 15-17
            {
                newPhase = MoonPhase.FullMoon;
            }
            else // Дни 18-28 (11 дней)
            {
                newPhase = MoonPhase.Waning;
            }

            // Проверяем, изменилась ли фаза, и если да - вызываем событие
            if (newPhase != _currentMoonPhase)
            {
                _currentMoonPhase = newPhase;
                OnMoonPhaseChanged?.Invoke(_currentMoonPhase);

                // Для отладки, чтобы видеть смену фазы в консоли. Потом можно убрать.
                Debug.Log($"Новый день: {_currentTime.day}. Фаза луны сменилась на: {_currentMoonPhase}");
            }
        }

        // --- ПУБЛИЧНЫЕ МЕТОДЫ ---
        // Позволяют другим скриптам безопасно получать информацию
        public GameTimestamp GetCurrentTimestamp() => _currentTime;
        public MoonPhase GetCurrentMoonPhase() => _currentMoonPhase;
    }
}
