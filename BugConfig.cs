using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBreach.Bugs
{
    /// <summary>
    /// Put this in the scene to enable bugs from the Inspector, or launch a build with
    /// "-bugs=DashInvulnerabilityLeak,StalePierceCounter" to enable them from the command line.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class BugConfig : MonoBehaviour
    {
        public List<BugId> activeBugs = new List<BugId>();
        public bool readCommandLine = true;

        void Awake()
        {
            BugRegistry.Clear();
            foreach (BugId id in activeBugs) BugRegistry.Set(id, true);

            if (!readCommandLine) return;
            const string prefix = "-bugs=";
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (!arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
                foreach (string name in arg.Substring(prefix.Length).Split(','))
                {
                    BugId parsed;
                    if (Enum.TryParse(name.Trim(), true, out parsed)) BugRegistry.Set(parsed, true);
                }
            }
        }
    }
}
