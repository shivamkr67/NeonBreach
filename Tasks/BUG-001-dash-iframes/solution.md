# BUG-001 — Reference solution

**Root cause:** `PlayerController.StartDash` grants invulnerability using `dashCooldown` (2 s) instead of `dashIframes` (0.25 s).

```csharp
// Assets/_Project/Scripts/Player/PlayerController.cs  (StartDash)
// BEFORE (bug active)
float iframes = BugRegistry.IsActive(BugId.DashInvulnerabilityLeak) ? dashCooldown : dashIframes;
Health.GrantInvulnerability(iframes);

// AFTER
Health.GrantInvulnerability(dashIframes);
```

**Why it's tricky:** the code *does* grant i-frames and the dash works, so the symptom ("sometimes I can't be hit")
only appears in playtesting when enemies are near right after a dash.
