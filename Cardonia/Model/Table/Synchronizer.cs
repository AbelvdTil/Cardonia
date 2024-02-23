namespace Cardonia.Model.Table;

public class Synchronizer
{
    public event Func<Task>? SynchronizeEvent;

    public void Synchronize()
    {
        SynchronizeEvent?.Invoke();
    }

}
