using System;

namespace Basis.Manager
{
    public class ManagerDispatcherFactory : IManagerDispatcherFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IManagerContext _context;

        public ManagerDispatcherFactory(IServiceProvider serviceProvider, IManagerContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public IManagerDispatcher<TManager> CreateFor<TManager>(TManager manager)
        {
            return new ManagerDispatcher<TManager>(_serviceProvider, _context, manager);
        }
    }
}