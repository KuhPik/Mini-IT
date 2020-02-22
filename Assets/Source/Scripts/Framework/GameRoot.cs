using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kuhpik
{
    public class GameRoot : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameConfig _config;
        [SerializeField] [Range(10, 60)] private int _updatesPerSecons;
        [SerializeField] private bool _isTapToStart;
        [SerializeField] private Button _startButton;

        private WaitForSeconds _timeStep;
        private FSMProcessor<GameState> _fsm;
        private static Dictionary<Type, MonoBehaviour> _systems;

        public void ChangeGameState(string name)
        {
            _fsm.State.Deactivate();
            _fsm.ChangeState(name);
            _fsm.State.Activate();
        }

        public static T GetSystems<T>() where T : class
        {
            return _systems[typeof(T)] as T;
        }

        public void GameRestart(int sceneIndex)
        {
            foreach (var system in _systems.Keys)
            {
                (system as IGameSystem).PerformAction<IDisposing>();
            }

            SceneManager.LoadScene(sceneIndex);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (_isTapToStart) _startButton.onClick.AddListener(InitSystems);
            else InitSystems();
        }

        private void InitSystems()
        {
            HandleGameStates();
            HandleTick();
        }

        private void HandleGameStates()
        {
            _fsm = new FSMProcessor<GameState>("Game", new GameState(_config, false, FindObjectOfType<TestSystem>()));

            _fsm.State.Activate();
        }

        private void HandleTick()
        {
            _timeStep = new WaitForSeconds(1f / _updatesPerSecons);
            StartCoroutine(GameRoutine());
        }

        private IEnumerator GameRoutine()
        {
            while (true)
            {
                yield return _timeStep;

                if (_fsm.State.IsInited)
                {
                    for (int i = 0; i < _fsm.State.RunningSystems.Length; i++)
                    {
                        _fsm.State.RunningSystems[i].OnRun();
                    }
                }
            }
        }
    }
}