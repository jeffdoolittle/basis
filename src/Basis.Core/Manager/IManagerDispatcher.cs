using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Basis.Manager
{
    public interface IManagerDispatcher<TManager>
    {
        T Do<T>(Expression<Func<TManager, T>> func, [CallerMemberName] string caller = "");
        void Do(Expression<Action<TManager>> action, [CallerMemberName] string caller = "");
    }
}