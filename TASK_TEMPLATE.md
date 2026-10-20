# Task template (micro1-style game evaluation task)

Every task in this folder follows the same structure so an AI agent (or a human) can attempt it
and a grader can verify it automatically.

```
Tasks/<ID>-<slug>/
  brief.md                 # what a player / designer / producer would actually write
  success_criteria.json    # inputs, expected visuals, verifiable in-game state
  solution.md              # root cause + reference fix (for bugs) or reference implementation (for features)
  reference/               # (feature tasks) drop-in reference source files
```

## Authoring checklist
1. Start from the clean project (`BugRegistry` empty) and confirm all non-BugRepro tests pass.
2. Pick or add a `BugId`; guard the faulty line with `BugRegistry.IsActive(...)`.
3. Write a **repro test** (`[Category("BugRepro")]`) that asserts the *buggy* behaviour — this proves the bug is real and observable.
4. Write a **fixed test** that asserts the *correct* behaviour — this is the acceptance test.
5. Write the brief in the voice of a non-programmer (symptoms, not code).
6. Define success criteria as input sequence + expected visual + checkable state with timestamps.
7. Playtest end-to-end by hand (enable the bug in `BugConfig`, press F1 to see the overlay).
8. Write the reference solution and re-run the full suite.
