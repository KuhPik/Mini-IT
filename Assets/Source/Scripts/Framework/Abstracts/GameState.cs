using System.Collections.Generic;

namespace Kuhpik
{
    public abstract class GameState
    {
        public IGameSystem[] Systems { get; private set; }
        public IRunning[] RunningSystems { get; private set; }

        protected bool _isInited;

        /// <summary>
        /// Add systems you want, then call base. method after.
        /// </summary>
        public virtual void Setup(GameConfig config)
        {
            if (!_isInited) Init(config);
        }

        protected virtual void Init(GameConfig config)
        {
            var runnings = new List<IRunning>();

            for (int i = 0; i < Systems.Length; i++)
            {
                if (Systems[i] is IRunning) runnings.Add(Systems[i] as IRunning);
                (Systems[i] as GameSystem).InjectConfig(config);
            }

            RunningSystems = runnings.ToArray();
            _isInited = true;
        }
    }
}