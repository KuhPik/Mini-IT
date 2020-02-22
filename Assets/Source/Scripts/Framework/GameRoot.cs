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

        private static Dictionary<Type, MonoBehaviour> _systems;

        private WaitForSeconds _timeStep;
        private FSMProcessor<GameState> _fsm;

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (_isTapToStart) _startButton.onClick.AddListener(InitSystems);
            else InitSystems();
        }

        private void InitSystems()
        {
            HandleGameStates();
            HandleAction<ISubscribing>();
            HandleAction<IIniting>();
            HandleTick();
        }

        private void HandleGameStates()
        {
            _fsm = new FSMProcessor<GameState>("Game", new PlayingState());
        }

        private void HandleAction<T>() where T : IGameSystem
        {
            for (int i = 0; i < _fsm.State.Systems.Length; i++)
            {
                _fsm.State.Systems[i].PerformAction<T>();
            }
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

                for (int i = 0; i < _fsm.State.RunningSystems.Length; i++)
                {
                    _fsm.State.RunningSystems[i].OnRun();
                }
            }
        }

        public static T GetSystems<T>() where T : class
        {
            return _systems[typeof(T)] as T;
        }

        public void GameRestart(int sceneIndex)
        {
            HandleAction<IDisposing>();
            SceneManager.LoadScene(sceneIndex);
        }
    }
}