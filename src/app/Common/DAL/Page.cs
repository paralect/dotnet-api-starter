using System.Collections.Generic;

namespace Common.DAL
{
    public class Page<TModel>
    {
        public int TotalPages { get; set; }
        public long Count { get; set; }
        public ICollection<TModel> Items { get; set; }
    }
}
