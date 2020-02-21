using System.Collections.Generic;
using UnityEngine;

public sealed class CommandProcessor
{
    private List<ICommand> _commands;
    private int _maxCount;
    private int _index;

    public CommandProcessor(int maxCount)
    {
        _commands = new List<ICommand>();
        _maxCount = maxCount;
    }

    public void Process(ICommand command)
    {
        _index = Mathf.Clamp(_index++, 0, _maxCount - 1);
        _commands.Push(command, _maxCount);
        command.Execute();
    }

    public void Undo()
    {
        _commands[_index].Undo();
        _commands.RemoveAt(_index);
        _index = Mathf.Clamp(_index--, 0, _maxCount - 1);
    }
}