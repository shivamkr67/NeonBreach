using UnityEngine;
using NeonBreach.Bugs;
using NeonBreach.Combat;
using NeonBreach.Core;

namespace NeonBreach.Player
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement")]
        public float moveSpeed = 6.5f;
        public float maxHealth = 10f;

        [Header("Dash")]
        public float dashSpeed = 18f;
        public float dashDuration = 0.18f;
        public float dashCooldown = 2f;
        [Tooltip("How long the player is invulnerable after starting a dash")]
        public float dashIframes = 0.25f;

        public CharacterController Controller { get; private set; }
        public Health Health { get; private set; }
        public PlayerWeapon Weapon { get; private set; }
        public IInputSource InputSource { get; set; }
        public Vector3 AimDirection { get; private set; }
        public bool IsDashing { get { return dashing; } }
        public float DashCooldownRemaining { get { return Mathf.Max(0f, nextDashTime - Time.time); } }

        bool dashing;
        float dashEndTime;
        float nextDashTime;
        Vector3 dashDirection;

        public Vector3 MuzzlePosition { get { return transform.position + AimDirection * 0.8f; } }

        void Awake()
        {
            Instance = this;
            Controller = GetComponent<CharacterController>();
            Health = GetComponent<Health>();
            Weapon = GetComponent<PlayerWeapon>();
            if (InputSource == null) InputSource = GetComponent<IInputSource>();
            AimDirection = Vector3.forward;

            Health.ResetHealth(maxHealth);
            Health.Damaged += OnDamaged;
            Health.Died += OnDied;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (Health != null)
            {
                Health.Damaged -= OnDamaged;
                Health.Died -= OnDied;
            }
        }

        void Update()
        {
            if (Time.timeScale == 0f || Health.IsDead) return;

            PlayerInputState s = new PlayerInputState { WeaponSlot = -1 };
            bool hasInput = InputSource != null;
            if (hasInput) s = InputSource.Read(transform.position);

            if (s.WeaponSlot >= 0 && Weapon != null) Weapon.Select(s.WeaponSlot);

            if (hasInput) Aim(s.AimWorld);
            Move(s);

            if (s.Fire && Weapon != null) Weapon.TryFire(MuzzlePosition, AimDirection);
        }

        void Aim(Vector3 aimWorld)
        {
            Vector3 d = aimWorld - transform.position;
            d.y = 0f;
            if (d.sqrMagnitude < 0.01f) return;
            AimDirection = d.normalized;
            transform.rotation = Quaternion.LookRotation(AimDirection);
        }

        void Move(PlayerInputState s)
        {
            Vector3 moveInput = new Vector3(s.Move.x, 0f, s.Move.y);

            if (!dashing && s.DashPressed && Time.time >= nextDashTime)
                StartDash(moveInput.sqrMagnitude > 0.01f ? moveInput.normalized : AimDirection);

            if (dashing && Time.time >= dashEndTime) dashing = false;

            Vector3 velocity = dashing ? dashDirection * dashSpeed : moveInput * moveSpeed;
            Controller.Move(velocity * Time.deltaTime);

            Vector3 p = transform.position;
            if (!Mathf.Approximately(p.y, 1f)) transform.position = new Vector3(p.x, 1f, p.z);
        }

        void StartDash(Vector3 direction)
        {
            dashing = true;
            dashDirection = direction;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;

            float iframes = BugRegistry.IsActive(BugId.DashInvulnerabilityLeak) ? dashCooldown : dashIframes;
            Health.GrantInvulnerability(iframes);

            EventBus.Publish(new PlayerDashed { Direction = direction });
        }

        void OnDamaged(Health h, float amount, Vector3 source)
        {
            EventBus.Publish(new PlayerDamaged { Amount = amount, Remaining = h.Current });
        }

        void OnDied(Health h)
        {
            EventBus.Publish(new PlayerDied());
        }
    }
}
