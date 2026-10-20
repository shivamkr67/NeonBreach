# BUG-003 — Reference solution

**Root cause:** `WaveManager` decrements `AliveCount` in two places for one death: on `EnemyKilled` (legacy
bookkeeping) *and* on `EnemyDespawned`. Every enemy is counted twice, so the counter hits zero when half are dead.

```csharp
// Assets/_Project/Scripts/Systems/WaveManager.cs
// BEFORE (bug active)
void OnEnemyKilled(EnemyKilled e)
{
    if (BugRegistry.IsActive(BugId.WaveDoubleDecrement))
        AliveCount = Mathf.Max(0, AliveCount - 1);
}

// AFTER — single source of truth: EnemyDespawned (fires for kills AND any other removal)
// Delete OnEnemyKilled and its Subscribe/Unsubscribe lines.
```
