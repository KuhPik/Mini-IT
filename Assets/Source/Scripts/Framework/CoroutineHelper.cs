using System.Collections.Generic;
using UnityEngine;

namespace Kuhpik
{
    /// <summary>
    /// GC-Free helper for Coroutines
    /// </summary>
    public static class CoroutineHelper
    {
        private static Dictionary<float, WaitForSeconds> _delays = new Dictionary<float, WaitForSeconds>();

        /// <summary>
        /// Returns GC-Free WaitForSeconds with specified time. If there is none - creates it.
        /// </summary>
        public static WaitForSeconds GetDelay(float time)
        {
            if (!_delays.ContainsKey(time)) _delays.Add(time, new WaitForSeconds(time));
            return _delays[time];
        }

        /// <summary>
        /// Removes existing WaitForSeconds.
        /// </summary>
        public static void Remove(float time)
        {
            if (_delays.ContainsKey(time)) _delays.Remove(time);
        }

        /// <summary>
        /// Completely clears dictionary.
        /// </summary>
        public static void Clear()
        {
            _delays.Clear();
        }
    }
}