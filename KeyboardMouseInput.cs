using UnityEngine;

namespace NeonBreach.Player
{
    public struct PlayerInputState
    {
        public Vector2 Move;          // x = strafe, y = forward (world Z)
        public Vector3 AimWorld;      // world-space point the player is aiming at
        public bool Fire;
        public bool DashPressed;
        public int WeaponSlot;        // -1 = no change
        public bool PausePressed;
        public bool RestartPressed;
    }

    /// <summary>Abstraction so the player can be driven by keyboard, a bot, or a test script.</summary>
    public interface IInputSource
    {
        PlayerInputState Read(Vector3 playerPosition);
    }

    /// <summary>
    /// Uses the legacy Input Manager (Unity 2022.3 default). On Unity 6 set
    /// Project Settings > Player > Active Input Handling = Both.
    /// </summary>
    public class KeyboardMouseInput : MonoBehaviour, IInputSource
    {
        public Camera cam;

        public PlayerInputState Read(Vector3 playerPosition)
        {
            var s = new PlayerInputState();
            s.Move = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
            s.Fire = Input.GetMouseButton(0);
            s.DashPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift);
            s.PausePressed = Input.GetKeyDown(KeyCode.Escape);
            s.RestartPressed = Input.GetKeyDown(KeyCode.R);
            s.WeaponSlot = Input.GetKeyDown(KeyCode.Alpha1) ? 0
                         : Input.GetKeyDown(KeyCode.Alpha2) ? 1
                         : Input.GetKeyDown(KeyCode.Alpha3) ? 2 : -1;

            s.AimWorld = playerPosition + Vector3.forward;
            Camera c = cam != null ? cam : Camera.main;
            if (c != null)
            {
                var plane = new Plane(Vector3.up, playerPosition);
                Ray ray = c.ScreenPointToRay(Input.mousePosition);
                float distance;
                if (plane.Raycast(ray, out distance)) s.AimWorld = ray.GetPoint(distance);
            }
            return s;
        }
    }
}
