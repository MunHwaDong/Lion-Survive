public class GameState : IState
{
    public void EnterState()
    {
        GameManager.Instance.invoker.ExecuteCommand(new InitPointCommand());
    }

    public void UpdateState()
    {
        GameManager.Instance.invoker.ExecuteCommands(GetType());
        
        if(GameManager.Instance.IsOnBossBattle?.Invoke() is true)
            GameManager.Instance.ChangeState(new BossState());
    }

    public void ExitState()
    {

    }
}