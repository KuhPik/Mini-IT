using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kuhpik
{
    [System.Serializable]
    public struct PoolingData
    {
        public GameObject prefab;
        public ObjectType type;
        public int preloaded;
    }

    public enum ObjectType
    {

    }

    public sealed class PoolingSystem : MonoBehaviour
    {
        public static PoolingSystem Instance;
        [SerializeField] private PoolingData[] poolingDatas;
        private Dictionary<ObjectType, GameObject> prefabDictionary;
        private Dictionary<ObjectType, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Debug.LogError("Wrong scene was loaded"); DestroyImmediate(gameObject); }
        }

        private void Start()
        {
            poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();
            prefabDictionary = new Dictionary<ObjectType, GameObject>();

            for (int i = 0; i < poolingDatas.Length; i++)
            {
                var _type = poolingDatas[i].type;

                poolDictionary.Add(_type, new Queue<GameObject>());
                prefabDictionary.Add(_type, poolingDatas[i].prefab);

                for (int j = 0; j < poolingDatas[i].preloaded; j++)
                {
                    poolDictionary[_type].Enqueue(InstantiateObject(_type, false));
                }
            }
        }

        /// <summary>
        /// Get object from pool
        /// </summary>
        public GameObject GetObject(ObjectType _type)
        {
            var _object = poolDictionary[_type].Count != 0 ? poolDictionary[_type].Dequeue() : InstantiateObject(_type);
            _object.SetActive(true);
            return _object;
        }

        /// <summary>
        /// Pool object back
        /// </summary>
        public void PoolObject(GameObject _object, ObjectType _type)
        {
            poolDictionary[_type].Enqueue(_object);
            _object.SetActive(false);
        }

        /// <summary>
        /// Pool object back after some time (like Destroy with time param)
        /// </summary>
        public void PoolObject(GameObject _object, ObjectType _type, float _time)
        {
            StartCoroutine(PoolRoutine(_object, _type, _time));
        }

        private IEnumerator PoolRoutine(GameObject _object, ObjectType _type, float _time)
        {
            yield return new WaitForSeconds(_time);
            PoolObject(_object, _type);
        }

        private GameObject InstantiateObject(ObjectType _type)
        {
            var _object = Instantiate(prefabDictionary[_type]);
            DontDestroyOnLoad(_object);
            return _object;
        }

        private GameObject InstantiateObject(ObjectType _type, bool _activeState)
        {
            var _object = InstantiateObject(_type);
            _object.SetActive(_activeState);
            return _object;
        }
    }
}