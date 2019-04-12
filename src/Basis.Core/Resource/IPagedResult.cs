using System.Collections.Generic;

namespace Basis.Resource
{
    public interface IPagedResult<out T>
    {
        long TotalRecords { get; }
        long TotalPages { get; }
        IReadOnlyList<T> Items { get; }
    }
}