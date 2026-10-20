using UnityEngine;
using NeonBreach.Player;

namespace NeonBreach.Testing
{
    /// <summary>Deterministic input for tests and task verification. Fire/dash are one-shot per queue call.</summary>
    public class ScriptedInput : IInputSource
    {
        public Vector2 Move;
        public Vector3 AimWorld;
        public bool HoldFire;

        bool fireQueued, dashQueued;

        public void QueueFire() { fireQueued = true; }
        public void QueueDash() { dashQueued = true; }

        public PlayerInputState Read(Vector3 playerPosition)
        {
            var s = new PlayerInputState
            {
                Move = Move,
                AimWorld = AimWorld == Vector3.zero ? playerPosition + Vector3.forward : AimWorld,
                Fire = HoldFire || fireQueued,
                DashPressed = dashQueued,
                WeaponSlot = -1
            };
            fireQueued = false;
            dashQueued = false;
            return s;
        }
    }
}
