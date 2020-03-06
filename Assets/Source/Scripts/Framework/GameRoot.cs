using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        private Dictionary<Type, object> _injections;
        private static Dictionary<Type, MonoBehaviour> _systems;

        public void ChangeGameState(string name)
        {
            _fsm.State.Deactivate();
            _fsm.ChangeState(name);

            Inject(_fsm.State.Systems);
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
            HandleInjections();
            HandleTick();

            _fsm.State.Activate();
        }

        private void HandleGameStates()
        {
            _fsm = new FSMProcessor<GameState>("Game", new GameState(false, FindObjectOfType<TestSystem>()));
        }

        private void HandleInjections()
        {
            _injections = new Dictionary<Type, object>();

            //Injections
            _injections.Add(typeof(GameConfig), _config);

            //process
            Inject(_fsm.State.Systems);
        }

        private void Inject(IEnumerable<IGameSystem> systems)
        {
            if (_injections == null || _injections.Count == 0) return;

            foreach (var system in systems)
            {
                var type = system.GetType();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (var pair in _injections)
                    {
                        if (field.FieldType.IsAssignableFrom(pair.Key))
                        {
                            field.SetValue(system, pair.Value);
                        }
                    }
                }
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