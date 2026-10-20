using UnityEngine;
using NeonBreach.Enemies;
using NeonBreach.Player;

namespace NeonBreach.Testing
{
    /// <summary>
    /// Simple playtest bot: aims at the nearest enemy, kites away when enemies get close, dashes
    /// when cornered. Used for automated smoke tests and as an F2 autoplay toggle for demo footage.
    /// </summary>
    public class BotInput : IInputSource
    {
        public PlayerInputState Read(Vector3 p)
        {
            var s = new PlayerInputState { WeaponSlot = -1, AimWorld = p + Vector3.forward };

            EnemyAI nearest = null;
            float best = float.MaxValue;
            foreach (EnemyAI e in EnemyAI.Active)
            {
                if (e == null) continue;
                float d = (e.transform.position - p).sqrMagnitude;
                if (d < best) { best = d; nearest = e; }
            }

            Vector3 move = Vector3.zero;
            Vector3 toCenter = new Vector3(-p.x, 0f, -p.z);
            if (toCenter.magnitude > 10f) move += toCenter.normalized;

            if (nearest != null)
            {
                Vector3 to = nearest.transform.position - p;
                to.y = 0f;
                float dist = to.magnitude;
                Vector3 dir = dist > 0.01f ? to / dist : Vector3.forward;

                s.AimWorld = nearest.transform.position;
                s.Fire = true;
                if (dist < 6f) move += -dir;
                s.DashPressed = dist < 2f;
            }

            s.Move = Vector2.ClampMagnitude(new Vector2(move.x, move.z), 1f);
            return s;
        }
    }
}
