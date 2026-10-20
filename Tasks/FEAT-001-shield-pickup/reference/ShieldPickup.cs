using UnityEngine;
using NeonBreach.Player;

namespace NeonBreach.Systems
{
    public class ShieldPickup : MonoBehaviour
    {
        public float amount = 4f;
        public float duration = 8f;
        public float pickupRadius = 1.2f;
        public float lifetime = 10f;

        float spawnTime;
        Vector3 basePosition;

        void Start()
        {
            spawnTime = Time.time;
            basePosition = transform.position;
        }

        void Update()
        {
            transform.Rotate(0f, 180f * Time.deltaTime, 0f);
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * 3f) * 0.15f);

            if (Time.time - spawnTime > lifetime) { Destroy(gameObject); return; }

            PlayerController p = PlayerController.Instance;
            if (p == null || p.Health.IsDead) return;

            Vector3 d = p.transform.position - transform.position;
            d.y = 0f;
            if (d.sqrMagnitude <= pickupRadius * pickupRadius)
            {
                p.Health.AddShield(amount, duration);
                Destroy(gameObject);
            }
        }
    }
}
