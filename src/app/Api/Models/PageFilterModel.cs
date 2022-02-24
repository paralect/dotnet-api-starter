using System.Collections.Generic;

namespace Api.Models
{
    public class PageFilterModel
    {
        public int Page { get; set; } = 5;
        public int PerPage { get; set; } = 50;
        public IDictionary<string, int> Sort { get; set; }
        public string SearchValue { get; set; }
    }

    public class Sort
    {
        public int CreatedOn { get; set; }
    }
}
