using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using NeonBreach.Bugs;
using NeonBreach.Data;
using NeonBreach.Player;
using NeonBreach.Systems;
using NeonBreach.Testing;
using NeonBreach.UI;
using NeonBreach.World;

namespace NeonBreach.Editor
{
    /// <summary>
    /// One click: NeonBreach > Build Demo Scene. Creates the ScriptableObject data assets and the
    /// Arena scene entirely from code, so the repo needs no binary art assets.
    /// Batch mode: Unity -batchmode -quit -projectPath . -executeMethod NeonBreach.Editor.SceneBuilder.BuildBatch
    /// </summary>
    public static class SceneBuilder
    {
        const string Root = "Assets/_Project";
        const string ScenePath = Root + "/Scenes/Arena.unity";

        [MenuItem("NeonBreach/Build Demo Scene")]
        public static void BuildMenu() { Build(); }

        public static void BuildBatch() { Build(); }

        public static void Build()
        {
            EnsureFolder(Root + "/Data");
            EnsureFolder(Root + "/Scenes");

            WeaponData pistol = Asset<WeaponData>("Weapon_Pistol", w =>
            {
                w.displayName = "Pulse Pistol"; w.damage = 1f; w.fireRate = 5f; w.projectileSpeed = 24f;
                w.range = 20f; w.pierce = 0; w.projectilesPerShot = 1; w.spreadDegrees = 0f; w.color = new Color(0.3f, 1f, 1f);
            });
            WeaponData scatter = Asset<WeaponData>("Weapon_Scatter", w =>
            {
                w.displayName = "Scatter"; w.damage = 0.7f; w.fireRate = 1.6f; w.projectileSpeed = 20f;
                w.range = 12f; w.pierce = 0; w.projectilesPerShot = 6; w.spreadDegrees = 35f; w.color = new Color(1f, 0.6f, 0.2f);
            });
            WeaponData rail = Asset<WeaponData>("Weapon_Rail", w =>
            {
                w.displayName = "Rail Lance"; w.damage = 3f; w.fireRate = 1f; w.projectileSpeed = 40f;
                w.range = 30f; w.pierce = 3; w.projectilesPerShot = 1; w.projectileScale = 0.3f; w.color = new Color(1f, 0.3f, 0.9f);
            });

            EnemyData grunt = Asset<EnemyData>("Enemy_Grunt", e =>
            {
                e.displayName = "Grunt"; e.maxHealth = 3f; e.moveSpeed = 2.6f; e.contactDamage = 1f; e.attackRange = 1.4f;
                e.attackCooldown = 0.9f; e.scoreValue = 10; e.color = new Color(0.9f, 0.2f, 0.25f); e.scale = 1f;
            });
            EnemyData runner = Asset<EnemyData>("Enemy_Runner", e =>
            {
                e.displayName = "Runner"; e.maxHealth = 1.5f; e.moveSpeed = 5f; e.contactDamage = 1f; e.attackRange = 1.2f;
                e.attackCooldown = 0.7f; e.scoreValue = 15; e.color = new Color(1f, 0.9f, 0.2f); e.scale = 0.7f;
            });
            EnemyData brute = Asset<EnemyData>("Enemy_Brute", e =>
            {
                e.displayName = "Brute"; e.maxHealth = 14f; e.moveSpeed = 1.6f; e.contactDamage = 3f; e.attackRange = 1.8f;
                e.attackCooldown = 1.4f; e.scoreValue = 50; e.color = new Color(0.6f, 0.3f, 0.9f); e.scale = 1.6f;
            });

            WaveSet waveSet = Asset<WaveSet>("WaveSet_Main", s =>
            {
                s.intermission = 3f;
                s.waves = new List<WaveDefinition>
                {
                    Wave("Warm-up", 0.7f, new Entry(grunt, 5)),
                    Wave("Rush", 0.45f, new Entry(grunt, 6), new Entry(runner, 4)),
                    Wave("Heavy", 0.6f, new Entry(grunt, 6), new Entry(brute, 1)),
                    Wave("Swarm", 0.25f, new Entry(runner, 14)),
                    Wave("Brute squad", 0.8f, new Entry(brute, 3), new Entry(grunt, 8)),
                    Wave("Final", 0.35f, new Entry(grunt, 10), new Entry(runner, 8), new Entry(brute, 3))
                };
            });
            AssetDatabase.SaveAssets();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ArenaFactory.CreateLight();
            ArenaFactory.CreateCamera();
            ArenaFactory.CreateGround();
            ArenaFactory.CreateWalls();
            PlayerController player = ArenaFactory.CreatePlayer(new Vector3(0f, 1f, 0f), new[] { pistol, scatter, rail });

            var systems = new GameObject("Systems");
            var score = systems.AddComponent<ScoreSystem>();
            var waves = systems.AddComponent<WaveManager>();
            waves.waveSet = waveSet;
            systems.AddComponent<BugConfig>();
            var gm = systems.AddComponent<GameManager>();
            gm.input = player.GetComponent<KeyboardMouseInput>();
            gm.score = score;
            gm.waves = waves;
            systems.AddComponent<Hud>();
            systems.AddComponent<AutoPlayToggle>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.Refresh();
            Debug.Log("NeonBreach: demo scene built at " + ScenePath + ". Press Play.");
        }

        struct Entry
        {
            public EnemyData Enemy; public int Count;
            public Entry(EnemyData e, int c) { Enemy = e; Count = c; }
        }

        static WaveDefinition Wave(string label, float interval, params Entry[] entries)
        {
            var def = new WaveDefinition { label = label, spawnInterval = interval };
            foreach (Entry e in entries) def.entries.Add(new SpawnEntry { enemy = e.Enemy, count = e.Count });
            return def;
        }

        static T Asset<T>(string name, Action<T> configure) where T : ScriptableObject
        {
            string path = Root + "/Data/" + name + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            configure(asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
