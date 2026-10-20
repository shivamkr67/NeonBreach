# FEAT-001 — Reference implementation

Files in `reference/`: `ShieldPickup.cs`, `PickupSpawner.cs`. Add both to `Assets/_Project/Scripts/Systems/`
(namespace `NeonBreach.Systems`), then apply the two patches below and add `PickupSpawner` to the `Systems` object.

## 1. `Health.cs` — shield state and absorption

```csharp
// new members
public float Shield { get; private set; }
float shieldUntil;
public float ActiveShield { get { return Time.time < shieldUntil ? Shield : 0f; } }

public void AddShield(float amount, float duration)
{
    Shield = Mathf.Max(Time.time < shieldUntil ? Shield : 0f, amount);
    shieldUntil = Time.time + duration;
}

// inside TakeDamage, right after the invulnerability / dead / <=0 early-out:
float shield = ActiveShield;
if (shield > 0f)
{
    float absorbed = Mathf.Min(shield, amount);
    Shield = shield - absorbed;
    amount -= absorbed;
    if (amount <= 0f) return true;   // fully absorbed: no HP loss, no death
}
```

## 2. `Hud.cs` — shield bar (inside `if (player != null)`)

```csharp
float shield = player.Health.ActiveShield;
if (shield > 0f)
    Bar(new Rect(20, 2, 260, 14), shield / 4f, new Color(0.3f, 0.9f, 1f));
```

## 3. Wiring
`PickupSpawner` subscribes to `EnemyKilled` on the event bus and spawns `ShieldPickup` orbs, so no existing system needs a
reference to pickups.

## Design notes
- Shield is tracked in `Health`, not the controller, so enemies/bosses can reuse it later.
- The orb uses a distance check against `PlayerController.Instance` instead of physics triggers, matching the
  rest of the project (no layers or rigidbodies required).
