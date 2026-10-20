# BUG-004 — Reference solution

**Root cause:** `lastKillTime` is only updated when the combo is 0 (first kill of a chain), so the window is
measured from the start of the chain instead of from the most recent kill.

```csharp
// Assets/_Project/Scripts/Systems/ScoreSystem.cs  (OnKilled)
// BEFORE (bug active)
if (Combo == 0 || !BugRegistry.IsActive(BugId.ComboTimerNotRefreshed))
    lastKillTime = Time.time;

// AFTER
lastKillTime = Time.time;
```
