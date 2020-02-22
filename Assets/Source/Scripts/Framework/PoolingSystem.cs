using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kuhpik
{
    public static class PoolingSystem
    {
        private static Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        /// <summary>
        /// Creates pool with specified id and capacity. You also can use GetObject that automatically creates pool.
        /// </summary>
        public static void CreatePool(string id, GameObject prefab, bool dontDestroy, int capacity = 64)
        {
            _prefabs.Add(id, prefab);

            var queue = new Queue<GameObject>();
            for (int i = 0; i < capacity; i++)
            {
                queue.Enqueue(InstantiateObject(id, false, dontDestroy));
            }

            _pools.Add(id, queue);
        }

        /// <summary>
        /// Get object from pool
        /// </summary>
        public static GameObject GetObject(string id)
        {
            var @object = _pools[id].Count != 0 ? _pools[id].Dequeue() : InstantiateObject(id);
            @object.SetActive(true);
            return @object;
        }

        /// <summary>
        /// Get object from pool. If there is no pool - creates it.
        /// </summary>
        public static GameObject GetObject(string id, GameObject prefab, bool dontDestroy, int capacity = 64)
        {
            if (!_pools.ContainsKey(id)) CreatePool(id, prefab, dontDestroy, capacity);
            return GetObject(id);
        }

        /// <summary>
        /// Pool object back
        /// </summary>
        public static void PoolObject(GameObject @object, string id)
        {
            _pools[id].Enqueue(@object);
            @object.SetActive(false);
        }

        /// <summary>
        /// Pool object back after some time (like Destroy with time param)
        /// </summary>
        public static async void PoolObject(GameObject @object, string id, float time)
        {
            await Task.Delay(TimeSpan.FromSeconds(time));
            PoolObject(@object, id);
        }

        private static GameObject InstantiateObject(string id, bool dontDestroy = false)
        {
            var @object = GameObject.Instantiate(_prefabs[id]);
            if (dontDestroy) GameObject.DontDestroyOnLoad(@object);
            return @object;
        }

        private static GameObject InstantiateObject(string id, bool activeState, bool dontDestroy = false)
        {
            var @object = InstantiateObject(id, dontDestroy);
            @object.SetActive(activeState);
            return @object;
        }
    }
}