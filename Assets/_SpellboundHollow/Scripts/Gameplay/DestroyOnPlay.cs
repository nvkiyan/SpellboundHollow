using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    /// <summary>
    /// Этот компонент уничтожает GameObject, на котором он находится,
    /// сразу после запуска игры. Идеально для объектов, нужных только в редакторе.
    /// </summary>
    public class DestroyOnPlay : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}