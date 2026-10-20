# BUG-002 — My gun randomly stops hurting enemies

**Reported by:** Designer

The first few seconds of a fight feel great, but after a while my bullets start vanishing on contact: the enemy
doesn't flash, doesn't get knocked back and doesn't lose HP. Pulse Pistol and Scatter both do it. It gets worse the
longer the run goes on, and restarting the level fixes it temporarily.

**Expected:** every bullet that touches an enemy deals its damage, no matter how many shots have been fired.
**Actual:** after bullets have been recycled, some hits stop dealing damage.

Don't change the weapon stats or the pool size.
