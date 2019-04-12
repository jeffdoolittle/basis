using System;

namespace Basis.Resource
{
    public interface ITransactionalSession : ISession, IDisposable
    {
        void Commit();
        void Rollback();
    }
}