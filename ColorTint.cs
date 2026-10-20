using UnityEngine;

namespace NeonBreach.World
{
    /// <summary>Applies a colour at runtime so scene objects need no material assets.</summary>
    public class ColorTint : MonoBehaviour
    {
        public Color color = Color.white;

        void Awake()
        {
            var r = GetComponent<Renderer>();
            if (r != null) r.material.color = color;
        }
    }
}
