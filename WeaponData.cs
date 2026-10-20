using UnityEngine;

namespace NeonBreach.Data
{
    [CreateAssetMenu(menuName = "NeonBreach/Weapon", fileName = "Weapon")]
    public class WeaponData : ScriptableObject
    {
        public string displayName = "Pistol";
        public float damage = 1f;
        [Tooltip("Shots per second")] public float fireRate = 5f;
        public float projectileSpeed = 24f;
        public float range = 20f;
        [Min(1)] public int projectilesPerShot = 1;
        public float spreadDegrees = 0f;
        [Tooltip("Extra enemies a projectile can pass through")] [Min(0)] public int pierce = 0;
        public Color color = Color.cyan;
        public float projectileScale = 0.25f;

        public float Cooldown { get { return 1f / Mathf.Max(0.01f, fireRate); } }
    }
}
