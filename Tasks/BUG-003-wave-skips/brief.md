# BUG-003 — Waves end while enemies are still alive

**Reported by:** Producer

During wave 2 the "WAVE CLEARED" banner pops up when I've only killed about half of the enemies. The next wave
starts and there are leftover enemies from the previous wave still chasing me, so the fights get way harder than
designed. Pressing F1 shows the wave manager thinks there are fewer enemies than there really are.

**Expected:** a wave is only cleared when every enemy from that wave is gone.
**Actual:** it clears early.

Please keep the event-based architecture (don't poll enemies every frame).
