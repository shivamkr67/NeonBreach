using UnityEngine;

namespace NeonBreach.Core
{
    public enum GameState { Boot, Playing, Paused, GameOver, Victory }

    public struct GameStateChanged { public GameState State; }
    public struct EnemyKilled { public Vector3 Position; public int ScoreValue; public string EnemyName; }
    public struct EnemyDespawned { public GameObject Enemy; }
    public struct PlayerDamaged { public float Amount; public float Remaining; }
    public struct PlayerDied { }
    public struct PlayerDashed { public Vector3 Direction; }
    public struct ShotFired { public string Weapon; }
    public struct WaveStarted { public int Wave; public int EnemyCount; }
    public struct WaveCleared { public int Wave; }
    public struct AllWavesCleared { }
    public struct ScoreChanged { public int Score; public int Combo; public int Multiplier; }
}
