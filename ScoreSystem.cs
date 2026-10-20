using UnityEngine;
using NeonBreach.Bugs;
using NeonBreach.Core;

namespace NeonBreach.Systems
{
    /// <summary>Score + combo. Every kill within comboWindow seconds of the previous one extends the combo.</summary>
    public class ScoreSystem : MonoBehaviour
    {
        public float comboWindow = 2.5f;
        public int maxCombo = 50;

        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int Multiplier { get { return 1 + Combo / 5; } }

        float lastKillTime;

        void OnEnable() { EventBus.Subscribe<EnemyKilled>(OnKilled); }
        void OnDisable() { EventBus.Unsubscribe<EnemyKilled>(OnKilled); }

        void OnKilled(EnemyKilled e)
        {
            // Restart the combo timer on every kill.
            if (Combo == 0 || !BugRegistry.IsActive(BugId.ComboTimerNotRefreshed))
                lastKillTime = Time.time;

            Combo = Mathf.Min(Combo + 1, maxCombo);
            Score += e.ScoreValue * Multiplier;
            Publish();
        }

        void Update()
        {
            if (Combo > 0 && Time.time - lastKillTime > comboWindow)
            {
                Combo = 0;
                Publish();
            }
        }

        void Publish()
        {
            EventBus.Publish(new ScoreChanged { Score = Score, Combo = Combo, Multiplier = Multiplier });
        }
    }
}
