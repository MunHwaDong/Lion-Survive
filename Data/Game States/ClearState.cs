using UnityEngine;

public class ClearState : IState
{
    public void EnterState()
    {
        GameManager.Instance.invoker.ExecuteCommands(this.GetType());
        GameUIController.Instance.SetGameOver();
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {

    }
}