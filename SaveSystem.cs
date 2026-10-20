using System;
using System.IO;
using UnityEngine;

namespace NeonBreach.Systems
{
    [Serializable]
    public class SaveData
    {
        public int highScore;
        public int bestWave;
    }

    /// <summary>JSON save file in persistentDataPath. PathOverride lets tests redirect it.</summary>
    public static class SaveSystem
    {
        public static string PathOverride;

        static string FilePath
        {
            get { return PathOverride ?? Path.Combine(Application.persistentDataPath, "neonbreach_save.json"); }
        }

        public static SaveData Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
                    if (data != null) return data;
                }
            }
            catch (Exception e) { Debug.LogWarning("Save load failed: " + e.Message); }
            return new SaveData();
        }

        /// <returns>true if this run set a new high score</returns>
        public static bool SubmitRun(int score, int wave)
        {
            SaveData d = Load();
            bool newHigh = score > d.highScore;
            if (newHigh) d.highScore = score;
            if (wave > d.bestWave) d.bestWave = wave;
            Save(d);
            return newHigh;
        }

        static void Save(SaveData d)
        {
            try
            {
                string dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(FilePath, JsonUtility.ToJson(d, true));
            }
            catch (Exception e) { Debug.LogWarning("Save failed: " + e.Message); }
        }
    }
}
