namespace MoatGate.RequestHandling
{
    public interface ICommand
    {
        Event ToEvent();
    }

    public interface ICommand<T> : ICommand
    {

    }
}
