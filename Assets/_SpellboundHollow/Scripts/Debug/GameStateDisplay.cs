using _SpellboundHollow.Scripts.Core;
using TMPro; // Важно: добавляем using для TextMeshPro
using UnityEngine;

/// <summary>
/// Временный диагностический скрипт.
/// Его единственная задача - в реальном времени отображать на экране
/// текущее значение GameState из GameManager.
/// </summary>
public class GameStateDisplay : MonoBehaviour
{
    // Ссылка на текстовый компонент, который будет отображать состояние.
    [SerializeField] private TMP_Text stateDisplayText;

    private void Update()
    {
        // Проверяем, что ссылка на текстовое поле установлена.
        if (stateDisplayText == null) return;
        
        // Проверяем, что GameManager существует, чтобы избежать ошибок при старте.
        if (GameManager.Instance == null) return;

        // Каждый кадр получаем текущее состояние из GameManager,
        // преобразуем его в строку и выводим на экран.
        stateDisplayText.text = $"Current State: {GameManager.Instance.CurrentState.ToString()}";
    }
}