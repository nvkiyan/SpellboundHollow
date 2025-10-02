namespace _SpellboundHollow.Scripts.UI
{
    using UnityEngine;
    using Characters;
    using Core;

    public class LetterUI : MonoBehaviour
    {
        private PlayerController _playerController;

        void Start()
        {
            // Проверяем, свободен ли игровой процесс. Если нет (например, уже запущен диалог),
            // то письмо не будет пытаться показаться.
            if (GameManager.Instance.CurrentState != GameState.Gameplay)
            {
                // Можно либо просто выйти, либо уничтожить объект, чтобы он не мешал.
                // Для начала просто выйдем.
                return; 
            }

            // Захватываем управление
            GameManager.Instance.SetGameState(GameState.Dialogue);
            _playerController = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            // Обрабатываем ввод, только если игра в состоянии диалога/UI.
            if (GameManager.Instance.CurrentState != GameState.Dialogue) return;
        
            if (Input.GetMouseButtonDown(0))
            {
                // При закрытии письма возвращаем управление.
                GameManager.Instance.SetGameState(GameState.Gameplay);
                gameObject.SetActive(false);
            }
        }

        private void CloseLetter()
        {
            gameObject.SetActive(false);
        }
    }
}