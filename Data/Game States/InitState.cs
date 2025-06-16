using UnityEngine;

public class InitState : IState
{
    public void EnterState()
    {
        Debug.Log("Entering InitState");
        DataController.Instance.ObserveData(Status.CURRENT_HP, GameManager.Instance.GameOverCondition);
    }

    //딱 한 프레임 Loop.
    public void UpdateState()
    {
        GameManager.Instance.invoker.ExecuteCommands(GetType());
        
        GameManager.Instance.ChangeState(new GameState());
    }

    public void ExitState()
    {
        
    }
}