using System.Collections.Generic;
using UnityEngine;
using NeonBreach.Combat;
using NeonBreach.Core;
using NeonBreach.Data;
using NeonBreach.Player;

namespace NeonBreach.Enemies
{
    /// <summary>Pooled melee enemy driven by a small state machine: Chase -> Attack -> Stunned.</summary>
    [RequireComponent(typeof(Health))]
    public class EnemyAI : MonoBehaviour, IPoolable
    {
        public enum State { Chase, Attack, Stunned }

        public static readonly List<EnemyAI> Active = new List<EnemyAI>();

        public State Current { get; private set; }
        public Health Health { get { return health; } }
        public EnemyData Data { get { return data; } }

        EnemyData data;
        Health health;
        Renderer rend;
        ComponentPool<EnemyAI> pool;
        Color baseColor;
        Vector3 knockback;
        float nextAttackTime, stunUntil, flashUntil;

        public static EnemyAI CreateInstance()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy";
            go.AddComponent<Health>();
            return go.AddComponent<EnemyAI>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { Active.Clear(); }

        void Awake()
        {
            health = GetComponent<Health>();
            rend = GetComponent<Renderer>();
            health.Damaged += OnDamaged;
            health.Died += OnDied;
        }

        void OnDestroy()
        {
            Active.Remove(this);
            if (health != null)
            {
                health.Damaged -= OnDamaged;
                health.Died -= OnDied;
            }
        }

        public void OnSpawn() { if (!Active.Contains(this)) Active.Add(this); }
        public void OnDespawn() { Active.Remove(this); }

        public void Init(EnemyData d, Vector3 position, ComponentPool<EnemyAI> owner)
        {
            data = d;
            pool = owner;
            transform.localScale = Vector3.one * d.scale;
            transform.position = new Vector3(position.x, d.scale, position.z);
            health.ResetHealth(d.maxHealth);
            baseColor = d.color;
            if (rend != null) rend.material.color = baseColor;
            knockback = Vector3.zero;
            flashUntil = 0f;
            Current = State.Chase;
            nextAttackTime = Time.time + 0.5f;
        }

        void Update()
        {
            if (data == null || health.IsDead) return;

            if (flashUntil > 0f && Time.time >= flashUntil)
            {
                flashUntil = 0f;
                if (rend != null) rend.material.color = baseColor;
            }

            if (knockback.sqrMagnitude > 0.01f)
            {
                transform.position += knockback * Time.deltaTime;
                knockback = Vector3.Lerp(knockback, Vector3.zero, 10f * Time.deltaTime);
            }

            PlayerController player = PlayerController.Instance;
            if (player == null || player.Health.IsDead) return;

            Vector3 to = player.transform.position - transform.position;
            to.y = 0f;
            float dist = to.magnitude;
            Vector3 dirToPlayer = dist > 0.001f ? to / dist : Vector3.forward;

            switch (Current)
            {
                case State.Stunned:
                    if (Time.time >= stunUntil) Current = State.Chase;
                    break;

                case State.Chase:
                    if (dist <= data.attackRange) { Current = State.Attack; break; }
                    transform.position += dirToPlayer * data.moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(dirToPlayer);
                    break;

                case State.Attack:
                    if (dist > data.attackRange * 1.25f) { Current = State.Chase; break; }
                    transform.rotation = Quaternion.LookRotation(dirToPlayer);
                    if (Time.time >= nextAttackTime)
                    {
                        nextAttackTime = Time.time + data.attackCooldown;
                        player.Health.TakeDamage(data.contactDamage, transform.position);
                    }
                    break;
            }
        }

        void OnDamaged(Health h, float amount, Vector3 source)
        {
            Vector3 away = transform.position - source;
            away.y = 0f;
            knockback = away.sqrMagnitude > 0.001f ? away.normalized * 4f : Vector3.zero;
            Current = State.Stunned;
            stunUntil = Time.time + 0.08f;
            flashUntil = Time.time + 0.06f;
            if (rend != null) rend.material.color = Color.white;
        }

        void OnDied(Health h)
        {
            EventBus.Publish(new EnemyKilled
            {
                Position = transform.position,
                ScoreValue = data != null ? data.scoreValue : 0,
                EnemyName = data != null ? data.displayName : "Enemy"
            });
            Despawn();
        }

        public void Despawn()
        {
            if (!gameObject.activeSelf) return;
            EventBus.Publish(new EnemyDespawned { Enemy = gameObject });
            if (pool != null) pool.Release(this);
            else { OnDespawn(); gameObject.SetActive(false); }
        }
    }
}
