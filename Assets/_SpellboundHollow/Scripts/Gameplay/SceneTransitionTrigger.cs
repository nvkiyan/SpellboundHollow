using _SpellboundHollow.Scripts.Core;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    public class SceneTransitionTrigger : MonoBehaviour, IInteractable
    {
        [Header("Transition Settings")]
        [SerializeField] private string sceneToLoad;
        [SerializeField] private Transform targetEntryPoint;
        
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2.5f;
        public float InteractionRadius => interactionRadius;

        public void Interact(Transform playerTransform)
        {
            if (string.IsNullOrEmpty(sceneToLoad) || targetEntryPoint == null)
            {
                Debug.LogError("SceneTransitionTrigger не настроен!", this);
                return;
            }

            // Проверяем, существует ли SceneTransitionManager, прежде чем его вызывать
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetEntryPoint.position);
            }
            else
            {
                Debug.LogError("SceneTransitionManager.Instance не найден! Переход невозможен.", this);
            }
        }
    }
}