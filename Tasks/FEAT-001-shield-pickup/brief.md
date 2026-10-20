# FEAT-001 — Shield pickup

**Request from design:**

We need a defensive power-up so players have a reason to stay aggressive.

- When an enemy dies there is a **15 % chance** it drops a glowing **cyan orb** at its position.
- Walking within ~1.2 units of the orb collects it. Uncollected orbs disappear after 10 seconds.
- Collecting grants a **shield worth 4 damage points that lasts 8 seconds**.
- Damage hits the shield first; only leftover damage reaches HP. When the 8 s are up, any remaining shield is lost.
- Picking up a second orb refreshes the timer (it does not stack above 4).
- The HUD shows a **cyan shield bar above the red HP bar** while a shield is active.
- Dashing still works as before.

Don't break existing tests.
