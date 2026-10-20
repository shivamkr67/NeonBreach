# FEAT-002 — Ricochet "Bouncer" weapon

**Request from design:**

Add a fourth weapon (key **4**): the **Bouncer**.

- Fires one bullet at 3 shots/s, 1.2 damage, speed 22, range 40, any colour you like.
- When the bullet hits an **arena wall** it bounces (mirror reflection) instead of disappearing, up to **3 times**.
- Each bounce makes the bullet deal **+25 % damage** (so the bullet deals 1.2, 1.5, 1.8, 2.1 over its life).
- A bullet that hits an enemy still follows the normal pierce rules (it does not bounce off enemies).
- Bullets must never leave the arena or get stuck inside a wall, even when fired straight into a corner.
- Existing weapons behave exactly as before.

Update the scene builder so a fresh `NeonBreach > Build Demo Scene` includes the new weapon on slot 4.
