namespace _SpellboundHollow.Scripts.UI
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using _SpellboundHollow.Scripts.Characters;

    public class LetterUI : MonoBehaviour
    {
        private PlayerController _playerController;

        void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            if (_playerController != null) _playerController.DisableMovement();
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                CloseLetter();
            }
        }

        private void CloseLetter()
        {
            if (_playerController != null) _playerController.EnableMovement();
            gameObject.SetActive(false);
        }
    }
}