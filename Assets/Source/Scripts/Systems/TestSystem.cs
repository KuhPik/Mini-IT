using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kuhpik
{
    public class TestSystem : GameSystem, IIniting, IRunning
    {
        [SerializeField] private float rotationSpeed;
        private Transform cube;

        void IIniting.OnInit()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = Vector3.one * 5;
            this.cube = cube.transform;

            Debug.Log(config);
        }

        void IRunning.OnRun()
        {
            cube.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }
}
