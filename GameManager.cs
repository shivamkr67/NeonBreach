using UnityEngine;
using UnityEngine.SceneManagement;
using NeonBreach.Core;
using NeonBreach.Player;

namespace NeonBreach.Systems
{
    /// <summary>Game state machine: Playing, Paused, GameOver, Victory. Owns pause, restart and saving.</summary>
    public class GameManager : MonoBehaviour
    {
        public KeyboardMouseInput input;
        public ScoreSystem score;
        public WaveManager waves;

        public GameState State { get; private set; }
        public int HighScore { get; private set; }
        public bool NewHighScore { get; private set; }

        void OnEnable()
        {
            EventBus.Subscribe<PlayerDied>(OnPlayerDied);
            EventBus.Subscribe<AllWavesCleared>(OnAllWavesCleared);
        }

        void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDied>(OnPlayerDied);
            EventBus.Unsubscribe<AllWavesCleared>(OnAllWavesCleared);
        }

        void OnDestroy() { Time.timeScale = 1f; }

        void Start()
        {
            HighScore = SaveSystem.Load().highScore;
            SetState(GameState.Playing);
        }

        void Update()
        {
            if (input == null) return;
            PlayerInputState s = input.Read(Vector3.zero);

            if (s.PausePressed)
            {
                if (State == GameState.Playing) SetState(GameState.Paused);
                else if (State == GameState.Paused) SetState(GameState.Playing);
            }
            if (s.RestartPressed && State != GameState.Playing) Restart();
        }

        void OnPlayerDied(PlayerDied e) { Finish(GameState.GameOver); }
        void OnAllWavesCleared(AllWavesCleared e) { Finish(GameState.Victory); }

        void Finish(GameState end)
        {
            if (State != GameState.Playing) return;
            int s = score != null ? score.Score : 0;
            int w = waves != null ? waves.CurrentWave : 0;
            NewHighScore = SaveSystem.SubmitRun(s, w);
            if (NewHighScore) HighScore = s;
            SetState(end);
        }

        void SetState(GameState next)
        {
            State = next;
            Time.timeScale = next == GameState.Paused ? 0f : 1f;
            EventBus.Publish(new GameStateChanged { State = next });
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
