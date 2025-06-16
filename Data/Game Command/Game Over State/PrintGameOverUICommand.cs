public class PrintGameOverUICommand : ICommand
{
    public void Execute()
    {
        GameUIController.Instance.ShowGameOverPanel();
    }
}