namespace Basis.Resource
{
    public interface IPagingHelper<out T> where T : class, new()
    {
        IPagedResult<T> GetPage(long currentPage, int pageSize);
    }
}