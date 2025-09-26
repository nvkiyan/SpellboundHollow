namespace _SpellboundHollow.Scripts.Gameplay
{
    using UnityEngine;
    using Core;

    public class InteractableStudyItem : MonoBehaviour
    {
        [Header("Данные")]
        [SerializeField] private StudyItemDataSO itemData;

        public void Study()
        {
            GameManager.Instance.GrimoireManager.AddStudiedItem(itemData);
            Destroy(gameObject); // Уничтожаем цветок после сбора
        }
    }
}
