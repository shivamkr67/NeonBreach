using UnityEngine;
using NeonBreach.Combat;
using NeonBreach.Data;
using NeonBreach.Player;

namespace NeonBreach.World
{
    /// <summary>
    /// Builds the arena and the player from primitives. Used by the editor scene builder AND by
    /// PlayMode tests, so the tested world is the same as the shipped world.
    /// </summary>
    public static class ArenaFactory
    {
        public const float DefaultHalfSize = 15f;

        public static Light CreateLight()
        {
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            return light;
        }

        public static Camera CreateCamera(float orthoSize = 17f)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f);
            go.transform.position = new Vector3(0f, 30f, 0f);
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            go.AddComponent<AudioListener>();
            return cam;
        }

        public static GameObject CreateGround(float half = DefaultHalfSize)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = "Ground";
            g.transform.position = new Vector3(0f, -0.5f, 0f);
            g.transform.localScale = new Vector3(half * 2f, 1f, half * 2f);
            g.AddComponent<ColorTint>().color = new Color(0.08f, 0.09f, 0.14f);
            return g;
        }

        public static GameObject CreateWalls(float half = DefaultHalfSize)
        {
            var root = new GameObject("Walls");
            float t = 1f;
            float len = half * 2f + 2f;
            Wall(root, new Vector3(0f, 1f, half + t * 0.5f), new Vector3(len, 2f, t));
            Wall(root, new Vector3(0f, 1f, -half - t * 0.5f), new Vector3(len, 2f, t));
            Wall(root, new Vector3(half + t * 0.5f, 1f, 0f), new Vector3(t, 2f, len));
            Wall(root, new Vector3(-half - t * 0.5f, 1f, 0f), new Vector3(t, 2f, len));
            return root;
        }

        static void Wall(GameObject parent, Vector3 pos, Vector3 scale)
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
            w.name = "Wall";
            w.transform.SetParent(parent.transform, false);
            w.transform.position = pos;
            w.transform.localScale = scale;
            w.AddComponent<ColorTint>().color = new Color(0.2f, 0.28f, 0.5f);
        }

        public static PlayerController CreatePlayer(Vector3 position, WeaponData[] weapons, bool keyboardInput = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.transform.position = position;
            Object.DestroyImmediate(go.GetComponent<Collider>());

            var cc = go.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;

            go.AddComponent<Health>();
            var weapon = go.AddComponent<PlayerWeapon>();
            weapon.weapons = weapons ?? new WeaponData[0];
            if (keyboardInput) go.AddComponent<KeyboardMouseInput>();
            go.AddComponent<ColorTint>().color = new Color(0.2f, 0.9f, 1f);
            var controller = go.AddComponent<PlayerController>();

            var nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nose.name = "Nose";
            Object.DestroyImmediate(nose.GetComponent<Collider>());
            nose.transform.SetParent(go.transform, false);
            nose.transform.localPosition = new Vector3(0f, 0.3f, 0.6f);
            nose.transform.localScale = new Vector3(0.25f, 0.25f, 0.5f);
            nose.AddComponent<ColorTint>().color = Color.white;

            return controller;
        }
    }
}
