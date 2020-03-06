using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kuhpik
{
    public static class PoolingSystem
    {
        private const int capacity = 16;

        private static Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        /// <summary>
        /// Creates pool with specified id. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(string id, GameObject prefab, int capacity = capacity, float poolTime = 0f, bool dontDestroy = false)
        {
            _prefabs.Add(id, prefab);

            var queue = new Queue<GameObject>();
            for (int i = 0; i < capacity; i++)
            {
                queue.Enqueue(InstantiateObject(id, false, dontDestroy));
            }

            _pools.Add(id, queue);
        }

        #region CreatePool adapters

        /// <summary>
        /// Creates pool with specified id. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(string id, GameObject prefab, int capacity = capacity, float poolTime = 0f)
        {
            CreatePool(id, prefab, capacity, poolTime, false);
        }

        /// <summary>
        /// Creates pool with specified id. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(string id, GameObject prefab, int capacity = capacity, bool dontDestroy = false)
        {
            CreatePool(id, prefab, capacity, 0, dontDestroy);
        }

        /// <summary>
        /// Creates pool with id that is gameobject's name. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(GameObject prefab, int capacity = capacity, float poolTime = 0f, bool dontDestroy = false)
        {
            CreatePool(prefab.name, prefab, capacity, poolTime, dontDestroy);
        }

        /// <summary>
        /// Creates pool with id that is gameobject's name. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(GameObject prefab, int capacity = capacity, float poolTime = 0f)
        {
            CreatePool(prefab.name, prefab, capacity, poolTime, false);
        }

        /// <summary>
        /// Creates pool with id that is gameobject's name. You can also create pool automatically by using GetObject().
        /// </summary>
        public static void CreatePool(GameObject prefab, int capacity = capacity, bool dontDestroy = false)
        {
            CreatePool(prefab.name, prefab, capacity, 0, dontDestroy);
        }

        #endregion CreatePool adapters

        #region GetObject

        /// <summary>
        /// Get object from pool
        /// </summary>
        public static GameObject GetObject(string id)
        {
            var @object = _pools[id].Count != 0 ? _pools[id].Dequeue() : InstantiateObject(id);
            @object.SetActive(true);
            return @object;
        }

        public static GameObject GetObject(GameObject prefab)
        {
            return GetObject(prefab.name);
        }

        #endregion GetObject

        #region GetObject with pool creation

        /// <summary>
        /// Get object from pool. If there is no pool - creates it.
        /// </summary>
        public static GameObject GetObject(string id, GameObject prefab, int capacity = capacity, float poolTime = 0f, bool dontDestroy = false)
        {
            if (!_pools.ContainsKey(id)) CreatePool(id, prefab, capacity, dontDestroy);
            return GetObject(id);
        }

        /// <summary>
        /// Get object from pool. If there is no pool - creates it with id that is gameobject's name.
        /// </summary>
        public static GameObject GetObject(GameObject prefab, int capacity = capacity, bool dontDestroy = false)
        {
            return GetObject(prefab.name, prefab, capacity, 0f, dontDestroy);
        }

        #endregion GetObject with pool creation

        #region Pooling object

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

        /// <summary>
        /// Pool object back with id that is gameobject's name. Specify id if you changed the object's name.
        /// </summary>
        public static void PoolObject(GameObject @object)
        {
            PoolObject(@object, @object.name);
        }

        /// <summary>
        /// Pool object back after some time (like Destroy with time param). Specify id if you changed the object's name.
        /// </summary>
        public static void PoolObject(GameObject @object, float time)
        {
            PoolObject(@object, @object.name, time);
        }

        #endregion Pooling object

        #region Instantiating

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

        #endregion
    }
}