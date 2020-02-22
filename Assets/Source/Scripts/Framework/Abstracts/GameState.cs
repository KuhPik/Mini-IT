using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kuhpik
{
    public sealed class GameState
    {
        public IGameSystem[] Systems { get; private set; }
        public IRunning[] RunningSystems { get; private set; }
        public bool IsInited { get; private set; }

        private bool _isRestarting;

        public GameState(GameConfig config, bool isRestarting, params MonoBehaviour[] systems)
        {
            Systems = systems.Select(x => x as IGameSystem).ToArray();
            _isRestarting = isRestarting;
            Setup(config);

            Perform<ISubscribing>();
        }

        public void Activate()
        {
            if ((IsInited && _isRestarting) || !IsInited)
            {
                Perform<IIniting>();
                IsInited = true;
            }
        }

        public void Deactivate()
        {
            if (_isRestarting && IsInited)
            {
                Perform<IDisposing>();
                IsInited = false;
            }
        }

        private void Setup(GameConfig config)
        {
            var runnings = new List<IRunning>();

            for (int i = 0; i < Systems.Length; i++)
            {
                if (Systems[i] is IRunning) runnings.Add(Systems[i] as IRunning);
                (Systems[i] as GameSystem).InjectConfig(config);
            }

            RunningSystems = runnings.ToArray();
        }

        private void Perform<T>() where T : IGameSystem
        {
            for (int i = 0; i < Systems.Length; i++)
            {
                Systems[i].PerformAction<T>();
            }
        }
    }
}