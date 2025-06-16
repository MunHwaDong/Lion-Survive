using System;
using System.Collections.Generic;

public class Invoker
{
    //TODO: private 로 다시 변경
    public Dictionary<Type, List<ICommand>> _stateBehaviours = new();

    public void RegisterCommand<T>(T state, ICommand command) where T : Type
    {
        if (_stateBehaviours.TryGetValue(state, out var behaviours))
        {
            behaviours.Add(command);
        }
        else
        {
            _stateBehaviours.Add(state, new List<ICommand> { command });
        }
    }

    public void ExecuteCommands<T>(T state) where T : Type
    {
        if (_stateBehaviours.TryGetValue(state, out List<ICommand> commands))
        {
            foreach (ICommand command in commands)
            {
                command.Execute();
            }
        }
    }

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
    }
}