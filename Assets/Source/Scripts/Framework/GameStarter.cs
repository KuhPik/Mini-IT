using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace Kuhpik
{
    public class GameStarter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameConfig _config;
        [SerializeField] [Range(10, 60)] private int _updatesPerSecons;
        [SerializeField] private bool _isTapToStart;
        [SerializeField] private Button _startButton;

        private WaitForSeconds _timeStep;

        private Dictionary<Type, IGameSystem> _systemsDictionary;
        private IRunning[] _runningSystems = new IRunning[0];
        private Type[] _indexes = new Type[0];

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (_isTapToStart) _startButton.onClick.AddListener(InitSystems);
            else InitSystems();
        }

        private void InitSystems()
        {
            HandleSystems();
            HandleAction<ISubscribing>();
            HandleAction<IIniting>();
            HandleTick();
        }

        private void HandleAction<T>() where T : IGameSystem
        {
            for (int i = 0; i < _indexes.Length; i++)
            {
                _systemsDictionary[_indexes[i]].PerformAction<T>();
            }
        }

        private void HandleTick()
        {
            _timeStep = new WaitForSeconds(1f / _updatesPerSecons);
            StartCoroutine(GameRoutine());
        }

        /// <summary>
        /// Thx to https://github.com/Leopotam
        /// Order of childs is important!!!
        /// </summary>
        private void HandleSystems()
        {
            var indexes = new List<Type>();
            var runnings = new List<IRunning>();
            _systemsDictionary = new Dictionary<Type, IGameSystem>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var systemGO = transform.GetChild(i).gameObject;

                if (!systemGO.activeSelf) return;
                if (systemGO.TryGetComponent<GameSystem>(out var system))
                {
                    _systemsDictionary.TryAddSystem(system.gameObject, ref indexes, ref runnings);
                    system.InjectConfig(_config);
                }
            }

            _indexes = indexes.ToArray();
            _runningSystems = runnings.ToArray();
        }

        private IEnumerator GameRoutine()
        {
            while (true)
            {
                yield return _timeStep;

                for (int i = 0; i < _runningSystems.Length; i++)
                {
                    _runningSystems[i].OnRun();
                }
            }
        }

        public T GetSystems<T>() where T : class
        {
            return _systemsDictionary[typeof(T)] as T;
        }

        public void GameRestart(int sceneIndex)
        {
            HandleAction<IDisposing>();
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
