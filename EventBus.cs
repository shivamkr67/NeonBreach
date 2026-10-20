using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBreach.Core
{
    /// <summary>
    /// Tiny type-safe publish/subscribe bus. Systems talk through events instead of
    /// direct references, which keeps gameplay systems decoupled and easy to test.
    /// </summary>
    public static class EventBus
    {
        static readonly List<Action> clearers = new List<Action>();

        static class Channel<T> where T : struct
        {
            public static Action<T> Handlers;
            static Channel() { clearers.Add(() => Handlers = null); }
        }

        public static void Subscribe<T>(Action<T> handler) where T : struct { Channel<T>.Handlers += handler; }
        public static void Unsubscribe<T>(Action<T> handler) where T : struct { Channel<T>.Handlers -= handler; }
        public static void Publish<T>(T evt) where T : struct { Channel<T>.Handlers?.Invoke(evt); }

        public static void ClearAll()
        {
            foreach (var clear in clearers) clear();
        }

        // Keeps the bus clean when "Enter Play Mode Options > Reload Domain" is disabled.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() { ClearAll(); }
    }
}
