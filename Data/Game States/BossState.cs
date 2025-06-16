public class BossState : IState
{
    public void EnterState()
    {

    }

    public void UpdateState()
    {
        GameManager.Instance.invoker.ExecuteCommands(this.GetType());
        
        if(GameManager.Instance.IsBossClear?.Invoke() is true)
            GameManager.Instance.ChangeState(new ClearState());
    }

    public void ExitState()
    {

    }
}