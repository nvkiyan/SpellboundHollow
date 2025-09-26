using UnityEngine;

// Эта структура используется для хранения конкретной точки во времени в игре.

[System.Serializable]

public struct GameTimestamp
{
    public int year;
    public int season; // 0 - Весна, 1 - Лето, 2 - Осень, 3 - Зима
    public int day;
    public int hour;
    public int minute;

    // Конструктор для удобного создания экземпляров
    public GameTimestamp(int year, int season, int day, int hour, int minute)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }
}