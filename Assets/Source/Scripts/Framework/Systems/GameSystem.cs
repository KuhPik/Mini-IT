using UnityEngine;

namespace Kuhpik
{
    public abstract class GameSystem : MonoBehaviour
    {
        protected GameConfig _config;
        public void InjectConfig(GameConfig config) { _config = config; }
    }
}
