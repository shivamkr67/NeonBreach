using System;
using UnityEngine;

namespace NeonBreach.Combat
{
    public interface IDamageable
    {
        /// <returns>true if damage was actually applied</returns>
        bool TakeDamage(float amount, Vector3 sourcePosition);
    }

    public class Health : MonoBehaviour, IDamageable
    {
        public float max = 10f;

        public float Current { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInvulnerable { get { return Time.time < invulnerableUntil; } }
        public float Normalized { get { return max > 0f ? Current / max : 0f; } }

        public event Action<Health, float, Vector3> Damaged;
        public event Action<Health> Died;

        float invulnerableUntil;

        void Awake() { Current = max; }

        public void ResetHealth(float newMax)
        {
            max = newMax;
            Current = newMax;
            IsDead = false;
            invulnerableUntil = 0f;
        }

        public void GrantInvulnerability(float duration)
        {
            invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + duration);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            Current = Mathf.Min(max, Current + amount);
        }

        public bool TakeDamage(float amount, Vector3 sourcePosition)
        {
            if (IsDead || amount <= 0f || IsInvulnerable) return false;

            Current = Mathf.Max(0f, Current - amount);
            if (Damaged != null) Damaged(this, amount, sourcePosition);

            if (Current <= 0f)
            {
                IsDead = true;
                if (Died != null) Died(this);
            }
            return true;
        }
    }
}
