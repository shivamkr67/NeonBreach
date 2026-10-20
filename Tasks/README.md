# Evaluation tasks

| ID | Type | Difficulty | Systems touched | Bug flag |
|----|------|-----------|-----------------|----------|
| BUG-001 | Bugfix | Easy | Player dash, Health | `DashInvulnerabilityLeak` |
| BUG-002 | Bugfix | Medium | Object pooling, Projectile, Weapon | `StalePierceCounter` |
| BUG-003 | Bugfix | Medium | Event bus, Wave manager, Enemy lifecycle | `WaveDoubleDecrement` |
| BUG-004 | Bugfix | Easy | Score / combo timing | `ComboTimerNotRefreshed` |
| FEAT-001 | Feature | Medium | Health, Event bus, Pickups, HUD | — |
| FEAT-002 | Feature | Hard | Projectile physics, WeaponData, Scene builder | — |

How to run a bug task: open `Arena.unity`, select the `Systems` object, add the bug id to
`BugConfig > Active Bugs`, press Play, and follow `brief.md`. Or launch a build with `-bugs=<BugId>`.
Press **F1** in game for the task-author debug overlay.

Run the acceptance tests: Window > General > Test Runner > PlayMode. Tests in category
`BugRepro` assert the buggy behaviour (they prove the bug is reproducible); the other tests assert correct behaviour.
