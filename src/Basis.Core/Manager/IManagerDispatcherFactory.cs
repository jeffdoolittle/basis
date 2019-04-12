namespace Basis.Manager
{
    public interface IManagerDispatcherFactory
    {
        IManagerDispatcher<TManager> CreateFor<TManager>(TManager manager);
    }
}