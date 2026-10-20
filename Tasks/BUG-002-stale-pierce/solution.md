# BUG-002 — Reference solution

**Root cause:** `Projectile.Launch` only recomputes `hitsRemaining` when the weapon *changes*. A pooled bullet that
already spent its budget (`hitsRemaining == 0`) is reused with the same weapon, so the defensive guard in
`HandleHit` despawns it before it deals damage.

```csharp
// Assets/_Project/Scripts/Combat/Projectile.cs  (Launch)
// BEFORE (bug active)
bool refreshBudget = BugRegistry.IsActive(BugId.StalePierceCounter) ? cachedWeapon != weapon : true;
if (refreshBudget) hitsRemaining = weapon.pierce + 1;
cachedWeapon = weapon;

// AFTER — pooled state must be fully re-initialised on every spawn
hitsRemaining = weapon.pierce + 1;
cachedWeapon = weapon;
```

**Lesson:** object pools must reset *all* mutable state in `Launch`/`OnSpawn`. Bugs like this only show up after the
pool has recycled objects, so they need sustained playtesting.
