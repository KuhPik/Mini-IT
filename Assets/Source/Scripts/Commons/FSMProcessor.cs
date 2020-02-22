using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class FSMProcessor<T>
{
    public T State { get; private set; }

    private Dictionary<string, T> _states = new Dictionary<string, T>();
    private Dictionary<string, IEnumerable<string>> _allowedTransition = new Dictionary<string, IEnumerable<string>>();

    private string _currentStateName;

    public FSMProcessor(string name, T state, params string[] allowedTransitions)
    {
        _currentStateName = name;
        State = state;
        AddState(name, state);
    }

    public void AddState(string name, T state, params string[] allowedTransitions)
    {
        _states.Add(name, state);
        AddTransition(name, allowedTransitions);
    }

    public void ChangeState(string name)
    {
        if (_allowedTransition.ContainsKey(_currentStateName) && !_allowedTransition[_currentStateName].Contains(name))
        {
            Debug.LogError($"Not allowed transition from {_currentStateName} to {name}!");
        }
        else
        {
            Debug.Log($"State changed to {name}!");
            State = _states[name];
            _currentStateName = name;
        }
    }

    public void AddTransition(string name, params string[] allowedTransitions)
    {
        _allowedTransition.Add(name, allowedTransitions);
    }
}