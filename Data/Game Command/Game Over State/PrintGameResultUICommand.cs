public class PrintGameResultUICommand : ICommand
{
    public void Execute()
    {
        GameUIController.Instance.ShowGameResultPanel();
    }
}