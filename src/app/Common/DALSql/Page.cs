using System.Collections.Generic;

namespace Common.DALSql
{
    public class Page<TModel>
    {
        public int TotalPages { get; set; }
        public long Count { get; set; }
        public IEnumerable<TModel> Items { get; set; }
    }
}
