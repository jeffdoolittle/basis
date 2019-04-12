using System.Collections.Generic;
using System.Data;
using Basis.Resource;

namespace Basis.Db
{
    internal class PagingHelper<T> : IPagingHelper<T> where T : class, new()
    {
        private readonly PagingHelper _inner;

        public PagingHelper(ISession session, IDialect dialect, string sql, string orderBy, params object[] parameters)
        {
            _inner = new PagingHelper(session, dialect, sql, orderBy, parameters);
        }

        public IPagedResult<T> GetPage(long currentPage, int pageSize)
        {
            if (currentPage < 1)
            {
                throw new ResourceException("currentPage must be greater than 0");
            }

            if (pageSize < 1)
            {
                throw new ResourceException("pageSize must be greater than 0");
            }

            var readerResult = _inner.ExecuteReader(currentPage, pageSize);

            var items = new List<T>();

            while (readerResult.Reader.Read())
            {
                items.Add(readerResult.Reader.MapDataTo<T>());
            }

            return new PagedResult
            {
                TotalPages = readerResult.TotalPages,
                TotalRecords = readerResult.TotalRecords,
                Items = items
            };
        }

        private class PagedResult : IPagedResult<T>
        {
            public long TotalRecords { get; set; }
            public long TotalPages { get; set; }
            public IReadOnlyList<T> Items { get; set; }
        }
    }

    internal class PagingHelper
    {
        private readonly ISession _session;
        private readonly IDialect _dialect;
        private readonly string _sql;
        private readonly string _orderBy;
        private readonly object[] _parameters;

        public PagingHelper(ISession session, IDialect dialect, string sql, string orderBy, params object[] parameters)
        {
            _session = session;
            _dialect = dialect;
            _sql = sql;
            _orderBy = orderBy;
            _parameters = parameters ?? new object[0];
        }

        public PagedResult ExecuteReader(long currentPage, int pageSize)
        {
            var pagedParameters = new List<object>(_parameters);
            var minRow = ((currentPage - 1) * pageSize) + 1;        // oracle rownum is indexed to 1, not zero
            var maxRow = minRow + pageSize - 1;                     // add pageSize - 1 to get correct number of total rows
            pagedParameters.Add(maxRow);
            pagedParameters.Add(minRow);

            var countQuery = $"SELECT COUNT(*) FROM ({_sql}) SUB";

            var totalRecords = (decimal)_session.ExecuteScalar(countQuery, _parameters);

            var pagedQuery = PagedResultQuery();

            var totalPages = totalRecords / pageSize;
            if (totalRecords % pageSize > 0)
            {
                totalPages += 1;
            }

            var response = new PagedResult
            {
                Reader = _session.ExecuteReader(pagedQuery, pagedParameters.ToArray()),
                TotalRecords = (long)totalRecords,
                TotalPages = (long)totalPages
            };

            return response;
        }

        private string PagedResultQuery()
        {
            var orderedSql = $"{_sql} {_orderBy}";
            var top = _parameters.Length;
            var bottom = _parameters.Length + 1;
            var pagedSql = $"SELECT * FROM (SELECT X.*, rownum rn FROM ({orderedSql}) X WHERE rownum <= :{top}) WHERE rn >= :{bottom}";
            return pagedSql;
        }

        internal class PagedResult
        {
            public long TotalRecords { get; set; }
            public long TotalPages { get; set; }
            public IDataReader Reader { get; set; }
        }
    }
}