using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBreach.Data
{
    [Serializable]
    public class SpawnEntry
    {
        public EnemyData enemy;
        [Min(1)] public int count = 3;
    }

    [Serializable]
    public class WaveDefinition
    {
        public string label = "Wave";
        public float spawnInterval = 0.6f;
        public List<SpawnEntry> entries = new List<SpawnEntry>();

        public int TotalEnemies()
        {
            int total = 0;
            foreach (SpawnEntry e in entries) total += e.count;
            return total;
        }
    }

    [CreateAssetMenu(menuName = "NeonBreach/Wave Set", fileName = "WaveSet")]
    public class WaveSet : ScriptableObject
    {
        public float intermission = 3f;
        public List<WaveDefinition> waves = new List<WaveDefinition>();
    }
}
