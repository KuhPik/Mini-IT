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
        [SerializeField] private GameConfig config;
        [SerializeField] [Range(10, 60)] private int updatesPerSecons;
        [SerializeField] private bool isTapToStart;
        [SerializeField] private Button startButton;

        private FSMProcessor<GameState> fsm;
        private Dictionary<Type, object> injections;
        private static Dictionary<Type, MonoBehaviour> systems;

        public void ChangeGameState(string name)
        {
            fsm.State.Deactivate();
            fsm.ChangeState(name);

            Inject(fsm.State.Systems);
            fsm.State.Activate();
        }

        public static T GetSystems<T>() where T : class
        {
            return systems[typeof(T)] as T;
        }

        public void GameRestart(int sceneIndex)
        {
            foreach (var system in systems.Keys)
            {
                (system as IGameSystem).PerformAction<IDisposing>();
            }

            SceneManager.LoadScene(sceneIndex);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (isTapToStart) startButton.onClick.AddListener(InitSystems);
            else InitSystems();
        }

        private void InitSystems()
        {
            HandleGameStates();
            HandleInjections();
            HandleTick();

            fsm.State.Activate();
        }

        private void HandleGameStates()
        {
            fsm = new FSMProcessor<GameState>("Game", new GameState(false, FindObjectOfType<TestSystem>()));
        }

        private void HandleInjections()
        {
            injections = new Dictionary<Type, object>();

            //Injections
            injections.Add(typeof(GameConfig), config);

            //process
            Inject(fsm.State.Systems);
        }

        private void Inject(IEnumerable<IGameSystem> systems)
        {
            if (injections == null || injections.Count == 0) return;

            foreach (var system in systems)
            {
                var type = system.GetType();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (var pair in injections)
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
            StartCoroutine(GameRoutine());
        }

        private IEnumerator GameRoutine()
        {
            while (true)
            {
                yield return CoroutineHelper.GetDelay(1f / updatesPerSecons);

                if (fsm.State.IsInited)
                {
                    for (int i = 0; i < fsm.State.RunningSystems.Length; i++)
                    {
                        fsm.State.RunningSystems[i].OnRun();
                    }
                }
            }
        }
    }
}