using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBreach.Core
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }

    /// <summary>Generic component pool. Avoids per-shot / per-enemy allocations and GC spikes.</summary>
    public sealed class ComponentPool<T> where T : Component
    {
        readonly Stack<T> free = new Stack<T>();
        readonly Func<T> factory;
        readonly Transform root;

        public int CountInactive { get { return free.Count; } }
        public int CountCreated { get; private set; }

        public ComponentPool(Func<T> factory, Transform root, int prewarm = 0)
        {
            this.factory = factory;
            this.root = root;
            for (int i = 0; i < prewarm; i++)
            {
                T item = Create();
                item.gameObject.SetActive(false);
                free.Push(item);
            }
        }

        T Create()
        {
            T item = factory();
            if (root != null) item.transform.SetParent(root, true);
            CountCreated++;
            return item;
        }

        public T Get()
        {
            T item = null;
            while (free.Count > 0 && item == null) item = free.Pop();
            if (item == null) item = Create();
            item.gameObject.SetActive(true);
            var poolable = item as IPoolable;
            if (poolable != null) poolable.OnSpawn();
            return item;
        }

        public void Release(T item)
        {
            if (item == null || !item.gameObject.activeSelf) return; // guards against double release
            var poolable = item as IPoolable;
            if (poolable != null) poolable.OnDespawn();
            item.gameObject.SetActive(false);
            free.Push(item);
        }
    }
}
