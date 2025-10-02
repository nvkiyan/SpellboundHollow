using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    /// <summary>
    /// Интерфейс для всех объектов, с которыми можно взаимодействовать.
    /// Требует от объекта предоставить радиус взаимодействия и метод для самого действия.
    /// </summary>
    public interface IInteractable
    {
        float InteractionRadius { get; }
        void Interact(Transform playerTransform);
    }
}