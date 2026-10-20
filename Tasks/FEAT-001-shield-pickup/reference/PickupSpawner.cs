using UnityEngine;
using NeonBreach.Core;

namespace NeonBreach.Systems
{
    public class PickupSpawner : MonoBehaviour
    {
        [Range(0f, 1f)] public float dropChance = 0.15f;

        void OnEnable() { EventBus.Subscribe<EnemyKilled>(OnKilled); }
        void OnDisable() { EventBus.Unsubscribe<EnemyKilled>(OnKilled); }

        void OnKilled(EnemyKilled e)
        {
            if (Random.value > dropChance) return;

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "ShieldPickup";
            Destroy(orb.GetComponent<Collider>());
            orb.transform.position = new Vector3(e.Position.x, 0.8f, e.Position.z);
            orb.transform.localScale = Vector3.one * 0.5f;
            orb.GetComponent<Renderer>().material.color = new Color(0.3f, 0.9f, 1f);
            orb.AddComponent<ShieldPickup>();
        }
    }
}
