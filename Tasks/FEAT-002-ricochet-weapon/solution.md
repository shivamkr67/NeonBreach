# FEAT-002 — Reference implementation (patches)

## 1. `WeaponData.cs`
```csharp
[Min(0)] public int ricochets = 0;
public float ricochetDamageBonus = 0.25f;
```

## 2. `Projectile.cs`

New fields: `int ricochetsLeft; float damageBonus;`

In `Launch(...)` (after `damage = weapon.damage;`):
```csharp
ricochetsLeft = weapon.ricochets;
damageBonus = weapon.ricochetDamageBonus;
```

Change the cast result handling so `HandleHit` receives the full hit:
```csharp
HandleHit(buffer[best]);
```
and replace `HandleHit`:
```csharp
void HandleHit(RaycastHit hit)
{
    Collider c = hit.collider;
    alreadyHit.Add(c);
    if (hitsRemaining <= 0) { Despawn(); return; }

    var target = c.GetComponentInParent<IDamageable>();
    if (target != null)
    {
        target.TakeDamage(damage, transform.position);
        hitsRemaining--;
    }
    else if (ricochetsLeft > 0)
    {
        // wall: mirror-reflect on the XZ plane, push out of the surface, grow damage
        Vector3 normal = hit.normal; normal.y = 0f; normal.Normalize();
        dir = Vector3.Reflect(dir, normal); dir.y = 0f; dir.Normalize();
        transform.position += normal * (radius + 0.02f);
        ricochetsLeft--;
        damage *= 1f + damageBonus;
        alreadyHit.Clear();
        return;
    }
    else
    {
        hitsRemaining = 0;
    }

    if (hitsRemaining <= 0) Despawn();
}
```

**Corner edge case:** a shot into a corner hits wall A, reflects into wall B, etc. Because `alreadyHit` is cleared
after each bounce and the bullet is pushed out along the normal, it can't tunnel or stick. The 3-bounce cap
guarantees termination; `range` guarantees cleanup.

## 3. `SceneBuilder.cs`
```csharp
WeaponData bouncer = Asset<WeaponData>("Weapon_Bouncer", w =>
{
    w.displayName = "Bouncer"; w.damage = 1.2f; w.fireRate = 3f; w.projectileSpeed = 22f;
    w.range = 40f; w.ricochets = 3; w.color = new Color(0.4f, 1f, 0.5f);
});
// ...CreatePlayer(..., new[] { pistol, scatter, rail, bouncer });
```
and in `KeyboardMouseInput`, extend the weapon-slot mapping with `Alpha4`.
