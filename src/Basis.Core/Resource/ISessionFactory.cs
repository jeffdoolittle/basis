using System.Data;

namespace Basis.Resource
{
    public interface ISessionFactory
    {
        ITransactionalSession OpenSession(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}