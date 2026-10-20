using UnityEngine;

namespace NeonBreach.Data
{
    [CreateAssetMenu(menuName = "NeonBreach/Enemy", fileName = "Enemy")]
    public class EnemyData : ScriptableObject
    {
        public string displayName = "Grunt";
        public float maxHealth = 3f;
        public float moveSpeed = 2.6f;
        public float contactDamage = 1f;
        public float attackRange = 1.4f;
        public float attackCooldown = 0.9f;
        public int scoreValue = 10;
        public Color color = Color.red;
        public float scale = 1f;
    }
}
