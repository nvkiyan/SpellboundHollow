using UnityEngine;

    // [CreateAssetMenu] позволяет нам создавать экземпляры этого объекта прямо в редакторе Unity
    // через меню Assets -> Create -> Time -> Time Settings
    [CreateAssetMenu(fileName = "TimeSettings", menuName = "Time/Time Settings")]
public class TimeSettingsSO : ScriptableObject
{
    [Header("Настройки продолжительности")]
    [Tooltip("Сколько реальных секунд длится один игровой день")]
    public float secondsInDay = 1200f; // 1200 секунд = 20 минут

    [Header("Календарь")]
    [Tooltip("Количество дней в одном сезоне")]
    public int daysInSeason = 28;
    [Tooltip("Названия сезонов, начиная с Весны")]
    public string[] seasonNames = { "Весна", "Лето", "Осень", "Зима" };

    [Header("Стартовая дата")]
    public int startingYear = 1;
    public int startingSeason = 0; // 0 = Весна
    public int startingDay = 1;
    public int startingHour = 6;
    public int startingMinute = 0;
}