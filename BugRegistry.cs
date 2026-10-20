using System.Collections.Generic;
using UnityEngine;

namespace NeonBreach.Bugs
{
    /// <summary>
    /// Every deliberately injected bug has an id. Gameplay code asks the registry whether a bug
    /// is active, so the same codebase can be the "clean" reference project OR any buggy variant
    /// used for AI-evaluation tasks. See /Tasks for the briefs and reference fixes.
    /// </summary>
    public enum BugId
    {
        None = 0,
        DashInvulnerabilityLeak = 1,
        StalePierceCounter = 2,
        WaveDoubleDecrement = 3,
        ComboTimerNotRefreshed = 4
    }

    public static class BugRegistry
    {
        static readonly HashSet<BugId> active = new HashSet<BugId>();

        public static IEnumerable<BugId> Active { get { return active; } }

        public static bool IsActive(BugId id) { return active.Contains(id); }

        public static void Set(BugId id, bool on)
        {
            if (id == BugId.None) return;
            if (on) active.Add(id); else active.Remove(id);
        }

        public static void Clear() { active.Clear(); }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { active.Clear(); }
    }
}
