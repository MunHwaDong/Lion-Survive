using UnityEngine;

public class GameOverState : IState
{
    public void EnterState()
    {
        GameManager.Instance.invoker.ExecuteCommand(new InitPointCommand());
        GameManager.Instance.invoker.ExecuteCommands(GetType());
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
 
    }
}