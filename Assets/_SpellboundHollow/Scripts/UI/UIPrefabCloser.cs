using _SpellboundHollow.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _SpellboundHollow.Scripts.UI
{
    public class UIPrefabCloser : MonoBehaviour
    {
        [Tooltip("Кнопка, которая будет закрывать это окно.")]
        [SerializeField] private Button closeButton;

        private void Start()
        {
            if (closeButton == null)
            {
                closeButton = GetComponentInChildren<Button>();
                if (closeButton == null)
                {
                    Debug.LogError("Кнопка 'Закрыть' не назначена и не найдена в дочерних объектах!", this);
                    return;
                }
            }
            
            closeButton.onClick.AddListener(CloseWindow);
        }
        
        public void CloseWindow()
        {
            if (GameManager.Instance.CurrentState == GameState.Paused)
            {
                GameManager.Instance.SetGameState(GameState.Gameplay);
            }
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseWindow);
            }
        }
    }
}