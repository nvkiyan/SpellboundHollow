using _SpellboundHollow.Scripts.Core;
using UnityEngine;

namespace _SpellboundHollow.Scripts.Gameplay
{
    public class SceneTransitionTrigger : MonoBehaviour, IInteractable
    {
        [Header("Transition Settings")]
        [Tooltip("Точное имя сцены для загрузки.")]
        [SerializeField] private string sceneToLoad;
        [Tooltip("ID точки входа (из компонента SceneEntryTrigger) в целевой сцене.")]
        [SerializeField] private string targetEntryPointId;
        
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2.5f;
        public float InteractionRadius => interactionRadius;

        public void Interact(Transform playerTransform)
        {
            if (string.IsNullOrEmpty(sceneToLoad) || string.IsNullOrEmpty(targetEntryPointId))
            {
                Debug.LogError("SceneTransitionTrigger не настроен! Укажите Scene To Load и Target Entry Point ID.", this);
                return;
            }
            
            if (SceneTransitionManager.Instance != null)
            {
                // Теперь передаем ID точки входа, а не позицию.
                SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetEntryPointId);
            }
            else
            {
                Debug.LogError("SceneTransitionManager.Instance не найден! Переход невозможен.", this);
            }
        }
    }
}