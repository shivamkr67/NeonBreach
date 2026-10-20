using System.Collections.Generic;
using UnityEngine;
using NeonBreach.Bugs;
using NeonBreach.Core;
using NeonBreach.Data;
using NeonBreach.Player;

namespace NeonBreach.Combat
{
    /// <summary>
    /// Pooled, swept-sphere projectile. Uses a sphere cast per frame (not physics bodies) so fast
    /// bullets never tunnel through enemies, regardless of frame rate.
    /// </summary>
    public class Projectile : MonoBehaviour, IPoolable
    {
        static readonly RaycastHit[] buffer = new RaycastHit[16];

        readonly HashSet<Collider> alreadyHit = new HashSet<Collider>();
        ComponentPool<Projectile> pool;
        WeaponData cachedWeapon;
        Renderer rend;
        Vector3 dir;
        float speed, damage, radius, remainingRange;
        int hitsRemaining;

        public static Projectile CreateInstance()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Projectile";
            DestroyImmediate(go.GetComponent<Collider>());
            return go.AddComponent<Projectile>();
        }

        void Awake() { rend = GetComponent<Renderer>(); }

        public void OnSpawn() { alreadyHit.Clear(); }
        public void OnDespawn() { }

        public void Launch(Vector3 position, Vector3 direction, WeaponData weapon, ComponentPool<Projectile> owner)
        {
            pool = owner;
            transform.position = position;
            dir = direction.normalized;
            speed = weapon.projectileSpeed;
            damage = weapon.damage;
            remainingRange = weapon.range;
            radius = weapon.projectileScale * 0.5f;
            transform.localScale = Vector3.one * weapon.projectileScale;
            if (rend == null) rend = GetComponent<Renderer>();
            if (rend != null) rend.material.color = weapon.color;

            // Refresh the pierce budget for every shot.
            bool refreshBudget = BugRegistry.IsActive(BugId.StalePierceCounter) ? cachedWeapon != weapon : true;
            if (refreshBudget) hitsRemaining = weapon.pierce + 1;
            cachedWeapon = weapon;
        }

        void Update()
        {
            float step = speed * Time.deltaTime;
            if (step <= 0f) return;

            Vector3 origin = transform.position;
            int count = Physics.SphereCastNonAlloc(origin, radius, dir, buffer, step, ~0, QueryTriggerInteraction.Ignore);

            int best = -1;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Collider c = buffer[i].collider;
                if (c == null || alreadyHit.Contains(c)) continue;
                if (c.GetComponentInParent<PlayerController>() != null) continue;
                if (buffer[i].distance < bestDistance) { bestDistance = buffer[i].distance; best = i; }
            }

            if (best >= 0)
            {
                transform.position = origin + dir * bestDistance;
                HandleHit(buffer[best].collider);
                if (!gameObject.activeSelf) return;
            }
            else
            {
                transform.position = origin + dir * step;
            }

            remainingRange -= step;
            if (remainingRange <= 0f) Despawn();
        }

        void HandleHit(Collider c)
        {
            alreadyHit.Add(c);

            // Defensive guard: a projectile with no hit budget must not deal damage.
            if (hitsRemaining <= 0) { Despawn(); return; }

            var target = c.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage, transform.position);
                hitsRemaining--;
            }
            else
            {
                hitsRemaining = 0; // hit a wall
            }

            if (hitsRemaining <= 0) Despawn();
        }

        void Despawn()
        {
            if (pool != null) pool.Release(this);
            else gameObject.SetActive(false);
        }
    }
}
