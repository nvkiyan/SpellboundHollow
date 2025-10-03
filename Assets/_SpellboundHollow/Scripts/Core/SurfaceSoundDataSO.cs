using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    [CreateAssetMenu(fileName = "NewSurfaceSoundData", menuName = "Spellbound Hollow/Surface Sound Data")]
    public class SurfaceSoundDataSO : ScriptableObject
    {
        [Tooltip("Тег поверхности, например, 'Surface_Grass'.")]
        public string surfaceTag;
        [Tooltip("Зацикленный звук шагов для этой поверхности.")]
        public AudioClip footstepSound;
    }
}