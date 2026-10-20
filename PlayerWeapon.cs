using UnityEngine;
using NeonBreach.Combat;
using NeonBreach.Core;
using NeonBreach.Data;

namespace NeonBreach.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        public WeaponData[] weapons = new WeaponData[0];
        public int prewarm = 32;

        ComponentPool<Projectile> pool;
        int index;
        float nextFireTime;

        public int Index { get { return index; } }
        public int ShotsFired { get; private set; }
        public ComponentPool<Projectile> Pool { get { return pool; } }
        public WeaponData Current { get { return weapons != null && weapons.Length > 0 ? weapons[Mathf.Clamp(index, 0, weapons.Length - 1)] : null; } }

        public void Select(int slot)
        {
            if (weapons == null || slot < 0 || slot >= weapons.Length) return;
            index = slot;
        }

        void EnsurePool()
        {
            if (pool != null) return;
            var root = new GameObject("ProjectilePool");
            pool = new ComponentPool<Projectile>(Projectile.CreateInstance, root.transform, prewarm);
        }

        public bool TryFire(Vector3 muzzle, Vector3 direction)
        {
            WeaponData w = Current;
            if (w == null || Time.time < nextFireTime) return false;

            EnsurePool();
            nextFireTime = Time.time + w.Cooldown;

            int n = Mathf.Max(1, w.projectilesPerShot);
            for (int i = 0; i < n; i++)
            {
                float angle = n == 1 ? 0f : Mathf.Lerp(-w.spreadDegrees * 0.5f, w.spreadDegrees * 0.5f, i / (float)(n - 1));
                Vector3 d = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                Projectile p = pool.Get();
                p.Launch(muzzle, d, w, pool);
            }

            ShotsFired++;
            EventBus.Publish(new ShotFired { Weapon = w.displayName });
            return true;
        }
    }
}
