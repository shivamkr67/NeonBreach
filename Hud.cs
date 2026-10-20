using UnityEngine;
using NeonBreach.Bugs;
using NeonBreach.Core;
using NeonBreach.Enemies;
using NeonBreach.Player;
using NeonBreach.Systems;

namespace NeonBreach.UI
{
    /// <summary>IMGUI heads-up display. F1 toggles the task-author debug overlay.</summary>
    public class Hud : MonoBehaviour
    {
        PlayerController player;
        ScoreSystem score;
        WaveManager waves;
        GameManager game;
        GUIStyle label, big;
        string banner = "";
        float bannerUntil;
        bool showDebug;

        void OnEnable()
        {
            EventBus.Subscribe<WaveStarted>(OnWaveStarted);
            EventBus.Subscribe<WaveCleared>(OnWaveCleared);
        }

        void OnDisable()
        {
            EventBus.Unsubscribe<WaveStarted>(OnWaveStarted);
            EventBus.Unsubscribe<WaveCleared>(OnWaveCleared);
        }

        void Start()
        {
            score = FindFirstObjectByType<ScoreSystem>();
            waves = FindFirstObjectByType<WaveManager>();
            game = FindFirstObjectByType<GameManager>();
        }

        void Update()
        {
            if (player == null) player = PlayerController.Instance;
            if (Input.GetKeyDown(KeyCode.F1)) showDebug = !showDebug;
        }

        void OnWaveStarted(WaveStarted e) { banner = "WAVE " + e.Wave; bannerUntil = Time.unscaledTime + 2f; }
        void OnWaveCleared(WaveCleared e) { banner = "WAVE " + e.Wave + " CLEARED"; bannerUntil = Time.unscaledTime + 2f; }

        void EnsureStyles()
        {
            if (label != null) return;
            label = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            label.normal.textColor = Color.white;
            big = new GUIStyle(label) { fontSize = 44, alignment = TextAnchor.MiddleCenter };
        }

        static void Bar(Rect r, float t, Color c)
        {
            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = c;
            GUI.DrawTexture(new Rect(r.x + 2f, r.y + 2f, (r.width - 4f) * Mathf.Clamp01(t), r.height - 4f), Texture2D.whiteTexture);
            GUI.color = prev;
        }

        void OnGUI()
        {
            EnsureStyles();
            float w = Screen.width, h = Screen.height;

            if (player != null)
            {
                Bar(new Rect(20, 20, 260, 18), player.Health.Normalized, new Color(0.9f, 0.2f, 0.3f));
                GUI.Label(new Rect(24, 38, 260, 24), "HP " + player.Health.Current.ToString("0.#") + " / " + player.Health.max.ToString("0.#"), label);

                float dash = 1f - (player.dashCooldown > 0f ? player.DashCooldownRemaining / player.dashCooldown : 0f);
                Bar(new Rect(20, h - 40, 200, 14), dash, new Color(0.3f, 0.8f, 1f));
                GUI.Label(new Rect(20, h - 66, 300, 24), "DASH [Space]", label);

                if (player.Weapon != null && player.Weapon.Current != null)
                    GUI.Label(new Rect(w - 320, h - 40, 300, 24), "[" + (player.Weapon.Index + 1) + "] " + player.Weapon.Current.displayName, label);
            }

            if (score != null)
            {
                GUI.Label(new Rect(20, 66, 300, 24), "SCORE " + score.Score, label);
                if (score.Combo > 1) GUI.Label(new Rect(20, 90, 300, 24), "COMBO x" + score.Combo + "  (mult " + score.Multiplier + ")", label);
            }

            if (waves != null && waves.waveSet != null)
                GUI.Label(new Rect(w - 320, 20, 300, 24), "WAVE " + waves.CurrentWave + " / " + waves.waveSet.waves.Count + "   Enemies: " + waves.AliveCount, label);

            if (game != null) GUI.Label(new Rect(w - 320, 44, 300, 24), "BEST " + game.HighScore, label);

            if (Time.unscaledTime < bannerUntil) GUI.Label(new Rect(0, h * 0.2f, w, 70), banner, big);

            if (game != null)
            {
                string msg = null;
                if (game.State == GameState.Paused) msg = "PAUSED\nESC to resume | R to restart";
                else if (game.State == GameState.GameOver) msg = "GAME OVER\nPress R to restart";
                else if (game.State == GameState.Victory) msg = "VICTORY\nPress R to play again";
                if (msg != null) GUI.Label(new Rect(0, h * 0.35f, w, 160), msg + (game.NewHighScore ? "\nNEW HIGH SCORE!" : ""), big);
            }

            GUI.Label(new Rect(20, h - 92, 500, 24), "WASD move | Mouse aim+fire | 1-3 weapons | F1 debug | F2 autoplay", label);

            if (showDebug)
            {
                string bugs = "";
                foreach (BugId b in BugRegistry.Active) bugs += b + " ";
                if (bugs.Length == 0) bugs = "(none)";
                GUI.Box(new Rect(20, 130, 460, 90), "");
                GUI.Label(new Rect(28, 134, 450, 24), "ACTIVE BUGS: " + bugs, label);
                GUI.Label(new Rect(28, 160, 450, 24), "Enemies on screen: " + EnemyAI.Active.Count + "  | WaveManager alive: " + (waves != null ? waves.AliveCount : 0), label);
                GUI.Label(new Rect(28, 186, 450, 24), "FPS: " + (1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime)).ToString("0"), label);
            }
        }
    }
}
