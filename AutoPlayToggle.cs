using UnityEngine;
using NeonBreach.Player;

namespace NeonBreach.Testing
{
    /// <summary>Press F2 in play mode to hand the player over to the bot (and back).</summary>
    public class AutoPlayToggle : MonoBehaviour
    {
        readonly BotInput bot = new BotInput();
        IInputSource human;
        bool botActive;

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F2)) return;
            PlayerController p = PlayerController.Instance;
            if (p == null) return;

            if (!botActive) { human = p.InputSource; p.InputSource = bot; }
            else p.InputSource = human;
            botActive = !botActive;
        }
    }
}
