using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kuhpik
{
    public class TestSystem : GameSystem, IIniting, IRunning
    {
        [SerializeField] private float _rotationSpeed;
        private Transform _cube;

        void IIniting.OnInit()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = Vector3.one * 5;
            _cube = cube.transform;

            Debug.Log(_config);
        }

        void IRunning.OnRun()
        {
            _cube.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
        }
    }
}
