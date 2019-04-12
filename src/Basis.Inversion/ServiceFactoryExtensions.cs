namespace Basis.Inversion
{
    public static class ServiceFactoryExtensions
    {
        public static TService GetService<TService>(this IServiceFactory services)
        {
            return (TService)services.GetService(typeof(TService));
        }
    }
}